using Godot;
using System.Collections.Generic;
using System.Linq;

public class GeneradorGrupos : IGeneradorFormato
{
    public void GenerarEstructura(FaseTorneo fase, List<string> equipos)
    {
        int porGrupo = fase.EquiposPorGrupo > 0 ? fase.EquiposPorGrupo : 4;
        int numeroGrupos = Mathf.CeilToInt((float)equipos.Count / porGrupo);

        List<string> ordenados = equipos.OrderBy(e => RankingFifaProvider.ObtenerPosicion(e)).ToList();

        var grupos = new List<GrupoTorneo>();
        for (int i = 0; i < numeroGrupos; i++)
            grupos.Add(new GrupoTorneo { Nombre = $"Grupo {(char)('A' + i)}" });

        for (int bombo = 0; bombo < porGrupo; bombo++)
        {
            List<string> equiposBombo = ordenados.Skip(bombo * numeroGrupos).Take(numeroGrupos).ToList();
            AleatorioUtil.Mezclar(equiposBombo);

            bool inverso = bombo % 2 == 1;
            for (int i = 0; i < equiposBombo.Count; i++)
            {
                int indiceGrupo = inverso ? (numeroGrupos - 1 - i) : i;
                grupos[indiceGrupo].Equipos.Add(equiposBombo[i]);
            }
        }

        foreach (GrupoTorneo grupo in grupos)
        {
            // Si la fase dice IdaYVuelta, usamos el generador doble. Si no, el simple.
            grupo.Calendario = fase.IdaYVuelta
                ? GeneradorFixture.GenerarFixtureIdaYVuelta(grupo.Equipos)
                : GeneradorFixture.GenerarRondasUnaVuelta(grupo.Equipos);

            grupo.TablaPosiciones = grupo.Equipos
                .Select(nombre => new EstadisticasEquipoGuardado { NombreEquipo = nombre })
                .ToList();
        }

        fase.Grupos = grupos;
    }

    public void ProcesarResultado(FaseTorneo fase, string equipoLocal, string equipoVisitante, int golesLocal, int golesVisitante)
    {
        GrupoTorneo grupo = fase.Grupos.FirstOrDefault(g =>
            g.Equipos.Contains(equipoLocal) && g.Equipos.Contains(equipoVisitante));
        if (grupo == null) return;

        PartidoFixture partido = grupo.Calendario.FirstOrDefault(p =>
            !p.Jugado && p.EquipoLocal == equipoLocal && p.EquipoVisitante == equipoVisitante);
        if (partido == null) return;

        partido.GolesLocal = golesLocal;
        partido.GolesVisitante = golesVisitante;
        partido.Jugado = true;

        EstadisticasHelper.RegistrarResultado(grupo.TablaPosiciones, equipoLocal, equipoVisitante, golesLocal, golesVisitante);
    }

    public bool FaseCompleta(FaseTorneo fase) => fase.Grupos.All(g => g.Calendario.All(p => p.Jugado));

    public List<string> ObtenerClasificados(FaseTorneo fase)
    {
        var primeros = new List<string>();
        var segundos = new List<string>();

        foreach (GrupoTorneo grupo in fase.Grupos)
        {
            List<string> tablaOrdenada = EstadisticasHelper.OrdenarTabla(grupo.TablaPosiciones);
            if (tablaOrdenada.Count > 0) primeros.Add(tablaOrdenada[0]);
            if (tablaOrdenada.Count > 1) segundos.Add(tablaOrdenada[1]);
        }

        primeros.AddRange(segundos);
        return primeros;
    }

    /// <summary>
    /// Separa a los clasificados en dos listas: los líderes (directos) y los mejores segundos (repechaje).
    /// </summary>
    public (List<string> Directos, List<string> MejoresSegundos) ObtenerClasificadosConRepechaje(FaseTorneo fase, int cantidadMejoresSegundos)
    {
        var campeones = new List<string>();
        var subcampeones = new List<EstadisticasEquipoGuardado>();

        foreach (GrupoTorneo grupo in fase.Grupos)
        {
            List<EstadisticasEquipoGuardado> tablaOrdenada = EstadisticasHelper.OrdenarPorCriterioFifa(grupo.TablaPosiciones);
            if (tablaOrdenada.Count > 0) campeones.Add(tablaOrdenada[0].NombreEquipo);
            if (tablaOrdenada.Count > 1) subcampeones.Add(tablaOrdenada[1]);
        }

        List<string> mejoresSegundos = EstadisticasHelper
            .OrdenarPorCriterioFifa(subcampeones)
            .Take(cantidadMejoresSegundos)
            .Select(e => e.NombreEquipo)
            .ToList();

        return (campeones, mejoresSegundos);
    }
}
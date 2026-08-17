using System.Collections.Generic;
using System.Linq;

public class GeneradorRoundRobin : IGeneradorFormato
{
    public void GenerarEstructura(FaseTorneo fase, List<string> equipos)
    {
        fase.Calendario = GeneradorFixture.GenerarFixtureIdaYVuelta(equipos);
        fase.TablaPosiciones = equipos
            .Select(nombre => new EstadisticasEquipoGuardado { NombreEquipo = nombre })
            .ToList();
        fase.JornadaActual = 1;
    }

    public void ProcesarResultado(FaseTorneo fase, string equipoLocal, string equipoVisitante, int golesLocal, int golesVisitante)
    {
        PartidoFixture partido = fase.Calendario.FirstOrDefault(p =>
            !p.Jugado && p.EquipoLocal == equipoLocal && p.EquipoVisitante == equipoVisitante);
        if (partido == null) return;

        partido.GolesLocal = golesLocal;
        partido.GolesVisitante = golesVisitante;
        partido.Jugado = true;

        EstadisticasHelper.RegistrarResultado(fase.TablaPosiciones, equipoLocal, equipoVisitante, golesLocal, golesVisitante);

        bool faltanPartidos = fase.Calendario.Any(p => p.Jornada == fase.JornadaActual && !p.Jugado);
        if (!faltanPartidos) fase.JornadaActual++;
    }

    public bool FaseCompleta(FaseTorneo fase) => fase.Calendario.All(p => p.Jugado);

    public List<string> ObtenerClasificados(FaseTorneo fase) => EstadisticasHelper.OrdenarTabla(fase.TablaPosiciones);
}
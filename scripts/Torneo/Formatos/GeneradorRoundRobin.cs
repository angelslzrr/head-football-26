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

    /// <summary>
    /// Separa la tabla final en dos grupos: los primeros N puestos (directos)
    /// y los siguientes M puestos (repechaje).
    /// </summary>
    public (List<string> Directos, List<string> Repechaje) ObtenerClasificadosConRepechaje(
        FaseTorneo fase, int cantidadDirectos, int cantidadRepechaje)
    {
        List<string> tablaOrdenada = EstadisticasHelper.OrdenarTabla(fase.TablaPosiciones);

        // Toma a los primeros N (Ej. los 6 primeros)
        List<string> directos = tablaOrdenada.Take(cantidadDirectos).ToList();
        
        // Se salta los N primeros, y toma los siguientes M (Ej. se salta 6, toma el 7mo)
        List<string> repechaje = tablaOrdenada.Skip(cantidadDirectos).Take(cantidadRepechaje).ToList();

        return (directos, repechaje);
    }
}
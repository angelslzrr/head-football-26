using System.Collections.Generic;

/// <summary>
/// Objeto raíz del estado del juego. 
/// Contiene toda la información necesaria para reconstruir una partida guardada.
/// Almacena metadatos, el calendario completo de partidos y la tabla de posiciones actual.
/// </summary>
public class TournamentState
{
    public string NombreEquipoJugador { get; set; } = "";
    public string Region { get; set; } = "";
    public string FechaGuardado { get; set; } = "";
    public int JornadaActual { get; set; } = 1;

    public List<PartidoFixture> Calendario { get; set; } = new List<PartidoFixture>();
    public List<EstadisticasEquipoGuardado> TablaPosiciones { get; set; } = new List<EstadisticasEquipoGuardado>();
}
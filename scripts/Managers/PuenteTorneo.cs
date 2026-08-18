using Godot;

public partial class PuenteTorneo : Node
{
    public static PuenteTorneo Instance { get; private set; }

    public bool PartidoDeTorneo { get; private set; } = false;

    public string EquipoJugador { get; private set; } = "";
    public string EquipoRival { get; private set; } = "";

    public bool JugadorEsLocal { get; private set; } = true;
    public bool EsFaseEliminacion { get; private set; } = false;

    public bool EsPartidoDeVuelta { get; private set; } = false;
    public int GolesGlobalPrevios { get; private set; } = 0;       // Goles del JUGADOR en la ida
    public int GolesGlobalPreviosRival { get; private set; } = 0;  // Goles del RIVAL en la ida

    public int GolesJugador { get; private set; } = 0;
    public int GolesRival { get; private set; } = 0;

    public override void _Ready()
    {
        Instance = this;
    }

    public void IniciarPartidoDeTorneo(string equipoJugador, string equipoRival, bool jugadorEsLocal, bool esFaseEliminacion,
        bool esPartidoDeVuelta = false, int golesGlobalPreviosJugador = 0, int golesGlobalPreviosRival = 0)
    {
        PartidoDeTorneo = true;
        EquipoJugador = equipoJugador;
        EquipoRival = equipoRival;
        JugadorEsLocal = jugadorEsLocal;
        EsFaseEliminacion = esFaseEliminacion;
        EsPartidoDeVuelta = esPartidoDeVuelta;
        GolesGlobalPrevios = golesGlobalPreviosJugador;
        GolesGlobalPreviosRival = golesGlobalPreviosRival;
    }

    public void GuardarResultado(int golesJugador, int golesRival)
    {
        GolesJugador = golesJugador;
        GolesRival = golesRival;
    }

    public void FinalizarPartidoDeTorneo()
    {
        PartidoDeTorneo = false;
        EquipoJugador = "";
        EquipoRival = "";
        EsFaseEliminacion = false;
        EsPartidoDeVuelta = false;
        GolesGlobalPrevios = 0;
        GolesGlobalPreviosRival = 0;
    }
}
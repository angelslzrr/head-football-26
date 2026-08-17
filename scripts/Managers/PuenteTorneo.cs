using Godot;

public partial class PuenteTorneo : Node
{
    public static PuenteTorneo Instance { get; private set; }

    public bool PartidoDeTorneo { get; private set; } = false;

    public string EquipoJugador { get; private set; } = "";
    public string EquipoRival { get; private set; } = "";
    
    public bool JugadorEsLocal { get; private set; } = true;

    // NUEVO: le dice al Hud si este partido puede terminar en Gol de Oro.
    public bool EsFaseEliminacion { get; private set; } = false;

    public int GolesJugador { get; private set; } = 0;
    public int GolesRival { get; private set; } = 0;

    public override void _Ready()
    {
        Instance = this;
    }

    public void IniciarPartidoDeTorneo(string equipoJugador, string equipoRival, bool jugadorEsLocal, bool esFaseEliminacion)
    {
        PartidoDeTorneo = true;
        EquipoJugador = equipoJugador;
        EquipoRival = equipoRival;
        JugadorEsLocal = jugadorEsLocal;
        EsFaseEliminacion = esFaseEliminacion;
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
    }
}
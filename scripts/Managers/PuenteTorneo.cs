using Godot;

/// <summary>
/// Singleton (Autoload) diseñado para la inyección de dependencias efímeras.
/// Su única responsabilidad es transportar datos temporales durante la transición 
/// entre la escena de Interfaz (TournamentHub) y la escena de Gameplay (Cancha).
/// No persiste en disco; su ciclo de vida se limpia tras cada partido.
/// </summary>
public partial class PuenteTorneo : Node
{
    public static PuenteTorneo Instance { get; private set; }

    public bool PartidoDeTorneo { get; private set; } = false;

    // Almacena los roles claros para el motor de gameplay, independientemente
    // de quién sea el local o el visitante en el fixture.
    public string EquipoJugador { get; private set; } = "";
    public string EquipoRival { get; private set; } = "";
    
    // Necesario para reconstruir el fixture correctamente al regresar al menú.
    public bool JugadorEsLocal { get; private set; } = true;

    public int GolesJugador { get; private set; } = 0;
    public int GolesRival { get; private set; } = 0;

    public override void _Ready()
    {
        Instance = this;
    }

    public void IniciarPartidoDeTorneo(string equipoJugador, string equipoRival, bool jugadorEsLocal)
    {
        PartidoDeTorneo = true;
        EquipoJugador = equipoJugador;
        EquipoRival = equipoRival;
        JugadorEsLocal = jugadorEsLocal;
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
    }
}
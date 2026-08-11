using Godot;

/// <summary>
/// Patrón Singleton global (Autoload). Gestiona el estado de sesión del partido actual.
/// Desacopla la lógica de marcadores de la escena de Cancha, permitiendo que nodos UI (Hud) 
/// reaccionen al estado subyacente mediante el patrón Observador (Signals).
/// </summary>
public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }

    public int GolesEquipo1 = 0;
    public int GolesEquipo2 = 0;

    public bool JuegoPausado = false;
    public bool PartidoTerminado = false;

    // Emisión de evento bajo demanda para orquestar animaciones o actualizaciones en HUD.
    [Signal] public delegate void GolAnotadoEventHandler(int equipo);

    public override void _Ready()
    {
        Instance = this;
    }

    public void AnotarGol(int equipo)
    {
        if (equipo == 1) GolesEquipo1++;
        else GolesEquipo2++;

        EmitSignal(SignalName.GolAnotado, equipo);
        GD.Print($"¡Gooooool! Equipo {equipo} — Marcador: {GolesEquipo1} - {GolesEquipo2}");
    }

    // Inicializador fundamental al transitar entre escenas de torneos. Evita contaminación de estado.
    public void ReiniciarPartido()
    {
        GolesEquipo1 = 0;
        GolesEquipo2 = 0;
        PartidoTerminado = false;
    }
}
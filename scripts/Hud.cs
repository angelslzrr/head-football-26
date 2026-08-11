using Godot;
using System.Linq;

/// <summary>
/// Capa CanvasLayer de interfaz de usuario renderizada sobre la simulación física (Cancha).
/// Utiliza datos inyectados de PuenteTorneo para mostrar las iniciales y banderas oficiales de FIFA.
/// Interviene en el paso de estado del partido (finalización por temporizador).
/// </summary>
public partial class Hud : CanvasLayer
{
    [Export] public int DuracionPartidoSegundos = 45;

    private Label _labelGoles1;
    private Label _labelGoles2;
    private Label _labelTiempo;
    private Timer _timerPartido;
    private int _tiempoRestante;

    private AudioStreamPlayer _silbatoFinal;

    public override void _Ready()
    {
        GameManager.Instance.ReiniciarPartido();

        _labelGoles1 = GetNode<Label>("FondoMarcador/LabelGoles1");
        _labelGoles2 = GetNode<Label>("FondoMarcador/LabelGoles2");
        _labelTiempo = GetNode<Label>("FondoMarcador/LabelTiempo");
        _timerPartido = GetNode<Timer>("TimerPartido");

        TextureButton btnPausa = GetNode<TextureButton>("BtnPausa");
        btnPausa.Pressed += () => 
        {
            var menuPausa = GetParent().GetNodeOrNull<MenuPausaController>("MenuPausa");
            if (menuPausa != null)
            {
                menuPausa.AlternarPausa();
            }
        };

        _silbatoFinal = GetNode<AudioStreamPlayer>("SilbatoFinal");

        _tiempoRestante = DuracionPartidoSegundos;

        // Suscripción al bus de eventos de la instancia GameManager.
        GameManager.Instance.GolAnotado += OnGolAnotado;
        _timerPartido.Timeout += OnSegundoPasado;

        if (PuenteTorneo.Instance.PartidoDeTorneo)
        {
            ConfigurarDatosTorneo();
        }

        ActualizarMarcador();
        ActualizarTiempo();
    }

    // Patrón de limpieza de memoria: elimina listeners huérfanos para evitar fugas y excepciones.
    public override void _ExitTree()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GolAnotado -= OnGolAnotado;
        }
    }

    private void ConfigurarDatosTorneo()
    {
        var equipos = RepositorioEquipos.ObtenerEquiposConmebol();
        var equipoJugador = equipos.FirstOrDefault(e => e.TeamName == PuenteTorneo.Instance.EquipoJugador);
        var equipoRival = equipos.FirstOrDefault(e => e.TeamName == PuenteTorneo.Instance.EquipoRival);

        if (equipoRival != null)
        {
            GetNode<TextureRect>("FondoMarcador/Bandera1").Texture = equipoRival.FlagTexture;
            GetNode<Label>("FondoMarcador/NombreEquipo1").Text = equipoRival.FifaCode;
        }

        if (equipoJugador != null)
        {
            GetNode<TextureRect>("FondoMarcador/Bandera2").Texture = equipoJugador.FlagTexture;
            GetNode<Label>("FondoMarcador/NombreEquipo2").Text = equipoJugador.FifaCode;
        }
    }

    private void OnGolAnotado(int equipo)
    {
        ActualizarMarcador();
    }

    private async void OnSegundoPasado()
    {
        if (GameManager.Instance.PartidoTerminado) return;

        _tiempoRestante--;
        ActualizarTiempo();

        if (_tiempoRestante <= 0)
        {
            _timerPartido.Stop();
            GameManager.Instance.PartidoTerminado = true;

            _silbatoFinal.Play();
            GD.Print("¡Fin del partido!");

            if (PuenteTorneo.Instance.PartidoDeTorneo)
            {
                await ToSignal(GetTree().CreateTimer(4f), SceneTreeTimer.SignalName.Timeout);

                PuenteTorneo.Instance.GuardarResultado(
                    GameManager.Instance.GolesEquipo2, 
                    GameManager.Instance.GolesEquipo1  
                );
                
                GetTree().ChangeSceneToFile("res://escenas/UI/TournamentHub.tscn");
            }
        }
    }

    private void ActualizarMarcador()
    {
        _labelGoles1.Text = GameManager.Instance.GolesEquipo1.ToString();
        _labelGoles2.Text = GameManager.Instance.GolesEquipo2.ToString();
    }

    private void ActualizarTiempo()
    {
        int minutos = _tiempoRestante / 60;
        int segundos = _tiempoRestante % 60;
        _labelTiempo.Text = $"{minutos:0}:{segundos:00}";
    }
}
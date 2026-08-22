using Godot;
using System.Linq;

public partial class Hud : CanvasLayer
{
    [Export] public int DuracionPartidoSegundos = 45;

    private Label _labelGoles1;
    private Label _labelGoles2;
    private Label _labelTiempo;
    private Timer _timerPartido;
    private int _tiempoRestante;

    private AudioStreamPlayer _silbatoFinal;

    private bool _enGolDeOro = false;

    public override void _Ready()
    {
        GameManager.Instance.ReiniciarPartido();

        _labelGoles1 = GetNode<Label>("FondoMarcador/LabelGoles1");
        _labelGoles2 = GetNode<Label>("FondoMarcador/LabelGoles2");
        _labelTiempo = GetNode<Label>("FondoMarcador/LabelTiempo");
        _timerPartido = GetNode<Timer>("TimerPartido");

        if (_labelTiempo.LabelSettings != null)
        {
            _labelTiempo.LabelSettings.FontSize = 29;
        }

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
        _enGolDeOro = false;

        GameManager.Instance.GolAnotado += OnGolAnotado;
        _timerPartido.Timeout += OnSegundoPasado;

        if (PuenteTorneo.Instance.PartidoDeTorneo)
        {
            ConfigurarDatosTorneo();
        }

        ActualizarMarcador();
        ActualizarTiempo();
    }

    public override void _ExitTree()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GolAnotado -= OnGolAnotado;
        }
    }

    private void ConfigurarDatosTorneo()
    {
        var equipoJugador = RepositorioEquipos.BuscarEquipo(PuenteTorneo.Instance.EquipoJugador);
        var equipoRival = RepositorioEquipos.BuscarEquipo(PuenteTorneo.Instance.EquipoRival);

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

    private async void OnGolAnotado(int equipo)
    {
        ActualizarMarcador();

        if (!_enGolDeOro) return;

        GameManager.Instance.PartidoTerminado = true;
        _silbatoFinal.Play();

        GD.Print("¡GOL DE ORO! Partido decidido en muerte súbita.");

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

    private async void OnSegundoPasado()
    {
        if (GameManager.Instance.PartidoTerminado) return;
        if (_enGolDeOro) return; 

        _tiempoRestante--;
        ActualizarTiempo();

        if (_tiempoRestante <= 0)
        {
            bool hayEmpate;

            if (PuenteTorneo.Instance.EsPartidoDeVuelta)
            {
                int globalJugador = GameManager.Instance.GolesEquipo2 + PuenteTorneo.Instance.GolesGlobalPrevios;
                int globalRival = GameManager.Instance.GolesEquipo1 + PuenteTorneo.Instance.GolesGlobalPreviosRival;
                hayEmpate = globalJugador == globalRival;
            }
            else
            {
                hayEmpate = GameManager.Instance.GolesEquipo1 == GameManager.Instance.GolesEquipo2;
            }

            bool aplicaGolDeOro = PuenteTorneo.Instance.PartidoDeTorneo
                && PuenteTorneo.Instance.EsFaseEliminacion
                && hayEmpate;

            if (aplicaGolDeOro)
            {
                IniciarGolDeOro();
                return;
            }

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

    private void IniciarGolDeOro()
    {
        _enGolDeOro = true;
        _timerPartido.Stop();

        _labelTiempo.Text = "GOL DE ORO";

        if (_labelTiempo.LabelSettings != null)
        {
            _labelTiempo.LabelSettings.FontSize = 20;
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
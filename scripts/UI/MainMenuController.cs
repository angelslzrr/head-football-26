using Godot;

/// <summary>
/// Controlador principal de la interfaz del Menú Principal.
/// Gestiona la navegación hacia las distintas modalidades de juego, la carga
/// de partidas guardadas, la salida de la aplicación y el sistema de control de audio persistente.
/// </summary>
public partial class MainMenuController : Control
{
    private const string RutaPartidoRapido = "res://escenas/cancha.tscn";
    private const string RutaSeleccionEquipos = "res://escenas/UI/SelectionMenu.tscn";
    private const string RutaTournamentHub = "res://escenas/UI/TournamentHub.tscn";

    private Button _btnPartidoRapido;
    private Button _btnMundial2026;
    private Button _btnSalir;

    private Control _overlayPartidaEnCurso;
    private Label _labelInfoPartida;
    private Button _btnContinuar;
    private Button _btnNuevoCampeonato;

    // Componentes del control de volumen flotante y persistencia de audio.
    private TextureButton _btnVolumenPopup;
    private PanelContainer _panelVolumen;
    private VSlider _sliderVolumen;
    private ConfigFile _config = new ConfigFile();
    private const string RUTA_CONFIG = "user://configuracion.cfg";

    public override void _Ready()
    {
        // Asegura la reproducción continua de la música global del menú si no está activa.
        var musica = GetNode<AudioStreamPlayer>("/root/MusicaGlobal");
        if (!musica.Playing) musica.Play();

        _btnPartidoRapido = GetNode<Button>("PanelBotones/BtnPartidoRapido");
        _btnMundial2026 = GetNode<Button>("PanelBotones/BtnMundial2026");
        _btnSalir = GetNode<Button>("PanelBotones/BtnSalir");

        _overlayPartidaEnCurso = GetNode<Control>("OverlayPartidaEnCurso");
        _labelInfoPartida = GetNode<Label>("OverlayPartidaEnCurso/CentroTarjeta/TarjetaPartida/ContenidoTarjeta/LabelInfo");
        _btnContinuar = GetNode<Button>("OverlayPartidaEnCurso/CentroTarjeta/TarjetaPartida/ContenidoTarjeta/Botones/BtnContinuar");
        _btnNuevoCampeonato = GetNode<Button>("OverlayPartidaEnCurso/CentroTarjeta/TarjetaPartida/ContenidoTarjeta/Botones/BtnNuevoCampeonato");

        _btnVolumenPopup = GetNode<TextureButton>("BtnVolumenPopup");
        _panelVolumen = GetNode<PanelContainer>("PanelVolumen");
        _sliderVolumen = GetNode<VSlider>("PanelVolumen/SliderVolumen");

        _btnPartidoRapido.Pressed += OnPartidoRapidoPresionado;
        _btnMundial2026.Pressed += OnMundial2026Presionado;
        _btnSalir.Pressed += OnSalirPresionado;

        _btnContinuar.Pressed += OnContinuarPresionado;
        _btnNuevoCampeonato.Pressed += OnNuevoCampeonatoPresionado;

        _btnVolumenPopup.Pressed += () => _panelVolumen.Visible = !_panelVolumen.Visible;
        _sliderVolumen.ValueChanged += OnVolumenCambiado;

        _overlayPartidaEnCurso.Visible = false;

        // Carga la configuración previa de audio guardada en disco por el usuario.
        if (_config.Load(RUTA_CONFIG) == Error.Ok)
        {
            float volumenGuardado = (float)_config.GetValue("Audio", "VolumenMaster", 0.5f);
            _sliderVolumen.Value = volumenGuardado;
            AplicarVolumen(volumenGuardado);
        }
        else
        {
            AplicarVolumen((float)_sliderVolumen.Value);
        }
    }

    private void OnMundial2026Presionado()
    {
        // Intercepta el flujo si existe una partida guardada activa para ofrecer continuarla.
        if (GestorGuardado.Instance.ExisteGuardado())
        {
            TournamentState estado = GestorGuardado.Instance.CargarTorneo();
            if (estado != null)
            {
                // Obtenemos el nombre de la fase actual para que sirva en cualquier confederación
                string textoFase = estado.FaseActual != null ? estado.FaseActual.Nombre : "Torneo Finalizado";
                
                _labelInfoPartida.Text = $"{estado.NombreEquipoJugador} — {textoFase}";
                _overlayPartidaEnCurso.Visible = true;
                return; 
            }
        }
        GetTree().ChangeSceneToFile(RutaSeleccionEquipos);
    }

    private void OnContinuarPresionado()
    {
        _overlayPartidaEnCurso.Visible = false;
        GetTree().ChangeSceneToFile(RutaTournamentHub);
    }

    private void OnNuevoCampeonatoPresionado()
    {
        _overlayPartidaEnCurso.Visible = false;
        GetTree().ChangeSceneToFile(RutaSeleccionEquipos);
    }

    private void OnPartidoRapidoPresionado()
    {
        GetTree().ChangeSceneToFile(RutaPartidoRapido);
    }

    private void OnSalirPresionado()
    {
        GetTree().Quit();
    }

    private void OnVolumenCambiado(double valorLineal)
    {
        AplicarVolumen((float)valorLineal);
        
        _config.SetValue("Audio", "VolumenMaster", valorLineal);
        _config.Save(RUTA_CONFIG);
    }

    // Convierte el valor lineal del slider a decibelios compatibles con el servidor de audio de Godot.
    private void AplicarVolumen(float valorLineal)
    {
        int indiceBus = AudioServer.GetBusIndex("Master");
        
        if (valorLineal == 0)
        {
            AudioServer.SetBusMute(indiceBus, true);
        }
        else
        {
            AudioServer.SetBusMute(indiceBus, false);
            AudioServer.SetBusVolumeDb(indiceBus, Mathf.LinearToDb(valorLineal));
        }
    }
}
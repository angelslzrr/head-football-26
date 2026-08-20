using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class TournamentHubController : Control
{
    private const string RutaMainMenu = "res://escenas/UI/MainMenu.tscn";
    private const string RutaCancha = "res://escenas/cancha.tscn";

    private Label _titulo;

    private Control _panelDetallesEquipo;
    private TextureRect _mapaRegionFondo;
    private TextureRect _previewBandera;
    private Label _previewNombre;
    private HBoxContainer _previewEstrellas;
    private TextureRect _previewCabeza;
    private TextureRect _previewCamiseta;
    private Texture2D _texEstrellaLlena;
    private Texture2D _texEstrellaMedia;

    private VBoxContainer _contenedorPosiciones;
    private VBoxContainer _listaFixture;
    private Label _mensajePartidoJugador;
    private Button _btnSimularJornada;
    private Button _btnVolver;
    private Button _btnVerMundo;

    private Control _panelPrevia;
    private TextureRect _cabezaLocal;
    private TextureRect _cabezaVisitante;
    private Label _nombresPrevia;

    private TournamentState _estado;
    private List<TeamData> _equipos;
    private IRenderizadorFase _renderizadorActual;

    private TabContainer _pestanas;
    private AcceptDialog _dialogoRepechaje;

    private OptionButton _dropdownHistorial;
    private int _indiceFaseVisualizada = 0;

    public override void _Ready()
    {
        var musica = GetNode<AudioStreamPlayer>("/root/MusicaGlobal");
        if (!musica.Playing) musica.Play();

        _titulo = GetNode<Label>("Layout/Titulo");
        _pestanas = GetNode<TabContainer>("Layout/Pestanas");
        _dialogoRepechaje = GetNode<AcceptDialog>("DialogoRepechaje");
        _dropdownHistorial = GetNode<OptionButton>("Layout/DropdownHistorial");

        _panelDetallesEquipo = GetNode<Control>("Layout/Pestanas/Posiciones/ContenedorPosiciones/PanelDetallesEquipo");
        _mapaRegionFondo = GetNode<TextureRect>("Layout/Pestanas/Posiciones/ContenedorPosiciones/PanelDetallesEquipo/MapaRegionFondo");
        _previewBandera = GetNode<TextureRect>("Layout/Pestanas/Posiciones/ContenedorPosiciones/PanelDetallesEquipo/ContenidoDetalles/PreviewBandera");
        _previewNombre = GetNode<Label>("Layout/Pestanas/Posiciones/ContenedorPosiciones/PanelDetallesEquipo/ContenidoDetalles/PreviewNombre");
        _previewEstrellas = GetNode<HBoxContainer>("Layout/Pestanas/Posiciones/ContenedorPosiciones/PanelDetallesEquipo/ContenidoDetalles/PreviewEstrellas");
        _previewCabeza = GetNode<TextureRect>("Layout/Pestanas/Posiciones/ContenedorPosiciones/PanelDetallesEquipo/ContenidoDetalles/ComposicionSprite/PreviewCabeza");
        _previewCamiseta = GetNode<TextureRect>("Layout/Pestanas/Posiciones/ContenedorPosiciones/PanelDetallesEquipo/ContenidoDetalles/ComposicionSprite/PreviewCamiseta");

        _texEstrellaLlena = GD.Load<Texture2D>("res://img/estrellaCompleta.png");
        _texEstrellaMedia = GD.Load<Texture2D>("res://img/estrellaMitad.png");

        _contenedorPosiciones = GetNode<VBoxContainer>("Layout/Pestanas/Posiciones/ContenedorPosiciones/FondoTabla/ScrollPosiciones/ContenedorDinamicoPosiciones");
        _listaFixture = GetNode<VBoxContainer>("Layout/Pestanas/Fixture/ContenedorFixture/FondoFixture/ScrollFixture/ListaFixture");

        _panelPrevia = GetNode<Control>("Layout/Pestanas/Fixture/ContenedorFixture/PanelPrevia");
        _cabezaLocal = GetNode<TextureRect>("Layout/Pestanas/Fixture/ContenedorFixture/PanelPrevia/ContenidoPrevia/DueloCabezones/CabezaLocal");
        _cabezaVisitante = GetNode<TextureRect>("Layout/Pestanas/Fixture/ContenedorFixture/PanelPrevia/ContenidoPrevia/DueloCabezones/CabezaVisitante");
        _nombresPrevia = GetNode<Label>("Layout/Pestanas/Fixture/ContenedorFixture/PanelPrevia/ContenidoPrevia/NombresPrevia");

        _mensajePartidoJugador = GetNode<Label>("Layout/MensajePartidoJugador");
        _btnSimularJornada = GetNode<Button>("Layout/BarraInferior/BtnSimularJornada");
        _btnVolver = GetNode<Button>("Layout/BarraInferior/BtnVolver");
        _btnVerMundo = GetNode<Button>("Layout/BarraInferior/BtnVerMundo");
        
        _btnSimularJornada.Pressed += AvanzarSimulacion;
        _btnVolver.Pressed += () => GetTree().ChangeSceneToFile(RutaMainMenu);
        _btnVerMundo.Pressed += () => GetTree().ChangeSceneToFile("res://escenas/UI/GlobalHub.tscn");

        _estado = GestorGuardado.Instance.CargarTorneo();
        if (_estado == null)
        {
            GD.PrintErr("No hay ningún torneo guardado. Volviendo al menú.");
            GetTree().ChangeSceneToFile(RutaMainMenu);
            return;
        }

        _equipos = RepositorioEquipos.ObtenerEquiposPorRegion(_estado.Region);

        if (PuenteTorneo.Instance.PartidoDeTorneo)
        {
            ResolverPartidoJugador();
            PuenteTorneo.Instance.FinalizarPartidoDeTorneo();
        }

        if (_estado.Fases == null || _estado.Fases.Count == 0)
        {
            var fases = RepositorioFormatos.ObtenerFormatoPorRegion(_estado.Region);
            var nombresEquipos = _equipos.Select(e => e.TeamName).ToList();
            GestorTorneo.IniciarTorneo(_estado, fases, nombresEquipos);
            GestorGuardado.Instance.GuardarTorneo(_estado);
        }

        CargarPanelJugador();
        SincronizarDropdownConPresente();
        _dropdownHistorial.ItemSelected += OnFaseHistorialSeleccionada;
        RedibujarTodo();
    }

    private bool EsEquipoDelJugador(string nombreEquipo) => nombreEquipo == _estado.NombreEquipoJugador;

    private void ResolverPartidoJugador()
    {
        bool jugadorEsLocal = PuenteTorneo.Instance.JugadorEsLocal;
        int golesLocal = jugadorEsLocal ? PuenteTorneo.Instance.GolesJugador : PuenteTorneo.Instance.GolesRival;
        int golesVisitante = jugadorEsLocal ? PuenteTorneo.Instance.GolesRival : PuenteTorneo.Instance.GolesJugador;

        string local = jugadorEsLocal ? _estado.NombreEquipoJugador : PuenteTorneo.Instance.EquipoRival;
        string visitante = jugadorEsLocal ? PuenteTorneo.Instance.EquipoRival : _estado.NombreEquipoJugador;

        FaseTorneo faseDelPartido = _estado.FaseActual; 

        GestorTorneo.ProcesarResultado(_estado, local, visitante, golesLocal, golesVisitante);

        DetectarEliminacionInmediata(faseDelPartido);

        GestorTorneo.AvanzarFaseSiCorresponde(_estado);
        DetectarEliminacionPorClasificacion(faseDelPartido);

        GestorGuardado.Instance.GuardarTorneo(_estado);
    }

    private void RedibujarTodo()
    {
        FaseTorneo faseAMostrar = FaseVisualizada;

        if (faseAMostrar == null)
        {
            foreach (Node hijo in _contenedorPosiciones.GetChildren()) hijo.QueueFree();
            foreach (Node hijo in _listaFixture.GetChildren()) hijo.QueueFree();
            _titulo.Text = $"{_estado.Region} — Torneo finalizado";
            ActualizarInterfazFase();
            _panelPrevia.Visible = false;
            return;
        }

        _renderizadorActual = RenderizadorFactory.ObtenerRenderizador(faseAMostrar.Tipo);
        _panelDetallesEquipo.Visible = !_renderizadorActual.OcultaPanelDetalleEquipo;
        AplicarVisibilidadPestanas(faseAMostrar.Tipo);

        _renderizadorActual.DibujarPosiciones(_contenedorPosiciones, faseAMostrar, _estado.NombreEquipoJugador);
        _renderizadorActual.DibujarFixture(_listaFixture, faseAMostrar, _estado.NombreEquipoJugador);

        string sufijo = EstaViendoPresente() ? "" : "  (Historial)";
        _titulo.Text = $"{_estado.Region} — {faseAMostrar.Nombre}{sufijo}";

        ActualizarInterfazFase();
        ActualizarPanelPrevia();
        DetectarYMostrarRepechaje();
        SimularMundoSiCorresponde();
        _btnVerMundo.Visible = _estado.MundoSimulado;
    }

    private void ActualizarInterfazFase()
    {
        if (!EstaViendoPresente())
        {
            _btnSimularJornada.Visible = false;
            _mensajePartidoJugador.Visible = true;
            _mensajePartidoJugador.Text = "Estás viendo el historial de una fase anterior. " +
                                           "Selecciona la fase (Actual) en el menú para volver a jugar.";
            return;
        }

        _btnSimularJornada.Visible = true;

        FaseTorneo fase = _estado.FaseActual;

        if (fase == null || EstaTorneoFinalizado())
        {
            _btnSimularJornada.Disabled = true;
            _btnSimularJornada.Text = "Torneo Finalizado";
            _mensajePartidoJugador.Visible = true;
            _mensajePartidoJugador.Text = _estado.JugadorEliminado
                ? "El torneo terminó. Revisa el árbol para ver quién se coronó campeón."
                : "¡El torneo ha concluido!";
            return;
        }

        if (_estado.JugadorEliminado)
        {
            _btnSimularJornada.Disabled = false;
            _btnSimularJornada.Text = "Simular resto del torneo";
            _mensajePartidoJugador.Visible = true;
            _mensajePartidoJugador.Text = "Fuiste eliminado de la competencia. Simula el resto para ver quién gana.";
            return;
        }

        _btnSimularJornada.Disabled = false;

        var unidadPendiente = GestorTorneo.ObtenerUnidadPendiente(fase);
        var partidoJugador = unidadPendiente.FirstOrDefault(p => EsEquipoDelJugador(p.Local) || EsEquipoDelJugador(p.Visitante));

        if (partidoJugador != default)
        {
            _btnSimularJornada.Text = "Jugar partido";
            _mensajePartidoJugador.Visible = true;
            _mensajePartidoJugador.Text = $"Te toca jugar: {partidoJugador.Local} vs {partidoJugador.Visitante}.";
        }
        else
        {
            _btnSimularJornada.Text = fase.Tipo == TipoFormato.Eliminacion ? "Simular ronda" : "Simular jornada";
            _mensajePartidoJugador.Visible = false;
        }
    }

    private void AvanzarSimulacion()
    {
        if (_estado.JugadorEliminado)
        {
            SimularRestoDelTorneo();
            return;
        }

        FaseTorneo fase = _estado.FaseActual;
        if (fase == null) return;

        var unidadPendiente = GestorTorneo.ObtenerUnidadPendiente(fase);
        var partidoJugador = unidadPendiente.FirstOrDefault(p => EsEquipoDelJugador(p.Local) || EsEquipoDelJugador(p.Visitante));

        if (partidoJugador != default)
        {
            bool jugadorEsLocal = EsEquipoDelJugador(partidoJugador.Local);
            string rival = jugadorEsLocal ? partidoJugador.Visitante : partidoJugador.Local;

            foreach (var partido in unidadPendiente)
            {
                if (EsEquipoDelJugador(partido.Local) || EsEquipoDelJugador(partido.Visitante)) continue;
                SimularYRegistrar(partido.Local, partido.Visitante);
            }
            GestorGuardado.Instance.GuardarTorneo(_estado);

            bool esEliminacion = fase.Tipo == TipoFormato.Eliminacion;
            bool esPartidoDeVuelta = false;
            int golesGlobalJugador = 0;
            int golesGlobalRival = 0;

            if (esEliminacion)
            {
                LlaveEliminacion llaveJugador = fase.Llaves.FirstOrDefault(l =>
                    !l.Jugado &&
                    ((l.EquipoLocal == _estado.NombreEquipoJugador && l.EquipoVisitante == rival) ||
                     (l.EquipoLocal == rival && l.EquipoVisitante == _estado.NombreEquipoJugador)));

                if (llaveJugador != null && llaveJugador.IdaYVuelta && llaveJugador.JugadoIda)
                {
                    esPartidoDeVuelta = true;
                    bool jugadorEsLocalOriginal = llaveJugador.EquipoLocal == _estado.NombreEquipoJugador;
                    golesGlobalJugador = jugadorEsLocalOriginal ? llaveJugador.GolesGlobalLocal : llaveJugador.GolesGlobalVisitante;
                    golesGlobalRival = jugadorEsLocalOriginal ? llaveJugador.GolesGlobalVisitante : llaveJugador.GolesGlobalLocal;
                }
            }

            PuenteTorneo.Instance.IniciarPartidoDeTorneo(
                _estado.NombreEquipoJugador, rival, jugadorEsLocal, esEliminacion,
                esPartidoDeVuelta, golesGlobalJugador, golesGlobalRival);

            GetTree().ChangeSceneToFile(RutaCancha);
            return;
        }

        foreach (var partido in unidadPendiente)
            SimularYRegistrar(partido.Local, partido.Visitante);

        FaseTorneo faseAntesDeAvanzar = fase;
        GestorTorneo.AvanzarFaseSiCorresponde(_estado);
        DetectarEliminacionPorClasificacion(faseAntesDeAvanzar);

        GestorGuardado.Instance.GuardarTorneo(_estado);
        SincronizarDropdownConPresente();
        RedibujarTodo();
    }

    private void SimularYRegistrar(string local, string visitante)
    {
        float estrellasLocal = ObtenerEstrellas(local);
        float estrellasVisitante = ObtenerEstrellas(visitante);
        (int golesLocal, int golesVisitante) = SimulationEngine.SimularPartido(estrellasLocal, estrellasVisitante);

        GestorTorneo.ProcesarResultado(_estado, local, visitante, golesLocal, golesVisitante);
    }

    private float ObtenerEstrellas(string nombreEquipo)
    {
        TeamData equipo = _equipos.FirstOrDefault(e => e.TeamName == nombreEquipo);
        return equipo?.StarRating ?? 3.0f;
    }

    private void CargarPanelJugador()
    {
        TeamData miEquipo = _equipos.FirstOrDefault(e => e.TeamName == _estado.NombreEquipoJugador);
        if (miEquipo == null) return;

        _previewNombre.Text = miEquipo.TeamName;
        _previewBandera.Texture = miEquipo.FlagTexture;
        _previewCabeza.Texture = miEquipo.CabezaTexture;
        _previewCamiseta.Texture = miEquipo.CamisetaTexture;

        string rutaMapa = _estado.Region switch
        {
            "Sudamérica" => "res://img/mapas/mapaConmebol.png",
            "Oceania" => "res://img/mapas/mapaOfc.png",
            _ => null
        };
        _mapaRegionFondo.Texture = (rutaMapa != null && ResourceLoader.Exists(rutaMapa)) ? GD.Load<Texture2D>(rutaMapa) : null;

        foreach (Node hijo in _previewEstrellas.GetChildren()) hijo.QueueFree();

        for (int i = 1; i <= 5; i++)
        {
            Control slot = new Control { CustomMinimumSize = new Vector2(24, 24) };

            if (miEquipo.StarRating >= i)
                slot.AddChild(CrearCapaEstrella(_texEstrellaLlena, Colors.White));
            else if (miEquipo.StarRating >= i - 0.5f)
            {
                slot.AddChild(CrearCapaEstrella(_texEstrellaLlena, new Color(0, 0, 0, 0.4f)));
                slot.AddChild(CrearCapaEstrella(_texEstrellaMedia, Colors.White));
            }
            else
                slot.AddChild(CrearCapaEstrella(_texEstrellaLlena, new Color(0, 0, 0, 0.4f)));

            _previewEstrellas.AddChild(slot);
        }
    }

    private TextureRect CrearCapaEstrella(Texture2D textura, Color colorFiltro)
    {
        return new TextureRect
        {
            Texture = textura,
            CustomMinimumSize = new Vector2(24, 24),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            Modulate = colorFiltro,
            LayoutMode = 1,
            AnchorsPreset = (int)Control.LayoutPreset.FullRect
        };
    }

    private void ActualizarPanelPrevia()
    {
        if (!EstaViendoPresente()) { _panelPrevia.Visible = false; return; }

        FaseTorneo fase = _estado.FaseActual;
        if (fase == null) { _panelPrevia.Visible = false; return; }

        var pendientes = GestorTorneo.ObtenerPartidosPendientes(fase);
        var proximo = pendientes.FirstOrDefault(p => EsEquipoDelJugador(p.Local) || EsEquipoDelJugador(p.Visitante));

        if (proximo == default)
        {
            _panelPrevia.Visible = false;
            return;
        }

        _panelPrevia.Visible = true;
        _nombresPrevia.Text = $"{proximo.Local} vs {proximo.Visitante}";

        TeamData equipoLocal = _equipos.FirstOrDefault(e => e.TeamName == proximo.Local);
        TeamData equipoVisitante = _equipos.FirstOrDefault(e => e.TeamName == proximo.Visitante);

        _cabezaLocal.Texture = equipoLocal?.CabezaTexture;
        _cabezaVisitante.Texture = equipoVisitante?.CabezaTexture;
    }

    private void DetectarEliminacionInmediata(FaseTorneo faseDelPartido)
    {
        if (faseDelPartido == null || faseDelPartido.Tipo != TipoFormato.Eliminacion) return;

        LlaveEliminacion llaveDelJugador = faseDelPartido.Llaves.FirstOrDefault(l =>
            l.EquipoLocal == _estado.NombreEquipoJugador || l.EquipoVisitante == _estado.NombreEquipoJugador);

        if (llaveDelJugador == null || !llaveDelJugador.Jugado) return; // Si falta la vuelta, sigue vivo

        if (llaveDelJugador.Ganador != _estado.NombreEquipoJugador)
        {
            _estado.JugadorEliminado = true;
        }
    }

    private void DetectarEliminacionPorClasificacion(FaseTorneo faseAntesDeAvanzar)
    {
        if (_estado.JugadorEliminado) return;
        if (faseAntesDeAvanzar == null || !faseAntesDeAvanzar.Completada) return;
        if (faseAntesDeAvanzar.Tipo == TipoFormato.Eliminacion) return; 

        bool sigueVivo = _estado.FaseActual != null
            && EquipoEstaEnFase(_estado.NombreEquipoJugador, _estado.FaseActual);

        if (!sigueVivo)
        {
            _estado.JugadorEliminado = true;
        }
    }

    private bool EquipoEstaEnFase(string equipo, FaseTorneo fase)
    {
        return fase.Tipo switch
        {
            TipoFormato.RoundRobin => fase.TablaPosiciones.Any(e => e.NombreEquipo == equipo),
            TipoFormato.Grupos => fase.Grupos.Any(g => g.Equipos.Contains(equipo)),
            TipoFormato.Eliminacion => fase.Llaves.Any(l => l.EquipoLocal == equipo || l.EquipoVisitante == equipo),
            _ => false
        };
    }

    private bool EstaTorneoFinalizado()
    {
        return _estado.FaseActual != null
            && _estado.FaseActual.Completada
            && _estado.FaseActualIndice == _estado.Fases.Count - 1;
    }

    private void SimularRestoDelTorneo()
    {
        while (!EstaTorneoFinalizado())
        {
            FaseTorneo faseActual = _estado.FaseActual;
            if (faseActual == null) break; 

            var pendientes = GestorTorneo.ObtenerUnidadPendiente(faseActual);

            if (pendientes.Count == 0)
            {
                if (!GestorTorneo.AvanzarFaseSiCorresponde(_estado)) break;
                continue;
            }

            foreach (var partido in pendientes)
            {
                SimularYRegistrar(partido.Local, partido.Visitante);
            }

            GestorTorneo.AvanzarFaseSiCorresponde(_estado);
        }

        GestorGuardado.Instance.GuardarTorneo(_estado);
        SincronizarDropdownConPresente(); 
        RedibujarTodo();
    }

    private void AplicarVisibilidadPestanas(TipoFormato tipo)
    {
        bool esEliminacion = tipo == TipoFormato.Eliminacion;

        _pestanas.TabsVisible = !esEliminacion;
        _pestanas.SetTabTitle(0, esEliminacion ? "Llaves" : "Posiciones");
        _pestanas.SetTabTitle(1, "Fixture");

        if (esEliminacion)
        {
            _pestanas.CurrentTab = 0;
        }
    }

    private void DetectarYMostrarRepechaje()
    {
        if (_estado.RepechajeMostrado) return;
        if (_estado.Region != "Oceania") return;
        if (!EstaTorneoFinalizado()) return;

        FaseTorneo faseFinal = _estado.FaseActual;
        if (faseFinal == null || faseFinal.Tipo != TipoFormato.Eliminacion) return;
        if (faseFinal.Llaves.Count == 0) return;

        int rondaMaxima = faseFinal.Llaves.Max(l => l.Ronda);
        LlaveEliminacion final = faseFinal.Llaves.FirstOrDefault(l => l.Ronda == rondaMaxima);
        if (final == null || !final.Jugado) return;

        bool jugadorEnLaFinal = EsEquipoDelJugador(final.EquipoLocal) || EsEquipoDelJugador(final.EquipoVisitante);
        bool jugadorFueSubcampeon = jugadorEnLaFinal && !EsEquipoDelJugador(final.Ganador);

        if (jugadorFueSubcampeon)
        {
            _dialogoRepechaje.DialogText = $"{_estado.NombreEquipoJugador} terminó subcampeón de Oceanía.\n" +
                                            "¡Clasificaste al Repechaje Intercontinental!";
            _dialogoRepechaje.PopupCentered();
        }

        _estado.RepechajeMostrado = true;
        GestorGuardado.Instance.GuardarTorneo(_estado);
    }

    private FaseTorneo FaseVisualizada =>
        (_indiceFaseVisualizada >= 0 && _indiceFaseVisualizada < _estado.Fases.Count)
            ? _estado.Fases[_indiceFaseVisualizada]
            : null;

    private bool EstaViendoPresente() => _indiceFaseVisualizada == _estado.FaseActualIndice;

    private void PoblarDropdownHistorial()
    {
        _dropdownHistorial.Clear();

        int maximoIndice = Mathf.Min(_estado.FaseActualIndice, _estado.Fases.Count - 1);

        for (int i = 0; i <= maximoIndice; i++)
        {
            string etiqueta = _estado.Fases[i].Nombre;
            if (i == _estado.FaseActualIndice) etiqueta += "  (Actual)";
            _dropdownHistorial.AddItem(etiqueta);
        }

        _dropdownHistorial.Select(_indiceFaseVisualizada);
    }

    private void SincronizarDropdownConPresente()
    {
        _indiceFaseVisualizada = _estado.FaseActualIndice;
        PoblarDropdownHistorial();
    }

    private void OnFaseHistorialSeleccionada(long indice)
    {
        _indiceFaseVisualizada = (int)indice;
        RedibujarTodo();
    }

    private void SimularMundoSiCorresponde()
    {
        if (_estado.MundoSimulado) return;
        if (!EstaTorneoFinalizado()) return;

        // 🔑 El cambio clave: tomamos TODAS las regiones y le restamos la del jugador.
        List<string> regionesASimular = RepositorioFormatos.TodasLasRegiones
            .Except(new[] { _estado.Region })
            .ToList();

        foreach (string regionRestante in regionesASimular)
        {
            List<FaseTorneo> fasesRestoDelMundo = RepositorioFormatos.ObtenerFormatoPorRegion(regionRestante);
            List<TeamData> equiposRestoDelMundo = RepositorioEquipos.ObtenerEquiposPorRegion(regionRestante);
            List<string> nombresRestoDelMundo = equiposRestoDelMundo.Select(e => e.TeamName).ToList();

            var estadoTemporal = new TournamentState();
            GestorTorneo.IniciarTorneo(estadoTemporal, fasesRestoDelMundo, nombresRestoDelMundo);
            GestorTorneo.SimularTorneoCompleto(estadoTemporal, equiposRestoDelMundo);

            _estado.RestoDelMundo.Add(new EliminatoriaRegion
            {
                Region = regionRestante,
                Fases = estadoTemporal.Fases
            });

            GD.Print($"🌍 Macro-simulación completada para: {regionRestante}");
        }

        _estado.MundoSimulado = true;
        GestorGuardado.Instance.GuardarTorneo(_estado);
    }
}
using Godot;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Controlador central del centro de torneos (TournamentHub).
/// Administra la visualización de la tabla de posiciones con cebra y resaltados,
/// el calendario de partidos (Fixture), la simulación automatizada de jornadas, 
/// la tarjeta de previa de partidos y la sincronización con el resultado del gameplay.
/// </summary>
public partial class TournamentHubController : Control
{
    private const string RutaMainMenu = "res://escenas/UI/MainMenu.tscn";
    private const string RutaCancha = "res://escenas/cancha.tscn";

    private static readonly Color ColorFilaPar = new Color(0.06f, 0.12f, 0.07f, 0.5f);
    private static readonly Color ColorFilaImpar = new Color(0.03f, 0.07f, 0.04f, 0.5f);
    private static readonly Color ColorFilaJugador = new Color(0.87f, 0.73f, 0f, 0.16f);
    private static readonly Color ColorBordeJugador = new Color(0.9f, 0.78f, 0.15f, 1f);
    private static readonly Color ColorEncabezado = new Color(0.9f, 0.78f, 0.15f, 1f);
    private static readonly Color ColorTransparente = new Color(0, 0, 0, 0);

    private TextureRect _mapaRegionFondo;
    private TextureRect _previewBandera;
    private Label _previewNombre;
    private HBoxContainer _previewEstrellas;
    private TextureRect _previewCabeza;
    private TextureRect _previewCamiseta;
    private Texture2D _texEstrellaLlena;
    private Texture2D _texEstrellaMedia;

    private Label _titulo;
    private GridContainer _gridPosiciones;
    private VBoxContainer _listaFixture;
    private Label _mensajePartidoJugador;
    private Button _btnSimularJornada;
    private Button _btnVolver;

    private Control _panelPrevia;
    private TextureRect _cabezaLocal;
    private TextureRect _cabezaVisitante;
    private Label _nombresPrevia;

    private TournamentState _estado;
    private List<TeamData> _equipos;

    public override void _Ready()
    {
        var musica = GetNode<AudioStreamPlayer>("/root/MusicaGlobal");
        if (!musica.Playing) musica.Play();
        _titulo = GetNode<Label>("Layout/Titulo");
        
        _mapaRegionFondo = GetNode<TextureRect>("Layout/Pestanas/Posiciones/ContenedorPosiciones/PanelDetallesEquipo/MapaRegionFondo");
        _previewBandera = GetNode<TextureRect>("Layout/Pestanas/Posiciones/ContenedorPosiciones/PanelDetallesEquipo/ContenidoDetalles/PreviewBandera");
        _previewNombre = GetNode<Label>("Layout/Pestanas/Posiciones/ContenedorPosiciones/PanelDetallesEquipo/ContenidoDetalles/PreviewNombre");
        _previewEstrellas = GetNode<HBoxContainer>("Layout/Pestanas/Posiciones/ContenedorPosiciones/PanelDetallesEquipo/ContenidoDetalles/PreviewEstrellas");
        _previewCabeza = GetNode<TextureRect>("Layout/Pestanas/Posiciones/ContenedorPosiciones/PanelDetallesEquipo/ContenidoDetalles/ComposicionSprite/PreviewCabeza");
        _previewCamiseta = GetNode<TextureRect>("Layout/Pestanas/Posiciones/ContenedorPosiciones/PanelDetallesEquipo/ContenidoDetalles/ComposicionSprite/PreviewCamiseta");

        _texEstrellaLlena = GD.Load<Texture2D>("res://img/estrellaCompleta.png");
        _texEstrellaMedia = GD.Load<Texture2D>("res://img/estrellaMitad.png");

        _gridPosiciones = GetNode<GridContainer>("Layout/Pestanas/Posiciones/ContenedorPosiciones/FondoTabla/ScrollPosiciones/GridPosiciones");
        _listaFixture = GetNode<VBoxContainer>("Layout/Pestanas/Fixture/ContenedorFixture/FondoFixture/ScrollFixture/ListaFixture");
        
        _panelPrevia = GetNode<Control>("Layout/Pestanas/Fixture/ContenedorFixture/PanelPrevia");
        _cabezaLocal = GetNode<TextureRect>("Layout/Pestanas/Fixture/ContenedorFixture/PanelPrevia/ContenidoPrevia/DueloCabezones/CabezaLocal");
        _cabezaVisitante = GetNode<TextureRect>("Layout/Pestanas/Fixture/ContenedorFixture/PanelPrevia/ContenidoPrevia/DueloCabezones/CabezaVisitante");
        _nombresPrevia = GetNode<Label>("Layout/Pestanas/Fixture/ContenedorFixture/PanelPrevia/ContenidoPrevia/NombresPrevia");
        
        _mensajePartidoJugador = GetNode<Label>("Layout/MensajePartidoJugador");
        _btnSimularJornada = GetNode<Button>("Layout/BarraInferior/BtnSimularJornada");
        _btnVolver = GetNode<Button>("Layout/BarraInferior/BtnVolver");

        _btnSimularJornada.Pressed += SimularJornadaActual;
        _btnVolver.Pressed += () => GetTree().ChangeSceneToFile(RutaMainMenu);

        _equipos = RepositorioEquipos.ObtenerEquiposConmebol();
        _estado = GestorGuardado.Instance.CargarTorneo();

        if (_estado == null)
        {
            GD.PrintErr("No hay ningún torneo guardado. Volviendo al menú.");
            GetTree().ChangeSceneToFile(RutaMainMenu);
            return;
        }

        // Procesa el resultado si el jugador viene de disputar un partido en la escena de Cancha.
        if (PuenteTorneo.Instance.PartidoDeTorneo)
        {
            bool jugadorEsLocal = PuenteTorneo.Instance.JugadorEsLocal;
            int golesLocal = jugadorEsLocal ? PuenteTorneo.Instance.GolesJugador : PuenteTorneo.Instance.GolesRival;
            int golesVisitante = jugadorEsLocal ? PuenteTorneo.Instance.GolesRival : PuenteTorneo.Instance.GolesJugador;

            ResolverPartidoJugador(golesLocal, golesVisitante);
            PuenteTorneo.Instance.FinalizarPartidoDeTorneo();
        }

        _titulo.Text = $"Eliminatoria — {_estado.Region}";

        // Genera el fixture y la tabla inicial si es un torneo completamente nuevo.
        if (_estado.Calendario.Count == 0)
        {
            List<string> nombres = _equipos.Select(e => e.TeamName).ToList();
            _estado.Calendario = GeneradorFixture.GenerarFixtureIdaYVuelta(nombres);
            _estado.TablaPosiciones = nombres
                .Select(nombre => new EstadisticasEquipoGuardado { NombreEquipo = nombre })
                .ToList();

            GestorGuardado.Instance.GuardarTorneo(_estado);
        }

        CargarPanelJugador();
        DibujarTablaPosiciones();
        DibujarFixture();

        ActualizarInterfazJornada();
        ActualizarPanelPrevia();
    }

    private void ActualizarInterfazJornada()
    {
        int ultimaJornada = _estado.Calendario.Max(p => p.Jornada);

        if (_estado.JornadaActual > ultimaJornada)
        {
            _btnSimularJornada.Disabled = true;
            _btnSimularJornada.Text = "Torneo Finalizado";
            _mensajePartidoJugador.Visible = true;
            _mensajePartidoJugador.Text = "¡La eliminatoria ha concluido!";
            return;
        }

        bool esUltimaJornada = _estado.JornadaActual == ultimaJornada;

        if (esUltimaJornada)
        {
            _btnSimularJornada.Text = "Jugar partido";
            _mensajePartidoJugador.Visible = true;
            _mensajePartidoJugador.Text = "Última jornada, todos juegan al mismo tiempo, los resultados se sabrán al terminar tu partido.";
        }
        else
        {
            PartidoFixture siguientePartido = _estado.Calendario.FirstOrDefault(p => p.Jornada == _estado.JornadaActual && !p.Jugado);
            if (siguientePartido == null) return;

            bool juegaEquipoJugador = siguientePartido.EquipoLocal == _estado.NombreEquipoJugador || siguientePartido.EquipoVisitante == _estado.NombreEquipoJugador;

            if (juegaEquipoJugador)
            {
                _btnSimularJornada.Text = "Jugar partido";
                _mensajePartidoJugador.Visible = true;
                _mensajePartidoJugador.Text = $"Te toca jugar: {siguientePartido.EquipoLocal} vs {siguientePartido.EquipoVisitante}.";
            }
            else
            {
                _btnSimularJornada.Text = "Simular partido";
                _mensajePartidoJugador.Visible = false;
            }
        }
    }

    private void SimularJornadaActual()
    {
        int ultimaJornada = _estado.Calendario.Max(p => p.Jornada);
        bool esUltimaJornada = _estado.JornadaActual == ultimaJornada;

        if (esUltimaJornada)
        {
            PartidoFixture partidoJugador = _estado.Calendario.FirstOrDefault(p =>
                p.Jornada == _estado.JornadaActual && !p.Jugado &&
                (p.EquipoLocal == _estado.NombreEquipoJugador || p.EquipoVisitante == _estado.NombreEquipoJugador));

            if (partidoJugador != null)
            {
                var otrosPartidos = _estado.Calendario.Where(p =>
                    p.Jornada == _estado.JornadaActual && !p.Jugado && p != partidoJugador).ToList();

                foreach (var otro in otrosPartidos)
                {
                    SimularYRegistrar(otro);
                }
                GestorGuardado.Instance.GuardarTorneo(_estado);

                bool jugadorEsLocal = partidoJugador.EquipoLocal == _estado.NombreEquipoJugador;
                string rival = jugadorEsLocal ? partidoJugador.EquipoVisitante : partidoJugador.EquipoLocal;

                PuenteTorneo.Instance.IniciarPartidoDeTorneo(_estado.NombreEquipoJugador, rival, jugadorEsLocal);
                GetTree().ChangeSceneToFile(RutaCancha);
                return;
            }
        }
        else
        {
            PartidoFixture siguientePartido = _estado.Calendario.FirstOrDefault(p => p.Jornada == _estado.JornadaActual && !p.Jugado);
            if (siguientePartido == null) return;

            bool juegaEquipoJugador = siguientePartido.EquipoLocal == _estado.NombreEquipoJugador || siguientePartido.EquipoVisitante == _estado.NombreEquipoJugador;

            if (juegaEquipoJugador)
            {
                bool jugadorEsLocal = siguientePartido.EquipoLocal == _estado.NombreEquipoJugador;
                string rival = jugadorEsLocal ? siguientePartido.EquipoVisitante : siguientePartido.EquipoLocal;

                PuenteTorneo.Instance.IniciarPartidoDeTorneo(_estado.NombreEquipoJugador, rival, jugadorEsLocal);
                GetTree().ChangeSceneToFile(RutaCancha);
                return;
            }

            SimularYRegistrar(siguientePartido);
            VerificarFinDeJornada();

            GestorGuardado.Instance.GuardarTorneo(_estado);
            DibujarTablaPosiciones();
            DibujarFixture();
            ActualizarInterfazJornada();
            ActualizarPanelPrevia();
        }
    }

    public void ResolverPartidoJugador(int golesLocal, int golesVisitante)
    {
        PartidoFixture partido = _estado.Calendario.FirstOrDefault(p =>
            p.Jornada == _estado.JornadaActual && !p.Jugado &&
            (p.EquipoLocal == _estado.NombreEquipoJugador || p.EquipoVisitante == _estado.NombreEquipoJugador));

        if (partido == null) return;

        RegistrarResultado(partido, golesLocal, golesVisitante);
        VerificarFinDeJornada();
        GestorGuardado.Instance.GuardarTorneo(_estado);

        DibujarTablaPosiciones();
        DibujarFixture();
        ActualizarInterfazJornada();
        ActualizarPanelPrevia();
    }

    private void VerificarFinDeJornada()
    {
        bool faltanPartidos = _estado.Calendario.Any(p => p.Jornada == _estado.JornadaActual && !p.Jugado);
        if (!faltanPartidos)
        {
            _estado.JornadaActual++;
        }
    }

    private void SimularYRegistrar(PartidoFixture partido)
    {
        float estrellasLocal = ObtenerEstrellas(partido.EquipoLocal);
        float estrellasVisitante = ObtenerEstrellas(partido.EquipoVisitante);
        (int golesLocal, int golesVisitante) = SimulationEngine.SimularPartido(estrellasLocal, estrellasVisitante);
        RegistrarResultado(partido, golesLocal, golesVisitante);
    }

    private void RegistrarResultado(PartidoFixture partido, int golesLocal, int golesVisitante)
    {
        partido.GolesLocal = golesLocal;
        partido.GolesVisitante = golesVisitante;
        partido.Jugado = true;

        EstadisticasEquipoGuardado local = _estado.TablaPosiciones.First(e => e.NombreEquipo == partido.EquipoLocal);
        EstadisticasEquipoGuardado visitante = _estado.TablaPosiciones.First(e => e.NombreEquipo == partido.EquipoVisitante);

        local.Jugados++;
        visitante.Jugados++;
        local.GolesFavor += golesLocal;
        local.GolesContra += golesVisitante;
        visitante.GolesFavor += golesVisitante;
        visitante.GolesContra += golesLocal;

        if (golesLocal > golesVisitante)
        {
            local.Ganados++;
            visitante.Perdidos++;
        }
        else if (golesLocal < golesVisitante)
        {
            visitante.Ganados++;
            local.Perdidos++;
        }
        else
        {
            local.Empatados++;
            visitante.Empatados++;
        }
    }

    private float ObtenerEstrellas(string nombreEquipo)
    {
        TeamData equipo = _equipos.FirstOrDefault(e => e.TeamName == nombreEquipo);
        return equipo?.StarRating ?? 3.0f;
    }

    // ================== TABLA DE POSICIONES ==================
    private void DibujarTablaPosiciones()
    {
        foreach (Node hijo in _gridPosiciones.GetChildren()) hijo.QueueFree();

        string[] encabezados = { "#", "Equipo", "PJ", "G", "E", "P", "DG", "Pts" };
        int[] anchos = { 60, 380, 70, 70, 70, 70, 80, 70 };

        for (int i = 0; i < encabezados.Length; i++)
        {
            AgregarCelda(encabezados[i], ColorTransparente, anchos[i], esEncabezado: true);
        }

        List<EstadisticasEquipoGuardado> ordenados = _estado.TablaPosiciones
            .OrderByDescending(e => e.Puntos)
            .ThenByDescending(e => e.DiferenciaGoles)
            .ThenByDescending(e => e.GolesFavor)
            .ToList();

        var celdasParaAnimar = new List<Control>();

        for (int i = 0; i < ordenados.Count; i++)
        {
            EstadisticasEquipoGuardado equipo = ordenados[i];
            bool esJugador = equipo.NombreEquipo == _estado.NombreEquipoJugador;
            Color colorFila = esJugador ? ColorFilaJugador : (i % 2 == 0 ? ColorFilaPar : ColorFilaImpar);
            string nombreMostrado = equipo.NombreEquipo;

            int inicioFila = _gridPosiciones.GetChildCount();

            AgregarCelda((i + 1).ToString(), colorFila, anchos[0], bordeIzquierdo: esJugador);
            AgregarCelda(nombreMostrado, colorFila, anchos[1], alineacion: HorizontalAlignment.Center);
            AgregarCelda(equipo.Jugados.ToString(), colorFila, anchos[2]);
            AgregarCelda(equipo.Ganados.ToString(), colorFila, anchos[3]);
            AgregarCelda(equipo.Empatados.ToString(), colorFila, anchos[4]);
            AgregarCelda(equipo.Perdidos.ToString(), colorFila, anchos[5]);
            AgregarCelda(equipo.DiferenciaGoles.ToString(), colorFila, anchos[6]);
            AgregarCelda(equipo.Puntos.ToString(), colorFila, anchos[7]);

            for (int c = inicioFila; c < _gridPosiciones.GetChildCount(); c++)
            {
                celdasParaAnimar.Add(_gridPosiciones.GetChild<Control>(c));
            }
        }

        AnimarAparicionEscalonada(celdasParaAnimar, celdasPorFila: 8);
    }

    private void AgregarCelda(
        string texto,
        Color colorFondo,
        int anchoMinimo,
        bool esEncabezado = false,
        bool bordeIzquierdo = false,
        HorizontalAlignment alineacion = HorizontalAlignment.Center)
    {
        var estilo = new StyleBoxFlat
        {
            BgColor = colorFondo,
            ContentMarginLeft = 6,
            ContentMarginRight = 6,
            ContentMarginTop = 8,
            ContentMarginBottom = 8
        };

        if (esEncabezado)
        {
            estilo.BorderColor = new Color(ColorEncabezado.R, ColorEncabezado.G, ColorEncabezado.B, 0.6f);
            estilo.BorderWidthBottom = 2;
        }

        if (bordeIzquierdo)
        {
            estilo.BorderColor = ColorBordeJugador;
            estilo.BorderWidthLeft = 3;
        }

        var panel = new PanelContainer();
        panel.AddThemeStyleboxOverride("panel", estilo);
        panel.CustomMinimumSize = new Vector2(anchoMinimo, 0);

        var label = new Label
        {
            Text = texto,
            HorizontalAlignment = alineacion,
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };

        if (esEncabezado) label.AddThemeColorOverride("font_color", ColorEncabezado);

        panel.AddChild(label);
        _gridPosiciones.AddChild(panel);
    }

    // ================== FIXTURE ==================
    private void DibujarFixture()
    {
        foreach (Node hijo in _listaFixture.GetChildren()) hijo.QueueFree();

        var porJornada = _estado.Calendario.GroupBy(p => p.Jornada).OrderBy(grupo => grupo.Key);
        var filasParaAnimar = new List<Control>();

        foreach (var grupo in porJornada)
        {
            _listaFixture.AddChild(CrearEncabezadoJornada(grupo.Key));

            foreach (PartidoFixture partido in grupo)
            {
                Control fila = CrearFilaPartido(partido);
                _listaFixture.AddChild(fila);
                filasParaAnimar.Add(fila);
            }

            _listaFixture.AddChild(new Control { CustomMinimumSize = new Vector2(0, 8) });
        }

        AnimarAparicionEscalonada(filasParaAnimar, celdasPorFila: 1);
    }

    private Control CrearEncabezadoJornada(int numeroJornada)
    {
        var contenedor = new VBoxContainer();
        contenedor.AddThemeConstantOverride("separation", 3);

        var label = new Label
        {
            Text = $"Jornada {numeroJornada}",
            HorizontalAlignment = HorizontalAlignment.Left
        };
        label.AddThemeFontSizeOverride("font_size", 17);
        label.AddThemeColorOverride("font_color", ColorEncabezado);

        var linea = new HSeparator();
        var estiloLinea = new StyleBoxFlat
        {
            BgColor = new Color(ColorEncabezado.R, ColorEncabezado.G, ColorEncabezado.B, 0.35f),
            ContentMarginTop = 1,
            ContentMarginBottom = 1
        };
        linea.AddThemeStyleboxOverride("separator", estiloLinea);

        contenedor.AddChild(label);
        contenedor.AddChild(linea);
        return contenedor;
    }

    private Control CrearFilaPartido(PartidoFixture partido)
    {
        bool jugadorLocal = partido.EquipoLocal == _estado.NombreEquipoJugador;
        bool jugadorVisitante = partido.EquipoVisitante == _estado.NombreEquipoJugador;
        bool esPartidoJugador = jugadorLocal || jugadorVisitante;

        var estiloFila = new StyleBoxFlat
        {
            BgColor = esPartidoJugador ? ColorFilaJugador : ColorTransparente,
            ContentMarginLeft = 8,
            ContentMarginRight = 8,
            ContentMarginTop = 8,
            ContentMarginBottom = 8
        };
        if (esPartidoJugador)
        {
            estiloFila.BorderColor = ColorBordeJugador;
            estiloFila.BorderWidthLeft = 3;
        }

        var panelFila = new PanelContainer();
        panelFila.AddThemeStyleboxOverride("panel", estiloFila);
        panelFila.CustomMinimumSize = new Vector2(550, 0);
        
        var fila = new HBoxContainer();
        fila.AddThemeConstantOverride("separation", 8);

        var labelLocal = new Label
        {
            Text = partido.EquipoLocal,
            HorizontalAlignment = HorizontalAlignment.Right,
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };
        if (jugadorLocal) labelLocal.AddThemeColorOverride("font_color", ColorBordeJugador);

        var labelVisitante = new Label
        {
            Text = partido.EquipoVisitante,
            HorizontalAlignment = HorizontalAlignment.Left,
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };
        if (jugadorVisitante) labelVisitante.AddThemeColorOverride("font_color", ColorBordeJugador);

        var contenedorMarcador = new PanelContainer { CustomMinimumSize = new Vector2(64, 0) };
        var estiloMarcador = new StyleBoxFlat
        {
            BgColor = new Color(0.02f, 0.05f, 0.03f, 0.6f),
            CornerRadiusTopLeft = 6,
            CornerRadiusTopRight = 6,
            CornerRadiusBottomLeft = 6,
            CornerRadiusBottomRight = 6,
            ContentMarginTop = 3,
            ContentMarginBottom = 3
        };
        contenedorMarcador.AddThemeStyleboxOverride("panel", estiloMarcador);

        var labelMarcador = new Label
        {
            Text = partido.Jugado ? $"{partido.GolesLocal} - {partido.GolesVisitante}" : "vs",
            HorizontalAlignment = HorizontalAlignment.Center
        };
        if (partido.Jugado)
        {
            labelMarcador.AddThemeFontSizeOverride("font_size", 16);
            labelMarcador.AddThemeColorOverride("font_color", ColorEncabezado);
        }
        else
        {
            labelMarcador.AddThemeColorOverride("font_color", new Color(1, 1, 1, 0.5f));
        }

        contenedorMarcador.AddChild(labelMarcador);

        fila.AddChild(labelLocal);
        fila.AddChild(contenedorMarcador);
        fila.AddChild(labelVisitante);

        panelFila.AddChild(fila);
        return panelFila;
    }

    private void AnimarAparicionEscalonada(List<Control> elementos, int celdasPorFila)
    {
        for (int i = 0; i < elementos.Count; i++)
        {
            Control elemento = elementos[i];
            int fila = i / celdasPorFila;

            elemento.Modulate = new Color(1, 1, 1, 0);
            Tween tween = CreateTween();
            tween.TweenInterval(fila * 0.025f);
            tween.TweenProperty(elemento, "modulate:a", 1.0f, 0.2f);
        }
    }

    private void AgregarCelda(string texto, bool negrita = false)
    {
        AgregarCelda(texto, ColorTransparente, 0, negrita);
    }

    // ================== PANEL DEL EQUIPO JUGADOR ==================
    private void CargarPanelJugador()
    {
        TeamData miEquipo = _equipos.FirstOrDefault(e => e.TeamName == _estado.NombreEquipoJugador);
        if (miEquipo == null) return;

        _previewNombre.Text = miEquipo.TeamName;
        _previewBandera.Texture = miEquipo.FlagTexture;
        _previewCabeza.Texture = miEquipo.CabezaTexture;
        _previewCamiseta.Texture = miEquipo.CamisetaTexture;

        string rutaMapa = _estado.Region == "Sudamérica" ? "res://img/mapas/mapaConmebol.png" : null;
        _mapaRegionFondo.Texture = (rutaMapa != null && ResourceLoader.Exists(rutaMapa)) ? GD.Load<Texture2D>(rutaMapa) : null;

        foreach (Node hijo in _previewEstrellas.GetChildren()) hijo.QueueFree();

        for (int i = 1; i <= 5; i++)
        {
            Control slot = new Control { CustomMinimumSize = new Vector2(24, 24) };

            if (miEquipo.StarRating >= i)
            {
                slot.AddChild(CrearCapaEstrella(_texEstrellaLlena, Colors.White));
            }
            else if (miEquipo.StarRating >= i - 0.5f)
            {
                slot.AddChild(CrearCapaEstrella(_texEstrellaLlena, new Color(0, 0, 0, 0.4f)));
                slot.AddChild(CrearCapaEstrella(_texEstrellaMedia, Colors.White));
            }
            else
            {
                slot.AddChild(CrearCapaEstrella(_texEstrellaLlena, new Color(0, 0, 0, 0.4f)));
            }
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
        PartidoFixture proximoPartido = _estado.Calendario.FirstOrDefault(p =>
            !p.Jugado &&
            (p.EquipoLocal == _estado.NombreEquipoJugador || p.EquipoVisitante == _estado.NombreEquipoJugador));

        if (proximoPartido == null)
        {
            _panelPrevia.Visible = false;
            return;
        }

        _panelPrevia.Visible = true;
        _nombresPrevia.Text = $"{proximoPartido.EquipoLocal} vs {proximoPartido.EquipoVisitante}";

        TeamData equipoLocal = _equipos.FirstOrDefault(e => e.TeamName == proximoPartido.EquipoLocal);
        TeamData equipoVisitante = _equipos.FirstOrDefault(e => e.TeamName == proximoPartido.EquipoVisitante);

        _cabezaLocal.Texture = equipoLocal?.CabezaTexture;
        _cabezaVisitante.Texture = equipoVisitante?.CabezaTexture;
    }
}
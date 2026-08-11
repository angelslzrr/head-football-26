using Godot;
using System.Collections.Generic;

/// <summary>
/// Controlador de la interfaz de selección de equipos y confederaciones.
/// Ofrece una experiencia visual interesante con previsualización dinámica de estadísticas,
/// mapas regionales, animación escalada de tarjetas (Juice) y composición de sprites por capas.
/// </summary>
public partial class SelectionMenuController : Control
{
    private GridContainer _gridEquipos;
    private Label _mensajeVacio;

    private TextureRect _mapaRegionFondo;
    private TextureRect _previewBandera;
    private Label _previewNombre;
    private HBoxContainer _previewEstrellas;
    
    private TextureRect _previewCabeza;
    private TextureRect _previewCamiseta;
    private Button _btnComenzar;

    private Texture2D _texEstrellaLlena;
    private Texture2D _texEstrellaMedia;

    private List<TeamData> _equiposSudamerica;
    private TeamData _equipoSeleccionado;

    public override void _Ready()
    {
        _gridEquipos = GetNode<GridContainer>("Layout/ContenidoPrincipal/PanelCentral/Scroll/GridEquipos");
        _mensajeVacio = GetNode<Label>("Layout/ContenidoPrincipal/PanelCentral/MensajeVacio");

        _mapaRegionFondo = GetNode<TextureRect>("Layout/ContenidoPrincipal/PanelDerecho/MapaRegionFondo");
        _previewBandera = GetNode<TextureRect>("Layout/ContenidoPrincipal/PanelDerecho/ContenidoDerecho/PreviewBandera");
        _previewNombre = GetNode<Label>("Layout/ContenidoPrincipal/PanelDerecho/ContenidoDerecho/PreviewNombre");
        _previewEstrellas = GetNode<HBoxContainer>("Layout/ContenidoPrincipal/PanelDerecho/ContenidoDerecho/PreviewEstrellas");
        
        _previewCabeza = GetNode<TextureRect>("Layout/ContenidoPrincipal/PanelDerecho/ContenidoDerecho/ComposicionSprite/PreviewCabeza");
        _previewCamiseta = GetNode<TextureRect>("Layout/ContenidoPrincipal/PanelDerecho/ContenidoDerecho/ComposicionSprite/PreviewCamiseta");
        _btnComenzar = GetNode<Button>("Layout/PanelInferior/BtnComenzar");

        _texEstrellaLlena = GD.Load<Texture2D>("res://img/estrellaCompleta.png");
        _texEstrellaMedia = GD.Load<Texture2D>("res://img/estrellaMitad.png");

        _btnComenzar.Disabled = true;
        _btnComenzar.Pressed += OnComenzarPresionado;

        ConectarFiltro("BtnSudamerica", "Sudamérica");
        ConectarFiltro("BtnEuropa", "Europa");
        ConectarFiltro("BtnNorteYCentro", "Norte y Centroamérica");
        ConectarFiltro("BtnAfrica", "África");
        ConectarFiltro("BtnAsia", "Asia");
        ConectarFiltro("BtnOceania", "Oceanía");

        _equiposSudamerica = RepositorioEquipos.ObtenerEquiposConmebol();
        MostrarRegion("Sudamérica");
    }

    private void ConectarFiltro(string nombreNodoBoton, string region)
    {
        Button boton = GetNodeOrNull<Button>($"Layout/ContenidoPrincipal/PanelIzquierdo/{nombreNodoBoton}");
        if (boton != null)
        {
            boton.Pressed += () => MostrarRegion(region);
        }
    }

    private void MostrarRegion(string region)
    {
        foreach (Node hijo in _gridEquipos.GetChildren())
        {
            hijo.QueueFree();
        }

        List<TeamData> equipos = region == "Sudamérica" ? _equiposSudamerica : new List<TeamData>();

        _mensajeVacio.Visible = equipos.Count == 0;

        foreach (TeamData equipo in equipos)
        {
            Button carta = new Button
            {
                CustomMinimumSize = new Vector2(220, 60),
                PivotOffset = new Vector2(110, 30)
            };

            HBoxContainer hbox = new HBoxContainer
            {
                Alignment = BoxContainer.AlignmentMode.Center,
                MouseFilter = Control.MouseFilterEnum.Ignore
            };
            hbox.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            hbox.AddThemeConstantOverride("separation", 12);

            if (equipo.FlagTexture != null)
            {
                TextureRect bandera = new TextureRect
                {
                    Texture = equipo.FlagTexture,
                    CustomMinimumSize = new Vector2(60, 40),
                    ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                    StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                    MouseFilter = Control.MouseFilterEnum.Ignore
                };
                hbox.AddChild(bandera);
            }

            Label texto = new Label
            {
                Text = equipo.TeamName,
                VerticalAlignment = VerticalAlignment.Center,
                MouseFilter = Control.MouseFilterEnum.Ignore
            };
            hbox.AddChild(texto);

            carta.AddChild(hbox);
            
            // Animaciones suaves (Tweens) al pasar el cursor sobre las tarjetas de equipo.
            carta.MouseEntered += () => 
            {
                Tween tween = carta.CreateTween();
                tween.TweenProperty(carta, "scale", new Vector2(1.05f, 1.05f), 0.1f).SetTrans(Tween.TransitionType.Sine);
            };

            carta.MouseExited += () => 
            {
                Tween tween = carta.CreateTween();
                tween.TweenProperty(carta, "scale", Vector2.One, 0.1f).SetTrans(Tween.TransitionType.Sine);
            };

            carta.Pressed += () => SeleccionarEquipo(equipo);
            _gridEquipos.AddChild(carta);
        }

        ActualizarMapaRegion(region);
        ResetearPreview();
    }

    private void ActualizarMapaRegion(string region)
    {
        string ruta = region == "Sudamérica" ? "res://img/mapas/mapaConmebol.png" : null;

        _mapaRegionFondo.Texture = (ruta != null && ResourceLoader.Exists(ruta))
            ? GD.Load<Texture2D>(ruta)
            : null;
    }

    private void ResetearPreview()
    {
        _equipoSeleccionado = null;
        _previewBandera.Texture = null;
        _previewNombre.Text = "Elige un equipo";
        _previewCabeza.Texture = null;
        _previewCamiseta.Texture = null;
        DibujarEstrellas(0f);
        _btnComenzar.Disabled = true;
    }

    private void SeleccionarEquipo(TeamData equipo)
    {
        _equipoSeleccionado = equipo;

        _previewBandera.Texture = equipo.FlagTexture;
        _previewNombre.Text = equipo.TeamName;
        DibujarEstrellas(equipo.StarRating);

        _previewCabeza.Texture = equipo.CabezaTexture;
        _previewCamiseta.Texture = equipo.CamisetaTexture;

        _btnComenzar.Disabled = false;
    }

    // Renderiza gráficamente las estrellas completas, medias o vacías (con tintes semitransparentes).
    private void DibujarEstrellas(float calificacion)
    {
        foreach (Node hijo in _previewEstrellas.GetChildren())
        {
            hijo.QueueFree();
        }

        for (int i = 1; i <= 5; i++)
        {
            Control slot = new Control
            {
                CustomMinimumSize = new Vector2(24, 24)
            };

            if (calificacion >= i)
            {
                slot.AddChild(CrearCapaEstrella(_texEstrellaLlena, Colors.White));
            }
            else if (calificacion >= i - 0.5f)
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

    private void OnComenzarPresionado()
    {
        if (_equipoSeleccionado == null) return;

        var estado = new TournamentState
        {
            NombreEquipoJugador = _equipoSeleccionado.TeamName,
            Region = _equipoSeleccionado.Region,
            FechaGuardado = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        GestorGuardado.Instance.GuardarTorneo(estado);
        GetTree().ChangeSceneToFile("res://escenas/UI/TournamentHub.tscn");
    }
}
using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class GlobalHubController : Control
{
    private const string RutaTournamentHub = "res://escenas/UI/TournamentHub.tscn";

    private VBoxContainer _panelIzquierdo;
    private OptionButton _dropdownFases;
    private TabContainer _pestanas;
    private VBoxContainer _contenedorPosiciones;
    private VBoxContainer _listaFixture;
    private Button _btnVolver;

    private TournamentState _estado;
    private EliminatoriaRegion _regionVisualizada;
    private int _indiceFaseVisualizada = 0;

    // Cache de TeamData por región para poder mostrar banderas
    private Dictionary<string, List<TeamData>> _cacheEquiposPorRegion = new();

    public override void _Ready()
    {
        _panelIzquierdo = GetNode<VBoxContainer>("Layout/ContenidoPrincipal/PanelIzquierdo");
        _dropdownFases = GetNode<OptionButton>("Layout/ContenidoPrincipal/PanelDerecho/DropdownFases");
        _pestanas = GetNode<TabContainer>("Layout/ContenidoPrincipal/PanelDerecho/Pestanas");
        _contenedorPosiciones = GetNode<VBoxContainer>("Layout/ContenidoPrincipal/PanelDerecho/Pestanas/Posiciones/FondoTabla/ScrollPosiciones/ContenedorDinamicoPosiciones");
        _listaFixture = GetNode<VBoxContainer>("Layout/ContenidoPrincipal/PanelDerecho/Pestanas/Fixture/FondoFixture/ScrollFixture/ListaFixture");
        _btnVolver = GetNode<Button>("Layout/BarraInferior/BtnVolver");

        _btnVolver.Pressed += () => GetTree().ChangeSceneToFile(RutaTournamentHub);
        _dropdownFases.ItemSelected += OnFaseSeleccionada;

        _estado = GestorGuardado.Instance.CargarTorneo();
        if (_estado == null || _estado.RestoDelMundo.Count == 0)
        {
            GD.PrintErr("No hay datos del Resto del Mundo. Volviendo al Hub.");
            GetTree().ChangeSceneToFile(RutaTournamentHub);
            return;
        }

        ConstruirBotonesDeRegion();
        MostrarRegion(_estado.RestoDelMundo[0]);
    }

    private void ConstruirBotonesDeRegion()
    {
        foreach (Node hijo in _panelIzquierdo.GetChildren()) hijo.QueueFree();

        foreach (EliminatoriaRegion region in _estado.RestoDelMundo)
        {
            Button boton = new Button
            {
                CustomMinimumSize = new Vector2(0, 50),
                Text = region.Region
            };
            boton.Pressed += () => MostrarRegion(region);
            _panelIzquierdo.AddChild(boton);
        }
    }

    private void MostrarRegion(EliminatoriaRegion region)
    {
        _regionVisualizada = region;
        _indiceFaseVisualizada = 0;

        PoblarDropdownFases();
        RedibujarFaseActual();
    }

    private void PoblarDropdownFases()
    {
        _dropdownFases.Clear();

        foreach (FaseTorneo fase in _regionVisualizada.Fases)
        {
            _dropdownFases.AddItem(fase.Nombre);
        }

        // Si hay clasificados directos, agregamos la opción virtual
        if (_regionVisualizada.ClasificadosDirectoAlMundial.Count > 0)
        {
            _dropdownFases.AddItem("⭐ Clasificados al Mundial");
        }

        _dropdownFases.Select(_indiceFaseVisualizada);
    }

    private void OnFaseSeleccionada(long indice)
    {
        _indiceFaseVisualizada = (int)indice;
        RedibujarFaseActual();
    }

    private void RedibujarFaseActual()
    {
        bool esVistaClasificados = _indiceFaseVisualizada == _regionVisualizada.Fases.Count;

        if (esVistaClasificados)
        {
            MostrarVistaClasificados();
            return;
        }

        if (_indiceFaseVisualizada < 0 || _indiceFaseVisualizada >= _regionVisualizada.Fases.Count) return;

        FaseTorneo fase = _regionVisualizada.Fases[_indiceFaseVisualizada];

        if (fase.Tipo == TipoFormato.Eliminacion)
        {
            _pestanas.TabsVisible = false;
            _pestanas.CurrentTab = 0;
        }
        else
        {
            _pestanas.TabsVisible = true;
        }

        IRenderizadorFase renderizador = RenderizadorFactory.ObtenerRenderizador(fase.Tipo);

        renderizador.DibujarPosiciones(_contenedorPosiciones, fase, "");
        renderizador.DibujarFixture(_listaFixture, fase, "");
    }

    private void MostrarVistaClasificados()
    {
        _pestanas.TabsVisible = false;
        _pestanas.CurrentTab = 0;

        foreach (Node hijo in _contenedorPosiciones.GetChildren()) hijo.QueueFree();

        List<TeamData> equiposDeLaRegion = ObtenerEquiposDeRegion(_regionVisualizada.Region);

        _contenedorPosiciones.AddChild(UiTorneoHelper.CrearEncabezadoSeccion(
            $"{_regionVisualizada.ClasificadosDirectoAlMundial.Count} selecciones clasificadas directo"));

        var flow = new HFlowContainer();
        flow.AddThemeConstantOverride("h_separation", 16);
        flow.AddThemeConstantOverride("v_separation", 16);
        _contenedorPosiciones.AddChild(flow);

        foreach (string nombreEquipo in _regionVisualizada.ClasificadosDirectoAlMundial)
        {
            TeamData equipo = equiposDeLaRegion.FirstOrDefault(e => e.TeamName == nombreEquipo);
            flow.AddChild(CrearTarjetaClasificado(nombreEquipo, equipo));
        }
    }

    private Control CrearTarjetaClasificado(string nombreEquipo, TeamData equipo)
    {
        var estilo = new StyleBoxFlat
        {
            BgColor = new Color(0.06f, 0.12f, 0.07f, 0.6f),
            BorderColor = UiTorneoHelper.ColorEncabezado,
            BorderWidthBottom = 2,
            ContentMarginLeft = 10,
            ContentMarginRight = 10,
            ContentMarginTop = 8,
            ContentMarginBottom = 8,
            CornerRadiusTopLeft = 6,
            CornerRadiusTopRight = 6,
            CornerRadiusBottomLeft = 6,
            CornerRadiusBottomRight = 6
        };

        var panel = new PanelContainer { CustomMinimumSize = new Vector2(180, 0) };
        panel.AddThemeStyleboxOverride("panel", estilo);

        var caja = new VBoxContainer();
        caja.AddThemeConstantOverride("separation", 6);

        if (equipo?.FlagTexture != null)
        {
            var bandera = new TextureRect
            {
                Texture = equipo.FlagTexture,
                CustomMinimumSize = new Vector2(0, 60),
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
            };
            caja.AddChild(bandera);
        }

        var nombre = new Label
        {
            Text = nombreEquipo,
            HorizontalAlignment = HorizontalAlignment.Center,
            AutowrapMode = TextServer.AutowrapMode.WordSmart
        };
        nombre.AddThemeColorOverride("font_color", UiTorneoHelper.ColorEncabezado);
        caja.AddChild(nombre);

        panel.AddChild(caja);
        return panel;
    }

    private List<TeamData> ObtenerEquiposDeRegion(string region)
    {
        if (!_cacheEquiposPorRegion.TryGetValue(region, out List<TeamData> equipos))
        {
            equipos = RepositorioEquipos.ObtenerEquiposPorRegion(region);
            _cacheEquiposPorRegion[region] = equipos;
        }
        return equipos;
    }
}
using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class GlobalHubController : Control
{
    private const string RutaTournamentHub = "res://escenas/UI/TournamentHub.tscn";

    private VBoxContainer _panelIzquierdo;
    private OptionButton _dropdownFases;
    private VBoxContainer _contenedorPosiciones;
    private VBoxContainer _listaFixture;
    private Button _btnVolver;

    private TournamentState _estado;
    private EliminatoriaRegion _regionVisualizada;
    private int _indiceFaseVisualizada = 0;

    public override void _Ready()
    {
        _panelIzquierdo = GetNode<VBoxContainer>("Layout/ContenidoPrincipal/PanelIzquierdo");
        _dropdownFases = GetNode<OptionButton>("Layout/ContenidoPrincipal/PanelDerecho/DropdownFases");
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
        _dropdownFases.Select(_indiceFaseVisualizada);
    }

    private void OnFaseSeleccionada(long indice)
    {
        _indiceFaseVisualizada = (int)indice;
        RedibujarFaseActual();
    }

    private void RedibujarFaseActual()
    {
        if (_indiceFaseVisualizada < 0 || _indiceFaseVisualizada >= _regionVisualizada.Fases.Count) return;

        FaseTorneo fase = _regionVisualizada.Fases[_indiceFaseVisualizada];
        IRenderizadorFase renderizador = RenderizadorFactory.ObtenerRenderizador(fase.Tipo);

        renderizador.DibujarPosiciones(_contenedorPosiciones, fase, "");
        renderizador.DibujarFixture(_listaFixture, fase, "");
    }
}
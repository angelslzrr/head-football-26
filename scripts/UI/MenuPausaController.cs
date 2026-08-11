using Godot;

/// <summary>
/// Controlador del menú de pausa durante el gameplay.
/// Utiliza ProcessMode = Always para permitir la escucha activa de la tecla Escape
/// incluso cuando el árbol de juego se encuentra pausado. Gestiona el flujo de abandono (Walkover).
/// </summary>
public partial class MenuPausaController : CanvasLayer
{
    private Button _btnReanudar;
    private Button _btnSalir;
    private ConfirmationDialog _dialogoConfirmarSalida;

    public override void _Ready()
    {
        _btnReanudar = GetNode<Button>("PanelCentral/CajaBotones/BtnReanudar");
        _btnSalir = GetNode<Button>("PanelCentral/CajaBotones/BtnSalir");
        _dialogoConfirmarSalida = GetNode<ConfirmationDialog>("DialogoConfirmarSalida");

        _btnReanudar.Pressed += Reanudar;
        _btnSalir.Pressed += () => _dialogoConfirmarSalida.PopupCentered();

        _dialogoConfirmarSalida.Confirmed += SalirDelPartido;

        Visible = false;
    }

    public override void _Process(double delta)
    {
        // Detecta la acción "ui_cancel" (Escape / Círculo en mando) asegurando
        // que no se active durante la transición de cierre de partido.
        if (Input.IsActionJustPressed("ui_cancel") && !GameManager.Instance.PartidoTerminado)
        {
            AlternarPausa();
        }
    }

    public void AlternarPausa()
    {
        if (Visible)
        {
            Reanudar();
        }
        else
        {
            Pausar();
        }
    }

    private void Pausar()
    {
        Visible = true;
        GetTree().Paused = true;

        // Adapta dinámicamente el mensaje de advertencia según si es partido de torneo o rápido.
        GetNode<Label>("DialogoConfirmarSalida/TextoDialogo").Text = PuenteTorneo.Instance.PartidoDeTorneo
            ? "Si sales ahora, este partido se contará como\nPERDIDO 0-3 por abandono."
            : "¿Seguro que quieres salir?\nPerderás el progreso de este partido.";
    }

    private void Reanudar()
    {
        Visible = false;
        GetTree().Paused = false;
    }

    private void SalirDelPartido()
    {
        // Despausa el árbol antes de cambiar de escena para evitar bloqueos en los botones de destino.
        GetTree().Paused = false;

        if (PuenteTorneo.Instance.PartidoDeTorneo)
        {
            // Aplica sanción reglamentaria por abandono (0-3).
            PuenteTorneo.Instance.GuardarResultado(0, 3);
            GetTree().ChangeSceneToFile("res://escenas/UI/TournamentHub.tscn");
        }
        else
        {
            GetTree().ChangeSceneToFile("res://escenas/UI/MainMenu.tscn");
        }
    }
}
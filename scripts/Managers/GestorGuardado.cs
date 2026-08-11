using Godot;
using System.Text.Json;

/// <summary>
/// Singleton (Autoload) responsable de la persistencia de datos.
/// Maneja la serialización y deserialización del estado del torneo hacia un archivo JSON.
/// Utiliza el directorio seguro "user://" garantizando permisos de escritura multiplataforma.
/// </summary>
public partial class GestorGuardado : Node
{
    public static GestorGuardado Instance { get; private set; }

    private const string RutaGuardado = "user://torneo_guardado.json";

    public override void _Ready()
    {
        Instance = this;
    }

    // Serializa el estado completo y lo escribe en disco de forma identada para fácil depuración.
    public void GuardarTorneo(TournamentState estado)
    {
        var opciones = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(estado, opciones);

        FileAccess archivo = FileAccess.Open(RutaGuardado, FileAccess.ModeFlags.Write);
        archivo.StoreString(json);
        archivo.Close();

        GD.Print($"Torneo guardado en {ProjectSettings.GlobalizePath(RutaGuardado)}");
    }

    // Lee el JSON del disco y lo reconstruye en objetos C#.
    public TournamentState CargarTorneo()
    {
        if (!FileAccess.FileExists(RutaGuardado))
        {
            GD.Print("No hay ningún torneo guardado todavía.");
            return null;
        }

        FileAccess archivo = FileAccess.Open(RutaGuardado, FileAccess.ModeFlags.Read);
        string json = archivo.GetAsText();
        archivo.Close();

        return JsonSerializer.Deserialize<TournamentState>(json);
    }

    public bool ExisteGuardado()
    {
        return FileAccess.FileExists(RutaGuardado);
    }
}
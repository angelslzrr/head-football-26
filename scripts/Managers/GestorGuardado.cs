using Godot;
using System.Text.Json;

public partial class GestorGuardado : Node
{
    public static GestorGuardado Instance { get; private set; }

    private const string RutaGuardado = "user://torneo_guardado.json";

    // Única fuente de verdad sobre qué versión de esquema entiende el juego
    // actual. Si en el futuro volvemos a cambiar TournamentState, alcanza
    // con subir este número — no hace falta tocar la lógica de abajo.
    private const int VersionActual = 3;

    public override void _Ready()
    {
        Instance = this;
    }

    public void GuardarTorneo(TournamentState estado)
    {
        estado.VersionGuardado = VersionActual;

        var opciones = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(estado, opciones);

        using FileAccess archivo = FileAccess.Open(RutaGuardado, FileAccess.ModeFlags.Write);
        archivo.StoreString(json);

        GD.Print($"Torneo guardado en {ProjectSettings.GlobalizePath(RutaGuardado)} (versión {VersionActual})");
    }

    public TournamentState CargarTorneo()
    {
        if (!FileAccess.FileExists(RutaGuardado))
        {
            GD.Print("No hay ningún torneo guardado todavía.");
            return null;
        }

        string json;
        using (FileAccess archivo = FileAccess.Open(RutaGuardado, FileAccess.ModeFlags.Read))
        {
            json = archivo.GetAsText();
        }

        if (string.IsNullOrWhiteSpace(json))
        {
            GD.PrintErr("El archivo de guardado está vacío. Se descarta.");
            return null;
        }

        int versionEncontrada = LeerVersionSinDeserializarTodo(json);

        if (versionEncontrada < VersionActual)
        {
            GD.Print($"El guardado es de una versión anterior (v{versionEncontrada}, se requiere v{VersionActual}). " +
                      "Se descarta y el juego debe iniciar un torneo nuevo.");
            return null;
        }

        try
        {
            TournamentState estado = JsonSerializer.Deserialize<TournamentState>(json);

            // Segunda barrera de seguridad: aunque la versión coincida, si por
            // algún motivo el JSON llegó truncado o corrupto y las Fases
            // quedaron vacías, tratamos el guardado como inválido en vez de
            // dejar que el juego arranque con un torneo sin fases jugables.
            if (estado == null)
            {
                GD.PrintErr("El guardado es nulo. Se descarta.");
                return null;
            }

            return estado;
        }
        catch (JsonException ex)
        {
            GD.PrintErr($"No se pudo leer el guardado (JSON corrupto): {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Inspecciona únicamente el campo "VersionGuardado" del JSON crudo, sin
    /// mapear el resto de la estructura contra TournamentState. Es necesario
    /// hacerlo así (y no confiar en el resultado de un Deserialize completo)
    /// porque la propiedad VersionGuardado tiene un valor por defecto (2) en
    /// la clase C#: si un guardado viejo NO tiene ese campo en el archivo,
    /// System.Text.Json rellenaría el default de la clase en vez de dejarlo
    /// en 0, y jamás detectaríamos que es un guardado antiguo.
    /// </summary>
    private int LeerVersionSinDeserializarTodo(string json)
    {
        try
        {
            using JsonDocument documento = JsonDocument.Parse(json);

            if (documento.RootElement.TryGetProperty("VersionGuardado", out JsonElement propVersion))
            {
                return propVersion.GetInt32();
            }

            return 1; // el campo directamente no existe: guardado pre-Hito 8
        }
        catch (JsonException)
        {
            return 0; // el archivo ni siquiera es JSON válido
        }
    }

    public bool ExisteGuardado()
    {
        return FileAccess.FileExists(RutaGuardado);
    }
}
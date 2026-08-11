using Godot;

/// <summary>
/// Recurso personalizado de Godot (Resource) que define la entidad de un Equipo.
/// Centraliza toda la data estática y los assets visuales (sprites, banderas).
/// La etiqueta [GlobalClass] permite crear y editar estos recursos directamente desde el inspector de Godot.
/// </summary>
[GlobalClass]
public partial class TeamData : Resource
{
    [Export] public string TeamName { get; set; } = "";
    [Export] public string Region { get; set; } = ""; 
    [Export] public Texture2D FlagTexture { get; set; }

    // Código estándar de 3 letras de la FIFA para asegurar compatibilidad en marcadores.
    [Export] public string FifaCode { get; set; } = "";
    
    // Define la fuerza del equipo (0.5 a 5.0). Utilizado por el motor de simulación y la IA.
    [Export] public float StarRating { get; set; } = 1.0f;

    // --- Assets para el Gameplay 2D ---
    [Export] public Texture2D CabezaTexture { get; set; }
    [Export] public Texture2D CamisetaTexture { get; set; }
    [Export] public Texture2D ChimpunTexture { get; set; }
}
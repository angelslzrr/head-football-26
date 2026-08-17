/// <summary>
/// Representa un cruce (partido) dentro de un árbol de eliminación directa.
/// </summary>
public class LlaveEliminacion
{
    public int Ronda { get; set; }
    public int Posicion { get; set; }

    public string EquipoLocal { get; set; } = "";
    public string EquipoVisitante { get; set; } = "";

    public bool Jugado { get; set; } = false;
    public int GolesLocal { get; set; } = 0;
    public int GolesVisitante { get; set; } = 0;

    public string Ganador { get; set; } = "";
}
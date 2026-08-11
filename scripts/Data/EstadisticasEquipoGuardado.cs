/// <summary>
/// Modelo de datos puro (POCO) utilizado exclusivamente para la serialización.
/// Representa las estadísticas de un equipo en la tabla de posiciones.
/// Al no depender de Godot (Resource/Node), facilita el guardado en formato JSON.
/// </summary>
public class EstadisticasEquipoGuardado
{
    public string NombreEquipo { get; set; } = "";
    public int Jugados { get; set; }
    public int Ganados { get; set; }
    public int Empatados { get; set; }
    public int Perdidos { get; set; }
    public int GolesFavor { get; set; }
    public int GolesContra { get; set; }

    // Propiedades calculadas dinámicamente en tiempo de ejecución.
    // No necesitan ser guardadas en disco ya que derivan de los otros datos.
    public int DiferenciaGoles => GolesFavor - GolesContra;
    public int Puntos => (Ganados * 3) + Empatados;
}
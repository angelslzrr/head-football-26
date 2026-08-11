/// <summary>
/// Clase envoltorio (Wrapper) utilizada en tiempo de ejecución para vincular 
/// los datos estáticos del equipo (TeamData) con sus estadísticas dinámicas.
/// A diferencia de EstadisticasEquipoGuardado, esta clase mantiene referencias directas a objetos complejos.
/// </summary>
public class TeamStats
{
    public TeamData Team { get; set; }
    public int Played { get; set; }
    public int Won { get; set; }
    public int Drawn { get; set; }
    public int Lost { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }

    public int GoalDifference => GoalsFor - GoalsAgainst;
    public int Points => (Won * 3) + Drawn;
}
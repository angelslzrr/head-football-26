/// <summary>
/// Representa un partido individual dentro del calendario del torneo.
/// Diseñado como una clase serializable para mantener el estado de cada encuentro 
/// (jugado o pendiente) dentro del archivo de guardado JSON.
/// </summary>
public class PartidoFixture
{
    public int Jornada { get; set; }
    public string EquipoLocal { get; set; } = "";
    public string EquipoVisitante { get; set; } = "";
    
    // Bandera para determinar si la simulación o el jugador ya resolvieron este encuentro.
    public bool Jugado { get; set; } = false;
    
    public int GolesLocal { get; set; } = 0;
    public int GolesVisitante { get; set; } = 0;
}
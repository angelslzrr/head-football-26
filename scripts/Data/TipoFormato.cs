/// <summary>
/// Define el formato competitivo de una fase del torneo.
/// Es el "selector" que le dice al resto del sistema qué generador usar.
/// </summary>
public enum TipoFormato
{
    RoundRobin,   // Todos contra todos, ida y vuelta
    Grupos,       // Todos contra todos dentro de grupos
    Eliminacion   // Llave de eliminación directa
}
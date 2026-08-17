using System.Collections.Generic;

/// <summary>
/// Encapsula un grupo individual dentro de una fase de tipo Grupos.
/// Es un mini-torneo con su calendario y su tabla de posiciones independiente.
/// </summary>
public class GrupoTorneo
{
    public string Nombre { get; set; } = ""; // "Grupo A", "Grupo B"
    public List<string> Equipos { get; set; } = new();
    public List<PartidoFixture> Calendario { get; set; } = new();
    public List<EstadisticasEquipoGuardado> TablaPosiciones { get; set; } = new();
}
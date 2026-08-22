using System.Collections.Generic;

/// <summary>
/// Representa UNA fase dentro de un torneo. Combina configuración de reglas
/// con el estado en tiempo real de esa fase.
/// </summary>
public class FaseTorneo
{
    public string Nombre { get; set; } = "";
    public TipoFormato Tipo { get; set; }
    public bool Completada { get; set; } = false;

    // --- Parámetros de reglas ---
    public int ClasificanPorGrupo { get; set; } = 2;   
    public int EquiposPorGrupo { get; set; } = 4;       
    public bool SorteoAleatorio { get; set; } = true;
    public bool RondaUnica { get; set; } = false;
    public bool IdaYVuelta { get; set; } = false;
    public bool LlavesIdaYVuelta { get; set; } = false;
    public bool DivideClasificados { get; set; } = false;
    public int CantidadClasificadosExtra { get; set; } = 0;

    // Solo aplica a fases tipo RoundRobin (como CONMEBOL).
    // Cuántos puestos de la tabla GENERAL son verdes (directo) y cuántos azules (repechaje).
    public int ZonaDirectaCantidad { get; set; } = 0;
    public int ZonaRepechajeCantidad { get; set; } = 0;

    // Solo aplica a fases tipo Eliminacion (Ej: Ronda 2 de África).
    // Si es true, el que gana la FINAL de esa llave se pinta de azul (Repechaje) en vez de verde.
    public bool GanadorEsRepechaje { get; set; } = false;
    // Si es true, el perdedor de la RONDA FINAL se pinta de azul (Repechaje).
    public bool PerdedorEsRepechaje { get; set; } = false;

    // Equipos que entran DIRECTAMENTE a esta fase.
    public List<string> EquiposDirectos { get; set; } = new();

    // Cuántos equipos totales participan inicialmente (0 = todos)
    public int EquiposParticipantesIniciales { get; set; } = 0;

    // Cuántos de los clasificados avanzan (-1 = todos)
    public int ClasificanASiguienteFase { get; set; } = -1;

    // --- Estado: RoundRobin ---
    public int JornadaActual { get; set; } = 1;
    public List<PartidoFixture> Calendario { get; set; } = new();
    public List<EstadisticasEquipoGuardado> TablaPosiciones { get; set; } = new();

    // --- Estado: Grupos ---
    public List<GrupoTorneo> Grupos { get; set; } = new();

    // --- Estado: Eliminación ---
    public List<LlaveEliminacion> Llaves { get; set; } = new();
}
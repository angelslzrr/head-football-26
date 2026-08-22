using System.Collections.Generic;

public class EliminatoriaRegion
{
    public string Region { get; set; } = "";
    public List<FaseTorneo> Fases { get; set; } = new();

    // NUEVO: Equipos que ya clasificaron directo al Mundial en esta región.
    public List<string> ClasificadosDirectoAlMundial { get; set; } = new();
}
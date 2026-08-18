using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Objeto raíz del estado del juego. Ya no asume un único formato: contiene
/// una lista ordenada de fases, cada una autocontenida con su propio tipo.
/// </summary>
public class TournamentState
{
    public string NombreEquipoJugador { get; set; } = "";
    public string Region { get; set; } = "";
    public string FechaGuardado { get; set; } = "";

    public int VersionGuardado { get; set; } = 2;

    public List<FaseTorneo> Fases { get; set; } = new();
    public int FaseActualIndice { get; set; } = 0;
    public bool JugadorEliminado { get; set; } = false;
    public bool RepechajeMostrado { get; set; } = false;

    public List<EliminatoriaRegion> RestoDelMundo { get; set; } = new();
    public bool MundoSimulado { get; set; } = false;

    // Propiedad de conveniencia que no se serializa en el JSON
    public FaseTorneo FaseActual =>
        (FaseActualIndice >= 0 && FaseActualIndice < Fases.Count) ? Fases[FaseActualIndice] : null;
}
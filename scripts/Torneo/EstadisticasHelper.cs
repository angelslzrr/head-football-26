using System.Collections.Generic;
using System.Linq;

public static class EstadisticasHelper
{
    public static void RegistrarResultado(List<EstadisticasEquipoGuardado> tabla, string local, string visitante, int golesLocal, int golesVisitante)
    {
        EstadisticasEquipoGuardado eqLocal = tabla.First(e => e.NombreEquipo == local);
        EstadisticasEquipoGuardado eqVisitante = tabla.First(e => e.NombreEquipo == visitante);

        eqLocal.Jugados++;
        eqVisitante.Jugados++;
        eqLocal.GolesFavor += golesLocal;
        eqLocal.GolesContra += golesVisitante;
        eqVisitante.GolesFavor += golesVisitante;
        eqVisitante.GolesContra += golesLocal;

        if (golesLocal > golesVisitante)
        {
            eqLocal.Ganados++;
            eqVisitante.Perdidos++;
        }
        else if (golesLocal < golesVisitante)
        {
            eqVisitante.Ganados++;
            eqLocal.Perdidos++;
        }
        else
        {
            eqLocal.Empatados++;
            eqVisitante.Empatados++;
        }
    }

    public static List<string> OrdenarTabla(List<EstadisticasEquipoGuardado> tabla)
    {
        return tabla
            .OrderByDescending(e => e.Puntos)
            .ThenByDescending(e => e.DiferenciaGoles)
            .ThenByDescending(e => e.GolesFavor)
            .Select(e => e.NombreEquipo)
            .ToList();
    }
}
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Calcula qué equipos están en zona de Clasificación Directa o Repechaje.
/// </summary>
public static class ZonasClasificacionHelper
{
    public static (HashSet<string> Directos, HashSet<string> Repechaje) ObtenerZonas(FaseTorneo fase)
    {
        var directos = new HashSet<string>();
        var repechaje = new HashSet<string>();

        switch (fase.Tipo)
        {
            case TipoFormato.RoundRobin:
                CalcularZonasRoundRobin(fase, directos, repechaje);
                break;

            case TipoFormato.Grupos:
                CalcularZonasGrupos(fase, directos, repechaje);
                break;
        }

        return (directos, repechaje);
    }

    private static void CalcularZonasRoundRobin(FaseTorneo fase, HashSet<string> directos, HashSet<string> repechaje)
    {
        if (fase.ZonaDirectaCantidad <= 0) return; 

        List<string> tablaOrdenada = EstadisticasHelper.OrdenarTabla(fase.TablaPosiciones);

        foreach (string equipo in tablaOrdenada.Take(fase.ZonaDirectaCantidad))
            directos.Add(equipo);

        foreach (string equipo in tablaOrdenada.Skip(fase.ZonaDirectaCantidad).Take(fase.ZonaRepechajeCantidad))
            repechaje.Add(equipo);
    }

    private static void CalcularZonasGrupos(FaseTorneo fase, HashSet<string> directos, HashSet<string> repechaje)
    {
        if (fase.DivideClasificados)
        {
            // Caso CAF: Líderes van directos, "mejores segundos" cruzados van a repechaje.
            var generador = new GeneradorGrupos();
            (List<string> lideres, List<string> mejoresSegundos) =
                generador.ObtenerClasificadosConRepechaje(fase, fase.CantidadClasificadosExtra);

            foreach (string equipo in lideres) directos.Add(equipo);
            foreach (string equipo in mejoresSegundos) repechaje.Add(equipo);
        }
        else
        {
            // Caso simple (CONCACAF, OFC): Los primeros 'N' de CADA grupo son directos.
            foreach (GrupoTorneo grupo in fase.Grupos)
            {
                List<string> tablaOrdenada = EstadisticasHelper.OrdenarTabla(grupo.TablaPosiciones);
                foreach (string equipo in tablaOrdenada.Take(fase.ClasificanPorGrupo))
                    directos.Add(equipo);
            }
        }
    }
}
using System.Collections.Generic;

public static class RepositorioFormatos
{
    // 📌 Única fuente de verdad de las regiones implementadas.
    public static readonly List<string> TodasLasRegiones = new()
    {
        "Sudamérica",
        "Oceania",
        "Norte y Centroamérica",
        "África"
    };
    
    public static List<FaseTorneo> ObtenerFormatoCONMEBOL()
    {
        return new List<FaseTorneo>
        {
            new FaseTorneo
            {
                Nombre = "Eliminatoria Sudamericana",
                Tipo = TipoFormato.RoundRobin,
                ZonaDirectaCantidad = 6,
                ZonaRepechajeCantidad = 1
            }
        };
    }

    public static List<FaseTorneo> ObtenerFormatoOFC()
    {
        return new List<FaseTorneo>
        {
            new FaseTorneo
            {
                Nombre = "Ronda 1",
                Tipo = TipoFormato.Eliminacion,
                SorteoAleatorio = true,           
                EquiposParticipantesIniciales = 4, 
                ClasificanASiguienteFase = 1       
            },
            new FaseTorneo
            {
                Nombre = "Ronda 2 — Fase de Grupos",
                Tipo = TipoFormato.Grupos,
                EquiposPorGrupo = 4,
                ClasificanASiguienteFase = -1      
            },
            new FaseTorneo
            {
                Nombre = "Ronda 3 — Semifinales y Final",
                Tipo = TipoFormato.Eliminacion,
                SorteoAleatorio = false,
                PerdedorEsRepechaje = true // ¡El perdedor de la final se va a repechaje!
            }
        };
    }

    public static List<FaseTorneo> ObtenerFormatoCONCACAF()
    {
        return new List<FaseTorneo>
        {
            new FaseTorneo
            {
                Nombre = "Ronda 1 — Play-in",
                Tipo = TipoFormato.Eliminacion,
                SorteoAleatorio = true,
                RondaUnica = true,
                LlavesIdaYVuelta = true,
                EquiposParticipantesIniciales = 4,
                ClasificanASiguienteFase = -1
            },
            new FaseTorneo
            {
                Nombre = "Ronda 2 — Fase de Grupos",
                Tipo = TipoFormato.Grupos,
                EquiposPorGrupo = 5,
                ClasificanASiguienteFase = -1
            },
            new FaseTorneo
            {
                Nombre = "Ronda 3 — Fase Final",
                Tipo = TipoFormato.Grupos,
                EquiposPorGrupo = 4,
                IdaYVuelta = true,
                ClasificanPorGrupo = 1,          // Solo el 1° de cada grupo va de verde (Directo)
                DivideClasificados = true,       // Activa la lógica de cruce de grupos
                CantidadClasificadosExtra = 2    // Solo los 2 mejores segundos de los 3 grupos van de azul (Repechaje)
            }
        };
    }

    public static List<FaseTorneo> ObtenerFormatoCAF()
    {
        return new List<FaseTorneo>
        {
            new FaseTorneo
            {
                Nombre = "Ronda 1 — Fase de Grupos",
                Tipo = TipoFormato.Grupos,
                EquiposPorGrupo = 6,
                IdaYVuelta = true,
                DivideClasificados = true,
                CantidadClasificadosExtra = 4
            },
            new FaseTorneo
            {
                Nombre = "Ronda 2 — Play-offs rumbo a Repesca",
                Tipo = TipoFormato.Eliminacion,
                SorteoAleatorio = false, // ¡Crucial para cruzar por Ranking!
                GanadorEsRepechaje = true   // El campeón de esta mini-llave va de azul
            }
        };
    }

    public static List<FaseTorneo> ObtenerFormatoPorRegion(string region)
    {
        return region switch
        {
            "Sudamérica" => ObtenerFormatoCONMEBOL(),
            "Oceania" => ObtenerFormatoOFC(),
            "Norte y Centroamérica" => ObtenerFormatoCONCACAF(),
            "África" => ObtenerFormatoCAF(),
            _ => new List<FaseTorneo>()
        };
    }
}
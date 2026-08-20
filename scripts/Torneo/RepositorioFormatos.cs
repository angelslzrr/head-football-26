using System.Collections.Generic;

public static class RepositorioFormatos
{
    // 📌 Única fuente de verdad de las regiones implementadas.
    public static readonly List<string> TodasLasRegiones = new()
    {
        "Sudamérica",
        "Oceania",
        "Norte y Centroamérica"
    };
    
    public static List<FaseTorneo> ObtenerFormatoCONMEBOL()
    {
        return new List<FaseTorneo>
        {
            new FaseTorneo
            {
                Nombre = "Eliminatoria Sudamericana",
                Tipo = TipoFormato.RoundRobin
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
                SorteoAleatorio = false            
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
                IdaYVuelta = true, // Aquí activamos la doble vuelta universal
                ClasificanASiguienteFase = -1 
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
            _ => new List<FaseTorneo>()
        };
    }
}
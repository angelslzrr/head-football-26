using Godot;
using System.Collections.Generic;

/// <summary>
/// Clase de utilidad estática responsable de generar el calendario del torneo.
/// Implementa un algoritmo matemático de "Round-Robin" modificado para asegurar 
/// equidad en las localías y aplicar reglas específicas de confederaciones (en este caso, CONMEBOL).
/// </summary>
public static class GeneradorFixture
{
    public static List<PartidoFixture> GenerarFixtureIdaYVuelta(List<string> nombresEquipos)
    {
        List<string> equipos = new List<string>(nombresEquipos);
        MezclarLista(equipos);

        // Si el número de equipos es impar, añadimos un equipo fantasma (Bye).
        bool esImpar = equipos.Count % 2 != 0;
        if (esImpar) equipos.Add(null);

        int n = equipos.Count;
        int rondas = n - 1;
        int partidosPorRonda = n / 2;

        var indices = new List<int>();
        for (int i = 0; i < n; i++) indices.Add(i);

        var idaSolamente = new List<PartidoFixture>();

        for (int ronda = 0; ronda < rondas; ronda++)
        {
            for (int i = 0; i < partidosPorRonda; i++)
            {
                int indiceLocal = indices[i];
                int indiceVisitante = indices[n - 1 - i];
                
                // Lógica de alternancia: Evita que un equipo juegue más de 2 veces 
                // consecutivas en la misma condición (Local/Visitante).
                bool localEsPrimero;
                if (i == 0)
                    localEsPrimero = (ronda % 2 == 0);
                else
                    localEsPrimero = (i % 2 == 1);

                string local = localEsPrimero ? equipos[indiceLocal] : equipos[indiceVisitante];
                string visitante = localEsPrimero ? equipos[indiceVisitante] : equipos[indiceLocal];

                if (local != null && visitante != null)
                {
                    idaSolamente.Add(new PartidoFixture
                    {
                        Jornada = ronda + 1,
                        EquipoLocal = local,
                        EquipoVisitante = visitante
                    });
                }
            }

            // Rotación circular de posiciones (Algoritmo Polygon / Round-Robin)
            int ultimo = indices[n - 1];
            for (int i = n - 1; i > 1; i--) indices[i] = indices[i - 1];
            indices[1] = ultimo;
        }

        var vuelta = new List<PartidoFixture>();
        foreach (PartidoFixture partido in idaSolamente)
        {
            // Adaptación específica de formato FIFA/CONMEBOL.
            // La fecha 1 se repite en la jornada 18, el resto mantiene un offset de +8.
            int jornadaVuelta;
            if (partido.Jornada == 1)
            {
                jornadaVuelta = 18;
            }
            else
            {
                jornadaVuelta = partido.Jornada + 8;
            }

            vuelta.Add(new PartidoFixture
            {
                Jornada = jornadaVuelta,
                EquipoLocal = partido.EquipoVisitante,
                EquipoVisitante = partido.EquipoLocal
            });
        }

        idaSolamente.AddRange(vuelta);
        return idaSolamente;
    }

    // Implementación del algoritmo de Fisher-Yates para barajar los equipos aleatoriamente.
    private static void MezclarLista<T>(List<T> lista)
    {
        for (int i = lista.Count - 1; i > 0; i--)
        {
            int j = GD.RandRange(0, i);
            (lista[i], lista[j]) = (lista[j], lista[i]);
        }
    }
}
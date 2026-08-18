using Godot;
using System.Collections.Generic;
using System.Linq;

public static class GeneradorFixture
{
    public static List<PartidoFixture> GenerarRondasUnaVuelta(List<string> nombresEquipos)
    {
        List<string> equipos = new List<string>(nombresEquipos);
        AleatorioUtil.Mezclar(equipos);

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

                bool localEsPrimero = (i == 0) ? (ronda % 2 == 0) : (i % 2 == 1);

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

            int ultimo = indices[n - 1];
            for (int i = n - 1; i > 1; i--) indices[i] = indices[i - 1];
            indices[1] = ultimo;
        }

        // Si es impar, arreglamos las localías para que sean equitativas.
        if (esImpar)
        {
            BalancearLocalias(idaSolamente, nombresEquipos.Count);
        }

        return idaSolamente;
    }

    private static void BalancearLocalias(List<PartidoFixture> partidos, int totalEquiposReales)
    {
        List<string> ordenEquipos = partidos
            .SelectMany(p => new[] { p.EquipoLocal, p.EquipoVisitante })
            .Distinct()
            .ToList();

        var indicePorEquipo = ordenEquipos
            .Select((nombre, indice) => (nombre, indice))
            .ToDictionary(x => x.nombre, x => x.indice);

        foreach (PartidoFixture partido in partidos)
        {
            int indiceLocal = indicePorEquipo[partido.EquipoLocal];
            int indiceVisitante = indicePorEquipo[partido.EquipoVisitante];

            if (!EsLocalSegunCirculante(indiceLocal, indiceVisitante, totalEquiposReales))
            {
                (partido.EquipoLocal, partido.EquipoVisitante) = (partido.EquipoVisitante, partido.EquipoLocal);
            }
        }
    }

    private static bool EsLocalSegunCirculante(int indiceA, int indiceB, int totalEquipos)
    {
        int diferencia = ((indiceB - indiceA) % totalEquipos + totalEquipos) % totalEquipos;
        return diferencia >= 1 && diferencia <= (totalEquipos - 1) / 2;
    }

    public static List<PartidoFixture> GenerarFixtureIdaYVuelta(List<string> nombresEquipos)
    {
        List<PartidoFixture> idaSolamente = GenerarRondasUnaVuelta(nombresEquipos);

        // Calculamos dinámicamente cuántas jornadas tuvo la ida.
        // Así sirve para 4 equipos (3 jornadas) o 10 equipos (9 jornadas).
        int rondasIda = idaSolamente.Count > 0 ? idaSolamente.Max(p => p.Jornada) : 0;

        var vuelta = new List<PartidoFixture>();
        foreach (PartidoFixture partido in idaSolamente)
        {
            vuelta.Add(new PartidoFixture
            {
                Jornada = partido.Jornada + rondasIda,
                EquipoLocal = partido.EquipoVisitante, // Invertimos la localía
                EquipoVisitante = partido.EquipoLocal
            });
        }

        idaSolamente.AddRange(vuelta);
        return idaSolamente;
    }
}
using Godot;
using System.Collections.Generic;

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

        return idaSolamente;
    }

    public static List<PartidoFixture> GenerarFixtureIdaYVuelta(List<string> nombresEquipos)
    {
        List<PartidoFixture> idaSolamente = GenerarRondasUnaVuelta(nombresEquipos);

        var vuelta = new List<PartidoFixture>();
        foreach (PartidoFixture partido in idaSolamente)
        {
            int jornadaVuelta = partido.Jornada == 1 ? 18 : partido.Jornada + 8;

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
}
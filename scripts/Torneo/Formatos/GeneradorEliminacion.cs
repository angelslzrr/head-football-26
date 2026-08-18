using System.Collections.Generic;
using System.Linq;

public class GeneradorEliminacion : IGeneradorFormato
{
    public void GenerarEstructura(FaseTorneo fase, List<string> equipos)
    {
        List<string> orden = new List<string>(equipos);
        if (fase.SorteoAleatorio) AleatorioUtil.Mezclar(orden);

        var llaves = new List<LlaveEliminacion>();
        for (int i = 0; i < orden.Count; i += 2)
        {
            llaves.Add(new LlaveEliminacion
            {
                Ronda = 1,
                Posicion = i / 2,
                EquipoLocal = orden[i],
                EquipoVisitante = orden[i + 1],
                IdaYVuelta = fase.LlavesIdaYVuelta
            });
        }

        fase.Llaves = llaves;
    }

    public void ProcesarResultado(FaseTorneo fase, string equipoLocal, string equipoVisitante, int golesLocal, int golesVisitante)
    {
        LlaveEliminacion llave = fase.Llaves.FirstOrDefault(l =>
            !l.Jugado &&
            ((l.EquipoLocal == equipoLocal && l.EquipoVisitante == equipoVisitante) ||
             (l.IdaYVuelta && l.EquipoLocal == equipoVisitante && l.EquipoVisitante == equipoLocal)));

        if (llave == null) return;

        if (!llave.IdaYVuelta)
        {
            if (golesLocal == golesVisitante)
            {
                (golesLocal, golesVisitante) = SimulationEngine.ResolverEmpateSiCorresponde(equipoLocal, equipoVisitante, golesLocal, golesVisitante);
            }

            llave.GolesLocalIda = golesLocal;
            llave.GolesVisitanteIda = golesVisitante;
            llave.JugadoIda = true;
            llave.Ganador = golesLocal >= golesVisitante ? equipoLocal : equipoVisitante;
        }
        else if (!llave.JugadoIda)
        {
            // Es la ida. Un empate acá es un resultado válido, no hay penales.
            llave.GolesLocalIda = golesLocal;
            llave.GolesVisitanteIda = golesVisitante;
            llave.JugadoIda = true;
        }
        else
        {
            // Es la vuelta.
            llave.GolesLocalVuelta = golesLocal;
            llave.GolesVisitanteVuelta = golesVisitante;
            llave.JugadoVuelta = true;

            if (llave.GolesGlobalLocal == llave.GolesGlobalVisitante)
            {
                // Empate global, desempata el de mejor ranking.
                int puestoLocalOriginal = RankingFifaProvider.ObtenerPosicion(llave.EquipoLocal);
                int puestoVisitanteOriginal = RankingFifaProvider.ObtenerPosicion(llave.EquipoVisitante);

                if (puestoLocalOriginal <= puestoVisitanteOriginal)
                    llave.GolesVisitanteVuelta++;
                else
                    llave.GolesLocalVuelta++;
            }

            llave.Ganador = llave.GolesGlobalLocal >= llave.GolesGlobalVisitante ? llave.EquipoLocal : llave.EquipoVisitante;
        }

        GenerarSiguienteRondaSiCorresponde(fase, llave.Ronda);
    }

    private void GenerarSiguienteRondaSiCorresponde(FaseTorneo fase, int rondaRecienJugada)
    {
        if (fase.RondaUnica) return;

        List<LlaveEliminacion> llavesDeEstaRonda = fase.Llaves
            .Where(l => l.Ronda == rondaRecienJugada)
            .OrderBy(l => l.Posicion)
            .ToList();

        if (llavesDeEstaRonda.Any(l => !l.Jugado)) return;
        if (llavesDeEstaRonda.Count == 1) return;

        List<string> ganadores = llavesDeEstaRonda.Select(l => l.Ganador).ToList();
        int siguienteRonda = rondaRecienJugada + 1;

        var nuevasLlaves = new List<LlaveEliminacion>();
        for (int i = 0; i < ganadores.Count; i += 2)
        {
            nuevasLlaves.Add(new LlaveEliminacion
            {
                Ronda = siguienteRonda,
                Posicion = i / 2,
                EquipoLocal = ganadores[i],
                EquipoVisitante = ganadores[i + 1],
                IdaYVuelta = fase.LlavesIdaYVuelta
            });
        }

        fase.Llaves.AddRange(nuevasLlaves);
    }

    public bool FaseCompleta(FaseTorneo fase)
    {
        if (fase.RondaUnica)
            return fase.Llaves.All(l => l.Jugado);

        int rondaFinal = fase.Llaves.Max(l => l.Ronda);
        List<LlaveEliminacion> llavesFinal = fase.Llaves.Where(l => l.Ronda == rondaFinal).ToList();
        return llavesFinal.Count == 1 && llavesFinal[0].Jugado;
    }

    public List<string> ObtenerClasificados(FaseTorneo fase)
    {
        if (fase.RondaUnica)
            return fase.Llaves.Select(l => l.Ganador).ToList();

        int rondaFinal = fase.Llaves.Max(l => l.Ronda);
        LlaveEliminacion final = fase.Llaves.First(l => l.Ronda == rondaFinal);

        string campeon = final.Ganador;
        string subcampeon = final.Ganador == final.EquipoLocal ? final.EquipoVisitante : final.EquipoLocal;

        return new List<string> { campeon, subcampeon };
    }
}
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
                EquipoVisitante = orden[i + 1]
            });
        }

        fase.Llaves = llaves;
    }

    public void ProcesarResultado(FaseTorneo fase, string equipoLocal, string equipoVisitante, int golesLocal, int golesVisitante)
    {
        LlaveEliminacion llave = fase.Llaves.FirstOrDefault(l =>
            !l.Jugado && l.EquipoLocal == equipoLocal && l.EquipoVisitante == equipoVisitante);
        if (llave == null) return;

        llave.GolesLocal = golesLocal;
        llave.GolesVisitante = golesVisitante;
        llave.Jugado = true;
        
        llave.Ganador = golesLocal >= golesVisitante ? equipoLocal : equipoVisitante;

        GenerarSiguienteRondaSiCorresponde(fase, llave.Ronda);
    }

    private void GenerarSiguienteRondaSiCorresponde(FaseTorneo fase, int rondaRecienJugada)
    {
        List<LlaveEliminacion> llavesDeEstaRonda = fase.Llaves
            .Where(l => l.Ronda == rondaRecienJugada)
            .OrderBy(l => l.Posicion)
            .ToList();

        if (llavesDeEstaRonda.Any(l => !l.Jugado)) return;
        if (llavesDeEstaRonda.Count == 1) return; // era la final

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
                EquipoVisitante = ganadores[i + 1]
            });
        }

        fase.Llaves.AddRange(nuevasLlaves);
    }

    public bool FaseCompleta(FaseTorneo fase)
    {
        int rondaFinal = fase.Llaves.Max(l => l.Ronda);
        List<LlaveEliminacion> llavesFinal = fase.Llaves.Where(l => l.Ronda == rondaFinal).ToList();
        return llavesFinal.Count == 1 && llavesFinal[0].Jugado;
    }

    public List<string> ObtenerClasificados(FaseTorneo fase)
    {
        int rondaFinal = fase.Llaves.Max(l => l.Ronda);
        LlaveEliminacion final = fase.Llaves.First(l => l.Ronda == rondaFinal);

        string campeon = final.Ganador;
        string subcampeon = final.Ganador == final.EquipoLocal ? final.EquipoVisitante : final.EquipoLocal;

        return new List<string> { campeon, subcampeon };
    }
}
using System.Collections.Generic;

public interface IGeneradorFormato
{
    void GenerarEstructura(FaseTorneo fase, List<string> equipos);
    void ProcesarResultado(FaseTorneo fase, string equipoLocal, string equipoVisitante, int golesLocal, int golesVisitante);
    bool FaseCompleta(FaseTorneo fase);
    List<string> ObtenerClasificados(FaseTorneo fase);
}
using Godot;

public static class SimulationEngine
{
    private const float VentajaLocalia = 0.15f;
    private const float PesoDiferenciaEstrellas = 0.22f;

    public static (int golesLocal, int golesVisitante) SimularPartido(float estrellasLocal, float estrellasVisitante)
    {
        float diferencia = estrellasLocal - estrellasVisitante;

        float lambdaLocal = Mathf.Clamp(1.3f + (diferencia * PesoDiferenciaEstrellas) + VentajaLocalia, 0.2f, 4.0f);
        float lambdaVisitante = Mathf.Clamp(1.3f - (diferencia * PesoDiferenciaEstrellas), 0.2f, 4.0f);

        int golesLocal = MuestrearPoisson(lambdaLocal);
        int golesVisitante = MuestrearPoisson(lambdaVisitante);

        return (golesLocal, golesVisitante);
    }

    public static (int golesLocal, int golesVisitante) ResolverEmpateSiCorresponde(
        string equipoLocal, string equipoVisitante, int golesLocal, int golesVisitante)
    {
        if (golesLocal != golesVisitante) return (golesLocal, golesVisitante);

        int puestoLocal = RankingFifaProvider.ObtenerPosicion(equipoLocal);
        int puestoVisitante = RankingFifaProvider.ObtenerPosicion(equipoVisitante);

        if (puestoLocal <= puestoVisitante)
        {
            golesLocal++;
        }
        else
        {
            golesVisitante++;
        }

        return (golesLocal, golesVisitante);
    }

    private static int MuestrearPoisson(float lambda)
    {
        float limite = Mathf.Exp(-lambda);
        int goles = -1;
        float productoAcumulado = 1f;

        do
        {
            goles++;
            productoAcumulado *= (float)GD.RandRange(0.0, 1.0);
        } while (productoAcumulado > limite);

        return goles;
    }
}
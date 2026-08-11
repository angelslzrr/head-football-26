using Godot;

/// <summary>
/// Motor de simulación estático puro. Desacoplado de la UI y del motor físico.
/// Utiliza modelos estadísticos profesionales para predecir marcadores en base al StarRating.
/// Al ser estático y puro, es fácilmente testeable de forma aislada.
/// </summary>
public static class SimulationEngine
{
    // Parámetro de diseño: Emula la ventaja estadística de jugar como local.
    private const float VentajaLocalia = 0.15f;

    // Multiplicador que define el impacto del StarRating en el cálculo de Goles Esperados (xG).
    private const float PesoDiferenciaEstrellas = 0.22f;

    public static (int golesLocal, int golesVisitante) SimularPartido(float estrellasLocal, float estrellasVisitante)
    {
        float diferencia = estrellasLocal - estrellasVisitante;

        // Lambda representa la media de goles esperados (xG) para la distribución.
        // Se aplica un Clamp para asegurar que los promedios se mantengan en rangos realistas (0.2 a 4.0).
        float lambdaLocal = Mathf.Clamp(1.3f + (diferencia * PesoDiferenciaEstrellas) + VentajaLocalia, 0.2f, 4.0f);
        float lambdaVisitante = Mathf.Clamp(1.3f - (diferencia * PesoDiferenciaEstrellas), 0.2f, 4.0f);

        int golesLocal = MuestrearPoisson(lambdaLocal);
        int golesVisitante = MuestrearPoisson(lambdaVisitante);

        return (golesLocal, golesVisitante);
    }

    /// <summary>
    /// Implementación del algoritmo de Knuth para generar números aleatorios 
    /// siguiendo una distribución de Poisson. Es el estándar matemático para simular 
    /// eventos independientes en un intervalo fijo (como goles en 90 minutos).
    /// </summary>
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
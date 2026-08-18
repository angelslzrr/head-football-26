public class LlaveEliminacion
{
    public int Ronda { get; set; }
    public int Posicion { get; set; }

    // Roles FIJOS de la ida.
    public string EquipoLocal { get; set; } = "";
    public string EquipoVisitante { get; set; } = "";

    public bool IdaYVuelta { get; set; } = false;

    public bool JugadoIda { get; set; } = false;
    public int GolesLocalIda { get; set; } = 0;
    public int GolesVisitanteIda { get; set; } = 0;

    // En la vuelta, "local" es EquipoVisitante jugando en su casa.
    public bool JugadoVuelta { get; set; } = false;
    public int GolesLocalVuelta { get; set; } = 0;
    public int GolesVisitanteVuelta { get; set; } = 0;

    public string Ganador { get; set; } = "";

    // --- Propiedades calculadas: se derivan solas cada vez que las llamas ---

    public bool Jugado => IdaYVuelta ? JugadoVuelta : JugadoIda;

    // Suma cruzada de goles para sacar el Global.
    public int GolesGlobalLocal => IdaYVuelta ? (GolesLocalIda + GolesVisitanteVuelta) : GolesLocalIda;
    public int GolesGlobalVisitante => IdaYVuelta ? (GolesVisitanteIda + GolesLocalVuelta) : GolesVisitanteIda;
}
using Godot;

/// <summary>
/// Controlador automatizado de IA que hereda la estructura física de Futbolista.
/// Intercepta los métodos de entrada virtuales para inyectar comandos calculados algorítmicamente.
/// Implementa una máquina de estados reactiva con fallos probabilísticos escalados al StarRating del equipo.
/// </summary>
public partial class FutbolistaIA : Futbolista
{
    [Export] public float DistanciaPatada = 60f;
    [Export] public float DistanciaSaltoX = 120f;
    [Export] public float UmbralSaltoY = 10f;

    [Export] public float ZonaMuertaX = 15f;

    [Export] public float LimiteAvanceSuelo = 1000f;
    [Export] public float NivelSueloPelota = 685f;
    [Export] public float AlturaAireMinima = 40f;

    // Escala base que define la dificultad/reacción de la IA según el equipo provisto por RepositorioEquipos.
    [Export] public float ReaccionEquipoFuerte = 0.03f;
    [Export] public float ReaccionEquipoDebil = 0.35f;

    [Export] public float ErrorPrediccionEquipoFuerte = 0f;
    [Export] public float ErrorPrediccionEquipoDebil = 70f;

    [Export] public float ProbabilidadFalloEquipoFuerte = 0.02f;
    [Export] public float ProbabilidadFalloEquipoDebil = 0.35f;

    private const float PosicionArcoPropio = 70f;

    public float StarRating { get; private set; } = 3.0f;

    private enum EstadoIA { Perseguir, Saltar, Patear, Defender }
    private EstadoIA _estadoActual = EstadoIA.Perseguir;

    private RigidBody2D _pelota;
    private Timer _timerReaccion;

    private float _direccionDecidida = 0f;
    private bool _saltarDecidido = false;
    private bool _patearDecidido = false;

    public override void _Ready()
    {
        base._Ready();

        _pelota = GetTree().GetFirstNodeInGroup("pelota") as RigidBody2D;

        _timerReaccion = new Timer();
        _timerReaccion.WaitTime = Mathf.Lerp(ReaccionEquipoDebil, ReaccionEquipoFuerte, NormalizarEstrellas());
        _timerReaccion.Autostart = true;
        _timerReaccion.Timeout += DecidirComportamiento;
        AddChild(_timerReaccion);
    }

    public void AplicarDificultad(float starRating)
    {
        StarRating = starRating;

        if (_timerReaccion != null)
        {
            _timerReaccion.WaitTime = Mathf.Lerp(ReaccionEquipoDebil, ReaccionEquipoFuerte, NormalizarEstrellas());
        }
    }

    private float NormalizarEstrellas()
    {
        return Mathf.Clamp((StarRating - 0.5f) / 4.5f, 0f, 1f);
    }

    // Ecuación cinemática paramétrica para predecir trayectoria a partir de gravedad y vectores actuales.
    private Vector2 PredecirPosicionPelota()
    {
        float horizonte = Mathf.Lerp(0.15f, 0.45f, NormalizarEstrellas());

        Vector2 gravedad = (Vector2)ProjectSettings.GetSetting("physics/2d/default_gravity_vector")
                          * (float)ProjectSettings.GetSetting("physics/2d/default_gravity");

        Vector2 posicionFutura = _pelota.GlobalPosition
            + (_pelota.LinearVelocity * horizonte)
            + (0.5f * gravedad * horizonte * horizonte);

        float margenError = Mathf.Lerp(ErrorPrediccionEquipoDebil, ErrorPrediccionEquipoFuerte, NormalizarEstrellas());
        posicionFutura.X += (float)GD.RandRange(-margenError, margenError);

        return posicionFutura;
    }

    // Núcleo del árbol de decisiones. Evalúa el estado del juego para asignar comandos.
    private void DecidirComportamiento()
    {
        if (_pelota == null) return;

        float distanciaPelotaAlArco = Mathf.Abs(_pelota.GlobalPosition.X - PosicionArcoPropio);
        float distanciaIAAlArco = Mathf.Abs(GlobalPosition.X - PosicionArcoPropio);

        if (distanciaPelotaAlArco < distanciaIAAlArco)
        {
            AplicarPanico(distanciaPelotaAlArco);
            return;
        }

        bool estaGanando = false;
        if (GameManager.Instance != null)
        {
            estaGanando = GameManager.Instance.GolesEquipo1 > GameManager.Instance.GolesEquipo2;
        }

        Vector2 posicionPredicha = PredecirPosicionPelota();
        Vector2 haciaPelota = posicionPredicha - GlobalPosition;
        float distanciaX = Mathf.Abs(haciaPelota.X);
        bool pelotaAdelante = Mathf.Sign(haciaPelota.X) == Mathf.Sign(DireccionJugador);
        bool pelotaBaja = haciaPelota.Y > -30f;

        float direccionDeseada = 0f;
        EstadoIA estadoDeseado = EstadoIA.Perseguir;

        if (estaGanando && _pelota.GlobalPosition.X > 620f)
        {
            float posicionDefensivaBase = 120f;
            float distanciaAlPunto = posicionDefensivaBase - GlobalPosition.X;

            direccionDeseada = Mathf.Abs(distanciaAlPunto) > 20f ? Mathf.Sign(distanciaAlPunto) : 0f;
            estadoDeseado = EstadoIA.Perseguir;
        }
        else
        {
            if (!pelotaAdelante && distanciaX > 30f)
            {
                direccionDeseada = -1f;

                if (distanciaX < 80f && !pelotaBaja)
                {
                    estadoDeseado = EstadoIA.Saltar;
                }
            }
            else
            {
                direccionDeseada = distanciaX < ZonaMuertaX ? 0f : Mathf.Sign(haciaPelota.X);
            }

            if (estadoDeseado != EstadoIA.Saltar)
            {
                if (distanciaX < DistanciaPatada && pelotaAdelante && pelotaBaja)
                {
                    estadoDeseado = EstadoIA.Patear;
                }
                else if (distanciaX < DistanciaSaltoX && haciaPelota.Y < -UmbralSaltoY)
                {
                    estadoDeseado = EstadoIA.Saltar;
                }
            }
        }

        // Lógica de "Zonificación": Previene congelamiento al forzar patrullajes fluidos 
        // cuando el jugador se excede en ataque.
        if (_pelota.GlobalPosition.X > LimiteAvanceSuelo)
        {
            float posicionEspera = LimiteAvanceSuelo - 150f; 
            
            if (GlobalPosition.X > posicionEspera)
            {
                direccionDeseada = -1f; 
            }
            else if (GlobalPosition.X < posicionEspera - 30f)
            {
                direccionDeseada = 1f; 
            }
            else 
            {
                direccionDeseada = 0f; 
            }
        }
        else if (GlobalPosition.X >= LimiteAvanceSuelo && direccionDeseada > 0)
        {
            direccionDeseada = -1f;
        }

        AplicarDecision(direccionDeseada, estadoDeseado);
    }

    private void AplicarPanico(float distanciaPelotaAlArco)
    {
        float direccionDeseada = Mathf.Sign(PosicionArcoPropio - GlobalPosition.X);
        EstadoIA estadoDeseado = EstadoIA.Defender;

        bool pelotaAlta = (NivelSueloPelota - _pelota.GlobalPosition.Y) > AlturaAireMinima;
        
        // Evita autogoles de barrida, obligando a la IA a saltar si el balón pasó rasante.
        if (!pelotaAlta)
        {
            estadoDeseado = EstadoIA.Saltar;
        }
        else
        {
            bool pelotaViajaHaciaElArco = Mathf.Sign(_pelota.LinearVelocity.X) == Mathf.Sign(PosicionArcoPropio - _pelota.GlobalPosition.X);

            if (pelotaViajaHaciaElArco && distanciaPelotaAlArco < DistanciaSaltoX)
            {
                estadoDeseado = EstadoIA.Saltar;
            }
        }

        AplicarDecision(direccionDeseada, estadoDeseado);
    }

    private void AplicarDecision(float direccionDeseada, EstadoIA estadoDeseado)
    {
        bool decisionDeSalto = estadoDeseado == EstadoIA.Saltar;

        // Inyección de "Error humano": Omite decisiones tácticas acertadas en base a RNG escalado.
        if (decisionDeSalto)
        {
            float probabilidadFallo = Mathf.Lerp(ProbabilidadFalloEquipoDebil, ProbabilidadFalloEquipoFuerte, NormalizarEstrellas());
            if (GD.Randf() < probabilidadFallo)
            {
                decisionDeSalto = false;
            }
        }

        _direccionDecidida = direccionDeseada;
        _estadoActual = estadoDeseado;
        _saltarDecidido = decisionDeSalto;
        _patearDecidido = estadoDeseado == EstadoIA.Patear;
    }

    // === MÉTODOS SOBRESCRITOS (INYECCIÓN DE IA A BASE CLASE) ===
    protected override float ObtenerDireccion()
    {
        return _direccionDecidida;
    }

    protected override bool ObtenerSaltar()
    {
        bool resultado = _saltarDecidido;
        _saltarDecidido = false;
        return resultado;
    }

    protected override bool ObtenerPatear()
    {
        return _patearDecidido;
    }
}
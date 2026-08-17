using Godot;

/// <summary>
/// Clase base para las entidades físicas de los jugadores (CharacterBody2D).
/// Implementa la lógica de cinemática (movimiento, salto), la mecánica de golpeo mediante 
/// interpolación rotacional de un Area2D, y la resolución de colisiones con el balón.
/// </summary>
public partial class Futbolista : CharacterBody2D
{
    [Export] public float Velocidad = 250f;
    [Export] public float FuerzaSalto = 450f;
    [Export] public float Gravedad = 950f;

    [Export] public float FuerzaPatada = 600f;
    [Export] public float AnguloMaximo = 90f;
    [Export] public float VelocidadSubida = 600f;
    [Export] public float VelocidadBajada = 450f;

    [Export] public float SensibilidadVerticalPatada = 1.8f;
    [Export] public float ElevacionMinima = 0.2f;

    [Export] public float ImpulsoMaximoColision = 900f;

    [Export] public float DireccionJugador = -1f;

    [Export] public Texture2D TexturaCabeza;
    [Export] public Texture2D TexturaCamiseta;
    [Export] public Texture2D TexturaChimpun;

    private Area2D _chimpun;
    private bool _yaGolpeoEstaPatada = false;

    public override void _Ready()
    {
        _chimpun = GetNode<Area2D>("Chimpun");

        AplicarTexturas(TexturaCabeza, TexturaCamiseta, TexturaChimpun);

        SafeMargin = 0.5f;
    }

    // Interfaz pública para aplicar assets dinámicos provistos por el TeamData.
    public void AplicarEquipo(TeamData equipo)
    {
        if (equipo == null) return;

        AplicarTexturas(equipo.CabezaTexture, equipo.CamisetaTexture, equipo.ChimpunTexture);
    }

    private void AplicarTexturas(Texture2D cabeza, Texture2D camiseta, Texture2D chimpun)
    {
        if (cabeza != null)
            GetNode<Sprite2D>("CabezaSprite").Texture = cabeza;

        if (camiseta != null)
            GetNode<Sprite2D>("CamisetaSprite").Texture = camiseta;

        if (chimpun != null)
            GetNode<Sprite2D>("Chimpun/ChimpunSprite").Texture = chimpun;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        if (!IsOnFloor()) velocity.Y += Gravedad * (float)delta;

        // Detiene cinemáticas de inmediato si el estado global dicta pausa.
        if (GameManager.Instance != null && (GameManager.Instance.JuegoPausado || GameManager.Instance.PartidoTerminado))
        {
            velocity.X = 0;
            Velocity = velocity;
            MoveAndSlide();

            if (_chimpun.RotationDegrees > 0f)
            {
                _chimpun.RotationDegrees = Mathf.Max(_chimpun.RotationDegrees - VelocidadBajada * (float)delta, 0f);
            }

            return;
        }

        if (IsOnFloor() && ObtenerSaltar()) velocity.Y = -FuerzaSalto;

        // Limita la velocidad de rebote vertical hacia arriba.
        if (velocity.Y < -FuerzaSalto * 1.2f)
        {
            velocity.Y = -FuerzaSalto * 1.2f;
        }

        float direccion = ObtenerDireccion();
        velocity.X = direccion * Velocidad;

        Vector2 velocidadAntesDeChocar = velocity;
        Velocity = velocity;

        MoveAndSlide();

        if (Velocity.Y < -FuerzaSalto * 1.3f)
        {
            Velocity = new Vector2(Velocity.X, -FuerzaSalto * 1.3f);
        }

        // Resolución customizada de colisiones contra el RigidBody2D (Pelota).
        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            KinematicCollision2D colision = GetSlideCollision(i);

            if (colision.GetCollider() is RigidBody2D pelota)
            {
                Vector2 normal = colision.GetNormal();

                bool aterrizajeSobrePelota = normal.Y < -0.7f && velocidadAntesDeChocar.Y >= 0f;

                if (aterrizajeSobrePelota)
                {
                    Vector2 vCorregida = Velocity;
                    if (vCorregida.Y < 0f) vCorregida.Y = 0f;
                    Velocity = vCorregida;
                }
                else
                {
                    if (normal.Y > -0.5f)
                    {
                        Velocity = velocidadAntesDeChocar;
                    }

                    Vector2 direccionImpacto = -normal;

                    float fuerzaImpacto = Mathf.Min(
                        Mathf.Max(velocidadAntesDeChocar.Length() * 0.8f, 150f),
                        ImpulsoMaximoColision
                    );
                    pelota.ApplyCentralImpulse(direccionImpacto * fuerzaImpacto);
                }
            }
        }

        ProcesarMovimientoPie((float)delta);
    }

    private void ProcesarMovimientoPie(float delta)
    {
        float anguloActual = _chimpun.RotationDegrees;

        if (ObtenerPatear())
        {
            if (anguloActual < AnguloMaximo)
            {
                _chimpun.RotationDegrees = Mathf.Min(anguloActual + VelocidadSubida * delta, AnguloMaximo);
            }
            RevisarImpactoPatada();
        }
        else
        {
            if (anguloActual > 0f)
            {
                _chimpun.RotationDegrees = Mathf.Max(anguloActual - VelocidadBajada * delta, 0f);
            }
            else
            {
                _yaGolpeoEstaPatada = false;
            }
        }
    }

    // Calcula vectores direccionales para transferir impulsos al balón (Shoot Mechanic).
    private void RevisarImpactoPatada()
    {
        foreach (Node2D body in _chimpun.GetOverlappingBodies())
        {
            if (body is RigidBody2D pelota)
            {
                Vector2 offsetImpacto = pelota.GlobalPosition - _chimpun.GlobalPosition;

                offsetImpacto.Y *= SensibilidadVerticalPatada;
                Vector2 direccionImpacto = offsetImpacto.Normalized();

                if (direccionImpacto.Y > -ElevacionMinima)
                {
                    direccionImpacto.Y = -ElevacionMinima;
                    direccionImpacto = direccionImpacto.Normalized();
                }

                if (!_yaGolpeoEstaPatada)
                {
                    pelota.LinearVelocity = pelota.LinearVelocity * 0.3f;
                    Vector2 impulsoDelJugador = Velocity * 0.6f;
                    Vector2 impulsoTotal = (direccionImpacto * FuerzaPatada) + impulsoDelJugador;

                    if (impulsoTotal.Length() > ImpulsoMaximoColision)
                    {
                        impulsoTotal = impulsoTotal.Normalized() * ImpulsoMaximoColision;
                    }

                    pelota.ApplyCentralImpulse(impulsoTotal);
                    _yaGolpeoEstaPatada = true;
                }
                else
                {
                    pelota.ApplyCentralImpulse(direccionImpacto * 15f);
                }
                break;
            }
        }
    }

    // Métodos virtuales que permiten a las clases hijas (Humano o IA) inyectar sus decisiones.
    protected virtual float ObtenerDireccion()
    {
        return Input.GetAxis("mover_izquierda", "mover_derecha");
    }

    protected virtual bool ObtenerSaltar()
    {
        return Input.IsActionJustPressed("saltar");
    }

    protected virtual bool ObtenerPatear()
    {
        return Input.IsActionPressed("patear");
    }
}
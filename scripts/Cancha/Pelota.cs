using Godot;

/// <summary>
/// Control de integración de fuerzas para la entidad RigidBody2D del Balón.
/// Impide fallos cinemáticos extremando la velocidad máxima, sin importar 
/// bugs o transferencias extremas de impulso procedentes de los jugadores.
/// </summary>
public partial class Pelota : RigidBody2D
{
    private AudioStreamPlayer2D _sonidoRebote;

    [Export] public float UmbralVelocidad = 200.0f;
    [Export] public float VelocidadMaxima = 1400f;

    public override void _Ready()
    {
        _sonidoRebote = GetNode<AudioStreamPlayer2D>("SonidoRebote");
    }

    // Intercepción a bajo nivel del PhysicsServer2D en cada Tick Físico.
    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        if (state.LinearVelocity.Length() > VelocidadMaxima)
        {
            state.LinearVelocity = state.LinearVelocity.Normalized() * VelocidadMaxima;
        }
    }

    private void _OnBodyEntered(Node body)
    {
        if (_sonidoRebote != null && LinearVelocity.Length() > UmbralVelocidad)
        {
            // Alteración dinámica del tono para generar realismo perceptivo en choques iterativos.
            _sonidoRebote.PitchScale = (float)GD.RandRange(0.9f, 1.1f);

            _sonidoRebote.Stop();
            _sonidoRebote.Play();
        }
    }
}
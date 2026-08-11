using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Controlador principal de la escena de juego 2D (Gameplay).
/// Orquesta el ciclo de vida del partido, la instanciación de texturas de los equipos,
/// el control del flujo del tiempo (pausas al anotar) y la reposición física de entidades.
/// </summary>
public partial class Cancha : Node2D
{
    [Export] public RigidBody2D Pelota;
    [Export] public Vector2 PosicionInicialPelota = new Vector2(640, 300);

    [Export] public CharacterBody2D Jugador1;
    [Export] public Vector2 PosicionInicialJugador1 = new Vector2(946, 618);

    [Export] public CharacterBody2D Rival;
    [Export] public Vector2 PosicionInicialRival = new Vector2(334, 618);

    private bool _golEnProceso = false;

    private AudioStreamPlayer _silbatoInicio;
    private AudioStreamPlayer _gritoGol;

    public override async void _Ready()
    {
        GetNode<AudioStreamPlayer>("/root/MusicaGlobal").Stop();
        
        // Garantiza que el estado global esté limpio antes de arrancar la simulación física.
        GameManager.Instance.ReiniciarPartido();

        _silbatoInicio = GetNode<AudioStreamPlayer>("SilbatoInicio");
        _gritoGol = GetNode<AudioStreamPlayer>("GritoGol");

        // Inyección de dependencias visuales si el partido proviene del flujo del torneo.
        if (PuenteTorneo.Instance.PartidoDeTorneo)
        {
            ConfigurarEquiposDeTorneo();
        }

        GameManager.Instance.JuegoPausado = true;
        _silbatoInicio.Play();

        await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);

        GameManager.Instance.JuegoPausado = false;
    }

    private void ConfigurarEquiposDeTorneo()
    {
        List<TeamData> equipos = RepositorioEquipos.ObtenerEquiposConmebol();

        TeamData equipoJugador = equipos.FirstOrDefault(e => e.TeamName == PuenteTorneo.Instance.EquipoJugador);
        TeamData equipoRival = equipos.FirstOrDefault(e => e.TeamName == PuenteTorneo.Instance.EquipoRival);

        // Polimorfismo: Jugador1 y Rival heredan de Futbolista, por lo que comparten la interfaz de configuración visual.
        (Jugador1 as Futbolista)?.AplicarEquipo(equipoJugador);
        (Rival as Futbolista)?.AplicarEquipo(equipoRival);

        if (Rival is FutbolistaIA rivalIA && equipoRival != null)
        {
            rivalIA.AplicarDificultad(equipoRival.StarRating);
        }
    }

    private void OnPorteriaIzquierdaBodyEntered(Node2D body)
    {
        if (body is RigidBody2D && !_golEnProceso && !GameManager.Instance.PartidoTerminado)
        {
            ProcesarGol(2);
        }
    }

    private void OnPorteriaDerechaBodyEntered(Node2D body)
    {
        if (body is RigidBody2D && !_golEnProceso && !GameManager.Instance.PartidoTerminado)
        {
            ProcesarGol(1);
        }
    }

    // Gestiona la cinemática de gol, actualiza el marcador y reinicia las posiciones usando el PhysicsServer2D.
    private async void ProcesarGol(int equipo)
    {
        _golEnProceso = true;

        _gritoGol.VolumeDb = -7f;
        _gritoGol.Play();

        Tween tween = CreateTween();
        tween.TweenProperty(_gritoGol, "volume_db", -30f, 2.0f);

        GameManager.Instance.AnotarGol(equipo);
        GameManager.Instance.JuegoPausado = true;

        await ToSignal(GetTree().CreateTimer(1.5f), SceneTreeTimer.SignalName.Timeout);

        // Manipulación directa del estado físico del RigidBody para evitar bugs de interpolación.
        Transform2D nuevaPosicion = new Transform2D(0, PosicionInicialPelota);
        PhysicsServer2D.BodySetState(Pelota.GetRid(), PhysicsServer2D.BodyState.Transform, nuevaPosicion);
        PhysicsServer2D.BodySetState(Pelota.GetRid(), PhysicsServer2D.BodyState.LinearVelocity, Vector2.Zero);
        PhysicsServer2D.BodySetState(Pelota.GetRid(), PhysicsServer2D.BodyState.AngularVelocity, 0f);

        if (Jugador1 != null)
        {
            Jugador1.GlobalPosition = PosicionInicialJugador1;
            Jugador1.Velocity = Vector2.Zero;
        }

        if (Rival != null)
        {
            Rival.GlobalPosition = PosicionInicialRival;
            Rival.Velocity = Vector2.Zero;
        }

        _gritoGol.Stop();

        if (!GameManager.Instance.PartidoTerminado)
        {
            _silbatoInicio.Play();
            await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
            GameManager.Instance.JuegoPausado = false;
        }

        _golEnProceso = false;
    }
}
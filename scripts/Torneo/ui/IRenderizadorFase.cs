using Godot;

public interface IRenderizadorFase
{
    bool OcultaPanelDetalleEquipo { get; }
    void DibujarPosiciones(Control contenedorPosiciones, FaseTorneo fase, string nombreEquipoJugador);
    void DibujarFixture(Control contenedorFixture, FaseTorneo fase, string nombreEquipoJugador);
}
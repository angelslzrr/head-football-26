using Godot;
using System.Collections.Generic;
using System.Linq;

public class RenderizadorRoundRobin : IRenderizadorFase
{
    public bool OcultaPanelDetalleEquipo => false;

    public void DibujarPosiciones(Control contenedorPosiciones, FaseTorneo fase, string nombreEquipoJugador)
    {
        foreach (Node hijo in contenedorPosiciones.GetChildren()) hijo.QueueFree();

        var grid = new GridContainer { Columns = 8 };
        grid.AddThemeConstantOverride("h_separation", 2);
        grid.AddThemeConstantOverride("v_separation", 2);
        contenedorPosiciones.AddChild(grid);

        var (directos, repechaje) = ZonasClasificacionHelper.ObtenerZonas(fase);
        UiTorneoHelper.DibujarTabla(grid, fase.TablaPosiciones, nombreEquipoJugador, directos, repechaje);
    }

    public void DibujarFixture(Control contenedorFixture, FaseTorneo fase, string nombreEquipoJugador)
    {
        foreach (Node hijo in contenedorFixture.GetChildren()) hijo.QueueFree();

        var porJornada = fase.Calendario.GroupBy(p => p.Jornada).OrderBy(g => g.Key);
        var filasParaAnimar = new List<Control>();

        foreach (var grupo in porJornada)
        {
            contenedorFixture.AddChild(UiTorneoHelper.CrearEncabezadoSeccion($"Jornada {grupo.Key}"));

            foreach (PartidoFixture p in grupo)
            {
                Control fila = UiTorneoHelper.CrearFilaPartido(p.EquipoLocal, p.EquipoVisitante, p.Jugado, p.GolesLocal, p.GolesVisitante, nombreEquipoJugador);
                contenedorFixture.AddChild(fila);
                filasParaAnimar.Add(fila);
            }

            contenedorFixture.AddChild(new Control { CustomMinimumSize = new Vector2(0, 8) });
        }

        UiTorneoHelper.AnimarAparicionEscalonada(filasParaAnimar, 1);
    }
}
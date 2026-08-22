using Godot;
using System.Collections.Generic;

public class RenderizadorGrupos : IRenderizadorFase
{
    public bool OcultaPanelDetalleEquipo => false;

    public void DibujarPosiciones(Control contenedorPosiciones, FaseTorneo fase, string nombreEquipoJugador)
    {
        foreach (Node hijo in contenedorPosiciones.GetChildren()) hijo.QueueFree();

        var flow = new HFlowContainer();
        flow.AddThemeConstantOverride("h_separation", 20);
        flow.AddThemeConstantOverride("v_separation", 20);
        contenedorPosiciones.AddChild(flow);

        // Obtenemos quién va de verde y quién de azul para toda la fase
        var (directos, repechaje) = ZonasClasificacionHelper.ObtenerZonas(fase);

        foreach (GrupoTorneo grupo in fase.Grupos)
        {
            var columna = new VBoxContainer { CustomMinimumSize = new Vector2(420, 0) };

            var titulo = new Label { Text = grupo.Nombre, HorizontalAlignment = HorizontalAlignment.Left };
            titulo.AddThemeFontSizeOverride("font_size", 18);
            titulo.AddThemeColorOverride("font_color", UiTorneoHelper.ColorEncabezado);
            columna.AddChild(titulo);

            var grid = new GridContainer { Columns = 8 };
            columna.AddChild(grid);

            flow.AddChild(columna);

            // Le pasamos las zonas al dibujante
            UiTorneoHelper.DibujarTabla(grid, grupo.TablaPosiciones, nombreEquipoJugador, directos, repechaje);
        }
    }

    public void DibujarFixture(Control contenedorFixture, FaseTorneo fase, string nombreEquipoJugador)
    {
        foreach (Node hijo in contenedorFixture.GetChildren()) hijo.QueueFree();

        var filasParaAnimar = new List<Control>();

        foreach (GrupoTorneo grupo in fase.Grupos)
        {
            contenedorFixture.AddChild(UiTorneoHelper.CrearEncabezadoSeccion(grupo.Nombre));

            foreach (PartidoFixture p in grupo.Calendario)
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
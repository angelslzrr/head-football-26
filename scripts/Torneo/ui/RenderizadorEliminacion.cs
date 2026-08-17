using Godot;
using System.Collections.Generic;
using System.Linq;

public class RenderizadorEliminacion : IRenderizadorFase
{
    public bool OcultaPanelDetalleEquipo => true;

    private const int AnchoCaja = 260;
    private const int AltoCaja = 60;
    private const int SeparacionBase = 16;

    public void DibujarPosiciones(Control contenedorPosiciones, FaseTorneo fase, string nombreEquipoJugador)
    {
        foreach (Node hijo in contenedorPosiciones.GetChildren()) hijo.QueueFree();
        if (fase.Llaves.Count == 0) return;

        var filas = new HBoxContainer();
        filas.AddThemeConstantOverride("separation", 40);
        contenedorPosiciones.AddChild(filas);

        int rondaMaxima = fase.Llaves.Max(l => l.Ronda);

        for (int ronda = 1; ronda <= rondaMaxima; ronda++)
        {
            List<LlaveEliminacion> llavesRonda = fase.Llaves
                .Where(l => l.Ronda == ronda)
                .OrderBy(l => l.Posicion)
                .ToList();

            var columna = new VBoxContainer { CustomMinimumSize = new Vector2(AnchoCaja, 0) };

            float separacion = SeparacionBase * Mathf.Pow(2, ronda - 1);
            columna.AddThemeConstantOverride("separation", (int)separacion);

            var titulo = new Label
            {
                Text = NombreDeRonda(ronda, rondaMaxima),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            titulo.AddThemeColorOverride("font_color", UiTorneoHelper.ColorEncabezado);
            titulo.AddThemeFontSizeOverride("font_size", 16);
            columna.AddChild(titulo);

            if (ronda > 1)
                columna.AddChild(new Control { CustomMinimumSize = new Vector2(0, separacion / 2f) });

            foreach (LlaveEliminacion llave in llavesRonda)
                columna.AddChild(CrearCajaLlave(llave, nombreEquipoJugador));

            filas.AddChild(columna);
        }
    }

    private string NombreDeRonda(int ronda, int rondaMaxima)
    {
        return (rondaMaxima - ronda) switch
        {
            0 => "Final",
            1 => "Semifinales",
            2 => "Cuartos de Final",
            _ => $"Ronda {ronda}"
        };
    }

    private Control CrearCajaLlave(LlaveEliminacion llave, string nombreEquipoJugador)
    {
        bool esPartidoJugador = llave.EquipoLocal == nombreEquipoJugador || llave.EquipoVisitante == nombreEquipoJugador;

        var estilo = new StyleBoxFlat
        {
            BgColor = new Color(0.03f, 0.07f, 0.04f, 0.6f),
            ContentMarginLeft = 10,
            ContentMarginRight = 10,
            ContentMarginTop = 6,
            ContentMarginBottom = 6,
            CornerRadiusTopLeft = 6,
            CornerRadiusTopRight = 6,
            CornerRadiusBottomLeft = 6,
            CornerRadiusBottomRight = 6
        };
        if (esPartidoJugador)
        {
            estilo.BorderColor = UiTorneoHelper.ColorBordeJugador;
            estilo.BorderWidthLeft = 3;
        }

        var panel = new PanelContainer { CustomMinimumSize = new Vector2(AnchoCaja, AltoCaja) };
        panel.AddThemeStyleboxOverride("panel", estilo);

        var caja = new VBoxContainer();
        caja.AddChild(CrearFilaEquipo(llave.EquipoLocal, llave.Jugado ? llave.GolesLocal.ToString() : "", llave.Ganador == llave.EquipoLocal));
        caja.AddChild(CrearFilaEquipo(llave.EquipoVisitante, llave.Jugado ? llave.GolesVisitante.ToString() : "", llave.Ganador == llave.EquipoVisitante));

        panel.AddChild(caja);
        return panel;
    }

    private Control CrearFilaEquipo(string nombreEquipo, string goles, bool esGanador)
    {
        var fila = new HBoxContainer();

        var nombre = new Label
        {
            Text = string.IsNullOrEmpty(nombreEquipo) ? "Por definir" : nombreEquipo,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        if (esGanador) nombre.AddThemeColorOverride("font_color", UiTorneoHelper.ColorEncabezado);

        fila.AddChild(nombre);
        fila.AddChild(new Label { Text = goles, HorizontalAlignment = HorizontalAlignment.Right });
        return fila;
    }

    public void DibujarFixture(Control contenedorFixture, FaseTorneo fase, string nombreEquipoJugador)
    {
        foreach (Node hijo in contenedorFixture.GetChildren()) hijo.QueueFree();

        contenedorFixture.AddChild(new Label
        {
            Text = "El calendario de esta fase se muestra como árbol en la pestaña de Posiciones.",
            AutowrapMode = TextServer.AutowrapMode.WordSmart,
            HorizontalAlignment = HorizontalAlignment.Center
        });
    }
}
using Godot;
using System.Collections.Generic;
using System.Linq;

public static class UiTorneoHelper
{
    public static readonly Color ColorFilaPar = new Color(0.06f, 0.12f, 0.07f, 0.5f);
    public static readonly Color ColorFilaImpar = new Color(0.03f, 0.07f, 0.04f, 0.5f);
    public static readonly Color ColorFilaJugador = new Color(0.87f, 0.73f, 0f, 0.16f);
    public static readonly Color ColorBordeJugador = new Color(0.9f, 0.78f, 0.15f, 1f);
    public static readonly Color ColorEncabezado = new Color(0.9f, 0.78f, 0.15f, 1f);
    public static readonly Color ColorTransparente = new Color(0, 0, 0, 0);

    public static void AgregarCelda(GridContainer grid, string texto, Color colorFondo, int anchoMinimo,
        bool esEncabezado = false, bool bordeIzquierdo = false,
        HorizontalAlignment alineacion = HorizontalAlignment.Center)
    {
        var estilo = new StyleBoxFlat
        {
            BgColor = colorFondo,
            ContentMarginLeft = 6,
            ContentMarginRight = 6,
            ContentMarginTop = 8,
            ContentMarginBottom = 8
        };

        if (esEncabezado)
        {
            estilo.BorderColor = new Color(ColorEncabezado.R, ColorEncabezado.G, ColorEncabezado.B, 0.6f);
            estilo.BorderWidthBottom = 2;
        }

        if (bordeIzquierdo)
        {
            estilo.BorderColor = ColorBordeJugador;
            estilo.BorderWidthLeft = 3;
        }

        var panel = new PanelContainer();
        panel.AddThemeStyleboxOverride("panel", estilo);
        panel.CustomMinimumSize = new Vector2(anchoMinimo, 0);

        var label = new Label
        {
            Text = texto,
            HorizontalAlignment = alineacion,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };

        if (esEncabezado) label.AddThemeColorOverride("font_color", ColorEncabezado);

        panel.AddChild(label);
        grid.AddChild(panel);
    }

    public static void DibujarTabla(GridContainer grid, List<EstadisticasEquipoGuardado> tabla, string nombreEquipoJugador)
    {
        foreach (Node hijo in grid.GetChildren()) hijo.QueueFree();

        string[] encabezados = { "#", "Equipo", "PJ", "G", "E", "P", "DG", "Pts" };
        int[] anchos = { 50, 240, 55, 55, 55, 55, 60, 55 };

        for (int i = 0; i < encabezados.Length; i++)
            AgregarCelda(grid, encabezados[i], ColorTransparente, anchos[i], esEncabezado: true);

        List<EstadisticasEquipoGuardado> ordenados = tabla
            .OrderByDescending(e => e.Puntos)
            .ThenByDescending(e => e.DiferenciaGoles)
            .ThenByDescending(e => e.GolesFavor)
            .ToList();

        var celdasParaAnimar = new List<Control>();

        for (int i = 0; i < ordenados.Count; i++)
        {
            EstadisticasEquipoGuardado equipo = ordenados[i];
            bool esJugador = equipo.NombreEquipo == nombreEquipoJugador;
            Color colorFila = esJugador ? ColorFilaJugador : (i % 2 == 0 ? ColorFilaPar : ColorFilaImpar);

            int inicioFila = grid.GetChildCount();

            AgregarCelda(grid, (i + 1).ToString(), colorFila, anchos[0], bordeIzquierdo: esJugador);
            AgregarCelda(grid, equipo.NombreEquipo, colorFila, anchos[1], alineacion: HorizontalAlignment.Center);
            AgregarCelda(grid, equipo.Jugados.ToString(), colorFila, anchos[2]);
            AgregarCelda(grid, equipo.Ganados.ToString(), colorFila, anchos[3]);
            AgregarCelda(grid, equipo.Empatados.ToString(), colorFila, anchos[4]);
            AgregarCelda(grid, equipo.Perdidos.ToString(), colorFila, anchos[5]);
            AgregarCelda(grid, equipo.DiferenciaGoles.ToString(), colorFila, anchos[6]);
            AgregarCelda(grid, equipo.Puntos.ToString(), colorFila, anchos[7]);

            for (int c = inicioFila; c < grid.GetChildCount(); c++)
                celdasParaAnimar.Add(grid.GetChild<Control>(c));
        }

        AnimarAparicionEscalonada(celdasParaAnimar, 8);
    }

    public static Control CrearFilaPartido(string equipoLocal, string equipoVisitante, bool jugado, int golesLocal, int golesVisitante, string nombreEquipoJugador)
    {
        bool jugadorLocal = equipoLocal == nombreEquipoJugador;
        bool jugadorVisitante = equipoVisitante == nombreEquipoJugador;
        bool esPartidoJugador = jugadorLocal || jugadorVisitante;

        var estiloFila = new StyleBoxFlat
        {
            BgColor = esPartidoJugador ? ColorFilaJugador : ColorTransparente,
            ContentMarginLeft = 8,
            ContentMarginRight = 8,
            ContentMarginTop = 8,
            ContentMarginBottom = 8
        };
        if (esPartidoJugador)
        {
            estiloFila.BorderColor = ColorBordeJugador;
            estiloFila.BorderWidthLeft = 3;
        }

        var panelFila = new PanelContainer();
        panelFila.AddThemeStyleboxOverride("panel", estiloFila);
        panelFila.CustomMinimumSize = new Vector2(550, 0);

        var fila = new HBoxContainer();
        fila.AddThemeConstantOverride("separation", 8);

        var labelLocal = new Label
        {
            Text = equipoLocal,
            HorizontalAlignment = HorizontalAlignment.Right,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        if (jugadorLocal) labelLocal.AddThemeColorOverride("font_color", ColorBordeJugador);

        var labelVisitante = new Label
        {
            Text = equipoVisitante,
            HorizontalAlignment = HorizontalAlignment.Left,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        if (jugadorVisitante) labelVisitante.AddThemeColorOverride("font_color", ColorBordeJugador);

        var contenedorMarcador = new PanelContainer { CustomMinimumSize = new Vector2(64, 0) };
        var estiloMarcador = new StyleBoxFlat
        {
            BgColor = new Color(0.02f, 0.05f, 0.03f, 0.6f),
            CornerRadiusTopLeft = 6,
            CornerRadiusTopRight = 6,
            CornerRadiusBottomLeft = 6,
            CornerRadiusBottomRight = 6,
            ContentMarginTop = 3,
            ContentMarginBottom = 3
        };
        contenedorMarcador.AddThemeStyleboxOverride("panel", estiloMarcador);

        var labelMarcador = new Label
        {
            Text = jugado ? $"{golesLocal} - {golesVisitante}" : "vs",
            HorizontalAlignment = HorizontalAlignment.Center
        };
        if (jugado)
        {
            labelMarcador.AddThemeFontSizeOverride("font_size", 16);
            labelMarcador.AddThemeColorOverride("font_color", ColorEncabezado);
        }
        else
        {
            labelMarcador.AddThemeColorOverride("font_color", new Color(1, 1, 1, 0.5f));
        }

        contenedorMarcador.AddChild(labelMarcador);
        fila.AddChild(labelLocal);
        fila.AddChild(contenedorMarcador);
        fila.AddChild(labelVisitante);
        panelFila.AddChild(fila);
        return panelFila;
    }

    public static Control CrearEncabezadoSeccion(string texto)
    {
        var contenedor = new VBoxContainer();
        contenedor.AddThemeConstantOverride("separation", 3);

        var label = new Label { Text = texto, HorizontalAlignment = HorizontalAlignment.Left };
        label.AddThemeFontSizeOverride("font_size", 17);
        label.AddThemeColorOverride("font_color", ColorEncabezado);

        var linea = new HSeparator();
        var estiloLinea = new StyleBoxFlat
        {
            BgColor = new Color(ColorEncabezado.R, ColorEncabezado.G, ColorEncabezado.B, 0.35f),
            ContentMarginTop = 1,
            ContentMarginBottom = 1
        };
        linea.AddThemeStyleboxOverride("separator", estiloLinea);

        contenedor.AddChild(label);
        contenedor.AddChild(linea);
        return contenedor;
    }

    public static void AnimarAparicionEscalonada(List<Control> elementos, int celdasPorFila)
    {
        for (int i = 0; i < elementos.Count; i++)
        {
            Control elemento = elementos[i];
            if (!GodotObject.IsInstanceValid(elemento)) return;
            int fila = i / celdasPorFila;

            elemento.Modulate = new Color(1, 1, 1, 0);
            Tween tween = elemento.CreateTween();
            tween.TweenInterval(fila * 0.025f);
            tween.TweenProperty(elemento, "modulate:a", 1.0f, 0.2f);
        }
    }
}
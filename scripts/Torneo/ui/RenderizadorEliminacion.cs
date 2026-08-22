using Godot;
using System.Collections.Generic;
using System.Linq;

public class RenderizadorEliminacion : IRenderizadorFase
{
    public bool OcultaPanelDetalleEquipo => true;

    private const int AnchoCaja = 260;
    private const int AnchoCajaIdaYVuelta = 340;
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

            bool esIdaYVuelta = llavesRonda.Count > 0 && llavesRonda[0].IdaYVuelta;
            int anchoUsado = esIdaYVuelta ? AnchoCajaIdaYVuelta : AnchoCaja;

            var columna = new VBoxContainer { CustomMinimumSize = new Vector2(anchoUsado, 0) };

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

            // NUEVO: Detectamos si esta es la ronda final de la fase
            bool esRondaFinal = ronda == rondaMaxima;   

            foreach (LlaveEliminacion llave in llavesRonda)
            {
                // Le pasamos la 'fase' y 'esRondaFinal' a la función
                columna.AddChild(CrearCajaLlave(llave, anchoUsado, nombreEquipoJugador, fase, esRondaFinal));
            }

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

    private Control CrearCajaLlave(LlaveEliminacion llave, int ancho, string nombreEquipoJugador, FaseTorneo fase, bool esRondaFinal)
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

        var panel = new PanelContainer { CustomMinimumSize = new Vector2(ancho, AltoCaja) };
        panel.AddThemeStyleboxOverride("panel", estilo);

        var caja = new VBoxContainer();
        // Le añadimos nombreEquipoJugador al final de los parámetros
        caja.AddChild(CrearFilaEquipo(llave, esLocal: true, fase, esRondaFinal, nombreEquipoJugador));    
        caja.AddChild(CrearFilaEquipo(llave, esLocal: false, fase, esRondaFinal, nombreEquipoJugador));   

        panel.AddChild(caja);
        return panel;
    }

    private Control CrearFilaEquipo(LlaveEliminacion llave, bool esLocal, FaseTorneo fase, bool esRondaFinal, string nombreEquipoJugador)
    {
        string nombreEquipo = esLocal ? llave.EquipoLocal : llave.EquipoVisitante;
        bool esGanador = !string.IsNullOrEmpty(llave.Ganador) && llave.Ganador == nombreEquipo;
        bool esEquipoDelJugador = nombreEquipo == nombreEquipoJugador;

        var fila = new HBoxContainer();
        fila.AddThemeConstantOverride("separation", 4);

        var nombre = new Label
        {
            Text = string.IsNullOrEmpty(nombreEquipo) ? "Por definir" : nombreEquipo,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        
        // El texto SOLO es dorado si es el equipo del jugador. Si no, es blanco.
        if (esEquipoDelJugador) 
            nombre.AddThemeColorOverride("font_color", UiTorneoHelper.ColorEncabezado);
        else 
            nombre.AddThemeColorOverride("font_color", new Color(1, 1, 1));
            
        fila.AddChild(nombre);

        if (llave.IdaYVuelta)
        {
            fila.AddChild(CrearCeldaMarcador(ObtenerGolesIda(llave, esLocal), llave.JugadoIda));
            fila.AddChild(CrearCeldaMarcador(ObtenerGolesVuelta(llave, esLocal), llave.JugadoVuelta));
            fila.AddChild(new VSeparator());
            fila.AddChild(CrearCeldaMarcador(ObtenerGolesGlobal(llave, esLocal), llave.Jugado, esGlobal: true));
        }
        else
        {
            fila.AddChild(CrearCeldaMarcador(ObtenerGolesGlobal(llave, esLocal), llave.Jugado));
        }

        // ALINEACIÓN PERFECTA y COLORES DE ZONA
        Color colorZona = UiTorneoHelper.ColorTransparente;
        
        if (llave.Jugado) // Solo pintamos si el partido ya terminó
        {
            if (esGanador)
            {
                bool vaARepechaje = esRondaFinal && fase.GanadorEsRepechaje;
                colorZona = vaARepechaje ? UiTorneoHelper.ColorFilaRepechaje : UiTorneoHelper.ColorFilaDirecta;
            }
            else
            {
                // NUEVO: Si perdió la final y esa fase manda al perdedor al repechaje (OFC)
                if (esRondaFinal && fase.PerdedorEsRepechaje)
                {
                    colorZona = UiTorneoHelper.ColorFilaRepechaje;
                }
            }
        }

        var panelFila = new PanelContainer();
        var estiloFila = new StyleBoxFlat
        {
            BgColor = colorZona,
            ContentMarginLeft = 4,
            ContentMarginRight = 4,
            ContentMarginTop = 2,
            ContentMarginBottom = 2,
            CornerRadiusTopLeft = 4,
            CornerRadiusTopRight = 4,
            CornerRadiusBottomLeft = 4,
            CornerRadiusBottomRight = 4
        };
        panelFila.AddThemeStyleboxOverride("panel", estiloFila);
        panelFila.AddChild(fila);
        
        return panelFila;
    }

    // En la vuelta los roles de cancha se invierten: el que era local en la ida
    // ahora juega de visitante, así que sus goles quedan guardados en GolesVisitanteVuelta.
    private int ObtenerGolesIda(LlaveEliminacion l, bool esLocal) => esLocal ? l.GolesLocalIda : l.GolesVisitanteIda;
    private int ObtenerGolesVuelta(LlaveEliminacion l, bool esLocal) => esLocal ? l.GolesVisitanteVuelta : l.GolesLocalVuelta;
    private int ObtenerGolesGlobal(LlaveEliminacion l, bool esLocal) => esLocal ? l.GolesGlobalLocal : l.GolesGlobalVisitante;

    private Control CrearCeldaMarcador(int goles, bool jugado, bool esGlobal = false)
    {
        var contenedor = new PanelContainer { CustomMinimumSize = new Vector2(28, 0) };

        var estilo = new StyleBoxFlat
        {
            BgColor = esGlobal ? new Color(0.87f, 0.73f, 0f, 0.14f) : new Color(0.02f, 0.05f, 0.03f, 0.6f),
            ContentMarginLeft = 3,
            ContentMarginRight = 3,
            ContentMarginTop = 2,
            ContentMarginBottom = 2,
            CornerRadiusTopLeft = 4,
            CornerRadiusTopRight = 4,
            CornerRadiusBottomLeft = 4,
            CornerRadiusBottomRight = 4
        };
        contenedor.AddThemeStyleboxOverride("panel", estilo);

        var label = new Label
        {
            Text = jugado ? goles.ToString() : "-",
            HorizontalAlignment = HorizontalAlignment.Center
        };
        if (esGlobal) label.AddThemeColorOverride("font_color", UiTorneoHelper.ColorEncabezado);

        contenedor.AddChild(label);
        return contenedor;
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
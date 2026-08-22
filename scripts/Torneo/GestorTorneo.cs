using Godot;
using System.Collections.Generic;
using System.Linq;

public static class GestorTorneo
{
    public static IGeneradorFormato ObtenerGenerador(TipoFormato tipo)
    {
        return tipo switch
        {
            TipoFormato.RoundRobin => new GeneradorRoundRobin(),
            TipoFormato.Grupos => new GeneradorGrupos(),
            TipoFormato.Eliminacion => new GeneradorEliminacion(),
            _ => throw new System.Exception($"Tipo de formato no soportado: {tipo}")
        };
    }

    public static void IniciarTorneo(TournamentState estado, List<FaseTorneo> fases, List<string> equipos)
    {
        estado.Fases = fases;
        estado.FaseActualIndice = 0;

        FaseTorneo primeraFase = fases[0];

        bool haySeparacionPorRanking = primeraFase.EquiposParticipantesIniciales > 0
            && primeraFase.EquiposParticipantesIniciales < equipos.Count;

        List<string> entradaPrimeraFase;

        if (haySeparacionPorRanking)
        {
            List<string> ordenados = equipos
                .OrderByDescending(e => RankingFifaProvider.ObtenerPosicion(e))
                .ToList();

            entradaPrimeraFase = ordenados.Take(primeraFase.EquiposParticipantesIniciales).ToList();
            List<string> sembrados = ordenados.Skip(primeraFase.EquiposParticipantesIniciales).ToList();

            if (fases.Count > 1)
            {
                fases[1].EquiposDirectos = sembrados;
            }
        }
        else
        {
            entradaPrimeraFase = equipos;
        }

        ObtenerGenerador(primeraFase.Tipo).GenerarEstructura(primeraFase, entradaPrimeraFase);
    }

    public static void ProcesarResultado(TournamentState estado, string equipoLocal, string equipoVisitante, int golesLocal, int golesVisitante)
    {
        FaseTorneo faseActual = estado.FaseActual;
        if (faseActual == null) return;

        ObtenerGenerador(faseActual.Tipo).ProcesarResultado(faseActual, equipoLocal, equipoVisitante, golesLocal, golesVisitante);
    }

    public static List<(string Local, string Visitante)> ObtenerPartidosPendientes(FaseTorneo fase)
    {
        var pendientes = new List<(string, string)>();

        switch (fase.Tipo)
        {
            case TipoFormato.RoundRobin:
                pendientes.AddRange(fase.Calendario
                    .Where(p => !p.Jugado)
                    .Select(p => (p.EquipoLocal, p.EquipoVisitante)));
                break;

            case TipoFormato.Grupos:
                foreach (GrupoTorneo grupo in fase.Grupos)
                {
                    pendientes.AddRange(grupo.Calendario
                        .Where(p => !p.Jugado)
                        .Select(p => (p.EquipoLocal, p.EquipoVisitante)));
                }
                break;

            case TipoFormato.Eliminacion:
            foreach (LlaveEliminacion l in fase.Llaves.Where(l => !l.Jugado))
            {
                pendientes.Add((!l.IdaYVuelta || !l.JugadoIda)
                    ? (l.EquipoLocal, l.EquipoVisitante)     // ida
                    : (l.EquipoVisitante, l.EquipoLocal));   // vuelta: roles invertidos
            }
            break;
        }

        return pendientes;
    }

    public static bool AvanzarFaseSiCorresponde(TournamentState estado)
    {
        FaseTorneo faseActual = estado.FaseActual;
        if (faseActual == null) return false;

        IGeneradorFormato generadorActual = ObtenerGenerador(faseActual.Tipo);
        if (!generadorActual.FaseCompleta(faseActual)) return false;

        faseActual.Completada = true;

        List<string> clasificados;

        // 👇 LA MAGIA PARA ÁFRICA (Y bifurcaciones futuras)
        if (faseActual.DivideClasificados && faseActual.Tipo == TipoFormato.Grupos)
        {
            var generadorGrupos = (GeneradorGrupos)generadorActual;
            (List<string> directos, List<string> mejoresSegundos) =
                generadorGrupos.ObtenerClasificadosConRepechaje(faseActual, faseActual.CantidadClasificadosExtra);

            estado.ClasificadosDirectoAlMundial.AddRange(directos);
            
            // Aquí aplicamos tu regla: ordenamos a los 4 mejores segundos por Ranking FIFA
            clasificados = mejoresSegundos
                .OrderBy(equipo => RankingFifaProvider.ObtenerPosicion(equipo))
                .ToList();

            GD.Print($"🌍 {directos.Count} equipos clasificaron DIRECTO al Mundial: {string.Join(", ", directos)}");
        }
        else
        {
            clasificados = generadorActual.ObtenerClasificados(faseActual);

            if (faseActual.ClasificanASiguienteFase >= 0 && faseActual.ClasificanASiguienteFase < clasificados.Count)
            {
                clasificados = clasificados.Take(faseActual.ClasificanASiguienteFase).ToList();
            }
        }

        int siguienteIndice = estado.FaseActualIndice + 1;

        if (siguienteIndice >= estado.Fases.Count)
        {
            // ¡Magia para que TODAS las confederaciones tengan su estrellita!
            if (!faseActual.DivideClasificados)
            {
                if (faseActual.Tipo == TipoFormato.RoundRobin || faseActual.Tipo == TipoFormato.Grupos)
                {
                    var (directos, _) = ZonasClasificacionHelper.ObtenerZonas(faseActual);
                    estado.ClasificadosDirectoAlMundial.AddRange(directos);
                }
                else if (faseActual.Tipo == TipoFormato.Eliminacion)
                {
                    // Si el torneo termina en Eliminación (como OFC) y el ganador va directo
                    if (!faseActual.GanadorEsRepechaje && clasificados.Count > 0)
                    {
                        estado.ClasificadosDirectoAlMundial.Add(clasificados[0]);
                    }
                }
            }

            if (faseActual.Tipo == TipoFormato.Eliminacion && clasificados.Count > 0)
            {
                GD.Print($"🏆 Torneo finalizado. Campeón: {clasificados[0]}");
            }
            else
            {
                string listado = clasificados.Count > 0 ? string.Join(", ", clasificados) : "(sin clasificados)";
                GD.Print($"🏁 Torneo finalizado. Clasificados: {listado}");
            }
            return true; 
        }

        FaseTorneo faseSiguiente = estado.Fases[siguienteIndice];

        if (faseActual.Tipo == TipoFormato.Grupos && faseSiguiente.Tipo == TipoFormato.Eliminacion)
        {
            // Como le quitaste el SorteoAleatorio a la Fase 2, ReordenarCruceCruzado 
            // tomará tu lista ordenada y formará las llaves 1° vs 4° y 2° vs 3°.
            if (!faseSiguiente.SorteoAleatorio)
            {
                clasificados = ReordenarCruceCruzado(clasificados);
            }
        }

        List<string> entrada = new List<string>(clasificados);
        if (faseSiguiente.EquiposDirectos.Count > 0)
        {
            entrada.AddRange(faseSiguiente.EquiposDirectos);
        }

        ObtenerGenerador(faseSiguiente.Tipo).GenerarEstructura(faseSiguiente, entrada);
        estado.FaseActualIndice = siguienteIndice;

        GD.Print($"➡️  Fase completada: {faseActual.Nombre}. Comienza: {faseSiguiente.Nombre}");
        return true;
    }

    private static List<string> ReordenarCruceCruzado(List<string> clasificados)
    {
        int totalGrupos = clasificados.Count / 2;
        List<string> primeros = clasificados.Take(totalGrupos).ToList();
        List<string> segundos = clasificados.Skip(totalGrupos).ToList();

        var cruzado = new List<string>();
        for (int i = 0; i < totalGrupos; i++)
        {
            cruzado.Add(primeros[i]);
            cruzado.Add(segundos[(i + 1) % totalGrupos]);
        }
        return cruzado;
    }

    public static List<(string Local, string Visitante)> ObtenerUnidadPendiente(FaseTorneo fase)
    {
        switch (fase.Tipo)
        {
            case TipoFormato.RoundRobin:
            {
                var pendientes = fase.Calendario.Where(p => !p.Jugado).ToList();
                if (pendientes.Count == 0) return new();
                int minJornada = pendientes.Min(p => p.Jornada);
                return pendientes.Where(p => p.Jornada == minJornada)
                    .Select(p => (p.EquipoLocal, p.EquipoVisitante)).ToList();
            }
            case TipoFormato.Grupos:
            {
                var pendientes = fase.Grupos.SelectMany(g => g.Calendario).Where(p => !p.Jugado).ToList();
                if (pendientes.Count == 0) return new();
                int minJornada = pendientes.Min(p => p.Jornada);
                return pendientes.Where(p => p.Jornada == minJornada)
                    .Select(p => (p.EquipoLocal, p.EquipoVisitante)).ToList();
            }
            case TipoFormato.Eliminacion:
            {
                var pendientes = fase.Llaves.Where(l => !l.Jugado).ToList();
                if (pendientes.Count == 0) return new();
                int minRonda = pendientes.Min(l => l.Ronda);
                return pendientes.Where(l => l.Ronda == minRonda)
                    .Select(l => (!l.IdaYVuelta || !l.JugadoIda) ? (l.EquipoLocal, l.EquipoVisitante) : (l.EquipoVisitante, l.EquipoLocal))
                    .ToList();
            }
            default:
                return new();
        }
    }

    public static float ObtenerEstrellas(List<TeamData> equipos, string nombreEquipo)
    {
        TeamData equipo = equipos.FirstOrDefault(e => e.TeamName == nombreEquipo);
        return equipo?.StarRating ?? 3.0f;
    }

    public static void SimularYRegistrarPartido(TournamentState estado, string local, string visitante, List<TeamData> equipos)
    {
        float estrellasLocal = ObtenerEstrellas(equipos, local);
        float estrellasVisitante = ObtenerEstrellas(equipos, visitante);
        (int golesLocal, int golesVisitante) = SimulationEngine.SimularPartido(estrellasLocal, estrellasVisitante);

        ProcesarResultado(estado, local, visitante, golesLocal, golesVisitante);
    }

    public static bool TorneoFinalizado(TournamentState estado)
    {
        return estado.FaseActual != null
            && estado.FaseActual.Completada
            && estado.FaseActualIndice == estado.Fases.Count - 1;
    }

    public static void SimularTorneoCompleto(TournamentState estado, List<TeamData> equipos)
    {
        while (!TorneoFinalizado(estado))
        {
            FaseTorneo faseActual = estado.FaseActual;
            if (faseActual == null) break;

            var pendientes = ObtenerUnidadPendiente(faseActual);

            if (pendientes.Count == 0)
            {
                if (!AvanzarFaseSiCorresponde(estado)) break;
                continue;
            }

            foreach (var partido in pendientes)
            {
                SimularYRegistrarPartido(estado, partido.Local, partido.Visitante, equipos);
            }

            AvanzarFaseSiCorresponde(estado);
        }
    }
}
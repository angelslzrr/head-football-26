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
                pendientes.AddRange(fase.Llaves
                    .Where(l => !l.Jugado)
                    .Select(l => (l.EquipoLocal, l.EquipoVisitante)));
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

        List<string> clasificados = generadorActual.ObtenerClasificados(faseActual);

        if (faseActual.ClasificanASiguienteFase >= 0 && faseActual.ClasificanASiguienteFase < clasificados.Count)
        {
            clasificados = clasificados.Take(faseActual.ClasificanASiguienteFase).ToList();
        }

        int siguienteIndice = estado.FaseActualIndice + 1;

        if (siguienteIndice >= estado.Fases.Count)
        {
            GD.Print($"🏆 Torneo finalizado. Campeón: {(clasificados.Count > 0 ? clasificados[0] : "???")}");
            return true; 
        }

        FaseTorneo faseSiguiente = estado.Fases[siguienteIndice];

        if (faseActual.Tipo == TipoFormato.Grupos && faseSiguiente.Tipo == TipoFormato.Eliminacion)
        {
            clasificados = ReordenarCruceCruzado(clasificados);
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
                    .Select(l => (l.EquipoLocal, l.EquipoVisitante)).ToList();
            }
            default:
                return new();
        }
    }
}
using System;

public static class RenderizadorFactory
{
    public static IRenderizadorFase ObtenerRenderizador(TipoFormato tipo)
    {
        return tipo switch
        {
            TipoFormato.RoundRobin => new RenderizadorRoundRobin(),
            TipoFormato.Grupos => new RenderizadorGrupos(),
            TipoFormato.Eliminacion => new RenderizadorEliminacion(),
            _ => throw new Exception($"Tipo de formato sin renderizador: {tipo}")
        };
    }
}
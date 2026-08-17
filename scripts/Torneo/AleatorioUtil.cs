using Godot;
using System.Collections.Generic;

public static class AleatorioUtil
{
    public static void Mezclar<T>(List<T> lista)
    {
        for (int i = lista.Count - 1; i > 0; i--)
        {
            int j = GD.RandRange(0, i);
            (lista[i], lista[j]) = (lista[j], lista[i]);
        }
    }
}
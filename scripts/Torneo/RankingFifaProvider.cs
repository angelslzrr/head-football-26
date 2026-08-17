using Godot;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public static class RankingFifaProvider
{
    private static Dictionary<string, int> _ranking;

    private static void CargarSiHaceFalta()
    {
        if (_ranking != null) return;
        _ranking = new Dictionary<string, int>();

        const string ruta = "res://data/ranking_fifa.txt";
        if (!FileAccess.FileExists(ruta))
        {
            GD.PrintErr("No se encontró ranking_fifa.txt. El sorteo usará orden alfabético como respaldo.");
            return;
        }

        FileAccess archivo = FileAccess.Open(ruta, FileAccess.ModeFlags.Read);
        string contenido = archivo.GetAsText();
        archivo.Close();

        var regex = new Regex(@"^\s*(\d+)\.\s+(.+?)\s*$");
        foreach (string linea in contenido.Split('\n'))
        {
            Match m = regex.Match(linea);
            if (!m.Success) continue;

            int posicion = int.Parse(m.Groups[1].Value);
            string nombre = Normalizar(m.Groups[2].Value);
            _ranking[nombre] = posicion;
        }
    }

    public static int ObtenerPosicion(string nombreEquipo)
    {
        CargarSiHaceFalta();

        string clave = Normalizar(nombreEquipo);
        if (_ranking.TryGetValue(clave, out int posicion)) return posicion;

        GD.PrintErr($"'{nombreEquipo}' no está en el ranking FIFA. Se le asigna la última posición.");
        return int.MaxValue;
    }

    private static string Normalizar(string texto)
    {
        string sinTildes = string.Concat(
            texto.Normalize(NormalizationForm.FormD)
                 .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
        );
        return sinTildes.ToLowerInvariant().Trim();
    }
}
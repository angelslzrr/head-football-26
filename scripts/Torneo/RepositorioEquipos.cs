using Godot;
using System.Collections.Generic;

public static class RepositorioEquipos
{
    private struct DefinicionEquipo
    {
        public string Nombre;
        public string FifaCode;
        public string ArchivoCabeza;
        public string ArchivoCamiseta;
        public string ArchivoBandera;
    }

    public static List<TeamData> ObtenerEquiposPorRegion(string region)
    {
        return region switch
        {
            "Sudamérica" => ObtenerEquiposConmebol(),
            "Oceania" => ObtenerEquiposOFC(),
            _ => new List<TeamData>()
        };
    }

    public static List<TeamData> ObtenerEquiposConmebol()
    {
        var definiciones = new List<DefinicionEquipo>
        {
            new() { Nombre = "Argentina", FifaCode = "ARG", ArchivoCabeza = "cabezon_Messi_Arg.png", ArchivoCamiseta = "camiseta_Argentina.png", ArchivoBandera = "bandera_Argentina.png" },
            new() { Nombre = "Brasil", FifaCode = "BRA", ArchivoCabeza = "cabezon_Neymar_Bra.png", ArchivoCamiseta = "camiseta_Brasil.png", ArchivoBandera = "bandera_Brasil.png" },
            new() { Nombre = "Colombia", FifaCode = "COL", ArchivoCabeza = "cabezon_LuisDiaz_Col.png", ArchivoCamiseta = "camiseta_Colombia.png", ArchivoBandera = "bandera_Colombia.png" },
            new() { Nombre = "Uruguay", FifaCode = "URU", ArchivoCabeza = "cabezon_Darwin_Uru.png", ArchivoCamiseta = "camiseta_Uruguay.png", ArchivoBandera = "bandera_Uruguay.png" },
            new() { Nombre = "Ecuador", FifaCode = "ECU", ArchivoCabeza = "cabezon_Caicedo_Ecu.png", ArchivoCamiseta = "camiseta_Ecuador.png", ArchivoBandera = "bandera_Ecuador.png" },
            new() { Nombre = "Paraguay", FifaCode = "PAR", ArchivoCabeza = "cabezon_Enciso_Par.png", ArchivoCamiseta = "camiseta_Paraguay.png", ArchivoBandera = "bandera_Paraguay.png" },
            new() { Nombre = "Venezuela", FifaCode = "VEN", ArchivoCabeza = "cabezon_Soteldo_Ven.png", ArchivoCamiseta = "camiseta_Venezuela.png", ArchivoBandera = "bandera_Venezuela.png" },
            new() { Nombre = "Chile", FifaCode = "CHI", ArchivoCabeza = "cabezon_Brereton_Chi.png", ArchivoCamiseta = "camiseta_Chile.png", ArchivoBandera = "bandera_Chile.png" },
            new() { Nombre = "Perú", FifaCode = "PER", ArchivoCabeza = "cabezon_Guerrero_Per.png", ArchivoCamiseta = "camiseta_Peru.png", ArchivoBandera = "bandera_Peru.png" },
            new() { Nombre = "Bolivia", FifaCode = "BOL", ArchivoCabeza = "cabezon_Terceros_Bol.png", ArchivoCamiseta = "camiseta_Bolivia.png", ArchivoBandera = "bandera_Bolivia.png" },
        };

        return ConstruirEquipos("Sudamérica", "conmebol", definiciones);
    }

    public static List<TeamData> ObtenerEquiposOFC()
    {
        var definiciones = new List<DefinicionEquipo>
        {
            new() { Nombre = "Nueva Zelanda", FifaCode = "NZL", ArchivoCabeza = "cabezon_Wood_Nzl.png", ArchivoCamiseta = "camiseta_NuevaZelanda.png", ArchivoBandera = "bandera_NuevaZelanda.png" },
            new() { Nombre = "Vanuatu", FifaCode = "VAN", ArchivoCabeza = "cabezon_Kaltak_Van.png", ArchivoCamiseta = "camiseta_Vanuatu.png", ArchivoBandera = "bandera_Vanuatu.png" },
            new() { Nombre = "Nueva Caledonia", FifaCode = "NCL", ArchivoCabeza = "cabezon_Fulgini_Ncl.png", ArchivoCamiseta = "camiseta_NuevaCaledonia.png", ArchivoBandera = "bandera_NuevaCaledonia.png" },
            new() { Nombre = "Islas Salomon", FifaCode = "SOL", ArchivoCabeza = "cabezon_Lea'i_Sol.png", ArchivoCamiseta = "camiseta_IslasSalomon.png", ArchivoBandera = "bandera_IslasSalomon.png" },
            new() { Nombre = "Fiyi", FifaCode = "FIJ", ArchivoCabeza = "cabezon_Krishna_Fij.png", ArchivoCamiseta = "camiseta_Fiyi.png", ArchivoBandera = "bandera_Fiyi.png" },
            new() { Nombre = "Tahiti", FifaCode = "TAH", ArchivoCabeza = "cabezon_Tehau_Tah.png", ArchivoCamiseta = "camiseta_Tahiti.png", ArchivoBandera = "bandera_Tahiti.png" },
            new() { Nombre = "Papua Nueva Guinea", FifaCode = "PNG", ArchivoCabeza = "cabezon_Gunemba_Png.png", ArchivoCamiseta = "camiseta_PapuaNuevaGuinea.png", ArchivoBandera = "bandera_PapuaNuevaGuinea.png" },
            new() { Nombre = "Islas Cook", FifaCode = "COK", ArchivoCabeza = "cabezon_Saghabi_Cok.png", ArchivoCamiseta = "camiseta_IslasCook.png", ArchivoBandera = "bandera_IslasCook.png" },
            new() { Nombre = "Samoa", FifaCode = "SAM", ArchivoCabeza = "cabezon_Setefano_Sam.png", ArchivoCamiseta = "camiseta_Samoa.png", ArchivoBandera = "bandera_Samoa.png" },
            new() { Nombre = "Samoa Americana", FifaCode = "ASA", ArchivoCabeza = "cabezon_Salapu_Asa.png", ArchivoCamiseta = "camiseta_SamoaAmericana.png", ArchivoBandera = "bandera_SamoaAmericana.png" },
            new() { Nombre = "Tonga", FifaCode = "TGA", ArchivoCabeza = "cabezon_Polovili_Tga.png", ArchivoCamiseta = "camiseta_Tonga.png", ArchivoBandera = "bandera_Tonga.png" },
        };

        return ConstruirEquipos("Oceania", "ofc", definiciones);
    }

    private static List<TeamData> ConstruirEquipos(string region, string carpetaAssets, List<DefinicionEquipo> definiciones)
    {
        var equipos = new List<TeamData>();

        foreach (DefinicionEquipo def in definiciones)
        {
            equipos.Add(new TeamData
            {
                TeamName = def.Nombre,
                Region = region,
                FifaCode = def.FifaCode,
                StarRating = CalcularStarRatingPorRanking(def.Nombre),
                FlagTexture = CargarTexturaSegura($"res://img/banderas/{carpetaAssets}/{def.ArchivoBandera}"),
                CabezaTexture = CargarTexturaSegura($"res://img/cabezones/{carpetaAssets}/{def.ArchivoCabeza}"),
                CamisetaTexture = CargarTexturaSegura($"res://img/camisetas/{carpetaAssets}/{def.ArchivoCamiseta}"),
                ChimpunTexture = ObtenerChimpunAleatorio()
            });
        }

        return equipos;
    }

    private static float CalcularStarRatingPorRanking(string nombreEquipo)
    {
        int puesto = RankingFifaProvider.ObtenerPosicion(nombreEquipo);

        if (puesto <= 5) return 5.0f;
        if (puesto <= 15) return 4.5f;
        if (puesto <= 30) return 4.0f;
        if (puesto <= 55) return 3.5f;
        if (puesto <= 85) return 3.0f;
        if (puesto <= 115) return 2.5f;
        if (puesto <= 145) return 2.0f;
        if (puesto <= 175) return 1.5f;
        if (puesto <= 200) return 1.0f;
        return 0.5f;
    }

    private static Texture2D CargarTexturaSegura(string ruta)
    {
        if (!ResourceLoader.Exists(ruta))
        {
            GD.PrintErr($"Falta el asset: {ruta}");
            return null;
        }
        return GD.Load<Texture2D>(ruta);
    }

    private static Texture2D ObtenerChimpunAleatorio()
    {
        int randomNum = GD.RandRange(1, 15);
        return GD.Load<Texture2D>($"res://img/chimpunes/chimpun{randomNum}.png");
    }

    public static TeamData BuscarEquipo(string nombreEquipo)
    {
        // Juntamos todas las confederaciones en una sola lista para buscar
        var todosLosEquipos = new List<TeamData>();
        todosLosEquipos.AddRange(ObtenerEquiposConmebol());
        todosLosEquipos.AddRange(ObtenerEquiposOFC());

        return todosLosEquipos.Find(e => e.TeamName == nombreEquipo);
    }
}
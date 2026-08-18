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
            "Norte y Centroamérica" => ObtenerEquiposConcacaf(),
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

    public static List<TeamData> ObtenerEquiposConcacaf()
    {
        var definiciones = new List<DefinicionEquipo>
        {
            new() { Nombre = "Panamá", FifaCode = "PAN", ArchivoCabeza = "cabezon_Generico_Pan.png", ArchivoCamiseta = "camiseta_Panama.png", ArchivoBandera = "bandera_Panama.png" },
            new() { Nombre = "Costa Rica", FifaCode = "CRC", ArchivoCabeza = "cabezon_Generico_Crc.png", ArchivoCamiseta = "camiseta_CostaRica.png", ArchivoBandera = "bandera_CostaRica.png" },
            new() { Nombre = "Honduras", FifaCode = "HON", ArchivoCabeza = "cabezon_Generico_Hon.png", ArchivoCamiseta = "camiseta_Honduras.png", ArchivoBandera = "bandera_Honduras.png" },
            new() { Nombre = "Jamaica", FifaCode = "JAM", ArchivoCabeza = "cabezon_Generico_Jam.png", ArchivoCamiseta = "camiseta_Jamaica.png", ArchivoBandera = "bandera_Jamaica.png" },
            new() { Nombre = "Curazao", FifaCode = "CUW", ArchivoCabeza = "cabezon_Generico_Cuw.png", ArchivoCamiseta = "camiseta_Curazao.png", ArchivoBandera = "bandera_Curazao.png" },
            new() { Nombre = "Haití", FifaCode = "HAI", ArchivoCabeza = "cabezon_Generico_Hai.png", ArchivoCamiseta = "camiseta_Haiti.png", ArchivoBandera = "bandera_Haiti.png" },
            new() { Nombre = "Guatemala", FifaCode = "GUA", ArchivoCabeza = "cabezon_Generico_Gua.png", ArchivoCamiseta = "camiseta_Guatemala.png", ArchivoBandera = "bandera_Guatemala.png" },
            new() { Nombre = "El Salvador", FifaCode = "SLV", ArchivoCabeza = "cabezon_Generico_Slv.png", ArchivoCamiseta = "camiseta_ElSalvador.png", ArchivoBandera = "bandera_ElSalvador.png" },
            new() { Nombre = "Trinidad y Tobago", FifaCode = "TRI", ArchivoCabeza = "cabezon_Generico_Tri.png", ArchivoCamiseta = "camiseta_TrinidadYTobago.png", ArchivoBandera = "bandera_TrinidadYTobago.png" },
            new() { Nombre = "Surinam", FifaCode = "SUR", ArchivoCabeza = "cabezon_Generico_Sur.png", ArchivoCamiseta = "camiseta_Surinam.png", ArchivoBandera = "bandera_Surinam.png" },
            new() { Nombre = "Nicaragua", FifaCode = "NCA", ArchivoCabeza = "cabezon_Generico_Nca.png", ArchivoCamiseta = "camiseta_Nicaragua.png", ArchivoBandera = "bandera_Nicaragua.png" },
            new() { Nombre = "República Dominicana", FifaCode = "DOM", ArchivoCabeza = "cabezon_Generico_Dom.png", ArchivoCamiseta = "camiseta_RepublicaDominicana.png", ArchivoBandera = "bandera_RepublicaDominicana.png" },
            new() { Nombre = "Guyana", FifaCode = "GUY", ArchivoCabeza = "cabezon_Generico_Guy.png", ArchivoCamiseta = "camiseta_Guyana.png", ArchivoBandera = "bandera_Guyana.png" },
            new() { Nombre = "San Cristóbal y Nieves", FifaCode = "SKN", ArchivoCabeza = "cabezon_Generico_Skn.png", ArchivoCamiseta = "camiseta_SanCristobalYNieves.png", ArchivoBandera = "bandera_SanCristobalYNieves.png" },
            new() { Nombre = "Puerto Rico", FifaCode = "PUR", ArchivoCabeza = "cabezon_Generico_Pur.png", ArchivoCamiseta = "camiseta_PuertoRico.png", ArchivoBandera = "bandera_PuertoRico.png" },
            new() { Nombre = "Antigua y Barbuda", FifaCode = "ATG", ArchivoCabeza = "cabezon_Generico_Atg.png", ArchivoCamiseta = "camiseta_AntiguaYBarbuda.png", ArchivoBandera = "bandera_AntiguaYBarbuda.png" },
            new() { Nombre = "Granada", FifaCode = "GRN", ArchivoCabeza = "cabezon_Generico_Grn.png", ArchivoCamiseta = "camiseta_Granada.png", ArchivoBandera = "bandera_Granada.png" },
            new() { Nombre = "Cuba", FifaCode = "CUB", ArchivoCabeza = "cabezon_Generico_Cub.png", ArchivoCamiseta = "camiseta_Cuba.png", ArchivoBandera = "bandera_Cuba.png" },
            new() { Nombre = "Santa Lucía", FifaCode = "LCA", ArchivoCabeza = "cabezon_Generico_Lca.png", ArchivoCamiseta = "camiseta_SantaLucia.png", ArchivoBandera = "bandera_SantaLucia.png" },
            new() { Nombre = "Bermudas", FifaCode = "BER", ArchivoCabeza = "cabezon_Generico_Ber.png", ArchivoCamiseta = "camiseta_Bermudas.png", ArchivoBandera = "bandera_Bermudas.png" },
            new() { Nombre = "San Vicente y las Granadinas", FifaCode = "VIN", ArchivoCabeza = "cabezon_Generico_Vin.png", ArchivoCamiseta = "camiseta_SanVicenteYLasGranadinas.png", ArchivoBandera = "bandera_SanVicenteYLasGranadinas.png" },
            new() { Nombre = "Montserrat", FifaCode = "MSR", ArchivoCabeza = "cabezon_Generico_Msr.png", ArchivoCamiseta = "camiseta_Montserrat.png", ArchivoBandera = "bandera_Montserrat.png" },
            new() { Nombre = "Barbados", FifaCode = "BRB", ArchivoCabeza = "cabezon_Generico_Brb.png", ArchivoCamiseta = "camiseta_Barbados.png", ArchivoBandera = "bandera_Barbados.png" },
            new() { Nombre = "Belice", FifaCode = "BLZ", ArchivoCabeza = "cabezon_Generico_Blz.png", ArchivoCamiseta = "camiseta_Belice.png", ArchivoBandera = "bandera_Belice.png" },
            new() { Nombre = "Dominica", FifaCode = "DMA", ArchivoCabeza = "cabezon_Generico_Dma.png", ArchivoCamiseta = "camiseta_Dominica.png", ArchivoBandera = "bandera_Dominica.png" },
            new() { Nombre = "Aruba", FifaCode = "ARU", ArchivoCabeza = "cabezon_Generico_Aru.png", ArchivoCamiseta = "camiseta_Aruba.png", ArchivoBandera = "bandera_Aruba.png" },
            new() { Nombre = "Islas Caimán", FifaCode = "CAY", ArchivoCabeza = "cabezon_Generico_Cay.png", ArchivoCamiseta = "camiseta_IslasCaiman.png", ArchivoBandera = "bandera_IslasCaiman.png" },
            new() { Nombre = "Islas Turcas y Caicos", FifaCode = "TCA", ArchivoCabeza = "cabezon_Generico_Tca.png", ArchivoCamiseta = "camiseta_IslasTurcasYCaicos.png", ArchivoBandera = "bandera_IslasTurcasYCaicos.png" },
            new() { Nombre = "Bahamas", FifaCode = "BAH", ArchivoCabeza = "cabezon_Generico_Bah.png", ArchivoCamiseta = "camiseta_Bahamas.png", ArchivoBandera = "bandera_Bahamas.png" },
            new() { Nombre = "Islas Vírgenes Estadounidenses", FifaCode = "VIR", ArchivoCabeza = "cabezon_Generico_Vir.png", ArchivoCamiseta = "camiseta_IslasVirgenesEstadounidenses.png", ArchivoBandera = "bandera_IslasVirgenesEstadounidenses.png" },
            new() { Nombre = "Islas Vírgenes Británicas", FifaCode = "VGB", ArchivoCabeza = "cabezon_Generico_Vgb.png", ArchivoCamiseta = "camiseta_IslasVirgenesBritanicas.png", ArchivoBandera = "bandera_IslasVirgenesBritanicas.png" },
            new() { Nombre = "Anguila", FifaCode = "AIA", ArchivoCabeza = "cabezon_Generico_Aia.png", ArchivoCamiseta = "camiseta_Anguila.png", ArchivoBandera = "bandera_Anguila.png" },
        };

        return ConstruirEquipos("Norte y Centroamérica", "concacaf", definiciones);
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
        todosLosEquipos.AddRange(ObtenerEquiposConcacaf());

        return todosLosEquipos.Find(e => e.TeamName == nombreEquipo);
    }
}
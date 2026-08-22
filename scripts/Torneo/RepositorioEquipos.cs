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
            "África" => ObtenerEquiposCAF(),
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
            new() { Nombre = "Nueva Caledonia", FifaCode = "NCL", ArchivoCabeza = "cabezon_Fulgini_Ncl.png", ArchivoCamiseta = "camiseta_NuevaCaledonia.png", ArchivoBandera = "bandera_NuevaCaledonia.png" },
            new() { Nombre = "Islas Salomon", FifaCode = "SOL", ArchivoCabeza = "cabezon_Lea'i_Sol.png", ArchivoCamiseta = "camiseta_IslasSalomon.png", ArchivoBandera = "bandera_IslasSalomon.png" },
            new() { Nombre = "Fiyi", FifaCode = "FIJ", ArchivoCabeza = "cabezon_Krishna_Fij.png", ArchivoCamiseta = "camiseta_Fiyi.png", ArchivoBandera = "bandera_Fiyi.png" },
            new() { Nombre = "Tahiti", FifaCode = "TAH", ArchivoCabeza = "cabezon_Tehau_Tah.png", ArchivoCamiseta = "camiseta_Tahiti.png", ArchivoBandera = "bandera_Tahiti.png" },
            new() { Nombre = "Vanuatu", FifaCode = "VAN", ArchivoCabeza = "cabezon_Kaltak_Van.png", ArchivoCamiseta = "camiseta_Vanuatu.png", ArchivoBandera = "bandera_Vanuatu.png" },
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
            new() { Nombre = "Panamá", FifaCode = "PAN", ArchivoCabeza = "cabezon_Waterman_Pan.png", ArchivoCamiseta = "camiseta_Panama.png", ArchivoBandera = "bandera_Panama.png" },
            new() { Nombre = "Costa Rica", FifaCode = "CRC", ArchivoCabeza = "cabezon_Campbell_Crc.png", ArchivoCamiseta = "camiseta_CostaRica.png", ArchivoBandera = "bandera_CostaRica.png" },
            new() { Nombre = "Honduras", FifaCode = "HON", ArchivoCabeza = "cabezon_Palma_Hon.png", ArchivoCamiseta = "camiseta_Honduras.png", ArchivoBandera = "bandera_Honduras.png" },
            new() { Nombre = "Jamaica", FifaCode = "JAM", ArchivoCabeza = "cabezon_Bailey_Jam.png", ArchivoCamiseta = "camiseta_Jamaica.png", ArchivoBandera = "bandera_Jamaica.png" },
            new() { Nombre = "Curazao", FifaCode = "CUW", ArchivoCabeza = "cabezon_Chong_Cuw.png", ArchivoCamiseta = "camiseta_Curazao.png", ArchivoBandera = "bandera_Curazao.png" },
            new() { Nombre = "Haití", FifaCode = "HAI", ArchivoCabeza = "cabezon_Isidor_Hai.png", ArchivoCamiseta = "camiseta_Haiti.png", ArchivoBandera = "bandera_Haiti.png" },
            new() { Nombre = "Guatemala", FifaCode = "GUA", ArchivoCabeza = "cabezon_Mendez-Laing_Gua.png", ArchivoCamiseta = "camiseta_Guatemala.png", ArchivoBandera = "bandera_Guatemala.png" },
            new() { Nombre = "El Salvador", FifaCode = "SLV", ArchivoCabeza = "cabezon_Ordaz_Slv.png", ArchivoCamiseta = "camiseta_ElSalvador.png", ArchivoBandera = "bandera_ElSalvador.png" },
            new() { Nombre = "Trinidad y Tobago", FifaCode = "TRI", ArchivoCabeza = "cabezon_LeviGarcia_Tri.png", ArchivoCamiseta = "camiseta_TrinidadYTobago.png", ArchivoBandera = "bandera_TrinidadYTobago.png" },
            new() { Nombre = "Surinam", FifaCode = "SUR", ArchivoCabeza = "cabezon_Becker_Sur.png", ArchivoCamiseta = "camiseta_Surinam.png", ArchivoBandera = "bandera_Surinam.png" },
            new() { Nombre = "Nicaragua", FifaCode = "NCA", ArchivoCabeza = "cabezon_Barrera_Nca.png", ArchivoCamiseta = "camiseta_Nicaragua.png", ArchivoBandera = "bandera_Nicaragua.png" },
            new() { Nombre = "República Dominicana", FifaCode = "DOM", ArchivoCabeza = "cabezon_MarianoDiaz_Dom.png", ArchivoCamiseta = "camiseta_RepublicaDominicana.png", ArchivoBandera = "bandera_RepublicaDominicana.png" },
            new() { Nombre = "Guyana", FifaCode = "GUY", ArchivoCabeza = "cabezon_Jones_Guy.png", ArchivoCamiseta = "camiseta_Guyana.png", ArchivoBandera = "bandera_Guyana.png" },
            new() { Nombre = "San Cristóbal y Nieves", FifaCode = "SKN", ArchivoCabeza = "cabezon_Sawyers_Skn.png", ArchivoCamiseta = "camiseta_SanCristobalYNieves.png", ArchivoBandera = "bandera_SanCristobalYNieves.png" },
            new() { Nombre = "Puerto Rico", FifaCode = "PUR", ArchivoCabeza = "cabezon_DeLeon_Pur.png", ArchivoCamiseta = "camiseta_PuertoRico.png", ArchivoBandera = "bandera_PuertoRico.png" },
            new() { Nombre = "Antigua y Barbuda", FifaCode = "ATG", ArchivoCabeza = "cabezon_Griffith_Atg.png", ArchivoCamiseta = "camiseta_AntiguaYBarbuda.png", ArchivoBandera = "bandera_AntiguaYBarbuda.png" },
            new() { Nombre = "Granada", FifaCode = "GRN", ArchivoCabeza = "cabezon_Charles-Cook_Grn.png", ArchivoCamiseta = "camiseta_Granada.png", ArchivoBandera = "bandera_Granada.png" },
            new() { Nombre = "Cuba", FifaCode = "CUB", ArchivoCabeza = "cabezon_Hernandez_Cub.png", ArchivoCamiseta = "camiseta_Cuba.png", ArchivoBandera = "bandera_Cuba.png" },
            new() { Nombre = "Santa Lucía", FifaCode = "LCA", ArchivoCabeza = "cabezon_Elva_Lca.png", ArchivoCamiseta = "camiseta_SantaLucia.png", ArchivoBandera = "bandera_SantaLucia.png" },
            new() { Nombre = "Bermudas", FifaCode = "BER", ArchivoCabeza = "cabezon_Wells_Ber.png", ArchivoCamiseta = "camiseta_Bermudas.png", ArchivoBandera = "bandera_Bermudas.png" },
            new() { Nombre = "San Vicente y las Granadinas", FifaCode = "VIN", ArchivoCabeza = "cabezon_Anderson_Vin.png", ArchivoCamiseta = "camiseta_SanVicenteYLasGranadinas.png", ArchivoBandera = "bandera_SanVicenteYLasGranadinas.png" },
            new() { Nombre = "Montserrat", FifaCode = "MSR", ArchivoCabeza = "cabezon_Taylor_Msr.png", ArchivoCamiseta = "camiseta_Montserrat.png", ArchivoBandera = "bandera_Montserrat.png" },
            new() { Nombre = "Barbados", FifaCode = "BRB", ArchivoCabeza = "cabezon_Gale_Brb.png", ArchivoCamiseta = "camiseta_Barbados.png", ArchivoBandera = "bandera_Barbados.png" },
            new() { Nombre = "Belice", FifaCode = "BLZ", ArchivoCabeza = "cabezon_Bernardez_Blz.png", ArchivoCamiseta = "camiseta_Belice.png", ArchivoBandera = "bandera_Belice.png" },
            new() { Nombre = "Dominica", FifaCode = "DMA", ArchivoCabeza = "cabezon_Laville_Dma.png", ArchivoCamiseta = "camiseta_Dominica.png", ArchivoBandera = "bandera_Dominica.png" },
            new() { Nombre = "Aruba", FifaCode = "ARU", ArchivoCabeza = "cabezon_Ostiana_Aru.png", ArchivoCamiseta = "camiseta_Aruba.png", ArchivoBandera = "bandera_Aruba.png" },
            new() { Nombre = "Islas Caimán", FifaCode = "CAY", ArchivoCabeza = "cabezon_Seymour_Cay.png", ArchivoCamiseta = "camiseta_IslasCaiman.png", ArchivoBandera = "bandera_IslasCaiman.png" },
            new() { Nombre = "Islas Turcas y Caicos", FifaCode = "TCA", ArchivoCabeza = "cabezon_Forbes_Tca.png", ArchivoCamiseta = "camiseta_IslasTurcasYCaicos.png", ArchivoBandera = "bandera_IslasTurcasYCaicos.png" },
            new() { Nombre = "Bahamas", FifaCode = "BAH", ArchivoCabeza = "cabezon_StFleur_Bah.png", ArchivoCamiseta = "camiseta_Bahamas.png", ArchivoBandera = "bandera_Bahamas.png" },
            new() { Nombre = "Islas Vírgenes Estadounidenses", FifaCode = "VIR", ArchivoCabeza = "cabezon_Ramos_Vir.png", ArchivoCamiseta = "camiseta_IslasVirgenesEstadounidenses.png", ArchivoBandera = "bandera_IslasVirgenesEstadounidenses.png" },
            new() { Nombre = "Islas Vírgenes Británicas", FifaCode = "VGB", ArchivoCabeza = "cabezon_Javier_Vgb.png", ArchivoCamiseta = "camiseta_IslasVirgenesBritanicas.png", ArchivoBandera = "bandera_IslasVirgenesBritanicas.png" },
            new() { Nombre = "Anguila", FifaCode = "AIA", ArchivoCabeza = "cabezon_Scipio_Aia.png", ArchivoCamiseta = "camiseta_Anguila.png", ArchivoBandera = "bandera_Anguila.png" },
        };

        return ConstruirEquipos("Norte y Centroamérica", "concacaf", definiciones);
    }

    public static List<TeamData> ObtenerEquiposCAF()
    {
        var definiciones = new List<DefinicionEquipo>
        {
            new() { Nombre = "Marruecos", FifaCode = "MAR", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Marruecos.png", ArchivoBandera = "bandera_Marruecos.png" },
            new() { Nombre = "Senegal", FifaCode = "SEN", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Senegal.png", ArchivoBandera = "bandera_Senegal.png" },
            new() { Nombre = "Nigeria", FifaCode = "NGA", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Nigeria.png", ArchivoBandera = "bandera_Nigeria.png" },
            new() { Nombre = "Argelia", FifaCode = "ALG", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Argelia.png", ArchivoBandera = "bandera_Argelia.png" },
            new() { Nombre = "Egipto", FifaCode = "EGY", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Egipto.png", ArchivoBandera = "bandera_Egipto.png" },
            new() { Nombre = "Costa de Marfil", FifaCode = "CIV", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_CostaDeMarfil.png", ArchivoBandera = "bandera_CostaDeMarfil.png" },
            new() { Nombre = "Camerún", FifaCode = "CMR", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Camerun.png", ArchivoBandera = "bandera_Camerun.png" },
            new() { Nombre = "Túnez", FifaCode = "TUN", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Tunez.png", ArchivoBandera = "bandera_Tunez.png" },
            new() { Nombre = "RD Congo", FifaCode = "COD", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_RdCongo.png", ArchivoBandera = "bandera_RdCongo.png" },
            new() { Nombre = "Malí", FifaCode = "MLI", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Mali.png", ArchivoBandera = "bandera_Mali.png" },
            new() { Nombre = "Sudáfrica", FifaCode = "RSA", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Sudafrica.png", ArchivoBandera = "bandera_Sudafrica.png" },
            new() { Nombre = "Burkina Faso", FifaCode = "BFA", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_BurkinaFaso.png", ArchivoBandera = "bandera_BurkinaFaso.png" },
            new() { Nombre = "Cabo Verde", FifaCode = "CPV", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_CaboVerde.png", ArchivoBandera = "bandera_CaboVerde.png" },
            new() { Nombre = "Ghana", FifaCode = "GHA", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Ghana.png", ArchivoBandera = "bandera_Ghana.png" },
            new() { Nombre = "Guinea", FifaCode = "GUI", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Guinea.png", ArchivoBandera = "bandera_Guinea.png" },
            new() { Nombre = "Gabón", FifaCode = "GAB", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Gabon.png", ArchivoBandera = "bandera_Gabon.png" },
            new() { Nombre = "Angola", FifaCode = "ANG", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Angola.png", ArchivoBandera = "bandera_Angola.png" },
            new() { Nombre = "Uganda", FifaCode = "UGA", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Uganda.png", ArchivoBandera = "bandera_Uganda.png" },
            new() { Nombre = "Zambia", FifaCode = "ZAM", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Zambia.png", ArchivoBandera = "bandera_Zambia.png" },
            new() { Nombre = "Benín", FifaCode = "BEN", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Benin.png", ArchivoBandera = "bandera_Benin.png" },
            new() { Nombre = "Mozambique", FifaCode = "MOZ", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Mozambique.png", ArchivoBandera = "bandera_Mozambique.png" },
            new() { Nombre = "Madagascar", FifaCode = "MAD", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Madagascar.png", ArchivoBandera = "bandera_Madagascar.png" },
            new() { Nombre = "Guinea Ecuatorial", FifaCode = "EQG", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_GuineaEcuatorial.png", ArchivoBandera = "bandera_GuineaEcuatorial.png" },
            new() { Nombre = "Comoras", FifaCode = "COM", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Comoras.png", ArchivoBandera = "bandera_Comoras.png" },
            new() { Nombre = "Kenia", FifaCode = "KEN", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Kenia.png", ArchivoBandera = "bandera_Kenia.png" },
            new() { Nombre = "Libia", FifaCode = "LBY", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Libia.png", ArchivoBandera = "bandera_Libia.png" },
            new() { Nombre = "Tanzania", FifaCode = "TAN", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Tanzania.png", ArchivoBandera = "bandera_Tanzania.png" },
            new() { Nombre = "Mauritania", FifaCode = "MTN", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Mauritania.png", ArchivoBandera = "bandera_Mauritania.png" },
            new() { Nombre = "Níger", FifaCode = "NIG", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Niger.png", ArchivoBandera = "bandera_Niger.png" },
            new() { Nombre = "Gambia", FifaCode = "GAM", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Gambia.png", ArchivoBandera = "bandera_Gambia.png" },
            new() { Nombre = "Sudán", FifaCode = "SDN", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Sudan.png", ArchivoBandera = "bandera_Sudan.png" },
            new() { Nombre = "Togo", FifaCode = "TOG", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Togo.png", ArchivoBandera = "bandera_Togo.png" },
            new() { Nombre = "Namibia", FifaCode = "NAM", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Namibia.png", ArchivoBandera = "bandera_Namibia.png" },
            new() { Nombre = "Sierra Leona", FifaCode = "SLE", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_SierraLeona.png", ArchivoBandera = "bandera_SierraLeona.png" },
            new() { Nombre = "Ruanda", FifaCode = "RWA", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Ruanda.png", ArchivoBandera = "bandera_Ruanda.png" },
            new() { Nombre = "Malaui", FifaCode = "MWI", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Malaui.png", ArchivoBandera = "bandera_Malaui.png" },
            new() { Nombre = "Zimbabue", FifaCode = "ZIM", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Zimbabue.png", ArchivoBandera = "bandera_Zimbabue.png" },
            new() { Nombre = "Guinea-Bisáu", FifaCode = "GNB", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_GuineaBisau.png", ArchivoBandera = "bandera_GuineaBisau.png" },
            new() { Nombre = "Congo", FifaCode = "CGO", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Congo.png", ArchivoBandera = "bandera_Congo.png" },
            new() { Nombre = "República Centroafricana", FifaCode = "CTA", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_RepublicaCentroafricana.png", ArchivoBandera = "bandera_RepublicaCentroafricana.png" },
            new() { Nombre = "Liberia", FifaCode = "LBR", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Liberia.png", ArchivoBandera = "bandera_Liberia.png" },
            new() { Nombre = "Burundi", FifaCode = "BDI", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Burundi.png", ArchivoBandera = "bandera_Burundi.png" },
            new() { Nombre = "Etiopía", FifaCode = "ETH", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Etiopia.png", ArchivoBandera = "bandera_Etiopia.png" },
            new() { Nombre = "Lesoto", FifaCode = "LES", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Lesoto.png", ArchivoBandera = "bandera_Lesoto.png" },
            new() { Nombre = "Botsuana", FifaCode = "BOT", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Botsuana.png", ArchivoBandera = "bandera_Botsuana.png" },
            new() { Nombre = "Suazilandia", FifaCode = "SWZ", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Suazilandia.png", ArchivoBandera = "bandera_Suazilandia.png" },
            new() { Nombre = "Sudán del Sur", FifaCode = "SSD", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_SudanDelSur.png", ArchivoBandera = "bandera_SudanDelSur.png" },
            new() { Nombre = "Mauricio", FifaCode = "MRI", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Mauricio.png", ArchivoBandera = "bandera_Mauricio.png" },
            new() { Nombre = "Chad", FifaCode = "CHA", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Chad.png", ArchivoBandera = "bandera_Chad.png" },
            new() { Nombre = "Eritrea", FifaCode = "ERI", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Eritrea.png", ArchivoBandera = "bandera_Eritrea.png" },
            new() { Nombre = "Santo Tomé y Príncipe", FifaCode = "STP", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_SantoTomeYPrincipe.png", ArchivoBandera = "bandera_SantoTomeYPrincipe.png" },
            new() { Nombre = "Yibuti", FifaCode = "DJI", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Yibuti.png", ArchivoBandera = "bandera_Yibuti.png" },
            new() { Nombre = "Somalia", FifaCode = "SOM", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Somalia.png", ArchivoBandera = "bandera_Somalia.png" },
            new() { Nombre = "Seychelles", FifaCode = "SEY", ArchivoCabeza = "cabezon_generico.png", ArchivoCamiseta = "camiseta_Seychelles.png", ArchivoBandera = "bandera_Seychelles.png" }
        };

        return ConstruirEquipos("África", "caf", definiciones);
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
        todosLosEquipos.AddRange(ObtenerEquiposCAF());

        return todosLosEquipos.Find(e => e.TeamName == nombreEquipo);
    }
}
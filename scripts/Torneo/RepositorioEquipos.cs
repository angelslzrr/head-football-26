using Godot;
using System.Collections.Generic;

/// <summary>
/// Capa de acceso a datos estáticos (Data Repository).
/// Centraliza la instanciación e inicialización de todos los equipos y sus assets.
/// Su estructura permite una alta escalabilidad para añadir futuras confederaciones (UEFA, CONCACAF, etc.).
/// </summary>
public static class RepositorioEquipos
{
    private static Texture2D ObtenerChimpunAleatorio()
    {
        int randomNum = GD.RandRange(1, 15);
        return GD.Load<Texture2D>($"res://img/chimpunes/chimpun{randomNum}.png");
    }

    public static List<TeamData> ObtenerEquiposConmebol()
    {
        return new List<TeamData>
        {
            new TeamData { 
                TeamName = "Argentina", Region = "Sudamérica", StarRating = 5.0f, FifaCode = "ARG",
                FlagTexture = GD.Load<Texture2D>("res://img/banderas/conmebol/bandera_Argentina.png"),
                CabezaTexture = GD.Load<Texture2D>("res://img/cabezones/conmebol/cabezon_Messi_Arg.png"),
                CamisetaTexture = GD.Load<Texture2D>("res://img/camisetas/conmebol/camiseta_Argentina.png"),
                ChimpunTexture = ObtenerChimpunAleatorio() 
            },
            new TeamData { 
                TeamName = "Brasil", Region = "Sudamérica", StarRating = 4.5f, FifaCode = "BRA",
                FlagTexture = GD.Load<Texture2D>("res://img/banderas/conmebol/bandera_Brasil.png"),
                CabezaTexture = GD.Load<Texture2D>("res://img/cabezones/conmebol/cabezon_Neymar_Bra.png"),
                CamisetaTexture = GD.Load<Texture2D>("res://img/camisetas/conmebol/camiseta_Brasil.png"),
                ChimpunTexture = ObtenerChimpunAleatorio() 
            },
            new TeamData { 
                TeamName = "Colombia", Region = "Sudamérica", StarRating = 4.5f, FifaCode = "COL",
                FlagTexture = GD.Load<Texture2D>("res://img/banderas/conmebol/bandera_Colombia.png"),
                CabezaTexture = GD.Load<Texture2D>("res://img/cabezones/conmebol/cabezon_LuisDiaz_Col.png"),
                CamisetaTexture = GD.Load<Texture2D>("res://img/camisetas/conmebol/camiseta_Colombia.png"),
                ChimpunTexture = ObtenerChimpunAleatorio() 
            },
            new TeamData { 
                TeamName = "Uruguay", Region = "Sudamérica", StarRating = 4.0f, FifaCode = "URU",
                FlagTexture = GD.Load<Texture2D>("res://img/banderas/conmebol/bandera_Uruguay.png"),
                CabezaTexture = GD.Load<Texture2D>("res://img/cabezones/conmebol/cabezon_Darwin_Uru.png"),
                CamisetaTexture = GD.Load<Texture2D>("res://img/camisetas/conmebol/camiseta_Uruguay.png"),
                ChimpunTexture = ObtenerChimpunAleatorio() 
            },
            new TeamData { 
                TeamName = "Ecuador", Region = "Sudamérica", StarRating = 4.0f, FifaCode = "ECU",
                FlagTexture = GD.Load<Texture2D>("res://img/banderas/conmebol/bandera_Ecuador.png"),
                CabezaTexture = GD.Load<Texture2D>("res://img/cabezones/conmebol/cabezon_Caicedo_Ecu.png"),
                CamisetaTexture = GD.Load<Texture2D>("res://img/camisetas/conmebol/camiseta_Ecuador.png"),
                ChimpunTexture = ObtenerChimpunAleatorio() 
            },
            new TeamData { 
                TeamName = "Paraguay", Region = "Sudamérica", StarRating = 3.5f, FifaCode = "PAR",
                FlagTexture = GD.Load<Texture2D>("res://img/banderas/conmebol/bandera_Paraguay.png"),
                CabezaTexture = GD.Load<Texture2D>("res://img/cabezones/conmebol/cabezon_Enciso_Par.png"),
                CamisetaTexture = GD.Load<Texture2D>("res://img/camisetas/conmebol/camiseta_Paraguay.png"),
                ChimpunTexture = ObtenerChimpunAleatorio() 
            },
            new TeamData { 
                TeamName = "Venezuela", Region = "Sudamérica", StarRating = 3.5f, FifaCode = "VEN",
                FlagTexture = GD.Load<Texture2D>("res://img/banderas/conmebol/bandera_Venezuela.png"),
                CabezaTexture = GD.Load<Texture2D>("res://img/cabezones/conmebol/cabezon_Soteldo_Ven.png"),
                CamisetaTexture = GD.Load<Texture2D>("res://img/camisetas/conmebol/camiseta_Venezuela.png"),
                ChimpunTexture = ObtenerChimpunAleatorio() 
            },
            new TeamData { 
                TeamName = "Chile", Region = "Sudamérica", StarRating = 3.5f, FifaCode = "CHI",
                FlagTexture = GD.Load<Texture2D>("res://img/banderas/conmebol/bandera_Chile.png"),
                CabezaTexture = GD.Load<Texture2D>("res://img/cabezones/conmebol/cabezon_Brereton_Chi.png"),
                CamisetaTexture = GD.Load<Texture2D>("res://img/camisetas/conmebol/camiseta_Chile.png"),
                ChimpunTexture = ObtenerChimpunAleatorio() 
            },
            new TeamData { 
                TeamName = "Perú", Region = "Sudamérica", StarRating = 3.5f, FifaCode = "PER",
                FlagTexture = GD.Load<Texture2D>("res://img/banderas/conmebol/bandera_Peru.png"),
                CabezaTexture = GD.Load<Texture2D>("res://img/cabezones/conmebol/cabezon_Guerrero_Per.png"),
                CamisetaTexture = GD.Load<Texture2D>("res://img/camisetas/conmebol/camiseta_Peru.png"),
                ChimpunTexture = ObtenerChimpunAleatorio() 
            },
            new TeamData { 
                TeamName = "Bolivia", Region = "Sudamérica", StarRating = 3.0f, FifaCode = "BOL",
                FlagTexture = GD.Load<Texture2D>("res://img/banderas/conmebol/bandera_Bolivia.png"),
                CabezaTexture = GD.Load<Texture2D>("res://img/cabezones/conmebol/cabezon_Terceros_Bol.png"),
                CamisetaTexture = GD.Load<Texture2D>("res://img/camisetas/conmebol/camiseta_Bolivia.png"),
                ChimpunTexture = ObtenerChimpunAleatorio() 
            }
        };
    }
}
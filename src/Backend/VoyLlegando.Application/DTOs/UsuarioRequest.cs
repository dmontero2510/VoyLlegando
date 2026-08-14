namespace VoyLlegando.Application.DTOs;

public class UsuarioRequest
{
    public string Celular { get; set; } = "";
    public string Clave { get; set; } = "";
    public string Nombre { get; set; } = "";

    public string? Domicilio { get; set; }

    public string Iva { get; set; } = "1";
    public string Cuit { get; set; } = "";
    public string Email { get; set; } = "";

    public string Rol { get; set; } = "";

    public int? IdTranspor { get; set; }
    public int? IdPlanta { get; set; }
    public int? IdProduc { get; set; }

    // ---------------------------------------------
    // DATOS DE EMPRESA DE TRANSPORTE
    // ---------------------------------------------

    public string? PatChasis { get; set; }
    public string? PatAcopla { get; set; }

    public bool Batea { get; set; }
    public bool Corta { get; set; }
    public bool Larga { get; set; }
    public bool Escala { get; set; }

    // ---------------------------------------------
    // ESTADO
    //
    // Para E lo maneja el backend:
    // D = Disponible
    // V = Viajando
    // N = No Disponible
    //
    // No se debe enviar desde el cliente.
    // ---------------------------------------------

    public string? Estado { get; set; }
}
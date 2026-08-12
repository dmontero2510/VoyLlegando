using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using VoyLlegando.Application.Interfaces;
using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Infrastructure.Repositories;

public class TipoIvaRepository : ITipoIvaRepository
{
    private readonly string _connectionString;

    public TipoIvaRepository(
        IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No se encontró la cadena de conexión DefaultConnection.");
    }

    private NpgsqlConnection CrearConexion()
    {
        return new NpgsqlConnection(
            _connectionString);
    }

    public async Task<IEnumerable<TipoIva>> ObtenerTodosAsync()
    {
        const string sql = """
            SELECT
                id_iva      AS IdIva,
                descrip_iva AS DescripIva
            FROM tiposiva
            ORDER BY id_iva;
            """;

        using var connection =
            CrearConexion();

        return await connection
            .QueryAsync<TipoIva>(sql);
    }
}
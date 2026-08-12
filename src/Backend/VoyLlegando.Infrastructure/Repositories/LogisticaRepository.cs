using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using VoyLlegando.Application.Interfaces;
using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Infrastructure.Repositories;

public class LogisticaRepository
    : ILogisticaRepository
{
    private readonly string _connectionString;


    public LogisticaRepository(
        IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString(
                "DefaultConnection")
            ?? throw new InvalidOperationException(
                "No se encontró la cadena de conexión DefaultConnection.");
    }


    private NpgsqlConnection CrearConexion()
    {
        return new NpgsqlConnection(
            _connectionString);
    }


    // -------------------------------------------------------
    // OBTENER POR ID
    // -------------------------------------------------------

    public async Task<Logistica?>
        ObtenerPorIdAsync(
            int idTranspor)
    {
        using var connection =
            CrearConexion();


        const string sql = """
            SELECT
                id_transpor AS IdTranspor,
                nombre      AS Nombre,
                domicilio   AS Domicilio,
                iva         AS Iva,
                cuit        AS Cuit,
                habilitado  AS Habilitado
            FROM public.logisticas
            WHERE id_transpor = @IdTranspor;
            """;


        return await connection
            .QueryFirstOrDefaultAsync<Logistica>(
                sql,
                new
                {
                    IdTranspor = idTranspor
                });
    }


    // -------------------------------------------------------
    // OBTENER TODAS
    // -------------------------------------------------------

    public async Task<IEnumerable<Logistica>>
        ObtenerTodosAsync()
    {
        using var connection =
            CrearConexion();


        const string sql = """
            SELECT
                id_transpor AS IdTranspor,
                nombre      AS Nombre,
                domicilio   AS Domicilio,
                iva         AS Iva,
                cuit        AS Cuit,
                habilitado  AS Habilitado
            FROM public.logisticas
            ORDER BY nombre;
            """;


        return await connection
            .QueryAsync<Logistica>(
                sql);
    }


    // -------------------------------------------------------
    // CREAR
    // -------------------------------------------------------

    public async Task<int> CrearAsync(
        Logistica logistica)
    {
        using var connection =
            CrearConexion();


        const string sql = """
            INSERT INTO public.logisticas
            (
                nombre,
                domicilio,
                iva,
                cuit,
                habilitado
            )
            VALUES
            (
                @Nombre,
                @Domicilio,
                @Iva,
                @Cuit,
                @Habilitado
            )
            RETURNING id_transpor;
            """;


        return await connection
            .ExecuteScalarAsync<int>(
                sql,
                logistica);
    }
}
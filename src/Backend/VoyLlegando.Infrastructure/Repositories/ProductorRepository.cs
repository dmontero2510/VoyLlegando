using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using VoyLlegando.Application.Interfaces;
using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Infrastructure.Repositories;

public class ProductorRepository
    : IProductorRepository
{
    private readonly string _connectionString;

    public ProductorRepository(
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

    public async Task<IEnumerable<Productor>>
        ObtenerTodosAsync(
            int idTranspor)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            SELECT
                id_productor AS IdProductor,
                id_transpor  AS IdTranspor,
                nombre       AS Nombre,
                domicilio    AS Domicilio,
                iva          AS Iva,
                cuit         AS Cuit,
                habilitado   AS Habilitado
            FROM public.productores
            WHERE id_transpor = @IdTranspor
            ORDER BY nombre;
            """;

        return await connection
            .QueryAsync<Productor>(
                sql,
                new
                {
                    IdTranspor = idTranspor
                });
    }

    public async Task<Productor?>
        ObtenerPorIdAsync(
            int idProductor,
            int idTranspor)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            SELECT
                id_productor AS IdProductor,
                id_transpor  AS IdTranspor,
                nombre       AS Nombre,
                domicilio    AS Domicilio,
                iva          AS Iva,
                cuit         AS Cuit,
                habilitado   AS Habilitado
            FROM public.productores
            WHERE id_productor = @IdProductor
              AND id_transpor = @IdTranspor;
            """;

        return await connection
            .QueryFirstOrDefaultAsync<Productor>(
                sql,
                new
                {
                    IdProductor = idProductor,
                    IdTranspor = idTranspor
                });
    }

    public async Task<int> CrearAsync(
        Productor productor)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            INSERT INTO public.productores
            (
                id_transpor,
                nombre,
                domicilio,
                iva,
                cuit,
                habilitado
            )
            VALUES
            (
                @IdTranspor,
                @Nombre,
                @Domicilio,
                @Iva,
                @Cuit,
                @Habilitado
            )
            RETURNING id_productor;
            """;

        return await connection
            .ExecuteScalarAsync<int>(
                sql,
                productor);
    }

    public async Task ActualizarAsync(
        Productor productor)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            UPDATE public.productores
            SET
                nombre     = @Nombre,
                domicilio  = @Domicilio,
                iva        = @Iva,
                cuit       = @Cuit,
                habilitado = @Habilitado
            WHERE id_productor = @IdProductor
              AND id_transpor = @IdTranspor;
            """;

        await connection.ExecuteAsync(
            sql,
            productor);
    }

    public async Task BajaAsync(
        int idProductor,
        int idTranspor)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            UPDATE public.productores
            SET habilitado = false
            WHERE id_productor = @IdProductor
              AND id_transpor = @IdTranspor;
            """;

        await connection.ExecuteAsync(
            sql,
            new
            {
                IdProductor = idProductor,
                IdTranspor = idTranspor
            });
    }
}
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using VoyLlegando.Application.Interfaces;
using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Infrastructure.Repositories;

public class PlantaRepository
    : IPlantaRepository
{
    private readonly string _connectionString;

    public PlantaRepository(
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

    public async Task<IEnumerable<Planta>>
        ObtenerTodosAsync(
            int idTranspor)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            SELECT
                id_planta   AS IdPlanta,
                id_transpor AS IdTranspor,
                nombre      AS Nombre,
                domicilio   AS Domicilio,
                iva         AS Iva,
                cuit        AS Cuit,
                habilitado  AS Habilitado
            FROM public.plantas
            WHERE id_transpor = @IdTranspor
            ORDER BY nombre;
            """;

        return await connection
            .QueryAsync<Planta>(
                sql,
                new
                {
                    IdTranspor = idTranspor
                });
    }

    public async Task<Planta?>
        ObtenerPorIdAsync(
            int idPlanta,
            int idTranspor)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            SELECT
                id_planta   AS IdPlanta,
                id_transpor AS IdTranspor,
                nombre      AS Nombre,
                domicilio   AS Domicilio,
                iva         AS Iva,
                cuit        AS Cuit,
                habilitado  AS Habilitado
            FROM public.plantas
            WHERE id_planta = @IdPlanta
              AND id_transpor = @IdTranspor;
            """;

        return await connection
            .QueryFirstOrDefaultAsync<Planta>(
                sql,
                new
                {
                    IdPlanta = idPlanta,
                    IdTranspor = idTranspor
                });
    }

    public async Task<int> CrearAsync(
        Planta planta)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            INSERT INTO public.plantas
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
            RETURNING id_planta;
            """;

        return await connection
            .ExecuteScalarAsync<int>(
                sql,
                planta);
    }

    public async Task ActualizarAsync(
        Planta planta)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            UPDATE public.plantas
            SET
                nombre     = @Nombre,
                domicilio  = @Domicilio,
                iva        = @Iva,
                cuit       = @Cuit,
                habilitado = @Habilitado
            WHERE id_planta = @IdPlanta
              AND id_transpor = @IdTranspor;
            """;

        await connection.ExecuteAsync(
            sql,
            planta);
    }

    public async Task BajaAsync(
        int idPlanta,
        int idTranspor)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            UPDATE public.plantas
            SET habilitado = false
            WHERE id_planta = @IdPlanta
              AND id_transpor = @IdTranspor;
            """;

        await connection.ExecuteAsync(
            sql,
            new
            {
                IdPlanta = idPlanta,
                IdTranspor = idTranspor
            });
    }
}
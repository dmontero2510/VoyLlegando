using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using VoyLlegando.Application.Interfaces;
using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Infrastructure.Repositories;

public class DestinoRepository
    : IDestinoRepository
{
    private readonly string _connectionString;

    public DestinoRepository(
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
    // DESTINOS DE UNA PLANTA
    // -------------------------------------------------------

    public async Task<IEnumerable<Destino>>
        ObtenerPorPlantaAsync(
            int idPlanta)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            SELECT
                id_destino         AS IdDestino,
                id_planta          AS IdPlanta,
                descrip_destino    AS DescripDestino,
                fecha_vinculacion  AS FechaVinculacion,
                latitud            AS Latitud,
                longitud           AS Longitud
            FROM public.destinos
            WHERE id_planta = @IdPlanta
            ORDER BY descrip_destino;
            """;

        return await connection
            .QueryAsync<Destino>(
                sql,
                new
                {
                    IdPlanta =
                        idPlanta
                });
    }

    // -------------------------------------------------------
    // POR ID
    // -------------------------------------------------------

    public async Task<Destino?>
        ObtenerPorIdAsync(
            int idDestino)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            SELECT
                id_destino         AS IdDestino,
                id_planta          AS IdPlanta,
                descrip_destino    AS DescripDestino,
                fecha_vinculacion  AS FechaVinculacion,
                latitud            AS Latitud,
                longitud           AS Longitud
            FROM public.destinos
            WHERE id_destino = @IdDestino;
            """;

        return await connection
            .QueryFirstOrDefaultAsync<Destino>(
                sql,
                new
                {
                    IdDestino =
                        idDestino
                });
    }

    // -------------------------------------------------------
    // CREAR
    // -------------------------------------------------------

    public async Task<int> CrearAsync(
        Destino destino)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            INSERT INTO public.destinos
            (
                id_planta,
                descrip_destino,
                latitud,
                longitud
            )
            VALUES
            (
                @IdPlanta,
                @DescripDestino,
                @Latitud,
                @Longitud
            )
            RETURNING id_destino;
            """;

        return await connection
            .ExecuteScalarAsync<int>(
                sql,
                destino);
    }

    // -------------------------------------------------------
    // ACTUALIZAR
    // -------------------------------------------------------

    public async Task ActualizarAsync(
        Destino destino)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            UPDATE public.destinos
            SET
                id_planta        = @IdPlanta,
                descrip_destino  = @DescripDestino,
                latitud          = @Latitud,
                longitud         = @Longitud
            WHERE id_destino = @IdDestino;
            """;

        await connection.ExecuteAsync(
            sql,
            destino);
    }

    // -------------------------------------------------------
    // ELIMINAR
    // -------------------------------------------------------

    public async Task EliminarAsync(
        int idDestino)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            DELETE FROM public.destinos
            WHERE id_destino = @IdDestino;
            """;

        await connection.ExecuteAsync(
            sql,
            new
            {
                IdDestino =
                    idDestino
            });
    }
}
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using VoyLlegando.Application.Interfaces;
using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Infrastructure.Repositories;

public class CampoRepository
    : ICampoRepository
{
    private readonly string _connectionString;

    public CampoRepository(
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
    // CAMPOS DE UN PRODUCTOR
    // -------------------------------------------------------

    public async Task<IEnumerable<Campo>>
        ObtenerPorProductorAsync(
            int idProductor)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            SELECT
                id_campo       AS IdCampo,
                id_productor   AS IdProductor,
                descrip_campo  AS DescripCampo,
                latitud        AS Latitud,
                longitud       AS Longitud
            FROM public.campos
            WHERE id_productor = @IdProductor
            ORDER BY descrip_campo;
            """;

        return await connection
            .QueryAsync<Campo>(
                sql,
                new
                {
                    IdProductor =
                        idProductor
                });
    }

    // -------------------------------------------------------
    // POR ID
    // -------------------------------------------------------

    public async Task<Campo?>
        ObtenerPorIdAsync(
            int idCampo)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            SELECT
                id_campo       AS IdCampo,
                id_productor   AS IdProductor,
                descrip_campo  AS DescripCampo,
                latitud        AS Latitud,
                longitud       AS Longitud
            FROM public.campos
            WHERE id_campo = @IdCampo;
            """;

        return await connection
            .QueryFirstOrDefaultAsync<Campo>(
                sql,
                new
                {
                    IdCampo =
                        idCampo
                });
    }

    // -------------------------------------------------------
    // CREAR
    // -------------------------------------------------------

    public async Task<int> CrearAsync(
        Campo campo)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            INSERT INTO public.campos
            (
                id_productor,
                descrip_campo,
                latitud,
                longitud
            )
            VALUES
            (
                @IdProductor,
                @DescripCampo,
                @Latitud,
                @Longitud
            )
            RETURNING id_campo;
            """;

        return await connection
            .ExecuteScalarAsync<int>(
                sql,
                campo);
    }

    // -------------------------------------------------------
    // ACTUALIZAR
    // -------------------------------------------------------

    public async Task ActualizarAsync(
        Campo campo)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            UPDATE public.campos
            SET
                id_productor  = @IdProductor,
                descrip_campo = @DescripCampo,
                latitud       = @Latitud,
                longitud      = @Longitud
            WHERE id_campo = @IdCampo;
            """;

        await connection.ExecuteAsync(
            sql,
            campo);
    }

    // -------------------------------------------------------
    // ELIMINAR
    // -------------------------------------------------------

    public async Task EliminarAsync(
        int idCampo)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            DELETE FROM public.campos
            WHERE id_campo = @IdCampo;
            """;

        await connection.ExecuteAsync(
            sql,
            new
            {
                IdCampo =
                    idCampo
            });
    }
}
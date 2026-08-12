using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using VoyLlegando.Application.Interfaces;
using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Infrastructure.Repositories;

public class CerealRepository
    : ICerealRepository
{
    private readonly string _connectionString;

    public CerealRepository(
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
    // TODOS
    // -------------------------------------------------------

    public async Task<IEnumerable<Cereal>>
        ObtenerTodosAsync()
    {
        using var connection =
            CrearConexion();

        const string sql = """
            SELECT
                id_cereal      AS IdCereal,
                nombre_cereal  AS NombreCereal,
                habilitado     AS Habilitado
            FROM public.cereales
            ORDER BY nombre_cereal;
            """;

        return await connection
            .QueryAsync<Cereal>(
                sql);
    }

    // -------------------------------------------------------
    // HABILITADOS
    // -------------------------------------------------------

    public async Task<IEnumerable<Cereal>>
        ObtenerHabilitadosAsync()
    {
        using var connection =
            CrearConexion();

        const string sql = """
            SELECT
                id_cereal      AS IdCereal,
                nombre_cereal  AS NombreCereal,
                habilitado     AS Habilitado
            FROM public.cereales
            WHERE habilitado = true
            ORDER BY nombre_cereal;
            """;

        return await connection
            .QueryAsync<Cereal>(
                sql);
    }

    // -------------------------------------------------------
    // POR ID
    // -------------------------------------------------------

    public async Task<Cereal?>
        ObtenerPorIdAsync(
            int idCereal)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            SELECT
                id_cereal      AS IdCereal,
                nombre_cereal  AS NombreCereal,
                habilitado     AS Habilitado
            FROM public.cereales
            WHERE id_cereal = @IdCereal;
            """;

        return await connection
            .QueryFirstOrDefaultAsync<Cereal>(
                sql,
                new
                {
                    IdCereal =
                        idCereal
                });
    }

    // -------------------------------------------------------
    // CREAR
    // -------------------------------------------------------

    public async Task CrearAsync(
        Cereal cereal)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            INSERT INTO public.cereales
            (
                id_cereal,
                nombre_cereal,
                habilitado
            )
            VALUES
            (
                @IdCereal,
                @NombreCereal,
                @Habilitado
            );
            """;

        await connection.ExecuteAsync(
            sql,
            cereal);
    }

    // -------------------------------------------------------
    // ACTUALIZAR
    // -------------------------------------------------------

    public async Task ActualizarAsync(
        Cereal cereal)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            UPDATE public.cereales
            SET
                nombre_cereal = @NombreCereal,
                habilitado    = @Habilitado
            WHERE id_cereal = @IdCereal;
            """;

        await connection.ExecuteAsync(
            sql,
            cereal);
    }

    // -------------------------------------------------------
    // BAJA LOGICA
    // -------------------------------------------------------

    public async Task BajaAsync(
        int idCereal)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            UPDATE public.cereales
            SET habilitado = false
            WHERE id_cereal = @IdCereal;
            """;

        await connection.ExecuteAsync(
            sql,
            new
            {
                IdCereal =
                    idCereal
            });
    }
}
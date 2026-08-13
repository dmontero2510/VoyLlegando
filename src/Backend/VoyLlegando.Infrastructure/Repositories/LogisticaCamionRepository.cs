using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using VoyLlegando.Application.Interfaces;
using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Infrastructure.Repositories;

public class LogisticaCamionRepository
    : ILogisticaCamionRepository
{
    private readonly string _connectionString;


    public LogisticaCamionRepository(
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
    // OBTENER LOGISTICAS VINCULADAS
    // -------------------------------------------------------

    public async Task<IEnumerable<Logistica>>
        ObtenerVinculadasAsync(
            int idUsuario)
    {
        using var connection =
            CrearConexion();


        const string sql = """
            SELECT
                l.id_transpor AS IdTranspor,
                l.nombre      AS Nombre,
                l.domicilio   AS Domicilio,
                l.iva         AS Iva,
                l.cuit        AS Cuit,
                l.habilitado  AS Habilitado
            FROM public.logisticas l
            INNER JOIN public.logiscamion lc
                ON lc.id_transpor = l.id_transpor
            WHERE lc.id_usuario = @IdUsuario
              AND lc.habilitado = TRUE
            ORDER BY l.nombre;
            """;


        return await connection
            .QueryAsync<Logistica>(
                sql,
                new
                {
                    IdUsuario = idUsuario
                });
    }


    // -------------------------------------------------------
    // OBTENER LOGISTICAS DISPONIBLES
    // -------------------------------------------------------

    public async Task<IEnumerable<Logistica>>
        ObtenerDisponiblesAsync(
            int idUsuario)
    {
        using var connection =
            CrearConexion();


        const string sql = """
            SELECT
                l.id_transpor AS IdTranspor,
                l.nombre      AS Nombre,
                l.domicilio   AS Domicilio,
                l.iva         AS Iva,
                l.cuit        AS Cuit,
                l.habilitado  AS Habilitado
            FROM public.logisticas l
            WHERE l.habilitado = TRUE
              AND NOT EXISTS
              (
                  SELECT 1
                  FROM public.logiscamion lc
                  WHERE lc.id_transpor = l.id_transpor
                    AND lc.id_usuario = @IdUsuario
                    AND lc.habilitado = TRUE
              )
            ORDER BY l.nombre;
            """;


        return await connection
            .QueryAsync<Logistica>(
                sql,
                new
                {
                    IdUsuario = idUsuario
                });
    }


    // -------------------------------------------------------
    // VINCULAR
    // -------------------------------------------------------

    public async Task VincularAsync(
        int idTranspor,
        int idUsuario)
    {
        using var connection =
            CrearConexion();


        const string sql = """
            INSERT INTO public.logiscamion
            (
                id_transpor,
                id_usuario,
                fecha_vinculacion,
                habilitado
            )
            VALUES
            (
                @IdTranspor,
                @IdUsuario,
                CURRENT_TIMESTAMP,
                TRUE
            )
            ON CONFLICT
            (
                id_transpor,
                id_usuario
            )
            DO UPDATE
            SET
                habilitado = TRUE,
                fecha_vinculacion = CURRENT_TIMESTAMP;
            """;


        await connection.ExecuteAsync(
            sql,
            new
            {
                IdTranspor = idTranspor,
                IdUsuario = idUsuario
            });
    }


    // -------------------------------------------------------
    // DESVINCULAR
    // -------------------------------------------------------

    public async Task DesvincularAsync(
        int idTranspor,
        int idUsuario)
    {
        using var connection =
            CrearConexion();


        const string sql = """
            UPDATE public.logiscamion
            SET
                habilitado = FALSE
            WHERE id_transpor = @IdTranspor
              AND id_usuario = @IdUsuario;
            """;


        await connection.ExecuteAsync(
            sql,
            new
            {
                IdTranspor = idTranspor,
                IdUsuario = idUsuario
            });
    }


    // -------------------------------------------------------
    // ESTA VINCULADO
    // -------------------------------------------------------

    public async Task<bool> EstaVinculadoAsync(
        int idTranspor,
        int idUsuario)
    {
        using var connection =
            CrearConexion();


        const string sql = """
            SELECT EXISTS
            (
                SELECT 1
                FROM public.logiscamion
                WHERE id_transpor = @IdTranspor
                  AND id_usuario = @IdUsuario
                  AND habilitado = TRUE
            );
            """;


        return await connection
            .ExecuteScalarAsync<bool>(
                sql,
                new
                {
                    IdTranspor = idTranspor,
                    IdUsuario = idUsuario
                });
    }
}
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
    // OBTENER LOGISTICAS VINCULADAS / ACEPTADAS
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
              AND lc.estado = 'A'
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
    // OBTENER LOGISTICAS DISPONIBLES PARA SOLICITAR
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
                    AND lc.estado IN ('P', 'A')
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
    // SOLICITAR VINCULACION
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
                estado
            )
            VALUES
            (
                @IdTranspor,
                @IdUsuario,
                CURRENT_TIMESTAMP,
                'P'
            )
            ON CONFLICT
            (
                id_transpor,
                id_usuario
            )
            DO UPDATE
            SET
                estado = 'P',
                fecha_vinculacion = CURRENT_TIMESTAMP
            WHERE public.logiscamion.estado IN ('R', 'B');
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
                estado = 'B'
            WHERE id_transpor = @IdTranspor
              AND id_usuario = @IdUsuario
              AND estado = 'A';
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
    // ESTA VINCULADO / ACEPTADO
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
                  AND estado = 'A'
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
// -------------------------------------------------------
// SOLICITUDES PENDIENTES DE UNA LOGISTICA
// -------------------------------------------------------

public async Task<IEnumerable<Usuario>>
    ObtenerSolicitudesPendientesAsync(
        int idTranspor)
{
    using var connection =
        CrearConexion();


    const string sql = """
        SELECT
            u.id_usuario AS IdUsuario,
            u.nombre     AS Nombre,
            u.domicilio  AS Domicilio,
            u.iva        AS Iva,
            u.cuit       AS Cuit,
            u.celular    AS Celular,
            u.email      AS Email,
            u.rol        AS Rol,
            u.habilitado AS Habilitado,
            u.estado     AS Estado
        FROM public.logiscamion lc
        INNER JOIN public.usuarios u
            ON u.id_usuario = lc.id_usuario
        WHERE lc.id_transpor = @IdTranspor
          AND lc.estado = 'P'
          AND u.rol = 'E'
        ORDER BY
            lc.fecha_vinculacion,
            u.nombre;
        """;


    return await connection
        .QueryAsync<Usuario>(
            sql,
            new
            {
                IdTranspor = idTranspor
            });
}


// -------------------------------------------------------
// ACEPTAR SOLICITUD
// -------------------------------------------------------

public async Task AceptarSolicitudAsync(
    int idTranspor,
    int idUsuario)
{
    using var connection =
        CrearConexion();


    const string sql = """
        UPDATE public.logiscamion
        SET estado = 'A'
        WHERE id_transpor = @IdTranspor
          AND id_usuario = @IdUsuario
          AND estado = 'P';
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
// RECHAZAR SOLICITUD
// -------------------------------------------------------

public async Task RechazarSolicitudAsync(
    int idTranspor,
    int idUsuario)
{
    using var connection =
        CrearConexion();


    const string sql = """
        UPDATE public.logiscamion
        SET estado = 'R'
        WHERE id_transpor = @IdTranspor
          AND id_usuario = @IdUsuario
          AND estado = 'P';
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
// EMPRESAS ACEPTADAS Y DISPONIBLES DE UNA LOGISTICA
// -------------------------------------------------------

public async Task<IEnumerable<Usuario>>
    ObtenerEmpresasAceptadasDisponiblesAsync(
        int idTranspor)
{
    using var connection =
        CrearConexion();


    const string sql = """
        SELECT
            u.id_usuario AS IdUsuario,
            u.nombre     AS Nombre,
            u.domicilio  AS Domicilio,
            u.iva        AS Iva,
            u.cuit       AS Cuit,
            u.celular    AS Celular,
            u.email      AS Email,
            u.rol        AS Rol,
            u.habilitado AS Habilitado,
            u.estado     AS Estado
        FROM public.usuarios u
        INNER JOIN public.logiscamion lc
            ON lc.id_usuario = u.id_usuario
        WHERE lc.id_transpor = @IdTranspor
          AND lc.estado = 'A'
          AND u.rol = 'E'
          AND u.habilitado = TRUE
          AND u.estado = 'D'
        ORDER BY u.nombre;
        """;


    return await connection
        .QueryAsync<Usuario>(
            sql,
            new
            {
                IdTranspor = idTranspor
            });
}


// -------------------------------------------------------
// RELACIONES DE UNA LOGISTICA
// A = ACEPTADA
// R = RECHAZADA
// B = BAJA / BLOQUEADA
// -------------------------------------------------------

public async Task<IEnumerable<LogisticaCamionRelacion>>
    ObtenerRelacionesAsync(
        int idTranspor)
{
    using var connection =
        CrearConexion();


    const string sql = """
        SELECT
            u.id_usuario             AS IdUsuario,
            u.nombre                 AS Nombre,
            u.cuit                   AS Cuit,
            u.celular                AS Celular,
            u.email                  AS Email,
            u.habilitado             AS Habilitado,
            u.estado                 AS EstadoEmpresa,
            lc.estado                AS EstadoRelacion,
            COALESCE(er.descripcion, lc.estado)
                                     AS DescripcionEstado,
            lc.fecha_vinculacion     AS FechaVinculacion
        FROM public.logiscamion lc
        INNER JOIN public.usuarios u
            ON u.id_usuario = lc.id_usuario
        LEFT JOIN public.estarela er
            ON er.codigo = lc.estado
        WHERE lc.id_transpor = @IdTranspor
          AND u.rol = 'E'
          AND lc.estado IN ('A', 'R', 'B')
        ORDER BY
            CASE lc.estado
                WHEN 'A' THEN 1
                WHEN 'B' THEN 2
                WHEN 'R' THEN 3
                ELSE 4
            END,
            u.nombre;
        """;


    return await connection
        .QueryAsync<LogisticaCamionRelacion>(
            sql,
            new
            {
                IdTranspor = idTranspor
            });
}


// -------------------------------------------------------
// BLOQUEAR RELACION
// A -> B
// -------------------------------------------------------

public async Task<bool> BloquearRelacionAsync(
    int idTranspor,
    int idUsuario)
{
    using var connection =
        CrearConexion();


    const string sql = """
        UPDATE public.logiscamion
        SET estado = 'B'
        WHERE id_transpor = @IdTranspor
          AND id_usuario = @IdUsuario
          AND estado = 'A';
        """;


    var filas =
        await connection.ExecuteAsync(
            sql,
            new
            {
                IdTranspor = idTranspor,
                IdUsuario = idUsuario
            });


    return filas > 0;
}


// -------------------------------------------------------
// REHABILITAR RELACION
// R/B -> A
// -------------------------------------------------------

public async Task<bool> RehabilitarRelacionAsync(
    int idTranspor,
    int idUsuario)
{
    using var connection =
        CrearConexion();


    const string sql = """
        UPDATE public.logiscamion
        SET estado = 'A'
        WHERE id_transpor = @IdTranspor
          AND id_usuario = @IdUsuario
          AND estado IN ('R', 'B');
        """;


    var filas =
        await connection.ExecuteAsync(
            sql,
            new
            {
                IdTranspor = idTranspor,
                IdUsuario = idUsuario
            });


    return filas > 0;
}
}

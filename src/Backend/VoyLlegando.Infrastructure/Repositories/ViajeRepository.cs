using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using VoyLlegando.Application.Interfaces;
using VoyLlegando.Domain.Entities;
using VoyLlegando.Application.DTOs;

namespace VoyLlegando.Infrastructure.Repositories;

public class ViajeRepository : IViajeRepository
{
    private readonly string _connectionString;

    public ViajeRepository(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No se encontró la cadena de conexión DefaultConnection.");
    }

    private NpgsqlConnection CrearConexion()
    {
        return new NpgsqlConnection(_connectionString);
    }


    // -------------------------------------------------------
    // OBTENER TODOS
    // -------------------------------------------------------

    public async Task<IEnumerable<Viaje>> ObtenerTodosAsync()
    {
        using var connection = CrearConexion();

        const string sql = """
            SELECT
                v.id_viaje       AS IdViaje,
                v.id_transpor     AS IdTranspor,
                v.id_camionero    AS IdCamionero,
                u.nombre          AS NombreEmpresa,
                v.tipo            AS Tipo,
                v.fecha_pedido    AS FechaPedido,
                v.id_cereal       AS IdCereal,
                v.id_produc       AS IdProduc,
                v.id_origen       AS IdOrigen,
                v.id_planta       AS IdPlanta,
                v.id_destino      AS IdDestino,
                v.origen          AS Origen,
                v.destino         AS Destino,
                v.ctg             AS Ctg,
                v.kms             AS Kms,
                v.tarifa          AS Tarifa,
                v.estado          AS Estado,
                v.fecha_asigna    AS FechaAsigna,
                v.fecha_termina   AS FechaTermina,
                v.observaciones   AS Observaciones,
                v.batea           AS Batea,
                v.corta           AS Corta,
                v.larga           AS Larga,
                v.id_usuario      AS IdUsuario,

                c.latitud         AS LatitudOrigen,
                c.longitud        AS LongitudOrigen,

                d.latitud         AS LatitudDestino,
                d.longitud        AS LongitudDestino

            FROM public.viajes v

            LEFT JOIN public.usuarios u
                ON u.id_usuario = v.id_camionero

            LEFT JOIN public.campos c
                ON c.id_campo = v.id_origen

            LEFT JOIN public.destinos d
                ON d.id_destino = v.id_destino

            ORDER BY v.id_viaje DESC;
            """;

        return await connection.QueryAsync<Viaje>(sql);
    }


    // -------------------------------------------------------
    // OBTENER POR TRANSPORTE
    // -------------------------------------------------------

    public async Task<IEnumerable<Viaje>> ObtenerPorTransporAsync(
        int idTranspor)
    {
        using var connection = CrearConexion();

        const string sql = """
            SELECT
                v.id_viaje       AS IdViaje,
                v.id_transpor     AS IdTranspor,
                v.id_camionero    AS IdCamionero,
                u.nombre          AS NombreEmpresa,
                v.tipo            AS Tipo,
                v.fecha_pedido    AS FechaPedido,
                v.id_cereal       AS IdCereal,
                v.id_produc       AS IdProduc,
                v.id_origen       AS IdOrigen,
                v.id_planta       AS IdPlanta,
                v.id_destino      AS IdDestino,
                v.origen          AS Origen,
                v.destino         AS Destino,
                v.ctg             AS Ctg,
                v.kms             AS Kms,
                v.tarifa          AS Tarifa,
                v.estado          AS Estado,
                v.fecha_asigna    AS FechaAsigna,
                v.fecha_termina   AS FechaTermina,
                v.observaciones   AS Observaciones,
                v.batea           AS Batea,
                v.corta           AS Corta,
                v.larga           AS Larga,
                v.id_usuario      AS IdUsuario,

                c.latitud         AS LatitudOrigen,
                c.longitud        AS LongitudOrigen,

                d.latitud         AS LatitudDestino,
                d.longitud        AS LongitudDestino

            FROM public.viajes v

            LEFT JOIN public.usuarios u
                ON u.id_usuario = v.id_camionero

            LEFT JOIN public.campos c
                ON c.id_campo = v.id_origen

            LEFT JOIN public.destinos d
                ON d.id_destino = v.id_destino

            WHERE v.id_transpor = @IdTranspor

            ORDER BY v.id_viaje DESC;
            """;

        return await connection.QueryAsync<Viaje>(
            sql,
            new { IdTranspor = idTranspor });
    }


    // -------------------------------------------------------
    // OBTENER POR CAMIONERO / EMPRESA
    // -------------------------------------------------------

    public async Task<IEnumerable<Viaje>> ObtenerPorCamioneroAsync(
        int idCamionero)
    {
        using var connection = CrearConexion();

        const string sql = """
            SELECT
                v.id_viaje       AS IdViaje,
                v.id_transpor     AS IdTranspor,
                v.id_camionero    AS IdCamionero,
                u.nombre          AS NombreEmpresa,
                v.tipo            AS Tipo,
                v.fecha_pedido    AS FechaPedido,
                v.id_cereal       AS IdCereal,
                v.id_produc       AS IdProduc,
                v.id_origen       AS IdOrigen,
                v.id_planta       AS IdPlanta,
                v.id_destino      AS IdDestino,
                v.origen          AS Origen,
                v.destino         AS Destino,
                v.ctg             AS Ctg,
                v.kms             AS Kms,
                v.tarifa          AS Tarifa,
                v.estado          AS Estado,
                v.fecha_asigna    AS FechaAsigna,
                v.fecha_termina   AS FechaTermina,
                v.observaciones   AS Observaciones,
                v.batea           AS Batea,
                v.corta           AS Corta,
                v.larga           AS Larga,
                v.id_usuario      AS IdUsuario,

                c.latitud         AS LatitudOrigen,
                c.longitud        AS LongitudOrigen,

                d.latitud         AS LatitudDestino,
                d.longitud        AS LongitudDestino

            FROM public.viajes v

            LEFT JOIN public.usuarios u
                ON u.id_usuario = v.id_camionero

            LEFT JOIN public.campos c
                ON c.id_campo = v.id_origen

            LEFT JOIN public.destinos d
                ON d.id_destino = v.id_destino

            WHERE v.id_camionero = @IdCamionero

            ORDER BY v.id_viaje DESC;
            """;

        return await connection.QueryAsync<Viaje>(
            sql,
            new { IdCamionero = idCamionero });
    }


    // -------------------------------------------------------
    // OBTENER POR ID
    // -------------------------------------------------------

    public async Task<Viaje?> ObtenerPorIdAsync(int idViaje)
    {
        using var connection = CrearConexion();

        const string sql = """
            SELECT
                v.id_viaje       AS IdViaje,
                v.id_transpor     AS IdTranspor,
                v.id_camionero    AS IdCamionero,
                u.nombre          AS NombreEmpresa,
                v.tipo            AS Tipo,
                v.fecha_pedido    AS FechaPedido,
                v.id_cereal       AS IdCereal,
                v.id_produc       AS IdProduc,
                v.id_origen       AS IdOrigen,
                v.id_planta       AS IdPlanta,
                v.id_destino      AS IdDestino,
                v.origen          AS Origen,
                v.destino         AS Destino,
                v.ctg             AS Ctg,
                v.kms             AS Kms,
                v.tarifa          AS Tarifa,
                v.estado          AS Estado,
                v.fecha_asigna    AS FechaAsigna,
                v.fecha_termina   AS FechaTermina,
                v.observaciones   AS Observaciones,
                v.batea           AS Batea,
                v.corta           AS Corta,
                v.larga           AS Larga,
                v.id_usuario      AS IdUsuario,

                c.latitud         AS LatitudOrigen,
                c.longitud        AS LongitudOrigen,

                d.latitud         AS LatitudDestino,
                d.longitud        AS LongitudDestino

            FROM public.viajes v

            LEFT JOIN public.usuarios u
                ON u.id_usuario = v.id_camionero

            LEFT JOIN public.campos c
                ON c.id_campo = v.id_origen

            LEFT JOIN public.destinos d
                ON d.id_destino = v.id_destino

            WHERE v.id_viaje = @IdViaje;
            """;

        return await connection.QueryFirstOrDefaultAsync<Viaje>(
            sql,
            new { IdViaje = idViaje });
    }


    // -------------------------------------------------------
    // CREAR
    // -------------------------------------------------------

    public async Task<int> CrearAsync(Viaje viaje)
    {
        using var connection = CrearConexion();

        const string sql = """
            INSERT INTO public.viajes
            (
                id_transpor,
                id_camionero,
                tipo,
                id_cereal,
                id_produc,
                id_origen,
                id_planta,
                id_destino,
                origen,
                destino,
                ctg,
                kms,
                tarifa,
                estado,
                fecha_asigna,
                observaciones,
                batea,
                corta,
                larga,
                id_usuario
            )
            VALUES
            (
                @IdTranspor,
                @IdCamionero,
                @Tipo,
                @IdCereal,
                @IdProduc,
                @IdOrigen,
                @IdPlanta,
                @IdDestino,
                @Origen,
                @Destino,
                @Ctg,
                @Kms,
                @Tarifa,
                @Estado,
                @FechaAsigna,
                @Observaciones,
                @Batea,
                @Corta,
                @Larga,
                @IdUsuario
            )
            RETURNING id_viaje;
            """;

        return await connection.ExecuteScalarAsync<int>(
            sql,
            viaje);
    }


    // -------------------------------------------------------
    // ACTUALIZAR
    // -------------------------------------------------------

    public async Task ActualizarAsync(Viaje viaje)
    {
        using var connection = CrearConexion();

        const string sql = """
            UPDATE public.viajes
            SET
                id_camionero  = @IdCamionero,
                estado        = @Estado,
                fecha_asigna  = @FechaAsigna,
                fecha_termina = @FechaTermina,
                observaciones = @Observaciones,
                batea         = @Batea,
                corta         = @Corta,
                larga         = @Larga
            WHERE id_viaje = @IdViaje;
            """;

        await connection.ExecuteAsync(
            sql,
            viaje);
    }


    // -------------------------------------------------------
    // VIAJES PENDIENTES PARA EMPRESA
    // -------------------------------------------------------

    public async Task<IEnumerable<ViajePendienteResponse>>
        ObtenerPendientesParaEmpresaAsync(int idUsuario)
    {
        const string sql = """
            SELECT
                v.id_viaje       AS IdViaje,
                v.id_transpor     AS IdTranspor,
                v.logistica       AS Logistica,

                v.id_produc       AS IdProduc,
                v.productor       AS Productor,

                v.id_origen       AS IdOrigen,
                v.origen          AS Origen,

                v.id_planta       AS IdPlanta,
                v.planta          AS Planta,

                v.id_destino      AS IdDestino,
                v.destino         AS Destino,

                v.id_cereal       AS IdCereal,
                v.cereal          AS Cereal,

                v.fecha_pedido    AS FechaPedido,
                v.ctg             AS Ctg,
                v.kms             AS Kms,
                v.tarifa          AS Tarifa,

                v.estado          AS Estado,
                v.descrip_via     AS DescripVia,

                v.observaciones   AS Observaciones,

                v.batea           AS Batea,
                v.corta           AS Corta,
                v.larga           AS Larga

            FROM public.vw_viajes_detalle v

            INNER JOIN public.logiscamion lc
                ON lc.id_transpor = v.id_transpor

            WHERE lc.id_usuario = @IdUsuario
              AND lc.estado = 'A'
              AND v.estado = 'P'

            ORDER BY
                v.fecha_pedido,
                v.id_viaje;
            """;

        using var connection = CrearConexion();

        return await connection.QueryAsync<ViajePendienteResponse>(
            sql,
            new
            {
                IdUsuario = idUsuario
            });
    }
// -------------------------------------------------------
// TOMAR VIAJE PENDIENTE
// TRANSACCIONAL
// -------------------------------------------------------

public async Task TomarPendienteAsync(
    int idViaje,
    int idEmpresa)
{
    await using var connection =
        CrearConexion();

    await connection.OpenAsync();

    await using var transaction =
        await connection.BeginTransactionAsync();

    try
    {
        // ---------------------------------------------------
        // 1. BLOQUEAR EMPRESA
        // ---------------------------------------------------

        const string sqlEmpresa = """
            SELECT
                id_usuario AS IdUsuario,
                rol        AS Rol,
                habilitado AS Habilitado,
                estado     AS Estado
            FROM public.usuarios
            WHERE id_usuario = @IdEmpresa
            FOR UPDATE;
            """;

        var empresa =
            await connection
                .QueryFirstOrDefaultAsync<EmpresaTomarViaje>(
                    sqlEmpresa,
                    new
                    {
                        IdEmpresa = idEmpresa
                    },
                    transaction);

        if (empresa == null)
        {
            throw new InvalidOperationException(
                "La Empresa de Transporte no existe.");
        }

        if (empresa.Rol != "E")
        {
            throw new InvalidOperationException(
                "El usuario no es una Empresa de Transporte.");
        }

        if (!empresa.Habilitado)
        {
            throw new InvalidOperationException(
                "La Empresa de Transporte está deshabilitada.");
        }

        if (empresa.Estado != "D")
        {
            throw new InvalidOperationException(
                "La Empresa de Transporte no está disponible.");
        }


        // ---------------------------------------------------
        // 2. VERIFICAR QUE NO TENGA OTRO VIAJE ACTIVO
        // ---------------------------------------------------

        const string sqlViajeActivo = """
            SELECT EXISTS
            (
                SELECT 1
                FROM public.viajes
                WHERE id_camionero = @IdEmpresa
                  AND estado IN
                  (
                      'A',
                      'V',
                      'O',
                      'R',
                      'D'
                  )
            );
            """;

        var tieneViajeActivo =
            await connection
                .ExecuteScalarAsync<bool>(
                    sqlViajeActivo,
                    new
                    {
                        IdEmpresa = idEmpresa
                    },
                    transaction);

        if (tieneViajeActivo)
        {
            throw new InvalidOperationException(
                "La Empresa de Transporte ya tiene un viaje activo.");
        }


        // ---------------------------------------------------
        // 3. BLOQUEAR VIAJE
        // ---------------------------------------------------

        const string sqlViaje = """
            SELECT
                id_viaje      AS IdViaje,
                id_transpor    AS IdTranspor,
                id_camionero   AS IdCamionero,
                estado         AS Estado
            FROM public.viajes
            WHERE id_viaje = @IdViaje
            FOR UPDATE;
            """;

        var viaje =
            await connection
                .QueryFirstOrDefaultAsync<ViajeTomar>(
                    sqlViaje,
                    new
                    {
                        IdViaje = idViaje
                    },
                    transaction);

        if (viaje == null)
        {
            throw new InvalidOperationException(
                "El viaje no existe.");
        }


        // ---------------------------------------------------
        // 4. VERIFICAR QUE SIGA PENDIENTE
        // ---------------------------------------------------

        if (
            viaje.Estado != "P" ||
            viaje.IdCamionero.HasValue
        )
        {
            throw new InvalidOperationException(
                "El viaje acaba de ser tomado por otra empresa.");
        }


        // ---------------------------------------------------
        // 5. VERIFICAR VINCULO CON LOGISTICA
        // ---------------------------------------------------

const string sqlVinculo = """
    SELECT EXISTS
    (
        SELECT 1
        FROM public.logiscamion
        WHERE id_transpor = @IdTranspor
          AND id_usuario = @IdEmpresa
          AND estado = 'A'
    );
    """;

        var vinculado =
            await connection
                .ExecuteScalarAsync<bool>(
                    sqlVinculo,
                    new
                    {
                        IdTranspor =
                            viaje.IdTranspor,

                        IdEmpresa =
                            idEmpresa
                    },
                    transaction);

        if (!vinculado)
        {
            throw new InvalidOperationException(
                "La Empresa de Transporte no está vinculada a esta Logística.");
        }


        // ---------------------------------------------------
        // 6. ASIGNAR VIAJE
        // P -> A
        // ---------------------------------------------------

        const string sqlTomarViaje = """
            UPDATE public.viajes
            SET
                id_camionero = @IdEmpresa,
                estado = 'A',
                fecha_asigna = CURRENT_TIMESTAMP
            WHERE id_viaje = @IdViaje
              AND estado = 'P'
              AND id_camionero IS NULL;
            """;

        var filasViaje =
            await connection
                .ExecuteAsync(
                    sqlTomarViaje,
                    new
                    {
                        IdViaje =
                            idViaje,

                        IdEmpresa =
                            idEmpresa
                    },
                    transaction);

        if (filasViaje != 1)
        {
            throw new InvalidOperationException(
                "El viaje acaba de ser tomado por otra empresa.");
        }


        // ---------------------------------------------------
        // 7. EMPRESA
        // D -> V
        // ---------------------------------------------------

        const string sqlEmpresaViajando = """
            UPDATE public.usuarios
            SET estado = 'V'
            WHERE id_usuario = @IdEmpresa
              AND estado = 'D';
            """;

        var filasEmpresa =
            await connection
                .ExecuteAsync(
                    sqlEmpresaViajando,
                    new
                    {
                        IdEmpresa =
                            idEmpresa
                    },
                    transaction);

        if (filasEmpresa != 1)
        {
            throw new InvalidOperationException(
                "La Empresa de Transporte dejó de estar disponible.");
        }


        // ---------------------------------------------------
        // 8. REGISTRAR EVENTO
        // MISMA TRANSACCION
        // ---------------------------------------------------

        const string sqlEvento = """
            INSERT INTO public.viaje_eventos
            (
                id_viaje,
                estado_anterior,
                estado_nuevo,
                id_usuario,
                latitud,
                longitud,
                observaciones
            )
            VALUES
            (
                @IdViaje,
                'P',
                'A',
                @IdEmpresa,
                NULL,
                NULL,
                @Observaciones
            );
            """;

        await connection
            .ExecuteAsync(
                sqlEvento,
                new
                {
                    IdViaje =
                        idViaje,

                    IdEmpresa =
                        idEmpresa,

                    Observaciones =
                        "Viaje tomado por la Empresa de Transporte."
                },
                transaction);


        // ---------------------------------------------------
        // 9. CONFIRMAR TODO
        // ---------------------------------------------------

        await transaction.CommitAsync();
    }
    catch
    {
        await transaction.RollbackAsync();

        throw;
    }
}


// -------------------------------------------------------
// CLASES INTERNAS PARA TOMAR VIAJE
// -------------------------------------------------------

private sealed class EmpresaTomarViaje
{
    public int IdUsuario { get; set; }

    public string Rol { get; set; } =
        string.Empty;

    public bool Habilitado { get; set; }

    public string? Estado { get; set; }
}


private sealed class ViajeTomar
{
    public int IdViaje { get; set; }

    public int IdTranspor { get; set; }

    public int? IdCamionero { get; set; }

    public string Estado { get; set; } =
        string.Empty;
}
}
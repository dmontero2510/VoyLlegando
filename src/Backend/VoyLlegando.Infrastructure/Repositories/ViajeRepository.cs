using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using VoyLlegando.Application.Interfaces;
using VoyLlegando.Domain.Entities;

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
                v.id_transpor    AS IdTranspor,
                v.id_camionero   AS IdCamionero,
                v.tipo           AS Tipo,
                v.fecha_pedido   AS FechaPedido,
                v.id_cereal      AS IdCereal,
                v.id_produc      AS IdProduc,
                v.id_origen      AS IdOrigen,
                v.id_planta      AS IdPlanta,
                v.id_destino     AS IdDestino,
                v.origen         AS Origen,
                v.destino        AS Destino,
                v.ctg            AS Ctg,
                v.kms            AS Kms,
                v.tarifa         AS Tarifa,
                v.estado         AS Estado,
                v.fecha_asigna   AS FechaAsigna,
                v.fecha_termina  AS FechaTermina,
                v.observaciones  AS Observaciones,
                v.batea          AS Batea,
                v.corta          AS Corta,
                v.larga          AS Larga,
                v.id_usuario     AS IdUsuario,

                c.latitud        AS LatitudOrigen,
                c.longitud       AS LongitudOrigen,

                d.latitud        AS LatitudDestino,
                d.longitud       AS LongitudDestino

            FROM public.viajes v

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
                v.id_transpor    AS IdTranspor,
                v.id_camionero   AS IdCamionero,
                v.tipo            AS Tipo,
                v.fecha_pedido   AS FechaPedido,
                v.id_cereal      AS IdCereal,
                v.id_produc      AS IdProduc,
                v.id_origen      AS IdOrigen,
                v.id_planta      AS IdPlanta,
                v.id_destino     AS IdDestino,
                v.origen         AS Origen,
                v.destino        AS Destino,
                v.ctg            AS Ctg,
                v.kms            AS Kms,
                v.tarifa         AS Tarifa,
                v.estado         AS Estado,
                v.fecha_asigna   AS FechaAsigna,
                v.fecha_termina  AS FechaTermina,
                v.observaciones  AS Observaciones,
                v.batea          AS Batea,
                v.corta          AS Corta,
                v.larga          AS Larga,
                v.id_usuario     AS IdUsuario,

                c.latitud        AS LatitudOrigen,
                c.longitud       AS LongitudOrigen,

                d.latitud        AS LatitudDestino,
                d.longitud       AS LongitudDestino

            FROM public.viajes v

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
                v.id_transpor    AS IdTranspor,
                v.id_camionero   AS IdCamionero,
                v.tipo            AS Tipo,
                v.fecha_pedido   AS FechaPedido,
                v.id_cereal      AS IdCereal,
                v.id_produc      AS IdProduc,
                v.id_origen      AS IdOrigen,
                v.id_planta      AS IdPlanta,
                v.id_destino     AS IdDestino,
                v.origen         AS Origen,
                v.destino        AS Destino,
                v.ctg            AS Ctg,
                v.kms            AS Kms,
                v.tarifa         AS Tarifa,
                v.estado         AS Estado,
                v.fecha_asigna   AS FechaAsigna,
                v.fecha_termina  AS FechaTermina,
                v.observaciones  AS Observaciones,
                v.batea          AS Batea,
                v.corta          AS Corta,
                v.larga          AS Larga,
                v.id_usuario     AS IdUsuario,

                c.latitud        AS LatitudOrigen,
                c.longitud       AS LongitudOrigen,

                d.latitud        AS LatitudDestino,
                d.longitud       AS LongitudDestino

            FROM public.viajes v

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
                v.id_transpor    AS IdTranspor,
                v.id_camionero   AS IdCamionero,
                v.tipo           AS Tipo,
                v.fecha_pedido   AS FechaPedido,
                v.id_cereal      AS IdCereal,
                v.id_produc      AS IdProduc,
                v.id_origen      AS IdOrigen,
                v.id_planta      AS IdPlanta,
                v.id_destino     AS IdDestino,
                v.origen         AS Origen,
                v.destino        AS Destino,
                v.ctg            AS Ctg,
                v.kms             AS Kms,
                v.tarifa         AS Tarifa,
                v.estado         AS Estado,
                v.fecha_asigna   AS FechaAsigna,
                v.fecha_termina  AS FechaTermina,
                v.observaciones  AS Observaciones,
                v.batea          AS Batea,
                v.corta          AS Corta,
                v.larga          AS Larga,
                v.id_usuario     AS IdUsuario,

                c.latitud        AS LatitudOrigen,
                c.longitud       AS LongitudOrigen,

                d.latitud        AS LatitudDestino,
                d.longitud       AS LongitudDestino

            FROM public.viajes v

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

        await connection.ExecuteAsync(sql, viaje);
    }
}

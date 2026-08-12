using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using VoyLlegando.Application.Interfaces;
using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Infrastructure.Repositories;

public class ViajeEventoRepository
    : IViajeEventoRepository
{
    private readonly string _connectionString;

    public ViajeEventoRepository(
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

    public async Task<int> CrearAsync(
        ViajeEvento evento)
    {
        using var connection =
            CrearConexion();

        const string sql = """
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
                @EstadoAnterior,
                @EstadoNuevo,
                @IdUsuario,
                @Latitud,
                @Longitud,
                @Observaciones
            )
            RETURNING id_evento;
            """;

        return await connection
            .ExecuteScalarAsync<int>(
                sql,
                evento);
    }

    public async Task<IEnumerable<ViajeEvento>>
        ObtenerPorViajeAsync(
            int idViaje)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            SELECT
                id_evento      AS IdEvento,
                id_viaje       AS IdViaje,
                estado_anterior AS EstadoAnterior,
                estado_nuevo   AS EstadoNuevo,
                fecha          AS Fecha,
                id_usuario     AS IdUsuario,
                latitud        AS Latitud,
                longitud       AS Longitud,
                observaciones  AS Observaciones
            FROM public.viaje_eventos
            WHERE id_viaje = @IdViaje
            ORDER BY fecha;
            """;

        return await connection
            .QueryAsync<ViajeEvento>(
                sql,
                new
                {
                    IdViaje = idViaje
                });
    }
}
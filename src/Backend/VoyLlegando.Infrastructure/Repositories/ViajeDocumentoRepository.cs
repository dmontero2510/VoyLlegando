using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using VoyLlegando.Application.Interfaces;
using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Infrastructure.Repositories;

public class ViajeDocumentoRepository
    : IViajeDocumentoRepository
{
    private readonly string _connectionString;

    public ViajeDocumentoRepository(
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
        ViajeDocumento documento)
    {
        using var connection =
            CrearConexion();

        /*
         * Para CP:
         * un viaje puede tener una sola CP.
         *
         * Si ya existe:
         * - conserva el mismo id_documento
         * - reemplaza nombre y contenido
         * - actualiza la fecha
         *
         * Para cualquier otro tipo:
         * se mantiene el comportamiento normal
         * de insertar un nuevo documento.
         */

        if (string.Equals(
                documento.Tipo,
                "CP",
                StringComparison.OrdinalIgnoreCase))
        {
            const string sqlCP = """
                INSERT INTO public.viaje_documentos
                (
                    id_viaje,
                    tipo,
                    nombre_archivo,
                    contenido
                )
                VALUES
                (
                    @IdViaje,
                    'CP',
                    @NombreArchivo,
                    @Contenido
                )
                ON CONFLICT (id_viaje)
                    WHERE tipo = 'CP'
                DO UPDATE
                SET
                    nombre_archivo =
                        EXCLUDED.nombre_archivo,

                    contenido =
                        EXCLUDED.contenido,

                    fecha =
                        CURRENT_TIMESTAMP

                RETURNING id_documento;
                """;

            return await connection
                .ExecuteScalarAsync<int>(
                    sqlCP,
                    new
                    {
                        documento.IdViaje,
                        documento.NombreArchivo,
                        documento.Contenido
                    });
        }


        const string sql = """
            INSERT INTO public.viaje_documentos
            (
                id_viaje,
                tipo,
                nombre_archivo,
                contenido
            )
            VALUES
            (
                @IdViaje,
                @Tipo,
                @NombreArchivo,
                @Contenido
            )
            RETURNING id_documento;
            """;

        return await connection
            .ExecuteScalarAsync<int>(
                sql,
                documento);
    }


    public async Task<IEnumerable<ViajeDocumento>>
        ObtenerPorViajeAsync(
            int idViaje)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            SELECT
                id_documento AS IdDocumento,
                id_viaje AS IdViaje,
                tipo AS Tipo,
                nombre_archivo AS NombreArchivo,
                contenido AS Contenido,
                fecha AS Fecha
            FROM public.viaje_documentos
            WHERE id_viaje = @IdViaje
            ORDER BY fecha DESC,
                     id_documento DESC;
            """;

        return await connection
            .QueryAsync<ViajeDocumento>(
                sql,
                new
                {
                    IdViaje = idViaje
                });
    }


    public async Task<ViajeDocumento?>
        ObtenerPorIdAsync(
            int idDocumento)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            SELECT
                id_documento AS IdDocumento,
                id_viaje AS IdViaje,
                tipo AS Tipo,
                nombre_archivo AS NombreArchivo,
                contenido AS Contenido,
                fecha AS Fecha
            FROM public.viaje_documentos
            WHERE id_documento = @IdDocumento;
            """;

        return await connection
            .QueryFirstOrDefaultAsync<ViajeDocumento>(
                sql,
                new
                {
                    IdDocumento = idDocumento
                });
    }


    public async Task EliminarAsync(
        int idDocumento)
    {
        using var connection =
            CrearConexion();

        const string sql = """
            DELETE FROM public.viaje_documentos
            WHERE id_documento = @IdDocumento;
            """;

        await connection.ExecuteAsync(
            sql,
            new
            {
                IdDocumento = idDocumento
            });
    }
}
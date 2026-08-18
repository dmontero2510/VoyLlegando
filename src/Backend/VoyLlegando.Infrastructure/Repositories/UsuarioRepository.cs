using Dapper;
using VoyLlegando.Domain.Entities;
using VoyLlegando.Infrastructure.Database;
using VoyLlegando.Application.Interfaces;

namespace VoyLlegando.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly DbConnectionFactory _factory;

    public UsuarioRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<Usuario?> ObtenerPorCelularAsync(string celular)
    {
        using var connection = _factory.CreateConnection();

        const string sql = @"
SELECT
    id_usuario AS IdUsuario,
    nombre AS Nombre,
    domicilio AS Domicilio,
    iva AS Iva,
    cuit AS Cuit,
    celular AS Celular,
    clave AS Clave,
    debe_cambiar_clave AS DebeCambiarClave,
    email AS Email,
    rol AS Rol,
    habilitado AS Habilitado,
    id_transpor AS IdTranspor,
    id_planta AS IdPlanta,
    id_produc AS IdProduc,
    pat_chasis AS PatChasis,
    pat_acopla AS PatAcopla,
    batea AS Batea,
    corta AS Corta,
    larga AS Larga,
    escala AS Escala,
    estado AS Estado,
    latitud_actual   AS LatitudActual,
    longitud_actual  AS LongitudActual,
    fecha_ubicacion  AS FechaUbicacion
FROM usuarios
WHERE celular = @celular
LIMIT 1;";

        return await connection.QueryFirstOrDefaultAsync<Usuario>(
            sql,
            new { celular });
    }

    public async Task<bool> ExisteCelularAsync(string celular, int? excluirId = null)
    {
        using var connection = _factory.CreateConnection();

        const string sql = @"
SELECT COUNT(1)
FROM usuarios
WHERE celular = @celular
AND (@excluirId IS NULL OR id_usuario <> @excluirId);";

        var cantidad = await connection.ExecuteScalarAsync<int>(
            sql,
            new { celular, excluirId });

        return cantidad > 0;
    }

public async Task<IEnumerable<Usuario>> ObtenerTodosAsync()
{
    using var connection =
        _factory.CreateConnection();

    const string sql = """
        SELECT
            id_usuario  AS IdUsuario,
            celular     AS Celular,
            clave       AS Clave,
            debe_cambiar_clave AS DebeCambiarClave,
            nombre      AS Nombre,
            domicilio   AS Domicilio,
            iva         AS Iva,
            cuit        AS Cuit,
            email       AS Email,
            rol         AS Rol,
            habilitado  AS Habilitado,
            id_transpor AS IdTranspor,
            id_planta   AS IdPlanta,
            id_produc   AS IdProduc,
            pat_chasis  AS PatChasis,
            pat_acopla  AS PatAcopla,
            batea       AS Batea,
            corta       AS Corta,
            larga       AS Larga,
            escala      AS Escala,
            estado      AS Estado
        FROM public.usuarios
        ORDER BY id_usuario;
        """;

    return await connection
        .QueryAsync<Usuario>(
            sql
        );
}
public async Task<Usuario?> ObtenerPorIdAsync(int id)
{
    using var connection = _factory.CreateConnection();

    const string sql = @"
SELECT
    id_usuario AS IdUsuario,
    nombre AS Nombre,
    domicilio AS Domicilio,
    iva AS Iva,
    cuit AS Cuit,
    celular AS Celular,
    clave AS Clave,
    debe_cambiar_clave AS DebeCambiarClave,
    email AS Email,
    rol AS Rol,
    habilitado AS Habilitado,
    id_transpor AS IdTranspor,
    id_planta AS IdPlanta,
    id_produc AS IdProduc,
    pat_chasis AS PatChasis,
    pat_acopla AS PatAcopla,
    batea AS Batea,
    corta AS Corta,
    larga AS Larga,
    escala AS Escala,
    estado AS Estado,
    latitud_actual   AS LatitudActual,
    longitud_actual  AS LongitudActual,
    fecha_ubicacion  AS FechaUbicacion
FROM usuarios
WHERE id_usuario = @id;
";

    return await connection.QueryFirstOrDefaultAsync<Usuario>(
        sql,
        new { id }
	);
}
public async Task<int> CrearAsync(Usuario usuario)
{
    using var connection = _factory.CreateConnection();

    const string sql = @"
INSERT INTO usuarios
(
    nombre,
    domicilio,
    iva,
    cuit,
    celular,
    clave,
    email,
    rol,
    habilitado,
    id_transpor,
    id_planta,
    id_produc,
    pat_chasis,
    pat_acopla,
    batea,
    corta,
    larga,
    escala,
    estado
)
VALUES
(
    @Nombre,
    @Domicilio,
    @Iva,
    @Cuit,
    @Celular,
    @Clave,
    @Email,
    @Rol,
    @Habilitado,
    @IdTranspor,
    @IdPlanta,
    @IdProduc,
    @PatChasis,
    @PatAcopla,
    @Batea,
    @Corta,
    @Larga,
    @Escala,
    @Estado
)
RETURNING id_usuario;
";

    return await connection.ExecuteScalarAsync<int>(
        sql,
        usuario);
}
public async Task ActualizarAsync(Usuario usuario)
{
    using var connection = _factory.CreateConnection();

    const string sql = @"
UPDATE usuarios
SET
    nombre = @Nombre,
    domicilio = @Domicilio,
    iva = @Iva,
    cuit = @Cuit,
    celular = @Celular,
    email = @Email,
    rol = @Rol,
    latitud_actual  = @LatitudActual,
    longitud_actual = @LongitudActual,
    fecha_ubicacion = @FechaUbicacion,
    id_transpor = @IdTranspor,
    id_planta = @IdPlanta,
    id_produc = @IdProduc,
    pat_chasis = @PatChasis,
    pat_acopla = @PatAcopla,
    batea = @Batea,
    corta = @Corta,
    larga = @Larga,
    escala = @Escala,
    estado = @Estado
WHERE id_usuario = @IdUsuario;
";

    await connection.ExecuteAsync(sql, usuario);
}

    public async Task BajaAsync(int id)
    {
        using var connection = _factory.CreateConnection();

        await connection.ExecuteAsync(
            "UPDATE usuarios SET habilitado=false WHERE id_usuario=@id",
            new { id });
    }

    public async Task ActualizarClaveAsync(
        int idUsuario,
        string hash,
        bool debeCambiarClave = false)
    {
        using var connection = _factory.CreateConnection();

        await connection.ExecuteAsync(
            """
            UPDATE usuarios
            SET
                clave = @hash,
                debe_cambiar_clave = @debeCambiarClave
            WHERE id_usuario = @idUsuario;
            """,
            new
            {
                idUsuario,
                hash,
                debeCambiarClave
            });
    }
}

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
    estado AS Estado
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
        using var connection = _factory.CreateConnection();

        return await connection.QueryAsync<Usuario>(
            "SELECT * FROM usuarios ORDER BY id_usuario");
    }

    public async Task<Usuario?> ObtenerPorIdAsync(int id)
    {
        using var connection = _factory.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<Usuario>(
            "SELECT * FROM usuarios WHERE id_usuario = @id",
            new { id });
    }

    public async Task<int> CrearAsync(Usuario usuario)
    {
        using var connection = _factory.CreateConnection();

        const string sql = @"
INSERT INTO usuarios
(
    nombre,
    celular,
    clave,
    email,
    cuit,
    rol
)
VALUES
(
    @Nombre,
    @Celular,
    @Clave,
    @Email,
    @Cuit,
    @Rol
)
RETURNING id_usuario;";

        return await connection.ExecuteScalarAsync<int>(sql, usuario);
    }

    public async Task ActualizarAsync(Usuario usuario)
    {
        using var connection = _factory.CreateConnection();

        await connection.ExecuteAsync(
            "UPDATE usuarios SET nombre=@Nombre WHERE id_usuario=@IdUsuario",
            usuario);
    }

    public async Task BajaAsync(int id)
    {
        using var connection = _factory.CreateConnection();

        await connection.ExecuteAsync(
            "UPDATE usuarios SET habilitado=false WHERE id_usuario=@id",
            new { id });
    }

    public async Task ActualizarClaveAsync(int idUsuario, string hash)
    {
        using var connection = _factory.CreateConnection();

        await connection.ExecuteAsync(
            "UPDATE usuarios SET clave=@hash WHERE id_usuario=@idUsuario",
            new { idUsuario, hash });
    }
}
using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Application.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorCelularAsync(string celular);

    Task<IEnumerable<Usuario>> ObtenerTodosAsync();

    Task<Usuario?> ObtenerPorIdAsync(int id);

    Task<int> CrearAsync(Usuario usuario);

    Task ActualizarAsync(Usuario usuario);

    Task<bool> ExisteCelularAsync(string celular, int? excluirId = null);

    Task BajaAsync(int id);

    Task ActualizarClaveAsync(int idUsuario, string hash);
}
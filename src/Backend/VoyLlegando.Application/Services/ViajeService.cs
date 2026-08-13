using VoyLlegando.Application.DTOs;
using VoyLlegando.Application.Interfaces;
using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Application.Services;

public class ViajeService
{
    private readonly IViajeRepository _viajeRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IViajeEventoRepository _viajeEventoRepository;

    public ViajeService(
        IViajeRepository viajeRepository,
        IUsuarioRepository usuarioRepository,
        IViajeEventoRepository viajeEventoRepository)
    {
        _viajeRepository = viajeRepository;
        _usuarioRepository = usuarioRepository;
        _viajeEventoRepository = viajeEventoRepository;
    }

    public async Task TomarAsync(
        int idViaje,
        int idEmpresa)
    {
        await _viajeRepository
            .TomarPendienteAsync(
                idViaje,
                idEmpresa);
    }

    // -------------------------------------------------------
    // CREAR VIAJE
    // -------------------------------------------------------

    public async Task<int> CrearAsync(
        ViajeRequest request,
        int idUsuario,
        int idTranspor)
    {
        Viaje viaje = new()
        {
            IdTranspor = idTranspor,
            IdCamionero = request.IdCamionero,

            Tipo = request.Tipo,

            IdCereal = request.IdCereal,

            IdProduc = request.IdProduc,
            IdOrigen = request.IdOrigen,

            IdPlanta = request.IdPlanta,
            IdDestino = request.IdDestino,

            Origen = request.Origen,
            Destino = request.Destino,

            Ctg = request.Ctg,

            Kms = request.Kms,
            Tarifa = request.Tarifa,

            Observaciones = request.Observaciones,

            Batea = request.Batea,
            Corta = request.Corta,
            Larga = request.Larga,

            IdUsuario = idUsuario,

            Estado = "P",
            FechaAsigna = null,
            FechaTermina = null
        };

        // --------------------------------------------------
        // SIN EMPRESA ASIGNADA
        // --------------------------------------------------

        if (request.IdCamionero == null)
        {
            var idViaje =
                await _viajeRepository
                    .CrearAsync(viaje);

            await RegistrarEventoAsync(
                idViaje,
                null,
                "P",
                idUsuario,
                null,
                null,
                "Viaje creado.");

            return idViaje;
        }

        // --------------------------------------------------
        // EMPRESA ASIGNADA AL CREAR
        // --------------------------------------------------

        var empresa = await _usuarioRepository
            .ObtenerPorIdAsync(
                request.IdCamionero.Value);

        if (empresa == null)
            throw new InvalidOperationException(
                "La Empresa de Transporte no existe.");

        if (empresa.Rol != "E")
            throw new InvalidOperationException(
                "El usuario seleccionado no es una Empresa de Transporte.");

        if (!empresa.Habilitado)
            throw new InvalidOperationException(
                "La Empresa de Transporte está deshabilitada.");

        if (empresa.Estado != "D")
            throw new InvalidOperationException(
                "La Empresa de Transporte no está disponible.");

        // --------------------------------------------------
        // ASIGNACIÓN
        // --------------------------------------------------

        viaje.Estado = "A";
        viaje.FechaAsigna = DateTime.UtcNow;

        empresa.Estado = "V";

        await _usuarioRepository
            .ActualizarAsync(empresa);

        var nuevoId =
            await _viajeRepository
                .CrearAsync(viaje);

        await RegistrarEventoAsync(
            nuevoId,
            null,
            "A",
            idUsuario,
            null,
            null,
            "Viaje creado y asignado a Empresa de Transporte.");

        return nuevoId;
    }

    // -------------------------------------------------------
    // ASIGNAR VIAJE A EMPRESA DE TRANSPORTE
    // P -> A
    // -------------------------------------------------------

    public async Task<bool> AsignarAsync(
        int idViaje,
        int idEmpresa,
        int idTranspor,
        int idUsuario)
    {
        var viaje = await _viajeRepository
            .ObtenerPorIdAsync(idViaje);

        if (viaje == null)
            return false;

        ValidarNoInformado(viaje);

        if (viaje.Estado != "P")
            throw new InvalidOperationException(
                "El viaje no está pendiente.");

        if (viaje.IdTranspor != idTranspor)
            throw new InvalidOperationException(
                "El viaje no pertenece a esta Logística.");

        var empresa = await _usuarioRepository
            .ObtenerPorIdAsync(idEmpresa);

        if (empresa == null)
            throw new InvalidOperationException(
                "La Empresa de Transporte no existe.");

        if (empresa.Rol != "E")
            throw new InvalidOperationException(
                "El usuario seleccionado no es una Empresa de Transporte.");

        if (!empresa.Habilitado)
            throw new InvalidOperationException(
                "La Empresa de Transporte está deshabilitada.");

        if (empresa.Estado != "D")
            throw new InvalidOperationException(
                "La Empresa de Transporte no está disponible.");

        // --------------------------------------------------
        // ASIGNACIÓN
        // --------------------------------------------------

        var estadoAnterior =
            viaje.Estado;

        viaje.IdCamionero = idEmpresa;
        viaje.Estado = "A";
        viaje.FechaAsigna = DateTime.UtcNow;

        empresa.Estado = "V";

        await _usuarioRepository
            .ActualizarAsync(empresa);

        await _viajeRepository
            .ActualizarAsync(viaje);

        await RegistrarEventoAsync(
            viaje.IdViaje,
            estadoAnterior,
            "A",
            idUsuario,
            null,
            null,
            "Viaje asignado a Empresa de Transporte.");

        return true;
    }

    // -------------------------------------------------------
    // RECHAZAR VIAJE
    // A -> P
    // -------------------------------------------------------

    public async Task<bool> RechazarAsync(
        int idViaje,
        int idEmpresa)
    {
        var viaje = await _viajeRepository
            .ObtenerPorIdAsync(idViaje);

        if (viaje == null)
            return false;

        ValidarNoInformado(viaje);

        if (viaje.Estado != "A")
            throw new InvalidOperationException(
                "El viaje no está asignado.");

        if (viaje.IdCamionero != idEmpresa)
            throw new InvalidOperationException(
                "El viaje no pertenece a esta Empresa de Transporte.");

        var empresa = await _usuarioRepository
            .ObtenerPorIdAsync(idEmpresa);

        if (empresa == null)
            throw new InvalidOperationException(
                "La Empresa de Transporte no existe.");

        if (empresa.Estado != "V")
            throw new InvalidOperationException(
                "La Empresa de Transporte no está viajando.");

        // --------------------------------------------------
        // RECHAZO
        // --------------------------------------------------

        var estadoAnterior =
            viaje.Estado;

        viaje.Estado = "P";
        viaje.IdCamionero = null;
        viaje.FechaAsigna = null;

        empresa.Estado = "D";

        await _usuarioRepository
            .ActualizarAsync(empresa);

        await _viajeRepository
            .ActualizarAsync(viaje);

        await RegistrarEventoAsync(
            viaje.IdViaje,
            estadoAnterior,
            "P",
            idEmpresa,
            empresa.LatitudActual,
            empresa.LongitudActual,
            "Viaje rechazado por la Empresa de Transporte.");

        return true;
    }

    // -------------------------------------------------------
    // INICIAR VIAJE
    // A -> V
    // -------------------------------------------------------

    public async Task<bool> IniciarAsync(
        int idViaje,
        int idEmpresa)
    {
        var viaje = await _viajeRepository
            .ObtenerPorIdAsync(idViaje);

        if (viaje == null)
            return false;

        ValidarNoInformado(viaje);

        if (viaje.Estado != "A")
            throw new InvalidOperationException(
                "El viaje no está asignado.");

        if (viaje.IdCamionero != idEmpresa)
            throw new InvalidOperationException(
                "El viaje no pertenece a esta Empresa de Transporte.");

        var empresa = await _usuarioRepository
            .ObtenerPorIdAsync(idEmpresa);

        if (empresa == null)
            throw new InvalidOperationException(
                "La Empresa de Transporte no existe.");

        if (!empresa.Habilitado)
            throw new InvalidOperationException(
                "La Empresa de Transporte está deshabilitada.");

        // --------------------------------------------------
        // CAMBIO DE ESTADO
        // --------------------------------------------------

        var estadoAnterior =
            viaje.Estado;

        viaje.Estado = "V";

        await _viajeRepository
            .ActualizarAsync(viaje);

        await RegistrarEventoAsync(
            viaje.IdViaje,
            estadoAnterior,
            "V",
            idEmpresa,
            empresa.LatitudActual,
            empresa.LongitudActual,
            "Viaje iniciado por la Empresa de Transporte.");

        return true;
    }

    // -------------------------------------------------------
    // LLEGAR AL ORIGEN
    // V -> O
    // -------------------------------------------------------

    public async Task<bool> LlegarOrigenAsync(
        int idViaje,
        int idEmpresa)
    {
        var viaje = await _viajeRepository
            .ObtenerPorIdAsync(idViaje);

        if (viaje == null)
            return false;

        ValidarNoInformado(viaje);

        if (viaje.Estado != "V")
            throw new InvalidOperationException(
                "El viaje no está iniciado.");

        if (viaje.IdCamionero != idEmpresa)
            throw new InvalidOperationException(
                "El viaje no pertenece a esta Empresa de Transporte.");

        var empresa = await _usuarioRepository
            .ObtenerPorIdAsync(idEmpresa);

        if (empresa == null)
            throw new InvalidOperationException(
                "La Empresa de Transporte no existe.");

        if (!empresa.Habilitado)
            throw new InvalidOperationException(
                "La Empresa de Transporte está deshabilitada.");

        // --------------------------------------------------
        // CAMBIO DE ESTADO
        // --------------------------------------------------

        var estadoAnterior =
            viaje.Estado;

        viaje.Estado = "O";

        await _viajeRepository
            .ActualizarAsync(viaje);

        await RegistrarEventoAsync(
            viaje.IdViaje,
            estadoAnterior,
            "O",
            idEmpresa,
            empresa.LatitudActual,
            empresa.LongitudActual,
            "Empresa de Transporte llegó al origen.");

        return true;
    }

    // -------------------------------------------------------
    // SALIR DEL ORIGEN
    // O -> R
    // -------------------------------------------------------

    public async Task<bool> SalirOrigenAsync(
        int idViaje,
        int idEmpresa)
    {
        var viaje = await _viajeRepository
            .ObtenerPorIdAsync(idViaje);

        if (viaje == null)
            return false;

        ValidarNoInformado(viaje);

        if (viaje.Estado != "O")
            throw new InvalidOperationException(
                "El viaje no está en el origen.");

        if (viaje.IdCamionero != idEmpresa)
            throw new InvalidOperationException(
                "El viaje no pertenece a esta Empresa de Transporte.");

        var empresa = await _usuarioRepository
            .ObtenerPorIdAsync(idEmpresa);

        if (empresa == null)
            throw new InvalidOperationException(
                "La Empresa de Transporte no existe.");

        if (!empresa.Habilitado)
            throw new InvalidOperationException(
                "La Empresa de Transporte está deshabilitada.");

        // --------------------------------------------------
        // CAMBIO DE ESTADO
        // --------------------------------------------------

        var estadoAnterior =
            viaje.Estado;

        viaje.Estado = "R";

        await _viajeRepository
            .ActualizarAsync(viaje);

        await RegistrarEventoAsync(
            viaje.IdViaje,
            estadoAnterior,
            "R",
            idEmpresa,
            empresa.LatitudActual,
            empresa.LongitudActual,
            "Empresa de Transporte salió del origen.");

        return true;
    }

    // -------------------------------------------------------
    // LLEGAR AL DESTINO
    // R -> D
    // -------------------------------------------------------

    public async Task<bool> LlegarDestinoAsync(
        int idViaje,
        int idEmpresa)
    {
        var viaje = await _viajeRepository
            .ObtenerPorIdAsync(idViaje);

        if (viaje == null)
            return false;

        ValidarNoInformado(viaje);

        if (viaje.Estado != "R")
            throw new InvalidOperationException(
                "El viaje no está en ruta.");

        if (viaje.IdCamionero != idEmpresa)
            throw new InvalidOperationException(
                "El viaje no pertenece a esta Empresa de Transporte.");

        var empresa = await _usuarioRepository
            .ObtenerPorIdAsync(idEmpresa);

        if (empresa == null)
            throw new InvalidOperationException(
                "La Empresa de Transporte no existe.");

        if (!empresa.Habilitado)
            throw new InvalidOperationException(
                "La Empresa de Transporte está deshabilitada.");

        // --------------------------------------------------
        // CAMBIO DE ESTADO
        // --------------------------------------------------

        var estadoAnterior =
            viaje.Estado;

        viaje.Estado = "D";

        await _viajeRepository
            .ActualizarAsync(viaje);

        await RegistrarEventoAsync(
            viaje.IdViaje,
            estadoAnterior,
            "D",
            idEmpresa,
            empresa.LatitudActual,
            empresa.LongitudActual,
            "Empresa de Transporte llegó al destino.");

        return true;
    }

    // -------------------------------------------------------
    // TERMINAR VIAJE
    // D -> T
    // -------------------------------------------------------

    public async Task<bool> TerminarAsync(
        int idViaje,
        int idEmpresa)
    {
        var viaje = await _viajeRepository
            .ObtenerPorIdAsync(idViaje);

        if (viaje == null)
            return false;

        ValidarNoInformado(viaje);

        if (viaje.Estado != "D")
            throw new InvalidOperationException(
                "El viaje no está en destino.");

        if (viaje.IdCamionero != idEmpresa)
            throw new InvalidOperationException(
                "El viaje no pertenece a esta Empresa de Transporte.");

        var empresa = await _usuarioRepository
            .ObtenerPorIdAsync(idEmpresa);

        if (empresa == null)
            throw new InvalidOperationException(
                "La Empresa de Transporte no existe.");

        if (empresa.Estado != "V")
            throw new InvalidOperationException(
                "La Empresa de Transporte no está viajando.");

        // --------------------------------------------------
        // TERMINACIÓN
        // --------------------------------------------------

        var estadoAnterior =
            viaje.Estado;

        viaje.Estado = "T";
        viaje.FechaTermina = DateTime.UtcNow;

        empresa.Estado = "F";

        await _usuarioRepository
            .ActualizarAsync(empresa);

        await _viajeRepository
            .ActualizarAsync(viaje);

        await RegistrarEventoAsync(
            viaje.IdViaje,
            estadoAnterior,
            "T",
            idEmpresa,
            empresa.LatitudActual,
            empresa.LongitudActual,
            "Viaje terminado.");

        return true;
    }

    // -------------------------------------------------------
    // REGISTRAR EVENTO
    // -------------------------------------------------------

    private async Task RegistrarEventoAsync(
        int idViaje,
        string? estadoAnterior,
        string estadoNuevo,
        int idUsuario,
        decimal? latitud,
        decimal? longitud,
        string? observaciones)
    {
        ViajeEvento evento = new()
        {
            IdViaje = idViaje,

            EstadoAnterior =
                estadoAnterior,

            EstadoNuevo =
                estadoNuevo,

            IdUsuario =
                idUsuario,

            Latitud =
                latitud,

            Longitud =
                longitud,

            Observaciones =
                observaciones
        };

        await _viajeEventoRepository
            .CrearAsync(evento);
    }

    // -------------------------------------------------------
    // VIAJE INFORMADO = BLOQUEADO
    // -------------------------------------------------------

    private static void ValidarNoInformado(
        Viaje viaje)
    {
        if (viaje.Estado == "I")
        {
            throw new InvalidOperationException(
                "El viaje ya fue informado al sistema de Gestión y no puede modificarse.");
        }
    }
}


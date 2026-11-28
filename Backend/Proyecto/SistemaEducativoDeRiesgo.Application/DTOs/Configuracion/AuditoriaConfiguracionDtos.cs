namespace SistemaEducativoDeRiesgo.Application.DTOs.Configuracion
{
    public record CrearAuditoriaConfiguracionRequest(
        int ParametrosId,
        int UsuarioId,
        string? ValorAnterior,
        string? ValorNuevo
    );

    public record ActualizarAuditoriaConfiguracionRequest(
        int ParametrosId,
        int UsuarioId,
        string? ValorAnterior,
        string? ValorNuevo
    );

    public record AuditoriaConfiguracionResponse(
        int ParametrosId,
        int UsuarioId,
        string? ValorAnterior,
        string? ValorNuevo
    );
}

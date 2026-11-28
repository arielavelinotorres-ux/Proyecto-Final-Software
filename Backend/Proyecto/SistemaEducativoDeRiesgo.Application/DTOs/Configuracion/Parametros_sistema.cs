namespace SistemaEducativoDeRiesgo.Application.DTOs.Configuracion
{
    public record CrearParametrosSistemaRequest(
        string Clave,
        string? Valor,
        string? Descripcion,
        string? TipoDato
    );

    public record ActualizarParametrosSistemaRequest(
        string Clave,
        string? Valor,
        string? Descripcion,
        string? TipoDato
    );

    public record ParametrosSistemaResponse(
        string Clave,
        string? Valor,
        string? Descripcion,
        string? TipoDato
    );
}

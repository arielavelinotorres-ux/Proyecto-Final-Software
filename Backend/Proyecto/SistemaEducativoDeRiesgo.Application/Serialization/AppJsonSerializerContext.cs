using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using SistemaEducativoDeRiesgo.Application.DTOs;
using SistemaEducativoDeRiesgo.Application.DTOs.Usuarios;
using SistemaEducativoDeRiesgo.Application.DTOs.Comunicacion;

namespace SistemaEducativoDeRiesgo.Application.Serialization
{
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    // Respuestas comunes
    [JsonSerializable(typeof(IdResponse))]
    [JsonSerializable(typeof(MensajeResponse))]

    // Módulo Usuarios
    [JsonSerializable(typeof(LoginRequest))]
    [JsonSerializable(typeof(LoginResponse))]
    [JsonSerializable(typeof(CrearUsuarioRequest))]
    [JsonSerializable(typeof(ActualizarUsuarioPerfilRequest))]
    [JsonSerializable(typeof(ActualizarPasswordRequest))]
    [JsonSerializable(typeof(UsuarioResponse))]
    [JsonSerializable(typeof(List<UsuarioResponse>))]

    [JsonSerializable(typeof(CrearDocenteRequest))]
    [JsonSerializable(typeof(ActualizarDocenteRequest))]
    [JsonSerializable(typeof(DocenteResponse))]
    [JsonSerializable(typeof(List<DocenteResponse>))]

    [JsonSerializable(typeof(CrearEstudianteRequest))]
    [JsonSerializable(typeof(ActualizarEstudianteRequest))]
    [JsonSerializable(typeof(EstudianteResponse))]
    [JsonSerializable(typeof(List<EstudianteResponse>))]

    [JsonSerializable(typeof(GenerarTokenRecuperacionRequest))]
    [JsonSerializable(typeof(TokenGeneradoResponse))]
    [JsonSerializable(typeof(ValidarTokenRequest))]
    [JsonSerializable(typeof(ValidarTokenResponse))]

    // Módulo Comunicación

    [JsonSerializable(typeof(CrearForoRequest))]
    [JsonSerializable(typeof(ActualizarForoRequest))]
    [JsonSerializable(typeof(ForoResponse))]
    [JsonSerializable(typeof(List<ForoResponse>))]

    [JsonSerializable(typeof(CrearPublicacionForoRequest))]
    [JsonSerializable(typeof(ActualizarPublicacionForoRequest))]
    [JsonSerializable(typeof(PublicacionForoResponse))]
    [JsonSerializable(typeof(List<PublicacionForoResponse>))]

    [JsonSerializable(typeof(CrearMensajePrivadoRequest))]
    [JsonSerializable(typeof(MensajeRecibidoResponse))]
    [JsonSerializable(typeof(List<MensajeRecibidoResponse>))]
    [JsonSerializable(typeof(MensajeEnviadoResponse))]
    [JsonSerializable(typeof(List<MensajeEnviadoResponse>))]

    [JsonSerializable(typeof(CrearNotificacionRequest))]
    [JsonSerializable(typeof(NotificacionResponse))]
    [JsonSerializable(typeof(List<NotificacionResponse>))]

    public partial class AppJsonSerializerContext : JsonSerializerContext
    {
    }

}

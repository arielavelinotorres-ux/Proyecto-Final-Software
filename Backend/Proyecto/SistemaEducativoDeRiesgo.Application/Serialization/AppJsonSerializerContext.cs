using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using SistemaEducativoDeRiesgo.Application.DTOs;
using SistemaEducativoDeRiesgo.Application.DTOs.Usuarios;

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
    public partial class AppJsonSerializerContext : JsonSerializerContext
    {
    }
}

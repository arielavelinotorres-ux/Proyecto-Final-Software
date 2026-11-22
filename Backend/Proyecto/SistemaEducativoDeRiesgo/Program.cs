using SistemaEducativoDeRiesgo.Application.Interfaces.Usuarios;
using SistemaEducativoDeRiesgo.Application.Serialization;
using SistemaEducativoDeRiesgo.Application.Services.Usuarios;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Usuario;
using SistemaEducativoDeRiesgo.Insfrasctuture.Persistence;
using SistemaEducativoDeRiesgo.Insfrasctuture.Persistence.Usuarios;
using SistemaEducativoDeRiesgo.Insfrasctuture.Security;
using SistemaEducativoDeRiesgo.Web.Api.Endpoints;
using SistemaEducativoDeRiesgo.Web.Api.Middlewares;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});


builder.Services.AddOpenApi();

// Conexión y Seguridad
builder.Services.AddSingleton<SqlConnectionFactory>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

// Módulo Usuarios - Repositorios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IDocenteRepository, DocenteRepository>();
builder.Services.AddScoped<IEstudianteRepository, EstudianteRepository>();
builder.Services.AddScoped<IRecuperacionPasswordRepository, RecuperacionPasswordRepository>();

// Módulo Usuarios - Servicios
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IDocenteService, DocenteService>();
builder.Services.AddScoped<IEstudianteService, EstudianteService>();
builder.Services.AddScoped<IRecuperacionPasswordService, RecuperacionPasswordService>();

var app = builder.Build();

app.UseMiddleware<SqlExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Mapeo de Endpoints
app.MapUsuariosEndpoints();

app.Run();
# Informe de correcciones e integración del backend

*Proyecto:* HomeClass  
*Fecha:* 27 de noviembre de 2026  
*Alcance:* correcciones de compilación y configuración local del backend.

## 1. Resumen

Durante la integración del frontend con la API detecté errores de compilación en los contratos del módulo de configuración y un error HTTP 500 al consultar usuarios. Corregí las interfaces y DTO, retiré una opción de runtime incompatible con el proveedor SQL y actualicé la conexión de desarrollo para usar el SQL Server local.

La instancia local no tenía la base lms_db. Con autorización para crearla vacía y sin sobrescribir bases existentes, inicialicé el esquema mínimo de usuarios y sus procedimientos almacenados. Después de estos cambios, el backend compila y GET /api/usuarios/ responde HTTP 200 directamente y a través del proxy del frontend.

## 2. Errores encontrados y resolución

### 2.1 Interfaces declaradas como clases y DTO no accesibles

*Síntomas observados en Visual Studio 2022*

- CS0501: métodos declarados sin cuerpo en IFAuditoriaConfiguracion.cs.
- CS0246: no se encontraban los DTO de auditoría desde la interfaz.
- CS0051 y CS0050: los DTO usados por IFParametrosSistema tenían menor accesibilidad que los métodos.
- CS0006: no se encontró SistemaEducativoDeRiesgo.Application.dll.

*Causa*

Las clases IFAuditoriaConfiguracion y IFParametrosSistema representaban contratos de servicio, pero estaban declaradas como clases normales con firmas sin implementación. Además, los records de configuración estaban anidados dentro de clases internal, por lo que no podían utilizarse como tipos públicos de las interfaces.

*Corrección*

- Convertí ambas clases en interfaces públicas.
- Importé el namespace de los DTO en las interfaces.
- Moví los records de parámetros y auditoría a nivel del namespace y los declaré públicos.

*Archivos afectados*

- [IFAuditoriaConfiguracion.cs](./Proyecto/SistemaEducativoDeRiesgo.Application/Interfaces/Configuracion/IFAuditoriaConfiguracion.cs)
- [IFParametrosSistema.cs](./Proyecto/SistemaEducativoDeRiesgo.Application/Interfaces/Configuracion/IFParametrosSistema.cs)
- [AuditoriaConfiguracionDtos.cs](./Proyecto/SistemaEducativoDeRiesgo.Application/DTOs/Configuracion/AuditoriaConfiguracionDtos.cs)
- [Parametros_sistema.cs](./Proyecto/SistemaEducativoDeRiesgo.Application/DTOs/Configuracion/Parametros_sistema.cs)

El CS0006 era consecuencia de que el proyecto Application no generaba su ensamblado debido a los errores anteriores; no requería crear ni copiar manualmente una DLL.

### 2.2 Error de runtime por globalización invariante

*Síntoma*

La petición de usuarios devolvía 500 con la excepción:

text
System.NotSupportedException: Globalization Invariant Mode is not supported.


*Causa y resolución*

El proyecto web forzaba InvariantGlobalization=true. El proveedor SQL utilizado por la API necesita funcionalidad de globalización. Retiré esa propiedad del proyecto para utilizar el comportamiento normal del runtime.

*Archivo afectado:* [SistemaEducativoDeRiesgo.Web.Api.csproj](./Proyecto/SistemaEducativoDeRiesgo/SistemaEducativoDeRiesgo.Web.Api.csproj).

### 2.3 Cadena de conexión apuntaba a una instancia no disponible

*Síntoma*

Una vez corregido el error de runtime, el backend reportó un error de red o de instancia al conectarse a SQL Server.

*Causa*

La configuración de desarrollo apuntaba a DESKTOP-JESUS07\SQLEXPRESS01, una instancia que no estaba accesible desde este equipo. En la máquina de desarrollo se comprobó que estaba activo el servicio SQL Server predeterminado (MSSQLSERVER).

*Corrección*

Actualicé DefaultConnection para conectarse a localhost, manteniendo autenticación integrada y la base lms_db. No incluyo aquí valores de conexión.

*Archivo afectado:* [appsettings.Development.json](./Proyecto/SistemaEducativoDeRiesgo/appsettings.Development.json).

### 2.4 La base local no existía

Antes de inicializarla comprobé que lms_db no existía en el SQL Server local. Con autorización, la creé únicamente si estaba ausente y sin eliminar bases existentes.

Inicialicé el esquema mínimo que requiere la consulta de usuarios con estos scripts:

- [Usuarios.sql](../DB/Datos/Schemas/Usuarios.sql)
- [roles.sql](../DB/Datos/Tablas/Usuarios/roles.sql)
- [usuarios.sql](../DB/Datos/Tablas/Usuarios/usuarios.sql)
- [StoreProcedure_Usuarios.sql](../DB/Datos/Store%20Procedures/SP_Usuarios/StoreProcedure_Usuarios.sql)

No ejecuté [init_database.sql](../DB/Datos/init_database.sql), porque ese script elimina lms_db si ya existe. La base local quedó con las tablas Usuarios.roles y Usuarios.usuarios, tres roles base y ninguna cuenta de usuario.

## 3. Validación

| Comprobación | Resultado |
|---|---|
| Compilación del proyecto Application | Correcta, sin errores. |
| Compilación de SistemaEducativoDeRiesgo.Web.Api.csproj | Correcta, sin errores; quedan dos advertencias AOT (IL2026 e IL3050) en UsuariosEndpoints.cs, relacionadas con el overload de Results.Json. |
| GET http://localhost:5296/api/usuarios/ | HTTP 200. |
| GET http://127.0.0.1:5173/api/usuarios/ a través del proxy Vite | HTTP 200. |
| Datos de cuentas en la base local | Cero cuentas; no agregué usuarios ni contraseñas de prueba. |

No validé un inicio de sesión real porque la base recién creada no contiene cuentas. Las tablas y procedimientos de comunicación, foros, notificaciones y los demás módulos no se inicializaron como parte de esta corrección; sus endpoints pueden requerir su propio esquema antes de usarse en esta base nueva.

## 4. Archivos de backend modificados

- Proyecto/SistemaEducativoDeRiesgo.Application/DTOs/Configuracion/AuditoriaConfiguracionDtos.cs
- Proyecto/SistemaEducativoDeRiesgo.Application/DTOs/Configuracion/Parametros_sistema.cs
- Proyecto/SistemaEducativoDeRiesgo.Application/Interfaces/Configuracion/IFAuditoriaConfiguracion.cs
- Proyecto/SistemaEducativoDeRiesgo.Application/Interfaces/Configuracion/IFParametrosSistema.cs
- Proyecto/SistemaEducativoDeRiesgo/SistemaEducativoDeRiesgo.Web.Api.csproj
- Proyecto/SistemaEducativoDeRiesgo/appsettings.Development.json

La creación de lms_db y la ejecución de los scripts indicados son cambios del entorno SQL local; no modifican archivos SQL del repositorio ni eliminan datos.

## 5. Ejecución local

Desde la raíz del repositorio, iniciar la API:

powershell
Set-Location Backend\Proyecto\SistemaEducativoDeRiesgo
dotnet run --launch-profile http


El perfil HTTP utiliza el puerto 5296. El frontend de desarrollo consume la API mediante el proxy /api.
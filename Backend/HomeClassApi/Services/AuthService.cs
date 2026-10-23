using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Data;
using System.Text;
using HomeClassApi.Models;

namespace HomeClassApi.Services
{
    public static class AuthService
    {
        private static readonly string[] PermisosAdminDisponibles = {
            "general", "usuarios", "usuarios.crear", "usuarios.editar", "usuarios.eliminar", "usuarios.restablecer",
            "cursos", "cursos.consultar", "cursos.crear", "cursos.editar", "cursos.eliminar", "cursos.matricular",
            "reportes", "reportes.consultar", "auditoria.consultar"
        };
        public static UsuarioSeed BuscarPorCorreo(string correo)
        {
            var row = SqlData.Query(
                    "SELECT Id, Nombre, Correo, ClaveHash, Rol, Activo, IntentosFallidos, Bloqueado, SolicitudRestablecimiento, TipoAdmin, PermisosAdmin FROM dbo.Usuario WHERE Correo = @Correo",
                SqlData.Param("@Correo", SqlDbType.NVarChar, correo, 254)).FirstOrDefault();
            return row == null ? null : new UsuarioSeed
            {
                Id = (int)row["Id"],
                Nombre = (string)row["Nombre"],
                Correo = (string)row["Correo"],
                ClaveHash = (string)row["ClaveHash"],
                Rol = (string)row["Rol"],
                Activo = (bool)row["Activo"],
                IntentosFallidos = (int)row["IntentosFallidos"],
                Bloqueado = (bool)row["Bloqueado"],
                SolicitudRestablecimiento = (bool)row["SolicitudRestablecimiento"],
                TipoAdmin = (string)row["TipoAdmin"],
                PermisosAdminCsv = row["PermisosAdmin"] == DBNull.Value ? null : Convert.ToString(row["PermisosAdmin"])
            };
        }

        public static UsuarioRespuesta ValidarLogin(string correo, string clave, out bool bloqueado)
        {
            bloqueado = false;
            var usuario = BuscarPorCorreo(correo);
            if (usuario == null || !usuario.Activo)
            {
                return null;
            }

            if (usuario.Bloqueado)
            {
                bloqueado = true;
                return null;
            }

            if (!VerificarClave(clave, usuario.ClaveHash))
            {
                if (usuario.Rol == "estudiante" || usuario.Rol == "docente")
                {
                    SqlData.Execute(
                        "UPDATE dbo.Usuario SET IntentosFallidos = IntentosFallidos + 1, Bloqueado = CASE WHEN IntentosFallidos + 1 >= 5 THEN 1 ELSE 0 END, SolicitudRestablecimiento = CASE WHEN IntentosFallidos + 1 >= 5 THEN 1 ELSE SolicitudRestablecimiento END WHERE Id = @Id AND Bloqueado = 0",
                        SqlData.Param("@Id", SqlDbType.Int, usuario.Id));
                    var actualizada = BuscarPorCorreo(correo);
                    bloqueado = actualizada != null && actualizada.Bloqueado;
                }
                return null;
            }

            SqlData.Execute("UPDATE dbo.Usuario SET ClaveHash=@Hash, IntentosFallidos=0, Bloqueado=0, SolicitudRestablecimiento=0 WHERE Id=@Id",
                SqlData.Param("@Hash", SqlDbType.VarChar, usuario.ClaveHash.StartsWith("pbkdf2-sha256$", StringComparison.Ordinal) ? usuario.ClaveHash : Hash(clave), 120),
                SqlData.Param("@Id", SqlDbType.Int, usuario.Id));

            return new UsuarioRespuesta
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Rol = usuario.Rol,
                Activo = usuario.Activo,
                TipoAdmin = usuario.TipoAdmin,
                PermisosAdmin = PermisosAdmin(usuario.Rol, usuario.TipoAdmin, usuario.PermisosAdminCsv)
            };
        }

        public static UsuarioRespuesta ObtenerPorId(int id)
        {
            var row = SqlData.Query(
                "SELECT Id, Nombre, Correo, Rol, Activo, Bloqueado, SolicitudRestablecimiento, TipoAdmin, PermisosAdmin FROM dbo.Usuario WHERE Id = @Id",
                SqlData.Param("@Id", SqlDbType.Int, id)).FirstOrDefault();
            return row == null ? null : MapUsuario(row);
        }

        public static List<UsuarioRespuesta> ObtenerUsuarios()
        {
            return SqlData.Query("SELECT Id, Nombre, Correo, Rol, Activo, Bloqueado, SolicitudRestablecimiento, TipoAdmin, PermisosAdmin FROM dbo.Usuario ORDER BY Nombre")
                .Select(MapUsuario).ToList();
        }

        public static bool TienePermisoAdmin(UsuarioRespuesta usuario, string permiso)
        {
            if (usuario == null || usuario.Rol != "admin") return false;
            var permisos = usuario.PermisosAdmin ?? new List<string>();
            if (permisos.Contains("general") || permisos.Contains(permiso)) return true;
            var separador = permiso.IndexOf('.');
            if (separador > 0) return permisos.Contains(permiso.Substring(0, separador));
            return permisos.Any(p => p.StartsWith(permiso + ".", StringComparison.Ordinal));
        }

        public static List<string> PermisosAdmin(string rol, string tipoAdmin)
        {
            if (rol != "admin") return new List<string>();
            return PermisosAdmin(rol, tipoAdmin, null);
        }

        public static List<string> PermisosAdmin(string rol, string tipoAdmin, string almacenados)
        {
            if (rol != "admin") return new List<string>();
            var permisos = string.IsNullOrWhiteSpace(almacenados)
                ? new List<string>()
                : almacenados.Split(',').Select(p => p.Trim()).Where(p => PermisosAdminDisponibles.Contains(p)).Distinct().ToList();
            if (permisos.Count == 0)
            {
                if (tipoAdmin == "general") permisos.Add("general");
                else if (new[] { "usuarios", "cursos", "reportes" }.Contains(tipoAdmin))
                {
                    permisos.Add(tipoAdmin);
                    if (tipoAdmin == "reportes") permisos.Add("auditoria.consultar");
                }
            }
            if (permisos.Contains("general")) return PermisosAdminDisponibles.ToList();
            return permisos;
        }

        public static List<string> NormalizarPermisosAdmin(IEnumerable<string> solicitados, string tipoAdmin)
        {
            var permisos = solicitados == null
                ? new List<string>()
                : solicitados.Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p.Trim()).Distinct().ToList();
            if (permisos.Count == 0)
            {
                if (tipoAdmin == "general") permisos.Add("general");
                else if (new[] { "usuarios", "cursos", "reportes" }.Contains(tipoAdmin))
                {
                    permisos.Add(tipoAdmin);
                    if (tipoAdmin == "reportes") permisos.Add("auditoria.consultar");
                }
            }
            return permisos;
        }

        public static bool PermisosAdminValidos(IEnumerable<string> permisos)
        {
            var lista = permisos == null ? new List<string>() : permisos.ToList();
            return lista.Count > 0 && lista.All(PermisosAdminDisponibles.Contains) && (!lista.Contains("general") || lista.Count == 1);
        }

        public static bool PermisosAdminEquivalentes(UsuarioRespuesta usuario, IEnumerable<string> permisos)
        {
            var propuestos = new UsuarioRespuesta { Rol = "admin", PermisosAdmin = permisos.ToList() };
            return PermisosAdminDisponibles.All(permiso => TienePermisoAdmin(usuario, permiso) == TienePermisoAdmin(propuestos, permiso));
        }

        public static bool SolicitarRestablecimiento(string correo)
        {
            return SqlData.Execute(
                "UPDATE dbo.Usuario SET SolicitudRestablecimiento = 1 WHERE Correo = @Correo AND Activo = 1 AND Rol IN ('estudiante','docente')",
                SqlData.Param("@Correo", SqlDbType.NVarChar, correo.Trim(), 254)) > 0;
        }

        public static bool RestablecerClave(int id, string nueva)
        {
            return SqlData.Execute(
                "UPDATE dbo.Usuario SET ClaveHash = @Hash, IntentosFallidos = 0, Bloqueado = 0, SolicitudRestablecimiento = 0 WHERE Id = @Id AND Rol IN ('estudiante','docente')",
                SqlData.Param("@Hash", SqlDbType.VarChar, Hash(nueva), 120),
                SqlData.Param("@Id", SqlDbType.Int, id)) > 0;
        }

        public static IEnumerable<object> ObtenerCursos(string rol, int usuarioId)
        {
            var filtro = rol == "estudiante"
                ? "INNER JOIN dbo.Inscripcion i ON i.CursoId = c.Id AND i.EstudianteId = @UsuarioId"
                : string.Empty;
            var predicado = rol == "docente" ? "WHERE c.DocenteId = @UsuarioId " : string.Empty;
            var sql = "SELECT c.Id AS id, c.Codigo AS codigo, c.Nombre AS nombre, c.Periodo AS periodo, " +
                "c.Descripcion AS descripcion, c.DocenteId AS docenteId, COALESCE(d.Nombre, N'Sin docente') AS docente, " +
                "(SELECT COUNT(*) FROM dbo.Inscripcion i2 WHERE i2.CursoId = c.Id) AS estudiantes, " +
                "CASE WHEN @Rol = 'estudiante' THEN propio.promedio ELSE resumen.promedio END AS promedio, " +
                "CASE WHEN @Rol = 'estudiante' THEN propio.asistencia ELSE resumen.asistencia END AS asistencia, " +
                "CASE WHEN @Rol = 'estudiante' THEN CASE WHEN propio.promedio < 70 OR propio.asistencia < 75 THEN 1 ELSE 0 END ELSE COALESCE(resumen.enRiesgo,0) END AS enRiesgo, " +
                "propio.promedioGrupo AS promedioGrupo, CASE WHEN propio.promedio IS NULL AND propio.asistencia IS NULL THEN 'sin-datos' WHEN propio.promedio < 70 OR propio.asistencia < 75 THEN 'alto' WHEN propio.promedio < 80 OR propio.asistencia < 85 THEN 'moderado' ELSE 'bajo' END AS riesgo " +
                "FROM dbo.Curso c LEFT JOIN dbo.Usuario d ON d.Id = c.DocenteId " + filtro +
                " OUTER APPLY (SELECT AVG(CAST(m.promedio AS FLOAT)) AS promedio, AVG(CAST(m.asistencia AS FLOAT)) AS asistencia, SUM(CASE WHEN m.promedio < 70 OR m.asistencia < 75 THEN 1 ELSE 0 END) AS enRiesgo FROM dbo.Inscripcion i " +
                "OUTER APPLY (SELECT (SELECT SUM(g.Nota*e.Peso)/NULLIF(SUM(CASE WHEN g.Nota IS NOT NULL THEN e.Peso END),0) FROM dbo.Evaluacion e LEFT JOIN dbo.Calificacion g ON g.EvaluacionId=e.Id AND g.EstudianteId=i.EstudianteId WHERE e.CursoId=c.Id) AS promedio, " +
                "(SELECT 100.0*SUM(CASE WHEN a.Presente=1 THEN 1 ELSE 0 END)/NULLIF(COUNT(*),0) FROM dbo.Asistencia a WHERE a.CursoId=c.Id AND a.EstudianteId=i.EstudianteId) AS asistencia) m WHERE i.CursoId=c.Id) resumen " +
                "OUTER APPLY (SELECT (SELECT SUM(g.Nota*e.Peso)/NULLIF(SUM(CASE WHEN g.Nota IS NOT NULL THEN e.Peso END),0) FROM dbo.Evaluacion e LEFT JOIN dbo.Calificacion g ON g.EvaluacionId=e.Id AND g.EstudianteId=@UsuarioId WHERE e.CursoId=c.Id) AS promedio, " +
                "(SELECT 100.0*SUM(CASE WHEN a.Presente=1 THEN 1 ELSE 0 END)/NULLIF(COUNT(*),0) FROM dbo.Asistencia a WHERE a.CursoId=c.Id AND a.EstudianteId=@UsuarioId) AS asistencia, " +
                "(SELECT AVG(CAST(p.promedio AS FLOAT)) FROM dbo.Inscripcion i OUTER APPLY (SELECT SUM(g.Nota*e.Peso)/NULLIF(SUM(CASE WHEN g.Nota IS NOT NULL THEN e.Peso END),0) AS promedio FROM dbo.Evaluacion e LEFT JOIN dbo.Calificacion g ON g.EvaluacionId=e.Id AND g.EstudianteId=i.EstudianteId WHERE e.CursoId=c.Id) p WHERE i.CursoId=c.Id) AS promedioGrupo) propio " +
                predicado + "ORDER BY c.Nombre";
            return SqlData.Query(sql, SqlData.Param("@Rol", SqlDbType.VarChar, rol, 20), SqlData.Param("@UsuarioId", SqlDbType.Int, usuarioId));
        }

        private static UsuarioRespuesta MapUsuario(System.Collections.Generic.Dictionary<string, object> row)
        {
            return new UsuarioRespuesta
            {
                Id = (int)row["Id"],
                Nombre = (string)row["Nombre"],
                Correo = (string)row["Correo"],
                Rol = (string)row["Rol"],
                Activo = (bool)row["Activo"],
                Bloqueado = (bool)row["Bloqueado"],
                SolicitudRestablecimiento = (bool)row["SolicitudRestablecimiento"],
                TipoAdmin = (string)row["TipoAdmin"],
                PermisosAdmin = PermisosAdmin((string)row["Rol"], (string)row["TipoAdmin"], row["PermisosAdmin"] == DBNull.Value ? null : Convert.ToString(row["PermisosAdmin"]))
            };
        }

        public static string Hash(string value)
        {
            const int iteraciones = 120000;
            var sal = new byte[16];
            using (var generador = RandomNumberGenerator.Create()) generador.GetBytes(sal);
            using (var pbkdf2 = new Rfc2898DeriveBytes(value ?? string.Empty, sal, iteraciones, HashAlgorithmName.SHA256))
            {
                var clave = pbkdf2.GetBytes(32);
                return "pbkdf2-sha256$" + iteraciones + "$" + Convert.ToBase64String(sal) + "$" + Convert.ToBase64String(clave);
            }
        }

        public static bool VerificarClave(string value, string almacenada)
        {
            if (string.IsNullOrEmpty(almacenada)) return false;
            var partes = almacenada.Split('$');
            if (partes.Length == 4 && partes[0] == "pbkdf2-sha256")
            {
                try
                {
                    var iteraciones = int.Parse(partes[1]);
                    var sal = Convert.FromBase64String(partes[2]);
                    var esperada = Convert.FromBase64String(partes[3]);
                    using (var pbkdf2 = new Rfc2898DeriveBytes(value ?? string.Empty, sal, iteraciones, HashAlgorithmName.SHA256))
                    {
                        var calculada = pbkdf2.GetBytes(esperada.Length);
                        var diferencia = calculada.Length ^ esperada.Length;
                        for (var i = 0; i < calculada.Length && i < esperada.Length; i++) diferencia |= calculada[i] ^ esperada[i];
                        return diferencia == 0;
                    }
                }
                catch (FormatException) { return false; }
                catch (ArgumentException) { return false; }
            }

            return string.Equals(almacenada, HashLegado(value), StringComparison.OrdinalIgnoreCase);
        }

        private static string HashLegado(string value)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(value ?? string.Empty);
                var hash = sha256.ComputeHash(bytes);
                return string.Concat(hash.Select(b => b.ToString("x2")));
            }
        }
    }
}

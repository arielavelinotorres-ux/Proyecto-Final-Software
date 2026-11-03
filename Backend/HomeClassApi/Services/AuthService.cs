using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HomeClassApi.Services
{
    public static class AuthService
    {
        public static string Hash(string value)
        {
            const int iterations = 120000;
            var salt = new byte[16];
            using (var generator = RandomNumberGenerator.Create()) generator.GetBytes(salt);
            using (var pbkdf2 = new Rfc2898DeriveBytes(value ?? string.Empty, salt, iterations, HashAlgorithmName.SHA256))
            {
                var hash = pbkdf2.GetBytes(32);
                return "pbkdf2-sha256$" + iterations + "$" + Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(hash);
            }
        }

        public static bool VerificarClave(string value, string almacenada)
        {
            if (string.IsNullOrEmpty(almacenada)) return false;
            var parts = almacenada.Split('$');
            if (parts.Length == 4 && parts[0] == "pbkdf2-sha256")
            {
                try
                {
                    if (!int.TryParse(parts[1], out var iterations) || iterations < 10000 || iterations > 1000000)
                        return false;
                    var salt = Convert.FromBase64String(parts[2]);
                    var expected = Convert.FromBase64String(parts[3]);
                    if (salt.Length < 8 || expected.Length == 0) return false;
                    using (var pbkdf2 = new Rfc2898DeriveBytes(value ?? string.Empty, salt, iterations, HashAlgorithmName.SHA256))
                    {
                        var actual = pbkdf2.GetBytes(expected.Length);
                        return CompararConstante(actual, expected);
                    }
                }
                catch (FormatException) { return false; }
                catch (ArgumentException) { return false; }
                catch (OverflowException) { return false; }
            }

            return CompararConstante(
                Encoding.ASCII.GetBytes(almacenada.ToLowerInvariant()),
                Encoding.ASCII.GetBytes(HashLegado(value)));
        }

        private static string HashLegado(string value)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(value ?? string.Empty);
                return string.Concat(sha256.ComputeHash(bytes).Select(b => b.ToString("x2")));
            }
        }

        private static bool CompararConstante(byte[] actual, byte[] esperado)
        {
            var diferencia = actual.Length ^ esperado.Length;
            var longitud = Math.Min(actual.Length, esperado.Length);
            for (var i = 0; i < longitud; i++) diferencia |= actual[i] ^ esperado[i];
            return diferencia == 0;
        }
    }
}

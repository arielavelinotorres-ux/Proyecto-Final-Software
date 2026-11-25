using SistemaEducativoDeRiesgo.Application.Interfaces.Usuarios;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SistemaEducativoDeRiesgo.Insfrasctuture.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100_000;
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

        public string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);
            return $"{Convert.ToHexString(salt)}.{Convert.ToHexString(hash)}";
        }

        public bool Verify(string password, string passwordHash)
        {
            var partes = passwordHash.Split('.');
            if (partes.Length != 2) return false;

            byte[] salt = Convert.FromHexString(partes[0]);
            byte[] hashGuardado = Convert.FromHexString(partes[1]);
            byte[] hashCalculado = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);

            return CryptographicOperations.FixedTimeEquals(hashCalculado, hashGuardado);
        }
    }
}

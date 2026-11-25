using System;
using System.Collections.Generic;
using System.Text;
using SistemaEducativoDeRiesgo.Insfrasctuture.Security;

namespace SistemaEducativoDeRiesgo.Tests.Unit.Usuarios
{
    public class PasswordHasherTests
    {
        private readonly PasswordHasher _hasher = new();

        [Fact]
        public void HashYVerify_ConPasswordCorrecto_DebeRetornarTrue()
        {
            var hash = _hasher.Hash("ClaveSegura123");

            Assert.NotEqual("ClaveSegura123", hash);
            Assert.True(_hasher.Verify("ClaveSegura123", hash));
        }

        [Fact]
        public void Verify_ConPasswordIncorrecto_DebeRetornarFalse()
        {
            var hash = _hasher.Hash("ClaveSegura123");

            Assert.False(_hasher.Verify("OtraClave999", hash));
        }

        [Fact]
        public void Hash_MismoPasswordDosVeces_DebeGenerarHashesDistintosPorSalt()
        {
            var hash1 = _hasher.Hash("ClaveSegura123");
            var hash2 = _hasher.Hash("ClaveSegura123");

            Assert.NotEqual(hash1, hash2);
        }
    }
}

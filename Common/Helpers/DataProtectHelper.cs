using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public static class DataProtectHelper
    {
        public static byte[] Encrypt(string data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            return ProtectedData.Protect(byteData, null, DataProtectionScope.CurrentUser);
        }

        public static string Decrypt(byte[] data)
        {
            var byteDecrypted = ProtectedData.Unprotect(data, null, DataProtectionScope.CurrentUser);

            return Encoding.ASCII.GetString(byteDecrypted);
        }
    }
}

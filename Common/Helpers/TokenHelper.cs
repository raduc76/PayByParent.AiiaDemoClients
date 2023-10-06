using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public static class TokenHelper
    {
        const string TOKEN_FILE = "token.data";

        public static string GetLocalToken()
        {
            var result = string.Empty;

            if (File.Exists(TOKEN_FILE))
            {
                result = DataProtectHelper.Decrypt(File.ReadAllBytes(TOKEN_FILE));
            }

            return result;
        }

        public static void SaveLocalToken(string token)
        {
            File.WriteAllBytes(TOKEN_FILE, DataProtectHelper.Encrypt(token));
        }
    }
}

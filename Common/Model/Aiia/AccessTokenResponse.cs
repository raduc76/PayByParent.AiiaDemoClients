using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model.Aiia
{
    public class AccessTokenResponse
    {
        public string Access_token { get; set; }

        public int Expires_in { get; set; }

        public string Redirect_uri { get; set; }

        public string Refresh_token { get; set; }
    }
}

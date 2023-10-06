using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model.Aiia
{
    public class RefreshTokenRequest
    {
        [JsonProperty("grant_type")]
        public string Grant_type { get; set; }

        [JsonProperty("refresh_token")]
        public string Refresh_token { get; set; }
    }
}

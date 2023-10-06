using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model.Aiia
{
    public class CodeExchangeRequest
    {
        [JsonProperty("redirect_uri")]
        public string Redirect_uri { get; set; }

        [JsonProperty("grant_type")]
        public string Grant_type { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }
    }
}

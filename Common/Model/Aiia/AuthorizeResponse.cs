using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model.Aiia
{
    public class AuthorizeResponse
    {
        public string AuthorizationUrl { get; set; }

        public string AuthorizationId { get; set; }
    }
}

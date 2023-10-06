using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model.Aiia
{
    public class AuthorizeRequest
    {
        public List<string> PaymentIds {  get; set; }

        public string RedirectUrl { get; set; }
    }
}

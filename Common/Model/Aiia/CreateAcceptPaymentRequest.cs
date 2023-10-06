using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model.Aiia
{
    public class CreateAcceptPaymentRequest
    {
        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public string SchemeId { get; set; }

        public string Reference { get; set; }

        public string DestinationId { get; set; }

        public string RedirectUrl { get; set; }

        public string PreselectedCountry { get; set; }
    }
}

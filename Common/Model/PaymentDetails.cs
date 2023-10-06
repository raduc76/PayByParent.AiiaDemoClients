using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentClient.Model
{
    public class PaymentDetails
    {
        public string Merchant { get; set; }

        public string MerchantBank { get; set; }

        public string MerchantAccount { get; set; }

        public string Product { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public string PaymentUser { get; set; }

        public string AuthorizeUrl { get; set; }
    }
}

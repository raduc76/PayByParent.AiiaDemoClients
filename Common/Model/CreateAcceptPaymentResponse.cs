using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class CreateAcceptPaymentResponse
    {
        public string AuthorizationUrl { get; set; }

        public string PaymentId { get; set; }
    }
}

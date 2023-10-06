using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentClient.Model
{
    public enum ControlType
    {
        PaymentSummary,

        WaitPayment,

        Rejected,

        PaymentAuthorized
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model.Aiia
{
    public class Payment
    {
        public Amount Amount { get; set; }

        public Destination Destination { get; set; }

        public string Message { get; set; }

        public string TransactionText { get; set; }
    }
}

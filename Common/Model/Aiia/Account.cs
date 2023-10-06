using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model.Aiia
{
    public class Account
    {
        public AccountProvider AccountProvider { get; set; }

        public AccountNumber Number { get; set; }

        public Amount Available { get; set; }

        public string Id { get; set; }

        public string Currency { get; set; }

        public string DisplayName
        {
            get { return $"{AccountProvider.Name} : {Number.Bban}"; }
        }
    }
}

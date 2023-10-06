using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public enum CallbackType
    {
        Unknown,

        NoCallback,

        EmptyParams,

        AuthorizeCodeReceived,

        PaymentAuthorized
    }
}

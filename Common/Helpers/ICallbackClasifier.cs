using Common.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public interface ICallbackClasifier
    {
        CallbackType Classify(string callbackUri, string clientRedirectUrl, out NameValueCollection queryParams);
    }
}

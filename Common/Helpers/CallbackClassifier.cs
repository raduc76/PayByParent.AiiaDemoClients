using Common.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common.Helpers
{
    public class CallbackClassifier: ICallbackClasifier
    {
        public const string KEY_AUTHORIZE_CODE = "code";
        public const string KEY_AUTHORIZATION_ID = "authorizationId";
        public const string KEY_PAYMENT_ID = "paymentId";

        public CallbackType Classify(string callbackUri, string clientRedirectUrl, out NameValueCollection queryParams)
        {
            queryParams = null;
            var result = CallbackType.Unknown;

            if (!callbackUri.StartsWith(clientRedirectUrl))
                return CallbackType.NoCallback;

            Uri uri = new Uri(callbackUri);
            queryParams = HttpUtility.ParseQueryString(uri.Query);

            if (!queryParams.HasKeys())
                result = CallbackType.EmptyParams;
            else if (queryParams.AllKeys.Contains(KEY_AUTHORIZE_CODE))
                result = CallbackType.AuthorizeCodeReceived;
            else if (queryParams.AllKeys.Contains(KEY_AUTHORIZATION_ID) || queryParams.AllKeys.Contains(KEY_PAYMENT_ID))
                result = CallbackType.PaymentAuthorized;

            return result;

        }
    }
}

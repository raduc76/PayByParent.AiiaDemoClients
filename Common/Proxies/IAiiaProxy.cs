using Common.Model;
using Common.Model.Aiia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Proxy
{
    public interface IAiiaProxy
    {
        Task<ServiceResponse<ConnectResponse>> ConnectUser();

        Task<ServiceResponse<AccessTokenResponse>> CodeExchange(CodeExchangeRequest codeExchangeRequest);

        Task<ServiceResponse<AccessTokenResponse>> RefreshToken(RefreshTokenRequest refreshTokenRequest);

        Task<ServiceResponse<AccountsCollection>> GetAccounts(string accessToken);

        Task<ServiceResponse<PaymentResponse>> CreatePayment(PaymentRequest paymentRequest, string accountId, string accessToken);

        Task<ServiceResponse<AuthorizeResponse>> AuthorizePayment(AuthorizeRequest authorizeRequest, string accountId, string accessToken);

        Task<ServiceResponse<CreateAcceptPaymentResponse>> CreateAcceptPayment(CreateAcceptPaymentRequest createAcceptPaymentRequest);
    }
}

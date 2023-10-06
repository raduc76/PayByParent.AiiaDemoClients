using Common.Model;
using Common.Model.Aiia;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Windows;
using System.Windows.Markup;

namespace Common.Proxy
{
    public class AiiaProxy: IAiiaProxy
    {
        const string SCOPE = "accounts offline_access payments:inbound payments:outbound";
        const string RESPONSE_TYPE = "code";

        private readonly IHttpClientFactory httpClientFactory;
        private readonly AiiaSettings aiiaSettings;

        public AiiaProxy(IHttpClientFactory httpClientFactory, IOptions<AiiaSettings> serviceSettings)
        {
            this.httpClientFactory = httpClientFactory;
            this.aiiaSettings = serviceSettings.Value;
        }

        public async Task<ServiceResponse<ConnectResponse>> ConnectUser()
        {
            var client = httpClientFactory.CreateClient("NoRedirectHttpMessageHandler");
            
            var builder = new UriBuilder($"{aiiaSettings.BaseUrl}/v1/oauth/connect");
            builder.Port = -1;
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["client_id"] = aiiaSettings.ClientId;
            query["scope"] = SCOPE;
            query["redirect_uri"] = aiiaSettings.ClientRedirectUrl;
            query["response_type"] = RESPONSE_TYPE;
            builder.Query = query.ToString();
            string url = builder.ToString();
            var aiiaResponse = await client.GetAsync(url);

            if (aiiaResponse.StatusCode != System.Net.HttpStatusCode.Found)
            {
                return new ServiceResponse<ConnectResponse>(null, (int)aiiaResponse.StatusCode);//TODO add error in response
            }

            var connectResponse = new ConnectResponse { Location = HttpUtility.UrlDecode(aiiaResponse.Headers.Location.AbsoluteUri)};
            return new ServiceResponse<ConnectResponse>(connectResponse, (int)aiiaResponse.StatusCode);
        }

        public async Task<ServiceResponse<AccessTokenResponse>> CodeExchange(CodeExchangeRequest codeExchangeRequest)
        {
            var client = httpClientFactory.CreateClient("NoRedirectHttpMessageHandler");
            client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(aiiaSettings.ClientId, aiiaSettings.ClientSecret);

            string url = $"{aiiaSettings.BaseUrl}/v1/oauth/token";
            var aiiaResponse = await client.PostAsJsonAsync(url, codeExchangeRequest);
            if (aiiaResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return new ServiceResponse<AccessTokenResponse>(null, (int)aiiaResponse.StatusCode);//TODO add error in response
            }

            var aiiaContent = JsonConvert.DeserializeObject<AccessTokenResponse>(await aiiaResponse.Content.ReadAsStringAsync());
            var codeExchangeResponse = new AccessTokenResponse
            {
                Access_token = aiiaContent.Access_token,
                Refresh_token = aiiaContent.Refresh_token,
                Expires_in = aiiaContent.Expires_in,
                Redirect_uri = aiiaContent.Redirect_uri
            };
            return new ServiceResponse<AccessTokenResponse>(codeExchangeResponse, (int)aiiaResponse.StatusCode);
        }

        public async Task<ServiceResponse<AccessTokenResponse>> RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            var client = httpClientFactory.CreateClient("NoRedirectHttpMessageHandler");
            client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(aiiaSettings.ClientId, aiiaSettings.ClientSecret);

            string url = $"{aiiaSettings.BaseUrl}/v1/oauth/token";
            var aiiaResponse = await client.PostAsJsonAsync(url, refreshTokenRequest);
            if (aiiaResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return new ServiceResponse<AccessTokenResponse>(null, (int)aiiaResponse.StatusCode);//TODO add error in response
            }

            var aiiaContent = JsonConvert.DeserializeObject<AccessTokenResponse>(await aiiaResponse.Content.ReadAsStringAsync());
            var codeExchangeResponse = new AccessTokenResponse
            {
                Access_token = aiiaContent.Access_token,
                Refresh_token = aiiaContent.Refresh_token,
                Expires_in = aiiaContent.Expires_in,
                Redirect_uri = aiiaContent.Redirect_uri
            };
            return new ServiceResponse<AccessTokenResponse>(codeExchangeResponse, (int)aiiaResponse.StatusCode);
        }

        public async Task<ServiceResponse<AccountsCollection>> GetAccounts(string accessToken)
        {
            var client = httpClientFactory.CreateClient("NoRedirectHttpMessageHandler");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            string url = $"{aiiaSettings.BaseUrl}/v1/accounts";
            var aiiaResponse = await client.GetAsync(url);
            if (aiiaResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return new ServiceResponse<AccountsCollection>(null, (int)aiiaResponse.StatusCode);//TODO add error in response
            }

            var accounts = JsonConvert.DeserializeObject<AccountsCollection>(await aiiaResponse.Content.ReadAsStringAsync());

            return new ServiceResponse<AccountsCollection>(accounts, (int)aiiaResponse.StatusCode);
        }

        public async Task<ServiceResponse<PaymentResponse>> CreatePayment(PaymentRequest paymentRequest, string accountId, string accessToken)
        {
            var client = httpClientFactory.CreateClient("NoRedirectHttpMessageHandler");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            string url = $"{aiiaSettings.BaseUrl}/v2/accounts/{accountId}/payments";

            var aiiaResponse = await client.PostAsJsonAsync(url, paymentRequest);
            if (aiiaResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return new ServiceResponse<PaymentResponse>(null, (int)aiiaResponse.StatusCode);//TODO add error in response
            }

            var paymentResponse = JsonConvert.DeserializeObject<PaymentResponse>(await aiiaResponse.Content.ReadAsStringAsync());

            return new ServiceResponse<PaymentResponse>(paymentResponse, (int)aiiaResponse.StatusCode);
        }

        public async Task<ServiceResponse<AuthorizeResponse>> AuthorizePayment(AuthorizeRequest authorizeRequest, string accountId, string accessToken)
        {
            var client = httpClientFactory.CreateClient("NoRedirectHttpMessageHandler");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            string url = $"{aiiaSettings.BaseUrl}/v2/accounts/{accountId}/payment-authorizations";

            var aiiaResponse = await client.PostAsJsonAsync(url, authorizeRequest);
            if (aiiaResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return new ServiceResponse<AuthorizeResponse>(null, (int)aiiaResponse.StatusCode);//TODO add error in response
            }

            var authorizeResponse = JsonConvert.DeserializeObject<AuthorizeResponse>(await aiiaResponse.Content.ReadAsStringAsync());

            return new ServiceResponse<AuthorizeResponse>(authorizeResponse, (int)aiiaResponse.StatusCode);
        }

        public async Task<ServiceResponse<CreateAcceptPaymentResponse>> CreateAcceptPayment(CreateAcceptPaymentRequest createAcceptPaymentRequest)
        {
            var client = httpClientFactory.CreateClient("NoRedirectHttpMessageHandler");
            client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(aiiaSettings.ClientId, aiiaSettings.ClientSecret);

            string url = $"{aiiaSettings.BaseUrl}/v2/payments/accept";

            var aiiaResponse = await client.PostAsJsonAsync(url, createAcceptPaymentRequest);
            if (aiiaResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return new ServiceResponse<CreateAcceptPaymentResponse>(null, (int)aiiaResponse.StatusCode);//TODO add error in response
            }

            var createAcceptPaymentResponse = JsonConvert.DeserializeObject<CreateAcceptPaymentResponse>(await aiiaResponse.Content.ReadAsStringAsync());

            return new ServiceResponse<CreateAcceptPaymentResponse>(createAcceptPaymentResponse, (int)aiiaResponse.StatusCode);
        }
    }
}

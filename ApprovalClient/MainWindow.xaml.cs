using ApprovalClient.Model;
using Common.Helpers;
using Common.Model.Aiia;
using Common.Model;
using Common.Proxy;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PaymentClient.Model;

namespace ApprovalClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string GRANT_TYPE_AUTH = "authorization_code";
        const string GRANT_TYPE_REFRESH = "refresh_token";
        const string CODE = "code";

        const string MERCHANT_BANK_CODE = "1234";
        const string MERCHANT_ACCOUNT = "Funny Games Inc";

        const string APPROVER_USER_NAME = "RF Curca";

        private readonly IAiiaProxy aiiaProxy;
        private readonly ICallbackClasifier callbackClasifier;
        private readonly AiiaSettings aiiaSettings;
        private readonly HubConnection hubConnection;
        private readonly TokenDetails tokenDetails;
        private readonly PaymentDetails paymentDetails;

        private IDictionary<ControlType, UserControl> controls = new Dictionary<ControlType, UserControl>();

        public MainWindow(IAiiaProxy aiiaProxy, ICallbackClasifier callbackClasifier, HubConnection hubConnection, IOptions<AiiaSettings> aiiaSettings, PaymentDetails paymentDetails, TokenDetails tokenDetails)
        {
            this.aiiaProxy = aiiaProxy;
            this.callbackClasifier = callbackClasifier;
            this.tokenDetails = tokenDetails;
            this.aiiaSettings = aiiaSettings.Value;
            this.hubConnection = hubConnection;
            this.paymentDetails = paymentDetails;

            InitializeComponent();

            RegisterControls();

            webControl.webView.NavigationStarting += WebView_NavigationStarting;
        }

        private async void WebView_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            var callbackType = callbackClasifier.Classify(e.Uri, aiiaSettings.ClientRedirectUrl, out NameValueCollection queryParams);

            if (callbackType == CallbackType.NoCallback)
            {
                return;
            }

            e.Cancel = true;
            webControl.webView.CoreWebView2.Navigate("about:blank");

            if (callbackType == CallbackType.EmptyParams)
            {
                //TODO handle error 
            }
            if (callbackType == CallbackType.AuthorizeCodeReceived)
            {
                var aiiaRequest = new CodeExchangeRequest
                {
                    Grant_type = GRANT_TYPE_AUTH,
                    Code = queryParams[CODE],
                    Redirect_uri = aiiaSettings.ClientRedirectUrl
                };
                var codeExchangeResponse = await aiiaProxy.CodeExchange(aiiaRequest);
                if (codeExchangeResponse.StatusCode != (int)HttpStatusCode.OK)
                {
                    //TODO handle error 
                    return;
                }
                tokenDetails.AccessToken = codeExchangeResponse.Value.Access_token;
                TokenHelper.SaveLocalToken(codeExchangeResponse.Value.Refresh_token);

                DisplayControl(ControlType.Welcome);
            }
            if (callbackType == CallbackType.PaymentAuthorized)
            {
                DisplayControl(ControlType.Welcome);
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //DisplayControl(ControlType.Progress);
            //return;

            await hubConnection.StartAsync();
            await hubConnection.SendAsync("ConnectApproverUser", APPROVER_USER_NAME);

            hubConnection.On<string, string, string, decimal, string, string>("RequestPaymentFromParent", (paymentUser, merchant, product, amount, currency, authorizeUrl) =>
            {
                paymentDetails.Merchant = merchant;
                paymentDetails.MerchantBank = MERCHANT_BANK_CODE;
                paymentDetails.MerchantAccount = MERCHANT_ACCOUNT;
                paymentDetails.Product = product;
                paymentDetails.Amount = amount; 
                paymentDetails.Currency = currency;
                paymentDetails.AuthorizeUrl = authorizeUrl;
                paymentDetails.PaymentUser = paymentUser;

                Dispatcher.Invoke(() => {
                    approveControl.DisplayPaymentDetails();
                    DisplayControl(ControlType.Approve);
                    });
            });

            //check local token, if not present start onboarding
            var localToken = TokenHelper.GetLocalToken();
            if (!string.IsNullOrEmpty(localToken))
            {
                var refreshTokenRequest = new RefreshTokenRequest
                {
                    Grant_type = GRANT_TYPE_REFRESH,
                    Refresh_token = localToken
                };

                var refreshTokenResponse = await aiiaProxy.RefreshToken(refreshTokenRequest);
                if (refreshTokenResponse.StatusCode != (int)HttpStatusCode.OK)
                {
                    //TODO handle error 
                    return;
                }
                tokenDetails.AccessToken = refreshTokenResponse.Value.Access_token;
                TokenHelper.SaveLocalToken(refreshTokenResponse.Value.Refresh_token);

                DisplayControl(ControlType.Welcome);
            }
            else
            {
                DisplayControl(ControlType.Web);

                var response = await aiiaProxy.ConnectUser();

                await webControl.webView.EnsureCoreWebView2Async(null);
                webControl.webView.CoreWebView2.Navigate(response.Value.Location);
            }
        }

        private void RegisterControls()
        {
            controls.Add(ControlType.Web, webControl);
            controls.Add(ControlType.Welcome, welcomeControl);
            controls.Add(ControlType.Approve, approveControl);
            controls.Add(ControlType.AccountSelection, accountSelectionControl);
            controls.Add(ControlType.Progress, progressControl);
        }

        public void DisplayControl(ControlType controlType)
        {
            foreach (var child in grdMain.Children)
            {
                (child as UserControl).Visibility = Visibility.Collapsed;
            }

            controls[controlType].Visibility = Visibility.Visible;
        }

        public async Task WebControlNavigateTo(string url)
        {
            await webControl.webView.EnsureCoreWebView2Async(null);
            webControl.webView.CoreWebView2.Navigate(url);
        }
    }
}

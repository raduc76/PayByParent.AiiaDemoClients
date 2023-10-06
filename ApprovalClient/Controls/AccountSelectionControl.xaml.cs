using ApprovalClient.Model;
using Common.Model;
using Common.Model.Aiia;
using Common.Proxy;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using PaymentClient.Model;
using System;
using System.Collections.Generic;
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

namespace ApprovalClient.Controls
{
    /// <summary>
    /// Interaction logic for AccountSelectionControl.xaml
    /// </summary>
    public partial class AccountSelectionControl : UserControl
    {
        private readonly IAiiaProxy aiiaProxy;
        private readonly TokenDetails tokenDetails;
        private readonly PaymentDetails paymentDetails;
        private readonly AiiaSettings aiiaSettings;

        public AccountSelectionControl()
        {
            if (App.ServiceProvider != null)
            { 
                this.aiiaProxy = App.ServiceProvider.GetRequiredService<IAiiaProxy>();
                this.tokenDetails = App.ServiceProvider.GetRequiredService<TokenDetails>();
                this.paymentDetails = App.ServiceProvider.GetRequiredService<PaymentDetails>();
                this.aiiaSettings = App.ServiceProvider.GetRequiredService<IOptions<AiiaSettings>>().Value;
            }

            InitializeComponent();
        }

        public async Task LoadAccounts()
        {
            var accountsResponse = await aiiaProxy.GetAccounts(tokenDetails.AccessToken);
            if (accountsResponse.StatusCode != (int)HttpStatusCode.OK)
            {
                //TODO handle error 
                return;
            }

            cbxAccounts.ItemsSource = accountsResponse.Value.Accounts.Where(a => a.Available.Value > 0);
        }

        private async void btnPay_Click(object sender, RoutedEventArgs e)
        {
            var account = cbxAccounts.SelectedItem as Account;
            if (account == null)
            {
                MessageBox.Show("Please select an account");
                return;
            }

            if (account.Currency != paymentDetails.Currency)
            {
                MessageBox.Show($"Account currency {account.Currency} does not match payment currency {paymentDetails.Currency}");
                return;
            }

            var paymentRequest = new PaymentRequest
            {
                Payment = new Payment
                {
                    Amount = new Amount
                    {
                        Value = paymentDetails.Amount,
                        Currency = paymentDetails.Currency
                    },
                    Destination = new Destination
                    {
                        Bban = new Bban
                        {
                            BankCode = paymentDetails.MerchantBank,
                            AccountNumber = paymentDetails.MerchantAccount
                        },
                        Name = paymentDetails.Merchant
                    },
                    Message = "Online shopping",
                    TransactionText = "Online shopping"
                }
            };
            var paymentResponse = await aiiaProxy.CreatePayment(paymentRequest, (cbxAccounts.SelectedItem as Account).Id, tokenDetails.AccessToken);
            if (paymentResponse.StatusCode != (int)HttpStatusCode.OK)
            {
                //TODO handle error 
                return;
            }

            var authorizeRequest = new AuthorizeRequest
            {
                PaymentIds = new List<string> { paymentResponse.Value.PaymentId },
                RedirectUrl = aiiaSettings.ClientRedirectUrl
            };
            var authorizeResponse = await aiiaProxy.AuthorizePayment(authorizeRequest, (cbxAccounts.SelectedItem as Account).Id, tokenDetails.AccessToken);
            if (authorizeResponse.StatusCode != (int)HttpStatusCode.OK)
            {
                //TODO handle error 
                return;
            }

            var mainWindow = App.ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.DisplayControl(ControlType.Web);
            await mainWindow.WebControlNavigateTo(authorizeResponse.Value.AuthorizationUrl);
        }
    }
}

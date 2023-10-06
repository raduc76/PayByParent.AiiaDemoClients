using Common.Model.Aiia;
using Common.Model;
using Common.Proxy;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
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
using Microsoft.Extensions.Options;

namespace PaymentClient.Controls
{
    /// <summary>
    /// Interaction logic for ProductControl.xaml
    /// </summary>
    public partial class PaymentSummaryControl : UserControl
    {
        const string SCHEME_ID = "DanishDomesticCreditTransfer";
        const string REFERENCE = "1234567890";
        const string COUNTRY = "DK";
        const string DESTINATION_ID = "3b508ab0-b02a-404b-a07f-301c4a869fbd";

        private readonly HubConnection hubConnection;
        private readonly PaymentDetails paymentDetails;
        private readonly AiiaSettings aiiaSettings;
        private readonly IAiiaProxy aiiaProxy;

        public PaymentSummaryControl()
        {
            if (App.ServiceProvider != null)
            {
                this.hubConnection = App.ServiceProvider.GetRequiredService<HubConnection>();
                this.paymentDetails = App.ServiceProvider.GetRequiredService<PaymentDetails>();
                this.aiiaProxy = App.ServiceProvider.GetRequiredService<IAiiaProxy>();
                this.aiiaSettings = App.ServiceProvider.GetRequiredService<IOptions<AiiaSettings>>().Value;
            }

            InitializeComponent();
        }

        private async void btnPayWithParent_Click(object sender, RoutedEventArgs e)
        {
            var createAcceptPaymentRequest = new CreateAcceptPaymentRequest
            {
                Amount  = paymentDetails.Amount,
                Currency = paymentDetails.Currency,
                SchemeId = SCHEME_ID,
                Reference = REFERENCE,
                DestinationId  = DESTINATION_ID,
                RedirectUrl = aiiaSettings.ClientRedirectUrl,
                PreselectedCountry = COUNTRY
            };
            var aiiaResponse = await aiiaProxy.CreateAcceptPayment(createAcceptPaymentRequest);
            if (aiiaResponse.StatusCode != (int)HttpStatusCode.OK)
            {
                //TODO handle error 
                return;
            }

            await hubConnection.SendAsync("RequestPayment", paymentDetails.Merchant, paymentDetails.Product, paymentDetails.Amount, paymentDetails.Currency, aiiaResponse.Value.AuthorizationUrl);

            var mainWindow = App.ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.DisplayControl(ControlType.WaitPayment);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (paymentDetails != null)
            {
                txtMerchant.Text = paymentDetails.Merchant;
                txtProduct.Text = paymentDetails.Product;
                txtAmount.Text = $"{paymentDetails.Amount} {paymentDetails.Currency}";
            }
        }
    }
}

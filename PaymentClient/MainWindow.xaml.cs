using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using Microsoft.Web.WebView2.Core;
using PaymentClient.Model;
using Microsoft.Extensions.Options;
using System.Web;
using System.Diagnostics.Eventing.Reader;
using System.Collections.Specialized;
using System.Net;
using Microsoft.AspNetCore.SignalR.Client;
using System.Security.Policy;
using PaymentClient.Controls;
using Common.Helpers;
using Common.Model;

namespace PaymentClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string APPROVER_USER_NAME = "RF Curca";
        const string PAYMENT_USER_NAME = "AA Curca";

        const string MERCHANT = "CARLSBERG A/S";
        const string PRODUCT = "My happy pets - The game";
        const decimal AMOUNT = 19;
        const string CURRENCY = "DKK";

        private readonly HubConnection hubConnection;
        private readonly PaymentDetails paymentDetails;

        private IDictionary<ControlType, UserControl> controls = new Dictionary<ControlType, UserControl>();

        public MainWindow(HubConnection hubConnection, PaymentDetails paymentDetails)
        {
            this.hubConnection = hubConnection;

            this.paymentDetails = paymentDetails;
            this.paymentDetails.Merchant = MERCHANT;
            this.paymentDetails.Product = PRODUCT;
            this.paymentDetails.Amount = AMOUNT;
            this.paymentDetails.Currency = CURRENCY;

            InitializeComponent();

            RegisterControls();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await hubConnection.StartAsync();
            await hubConnection.SendAsync("ConnectPaymentUser", PAYMENT_USER_NAME, APPROVER_USER_NAME);
            hubConnection.On("RejectPaymentToClient", () =>
            {
                Dispatcher.Invoke(() =>
                {
                        DisplayControl(ControlType.Rejected);
                });
            });
            hubConnection.On<string>("NotifyPaymentAuthorizationToClient", (authId) =>
            {
                Dispatcher.Invoke(() => DisplayControl(ControlType.PaymentAuthorized));
            });

            DisplayControl(ControlType.PaymentSummary);
            return;
         }

        private void RegisterControls()
        {
            controls.Add(ControlType.PaymentSummary, paymentSummaryControl);
            controls.Add(ControlType.WaitPayment, waitPaymentControl);
            controls.Add(ControlType.Rejected, rejectedControl);
            controls.Add(ControlType.PaymentAuthorized, paymentAuthorizedControl);
        }

        public void DisplayControl(ControlType controlType)
        {
            foreach (var child in grdMain.Children)
            {
                (child as UserControl).Visibility = Visibility.Collapsed;
            }

            controls[controlType].Visibility = Visibility.Visible;
        }
    }
}

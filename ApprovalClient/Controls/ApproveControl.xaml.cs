using ApprovalClient.Model;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using PaymentClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for ApproveControl.xaml
    /// </summary>
    public partial class ApproveControl : UserControl
    {
        private readonly HubConnection hubConnection;
        private readonly PaymentDetails paymentDetails;

        public ApproveControl()
        {
            InitializeComponent();

            if (App.ServiceProvider != null)
            { 
                this.hubConnection = App.ServiceProvider.GetRequiredService<HubConnection>();
                this.paymentDetails = App.ServiceProvider.GetRequiredService<PaymentDetails>();
            }
        }

        public void DisplayPaymentDetails()
        {
            txtMerchant.Text = paymentDetails.Merchant;
            txtAmount.Text = $"{paymentDetails.Amount} {paymentDetails.Currency}";
            txtUser.Text = paymentDetails.PaymentUser;
        }

        private async void btnApprove_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = App.ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.DisplayControl(ControlType.Web);
            await mainWindow.WebControlNavigateTo(paymentDetails.AuthorizeUrl);
        }

        private async void btnReject_Click(object sender, RoutedEventArgs e)
        {
            await hubConnection.SendAsync("RejectPayment");
            var mainWindow = App.ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.DisplayControl(ControlType.Welcome);
        }
    }
}

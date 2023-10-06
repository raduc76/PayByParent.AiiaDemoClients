using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
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

namespace PaymentClient.Controls
{
    /// <summary>
    /// Interaction logic for RejectedControl.xaml
    /// </summary>
    public partial class RejectedControl : UserControl
    {
        public RejectedControl()
        {
            InitializeComponent();
        }

        private void btnRetry_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = App.ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.DisplayControl(ControlType.PaymentSummary);
        }
    }
}

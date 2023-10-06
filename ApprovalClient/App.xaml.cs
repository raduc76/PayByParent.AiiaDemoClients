using ApprovalClient.Model;
using Common.Helpers;
using Common.Model;
using Common.Proxy;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentClient.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace ApprovalClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient("NoRedirectHttpMessageHandler")
                .ConfigurePrimaryHttpMessageHandler(() =>
                    new HttpClientHandler
                    {
                        AllowAutoRedirect = false,
                    });

            IConfiguration settingsConfiguration = Configuration.GetSection(nameof(ServiceSettings));
            var serviceSettings = new ServiceSettings();
            settingsConfiguration.Bind(serviceSettings);
            services.Configure<ServiceSettings>(settingsConfiguration);

            IConfiguration aiiaSettingsConfiguration = Configuration.GetSection(nameof(AiiaSettings));
            var aiiaSettings = new AiiaSettings();
            aiiaSettingsConfiguration.Bind(aiiaSettings);
            services.Configure<AiiaSettings>(aiiaSettingsConfiguration);

            services.AddSingleton(h => new HubConnectionBuilder().WithUrl(serviceSettings.SignalRUrl).Build());
            services.AddSingleton<IAiiaProxy, AiiaProxy>();
            services.AddSingleton<ICallbackClasifier, CallbackClassifier>();
            services.AddSingleton<TokenDetails>();
            services.AddSingleton<PaymentDetails>();

            services.AddSingleton(typeof(MainWindow));
        }
    }
}

using FlashCommon;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace FlashWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Cosmos.Endpoint = Secret.Endpoint;
            Cosmos.AuthKey = Secret.AuthKey;

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceLocator.Init(serviceCollection.BuildServiceProvider());
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IAzure, AzureService>();
            services.AddSingleton<IFirebase, FirebaseService>();
            services.AddSingleton<IDynamicDB, DynamicDB>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set up the IDynamicDB object
            var db = ServiceLocator.GetInstance<IDynamicDB>();
            db.SetCurrentDB(ServiceLocator.GetInstance<IFirebase>());
        }

    }

}

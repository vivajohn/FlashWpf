using FlashCommon;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceLocator.Init(serviceCollection.BuildServiceProvider());
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDatabase, AzureService>();
            services.AddSingleton<IDynamicDB, DynamicDB>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set up the IDynamicDB object
            var db = ServiceLocator.GetInstance<IDynamicDB>();
            db.SetCurrentDB(ServiceLocator.GetInstance<IDatabase>());
        }

    }

}

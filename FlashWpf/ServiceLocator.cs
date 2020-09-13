using Microsoft.Extensions.DependencyInjection;
using System;

namespace FlashWpf
{
    // In WPF you can not inject objects into pages or user controls. This class
    // is a workaround for this behaviour.
    public static class ServiceLocator
    {
        private static IServiceProvider services;

        public static void Init(IServiceProvider provider) 
        {
            services = provider;
        }

        public static T GetInstance<T>() 
        {
            return services.GetService<T>();
        }
    }
}

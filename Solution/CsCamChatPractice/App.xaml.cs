using CommunityToolkit.Mvvm.DependencyInjection;
using CsCamChatPractice.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace CsCamChatPractice
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            Ioc.Default.ConfigureServices(serviceProvider);

            var clientWindow = serviceProvider.GetRequiredService<ClientChatWindow>();
            clientWindow.Show();

            var serWindow = serviceProvider.GetRequiredService<ServerChatWindow>();
            serWindow.Show();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ClientChatWindow>();
            services.AddSingleton<ClientChatViewModelBase, ClientChatViewModel>();

            services.AddSingleton<ServerChatWindow>();
            services.AddSingleton<ServerChatViewModelBase, ServerChatViewModel>();
        }
    }
}

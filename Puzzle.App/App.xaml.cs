using Microsoft.Extensions.Hosting;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Puzzle.App.Views;
using Puzzle.App.ViewModels;
using Puzzle.BL.Extensions;

namespace Puzzle.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) => 
                    { ConfigureServices(context.Configuration, services); })
                .Build();
        }

        private static void ConfigureServices(IConfiguration configuration,
            IServiceCollection services)
        {
            services.AddBLServices();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainViewModel>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }
    }
}

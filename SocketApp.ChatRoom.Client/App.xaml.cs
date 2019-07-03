using Autofac;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using SocketApp.ChatRoom.Client.IOC;
using System.Windows;

namespace SocketApp.ChatRoom.Client
{
    /// <summary>
    /// App.xaml 的互動邏輯
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ContainerBuilder builder = new ContainerBuilder();
            AutofacBootstrapper.RegisterType(builder);
            IContainer container = builder.Build();

            using (ILifetimeScope lifeTimeScope = container.BeginLifetimeScope())
            {
                ILoggerFactory loggerFactory = lifeTimeScope.Resolve<ILoggerFactory>();
                loggerFactory.AddNLog(new NLogProviderOptions
                {
                    CaptureMessageTemplates = true,
                    CaptureMessageProperties = true
                });

                IMainWindow mainWindow = lifeTimeScope.Resolve<IMainWindow>();
                mainWindow.ShowDialog();
            }
        }
    }
}
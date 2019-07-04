using Autofac;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using SocketApp.ChatRoom.Server.IOC;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace SocketApp.ChatRoom.Server
{
    /// <summary>
    /// App.xaml 的互動邏輯
    /// </summary>
    public partial class App
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) => this.LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");
            this.DispatcherUnhandledException += (s, e) => this.LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
            TaskScheduler.UnobservedTaskException += (s, e) => this.LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
        }

        private IContainer Container { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            ContainerBuilder builder = new ContainerBuilder();
            AutofacBootstrapper.RegisterType(builder);
            this.Container = builder.Build();

            using (ILifetimeScope lifeTimeScope = this.Container.BeginLifetimeScope())
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

        private void LogUnhandledException(Exception exception, string eventName)
        {
            using (ILifetimeScope lifetimeScope = this.Container.BeginLifetimeScope())
            {
                ILogger<App> logger = lifetimeScope.Resolve<ILogger<App>>();
                logger.LogError(exception, $"Error occur in {eventName}.");
            }
        }
    }
}
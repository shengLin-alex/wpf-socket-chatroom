using Autofac;
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
                IMainWindow mainWindow = lifeTimeScope.Resolve<IMainWindow>();
                mainWindow.ShowDialog();
            }
        }
    }
}
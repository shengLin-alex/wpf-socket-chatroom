using Autofac;
using SocketApp.ChatRoom.Client.DataBinding;

namespace SocketApp.ChatRoom.Client.IOC
{
    public static class AutofacBootstrapper
    {
        public static void RegisterType(ContainerBuilder builder)
        {
            builder.RegisterType<MainWindow>()
                  .As<IMainWindow>()
                  .UsingConstructor(typeof(IClientSideViewModel))
                  .InstancePerLifetimeScope();

            builder.RegisterType<ClientSideViewModel>()
                  .As<IClientSideViewModel>()
                  .InstancePerLifetimeScope();
        }
    }
}
using Autofac;
using SocketApp.ChatRoom.Client.DataBinding;

namespace SocketApp.ChatRoom.Client.IOC
{
    public static class AutofacBootstrapper
    {
        public static void RegisterType(ContainerBuilder bulder)
        {
            bulder.RegisterType<MainWindow>()
                  .As<IMainWindow>()
                  .UsingConstructor(typeof(IClientSideViewModel))
                  .InstancePerLifetimeScope();

            bulder.RegisterType<ClientSideViewModel>()
                  .As<IClientSideViewModel>()
                  .InstancePerLifetimeScope();
        }
    }
}
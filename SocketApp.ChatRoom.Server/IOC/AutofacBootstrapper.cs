using Autofac;
using SocketApp.ChatRoom.Server.DataBinding;

namespace SocketApp.ChatRoom.Server.IOC
{
    public static class AutofacBootstrapper
    {
        public static void RegisterType(ContainerBuilder bulder)
        {
            bulder.RegisterType<MainWindow>()
                  .As<IMainWindow>()
                  .UsingConstructor(typeof(IServerSideViewModel))
                  .InstancePerLifetimeScope();

            bulder.RegisterType<ServerSideViewModel>()
                  .As<IServerSideViewModel>()
                  .InstancePerLifetimeScope();
        }
    }
}
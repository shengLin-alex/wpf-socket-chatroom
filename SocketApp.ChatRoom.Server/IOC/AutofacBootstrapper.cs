using Autofac;
using SocketApp.ChatRoom.Server.DataBinding;

namespace SocketApp.ChatRoom.Server.IOC
{
    public static class AutofacBootstrapper
    {
        public static void RegisterType(ContainerBuilder builder)
        {
            builder.RegisterType<MainWindow>()
                  .As<IMainWindow>()
                  .UsingConstructor(typeof(IServerSideViewModel))
                  .InstancePerLifetimeScope();

            builder.RegisterType<ServerSideViewModel>()
                  .As<IServerSideViewModel>()
                  .InstancePerLifetimeScope();
        }
    }
}
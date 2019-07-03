using Autofac;
using Microsoft.Extensions.Logging;
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

            builder.RegisterType<LoggerFactory>()
                   .As<ILoggerFactory>()
                   .SingleInstance();

            builder.RegisterGeneric(typeof(Logger<>))
                   .As(typeof(ILogger<>))
                   .SingleInstance();
        }
    }
}
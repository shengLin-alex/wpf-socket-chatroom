using Autofac;
using Microsoft.Extensions.Logging;
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

            builder.RegisterType<LoggerFactory>()
                   .As<ILoggerFactory>()
                   .SingleInstance();

            builder.RegisterGeneric(typeof(Logger<>))
                   .As(typeof(ILogger<>))
                   .SingleInstance();
        }
    }
}
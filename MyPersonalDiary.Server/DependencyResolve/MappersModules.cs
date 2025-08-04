using Autofac;
using AutoMapper;
using System.Reflection;

namespace MyPersonalDiary.Server.DependencyResolve
{
    public class MappersModules : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var loggerFactory = c.Resolve<ILoggerFactory>();
                return new MapperConfiguration(cfg =>
                {
                    cfg.AddMaps(Assembly.GetExecutingAssembly());
                }, loggerFactory);
            })
            .SingleInstance();

            builder.Register(c =>
                c.Resolve<MapperConfiguration>()
                 .CreateMapper(c.Resolve))
            .As<IMapper>()
            .InstancePerLifetimeScope();
        }
    }
}

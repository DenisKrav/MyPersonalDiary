using Autofac;
using AutoMapper;
using MyPersonalDiary.BLL.Mappers;
using MyPersonalDiary.Server.Mappers;
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
                    cfg.AddProfile(new UserDTOProfile());

                    cfg.AddProfile(new AuthViewModelProfile());
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

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
            builder.RegisterType<DecryptDiaryRecordContentResolver>()
                   .AsSelf()
                   .InstancePerDependency();

            builder.RegisterType<DecryptDiaryImageDataResolver>()
                   .AsSelf()
                   .InstancePerDependency();

            builder.Register(ctx =>
            {
                var loggerFactory = ctx.Resolve<ILoggerFactory>();
                return new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile(new UserDTOProfile());
                    cfg.AddProfile(new UserViewModelProfile());

                    cfg.AddProfile(new AuthViewModelProfile());

                    cfg.AddProfile(new NoteViewModelProfile());
                    cfg.AddProfile(new NoteDTOProfile());
                }, loggerFactory);
            })
            .SingleInstance();

            builder.Register(ctx =>
            {
                var scope = ctx.Resolve<ILifetimeScope>();
                var config = ctx.Resolve<MapperConfiguration>();
                return config.CreateMapper(scope.Resolve);
            })
            .As<IMapper>()
            .InstancePerLifetimeScope();
        }
    }
}

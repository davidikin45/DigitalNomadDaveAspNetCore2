using AspNetCore.Base.Mapping;
using Autofac;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using AutoMapper.QueryableExtensions;
using System;

namespace AspNetCore.Base.DependencyInjection.Modules
{
    public class AutofacAutomapperModule : Module
    {      
        public Func<System.Reflection.Assembly, Boolean> Filter;

        protected override void Load(ContainerBuilder builder)
        {
            var config = new MapperConfiguration(cfg => {
                //https://github.com/AutoMapper/AutoMapper.Extensions.ExpressionMapping
                cfg.AddExpressionMapping();
                new AutoMapperConfiguration(cfg, Filter);
            });

            builder.RegisterInstance(config).As<MapperConfiguration>();
            builder.Register(ctx => config).As<IConfigurationProvider>();
            builder.Register(ctx => new ExpressionBuilder(config)).As<IExpressionBuilder>();
            builder.Register(c => { return config.CreateMapper(); }).As<IMapper>().SingleInstance();      
        }
    }
}

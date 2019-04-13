using Autofac;
using Microsoft.EntityFrameworkCore;
using SharpCheddar.Core;
using SharpCheddar.EntityFrameworkCore;

namespace Tests.EntityFrameworkCore.Common
{
    class TestModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            // for ef, you register your db context, which will get injected into your ef repos
            builder.RegisterType<TestDbContext>().As<DbContext>();

            // now register your repo
            builder.RegisterType<EntityFrameworkCoreRepository<EntityFrameworkCoreTests.MyModel, int>>()
                .As<IRepository<EntityFrameworkCoreTests.MyModel, int>>();
        }
    }
}

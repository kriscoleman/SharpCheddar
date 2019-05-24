using Autofac;
using SharpCheddar.Cassandra;
using SharpCheddar.Core;

namespace Tests.Cassandra.Common
{
    public class TestModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            var configurationSection = ConfigHelper.Configuration.GetSection("cassandra");

            builder.RegisterType<CassandraRepository<MyModel, int>>().As<IRepository<MyModel, int>>()
                .WithParameters(new[]
                {
                    new PositionalParameter(0, configurationSection["clusterAddress"]),
                    new PositionalParameter(1, configurationSection["keySpace"])
                });
        }
    }
}
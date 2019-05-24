using System;
using Microsoft.Extensions.Configuration;

namespace Tests
{
    internal static class ConfigHelper
    {
        static ConfigHelper()
        {
            var configurationBuilder =
                new ConfigurationBuilder();
            configurationBuilder
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            Configuration = configurationBuilder.Build();
        }

        public static IConfigurationRoot Configuration { get; private set; }
    }
}
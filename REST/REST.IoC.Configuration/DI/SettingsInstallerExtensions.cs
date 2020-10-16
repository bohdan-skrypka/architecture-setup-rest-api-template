using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace REST.IoC.Configuration.DI
{
    public static class SettingsInstallerExtensions
    {
        public static IWebHostBuilder ConfigureSettings(this IWebHostBuilder builder)
        {
            return builder.ConfigureServices((context, services) =>
            {
                var config = context.Configuration;

                //  services.Configure<ConnectionStrings>(config.GetSection("ConnectionStrings"));
                //   services.AddSingleton<ConnectionStrings>(
                //  ctx => ctx.GetService<IOptions<ConnectionStrings>>().Value)
            });
        }
    }
}

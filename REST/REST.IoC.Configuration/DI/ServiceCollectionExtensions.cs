using AutoMapper;
using REST.Services;
using REST.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using REST.API.Common.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.Options;
using REST.IoC.Configuration.Swagger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Repositories.DataContracts;
using DataAccess;
using DataAccess.Async;
using DataAccess.Sync;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;

namespace REST.IoC.Configuration.DI
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {
            if (services != null)
            {
                services.AddTransient<IUserService, UserService>();
            }
        }

        public static void ConfigureMappings(this IServiceCollection services)
        {
            if (services != null)
            {
                //Automap settings
                services.AddAutoMapper(Assembly.GetExecutingAssembly());
            }
        }

        public static void ConfigureSwagger(this IServiceCollection services, string xmlCommentsFilePath)
        {
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<SwaggerDefaultValues>();
                options.IncludeXmlComments(xmlCommentsFilePath);
            });
        }

        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(
                       o =>
                       {
                           //o.Conventions.Controller<UserController>().HasApiVersion(1, 0);
                           o.ReportApiVersions = true;
                           o.AssumeDefaultVersionWhenUnspecified = true;
                           o.DefaultApiVersion = new ApiVersion(1, 0);
                           o.ApiVersionReader = new UrlSegmentApiVersionReader();
                       }
                       );

            // note: the specified format code will format the version as "'v'major[.minor][-status]"
            services.AddVersionedApiExplorer(
            options =>
            {
                options.GroupNameFormat = "'v'VVV";

                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;
            });
        }

        public static void RegisterRepositoryWrapper(this IServiceCollection services)
        {
            //services.AddScoped<IRepositoryWrapperAsync, AsyncVersionRepositoryWrapper>();
            //services.AddScoped<IRepositoryWrapperSync, SyncVersionRepositoryWrapper>();
        }

        public static void ConfigureIpRateLimits(this IServiceCollection services, IConfiguration configuration)
        {
            // needed to load configuration from appsettings.json
            services.AddOptions();

            // needed to store rate limit counters and ip rules
            services.AddMemoryCache();

            //load general configuration from appsettings.json
            services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));

            //load ip rules from appsettings.json
            services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));

            // inject counter and rules stores
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            // Add framework services.
            services.AddMvc();

            // https://github.com/aspnet/Hosting/issues/793
            // the IHttpContextAccessor service is not registered by default.
            // the clientId/clientIp resolvers use it.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // configuration (resolvers, counter key builders)
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }
    }
}

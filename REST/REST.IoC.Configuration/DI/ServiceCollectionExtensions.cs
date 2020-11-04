using AspNetCoreRateLimit;
using AutoMapper;
using Common.Infrastructure;
using Common.Infrastructure.Caching;
using Common.Infrastructure.Enum;
using DataAccess.Async;
using EFCoreProvider;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Repositories.DataContracts;
using Repositories.DataContracts.Repo2;
using Repositories.DataContracts.Repo2.Repositories;
using REST.API.Common.Attributes;
using REST.IoC.Configuration.Swagger;
using REST.Services;
using REST.Services.Contracts;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Reflection;

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

        public static void RegisterRepositoryWrapper(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DatabaseContext>(options =>
                        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                        b => b.MigrationsAssembly(typeof(DatabaseContext).Assembly.FullName)));

            services.AddTransient<IRepositoryWrapperAsync, AsyncVersionRepositoryWrapper>();
            //  services.AddScoped<IRepositoryWrapperSync, SyncVersionRepositoryWrapper>();


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

        public static void ConfigureCachingInMemory(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CacheConfiguration>(configuration.GetSection("CacheConfiguration"));
            services.AddMemoryCache();
            services.AddTransient<InMemoryCacheService>();
            services.AddTransient<RedisCacheService>();
            services.AddTransient<Func<CacheTech, ICacheService>>(ServiceProvider => key =>
            {
                switch (key)
                {
                    case CacheTech.Redis:
                        return ServiceProvider.GetService<RedisCacheService>();
                    case CacheTech.InMemory:
                        return ServiceProvider.GetService<InMemoryCacheService>();
                    default:
                        return ServiceProvider.GetService<InMemoryCacheService>();
                }
            });
        }

        public static void ConfigureRedisCaching(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration = "localhost:6379";
            });
        }
    }
}

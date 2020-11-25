using AspNetCore.MaxConcurrentRequests.Middlewares;
using AspNetCoreRateLimit;
using Common.Infrastructure;
using Common.Infrastructure.Caching;
using Common.Infrastructure.Enum;
using DataAccess;
using Database.Context.DataContracts.Entities;
using EFCoreProvider;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Repositories.DataContracts;
using Repositories.DataContracts.Repo2;
using Repositories.DataContracts.Repo2.Repositories;
using REST.API.Common.Middlewares;
using REST.API.Common.Settings;
using REST.API.CQRS.Queries;
using REST.IoC.Configuration.DI;
using REST.IoC.Configuration.Filters;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Reflection;

#pragma warning disable CS1591
namespace REST.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostingEnvironment { get; private set; }

        private IConfigurationSection _appsettingsConfigurationSection;
        private AppSettings _appSettings;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            HostingEnvironment = env;
            Configuration = configuration;

            _appsettingsConfigurationSection = Configuration.GetSection(nameof(AppSettings));
            if (_appsettingsConfigurationSection == null)
                throw new NoNullAllowedException("No appsettings has been found");

            _appSettings = _appsettingsConfigurationSection.Get<AppSettings>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(logger => logger.AddSeq());

            if (!HostingEnvironment.IsDevelopment())
            {
                services.AddHsts(options =>
                {
                    options.Preload = true;
                    options.IncludeSubDomains = true;
                    options.MaxAge = TimeSpan.FromDays(60);
                    options.ExcludedHosts.Add("example.com");
                    options.ExcludedHosts.Add("www.example.com");
                });

                var isPortOk = int.TryParse(Configuration.GetSection("https_port").Value, out int httpsPort);
                if (!isPortOk)
                {
                    throw new Exception($"{nameof(isPortOk)} : {isPortOk}");
                }

                services.AddHttpsRedirection(opt =>
                {
                    opt.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                    opt.HttpsPort = httpsPort;
                });
            }

            services.AddControllers(opt => opt.Filters.Add<LogRequestTimeFilterAttribute>()).AddNewtonsoftJson();

            services.AddOptions();

            services.AddMemoryCache();
            services.ConfigureIpRateLimits(Configuration);

            //services.AddDbContext<DatabaseContext>(opt => opt.UseSqlServer(connectString));
            var connectString = Configuration.GetConnectionString("DefaultConnection");
            services.AddHangfire(x => x.UseSqlServerStorage(connectString));

            services.AddHangfireServer();

            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);

            #region Repositories
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient<IOwnerRepositoryCache, OwnerRepositoryCache>();
            #endregion

            services.RegisterRequestsValidationRules();
            services.AddTransient<IRequestHandler<FindAllOwnersQuery, List<Owner>>, FindOwnersHandler>();


            services.ConfigureCachingInMemory(Configuration);

            try
            {
                if (_appSettings.IsValid())
                {
                    services.Configure<AppSettings>(_appsettingsConfigurationSection);

                    services.AddControllers(
                        opt =>
                        {
                            //Custom filters, if needed
                            //opt.Filters.Add(typeof(CustomFilterAttribute));
                            opt.Filters.Add(new ProducesAttribute("application/json"));
                        }
                        ).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

                    //API versioning
                    services.ConfigureVersioning();

                    //SWAGGER
                    if (_appSettings.Swagger.Enabled)
                    {
                        services.ConfigureSwagger(XmlCommentsFilePath);
                    }

                    //Mappings
                    services.ConfigureMappings();

                    //Business settings            
                    services.ConfigureBusinessServices(Configuration);
                }
                else
                {
                }

                // internal logic DIs
                services.RegisterRepositoryWrapper(Configuration);
            }
            catch (Exception ex)
            {
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            try
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    // This will make the HTTP requests log as rich logs instead of plain text.
                    app.UseSerilogRequestLogging();

                    //Both alternatives are usable for general error handling:
                    // - middleware
                    // - UseExceptionHandler()

                    // app.UseMiddleware(typeof(ErrorHandlingMiddleware));

                    app.UseExceptionHandler(a => a.Run(async context =>
                    {
                        var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                        var exception = feature.Error;
                        var code = HttpStatusCode.InternalServerError;

                        if (exception is ArgumentNullException) code = HttpStatusCode.BadRequest;
                        else if (exception is ArgumentException) code = HttpStatusCode.BadRequest;
                        else if (exception is UnauthorizedAccessException) code = HttpStatusCode.Unauthorized;

                        // _logger.LogError($"GLOBAL ERROR HANDLER::HTTP:{code}::{exception.Message}");

                        //Known issue for now in System.Text.Json
                        //var result = JsonSerializer.Serialize<Exception>(exception, new JsonSerializerOptions { WriteIndented = true });

                        //Newtonsoft.Json serializer (should be replaced once the known issue in System.Text.Json will be solved)
                        var result = JsonConvert.SerializeObject(exception, Formatting.Indented);

                        context.Response.Clear();
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(result);
                    }));
                    app.UseHsts();
                }

                app.UseHttpsRedirection();

                app.UseMaxConcurrentRequests();
                app.UseIpRateLimiting();

                app.UseRouting();
                app.UseAuthorization();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

                app.UseRequestLocalization();

                //SWAGGER
                if (_appSettings.IsValid())
                {
                    if (_appSettings.Swagger.Enabled)
                    {
                        app.SwaggerUIConfig(provider);
                    }
                }

                app.UseHangfireDashboard("/jobs");
            }
            catch (Exception ex)
            {
            }
        }

        string XmlCommentsFilePath
        {
            get
            {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }
        }
    }
}

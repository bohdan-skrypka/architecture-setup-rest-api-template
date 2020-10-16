using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using AspNetCore.MaxConcurrentRequests.Middlewares;
using System;

namespace AspNetCore.MaxConcurrentRequests
{
    public class Star
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.Use(async (context, next) =>
            {
                await next.Invoke();
                await context.Response.WriteAsync("<div> Hello World from the middleware 1 </div>");
                await context.Response.WriteAsync("<div> Returning from the middleware 1 </div>");
            });

            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("<div> Hello World from the middleware 2 </div>");
                await next.Invoke();
                await context.Response.WriteAsync("<div> Returning from the middleware 2 </div>");
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("<div> Hello World from the middleware 3 </div>");
            });
            // loggerFactory.AddConsole();
            //Random random = new Random();

           // app.UseMaxConcurrentRequests()
            //    .Run(async (context) =>
            //    {
            //        //await Task.Delay(random.Next(300, 400));

            //        //await context.Response.WriteAsync("-- AspNetCore.MaxConcurrentConnections --");
            //    });
        }
    }
}

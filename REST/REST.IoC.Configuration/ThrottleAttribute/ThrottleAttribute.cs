using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Net;

namespace REST.IoC.Configuration.ThrottleAttribute
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ThrottleAttribute : ActionFilterAttribute
    {
        public string Name { get; set; }
        public int Seconds { get; set; }
        public string Message { get; set; }

        private static MemoryCache Cache { get; } = new MemoryCache(new MemoryCacheOptions());

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var key = string.Concat(Name, "-", context.HttpContext.Request.HttpContext.Connection.RemoteIpAddress);

            if (!Cache.TryGetValue(key, out bool entry))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(Seconds));

                Cache.Set(key, true, cacheEntryOptions);
            }
            else
            {
                if (string.IsNullOrEmpty(Message))
                {
                    Message = "You may only perform this action every {n} seconds.";
                }

                context.Result = new ContentResult { Content = Message.Replace("{n}", Seconds.ToString()) };
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            }
        }
    }
}

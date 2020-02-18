using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace TodoApi.Attributes
{   
    [AttributeUsage(AttributeTargets.Method)]

    //Action filters : Their execution surrounds the execution of action methods.  
    public class RequestRateLimitAttribute : ActionFilterAttribute
    {
        public string Name{get; set;}
        public int Seconds{get; set;}

        private static MemoryCache Cache { get; } = new MemoryCache(new MemoryCacheOptions());
        int RequestCounter = 0;
    
        //OnActionExecuting method : Called before the action executes, after model binding is complete.
        public override void OnActionExecuting(ActionExecutingContext context)
        {            
            var ipAddress = context.HttpContext.Request.HttpContext.Connection.RemoteIpAddress;
            var MemoryCacheKey = $"{Name}--{ipAddress}";
           
            //Gets the item associated with this key if present.
            if(!Cache.TryGetValue(MemoryCacheKey, out bool entry))
            {    
                //Reset counter
                RequestCounter = 0;
                
                //set and entry option with expiration time on memory cache 
                var CacheEntryOption = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(Seconds));
                Cache.Set(MemoryCacheKey, true, CacheEntryOption);

                //Add first request 
                RequestCounter++;
            }
            else
            {
                RequestCounter++;
            }

            if( RequestCounter > 5 ) 
            {
                context.Result = new ContentResult
                {
                    Content = $"Requests are limited to 5, every {Seconds} seconds.",
                };

                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                
            }
        }
}
}
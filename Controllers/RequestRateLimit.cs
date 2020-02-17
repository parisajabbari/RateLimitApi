using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace TodoApi.Controllers
{
    
    [AttributeUsage(AttributeTargets.Method)]

    //Action filters : Their execution surrounds the execution of action methods.
    //The framework provides an abstract ActionFilterAttribute that can be subclassed.
    //https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?view=aspnetcore-3.1#action-filters
    public class RequestRateLimitAttribute : ActionFilterAttribute
    {
        public string Name{get; set;}
        public int Seconds{get; set;}

        private static MemoryCache Cache { get; } = new MemoryCache(new MemoryCacheOptions());     
    

        //OnActionExecuting method : Called before the action executes, after model binding is complete.
        //https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.filters.iactionfilter.onactionexecuting?view=aspnetcore-3.1#Microsoft_AspNetCore_Mvc_Filters_IActionFilter_OnActionExecuting_Microsoft_AspNetCore_Mvc_Filters_ActionExecutingContext_
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var ipAddress = context.HttpContext.Request.HttpContext.Connection.RemoteIpAddress;
            var MemoryCacheKey = $"{Name}--{ipAddress}";

            //Gets the item associated with this key if present.
            if(!Cache.TryGetValue(MemoryCacheKey, out bool entry))
            {
                //set and entry option with expiration time on memory cache 
                var CacheEntryOption = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(Seconds));
                Cache.Set(MemoryCacheKey, true, CacheEntryOption);           
            }
            else
            {
                context.Result = new ContentResult
                {
                    Content = $"Requests are limited to 1, every {Seconds} seconds.",
                };

                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            }

        }
    }
}
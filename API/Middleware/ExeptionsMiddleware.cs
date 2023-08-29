using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.errors;

namespace API.Middleware
{
    public class ExeptionsMiddleware
    {
        readonly RequestDelegate _Next;
        readonly ILogger<ExeptionsMiddleware> _Logger;
        readonly IHostEnvironment _Env;
        public ExeptionsMiddleware(RequestDelegate next, ILogger<ExeptionsMiddleware> logger, IHostEnvironment env)
        {
            _Next = next;
            _Logger = logger;
            _Env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _Next(context);
            }
            catch (System.Exception ex)
            {
                
                _Logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = _Env.IsDevelopment() ? new ApiGloabExeptions( context.Response.StatusCode,ex.Message,ex.StackTrace.ToString())
                : new ApiGloabExeptions(context.Response.StatusCode,"Iternal Server Error");

                var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

                var json = JsonSerializer.Serialize(response,options);

                await context.Response.WriteAsync(json);
            }
        }

    }
}
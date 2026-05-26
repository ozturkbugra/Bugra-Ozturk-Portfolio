using BugraOzturkPortfolio.Business.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace BugraOzturkPortfolio.Web.Middlewares
{
    public class VisitorTrackerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        public VisitorTrackerMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            string path = context.Request.Path.Value ?? "";

            if (!path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase) &&
                !path.Contains(".") &&
                !path.StartsWith("/uploads", StringComparison.OrdinalIgnoreCase))
            {
                string ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
                string userAgent = context.Request.Headers["User-Agent"].ToString() ?? "Unknown";

                _ = Task.Run(async () =>
                {
                    try
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var scopedVisitorService = scope.ServiceProvider.GetRequiredService<IVisitorLogService>();
                            await scopedVisitorService.LogVisitAsync(ipAddress, userAgent);
                        }
                    }
                    catch
                    {
                    }
                });
            }
        }
    }
}
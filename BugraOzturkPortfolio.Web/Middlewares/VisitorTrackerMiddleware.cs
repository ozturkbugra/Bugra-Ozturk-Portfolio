using BugraOzturkPortfolio.Business.Abstract;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BugraOzturkPortfolio.Web.Middlewares
{
    public class VisitorTrackerMiddleware
    {
        private readonly RequestDelegate _next;

        public VisitorTrackerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IVisitorLogService visitorLogService)
        {
            string path = context.Request.Path.Value ?? "";

            if (!path.StartsWith("/Admin") &&
                !path.Contains(".") &&
                !path.StartsWith("/uploads"))
            {
                string ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
                string userAgent = context.Request.Headers["User-Agent"].ToString() ?? "Unknown";

                _ = Task.Run(() => visitorLogService.LogVisitAsync(ipAddress, userAgent));
            }

            await _next(context);
        }
    }
}
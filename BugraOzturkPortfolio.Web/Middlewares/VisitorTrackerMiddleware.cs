using BugraOzturkPortfolio.Business.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection; // IServiceScopeFactory için ekledik
using System.Threading.Tasks;

namespace BugraOzturkPortfolio.Web.Middlewares
{
    public class VisitorTrackerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory; // Scope fabrikasını enjekte ediyoruz

        public VisitorTrackerMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string path = context.Request.Path.Value ?? "";

            if (!path.StartsWith("/Admin") &&
                !path.Contains(".") &&
                !path.StartsWith("/uploads"))
            {
                string ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
                string userAgent = context.Request.Headers["User-Agent"].ToString() ?? "Unknown";

                // Arka plan thread'i için tamamen bağımsız, izole bir yaşam alanı (Scope) açıyoruz aga!
                _ = Task.Run(async () =>
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        // Servisi bu izole scope içinden çağırarak yeni bir DbContext örneğiyle çalıştırıyoruz
                        var scopedVisitorService = scope.ServiceProvider.GetRequiredService<IVisitorLogService>();
                        await scopedVisitorService.LogVisitAsync(ipAddress, userAgent);
                    }
                });
            }

            await _next(context);
        }
    }
}
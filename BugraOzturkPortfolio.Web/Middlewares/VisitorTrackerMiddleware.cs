using BugraOzturkPortfolio.Business.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
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
            // 1. Önce isteğin önünü açıyoruz, kullanıcı bekletilmeden sayfaya gitsin
            await _next(context);

            // 2. Sayfa yüklendikten sonra arkadan sessizce loglama operasyonunu başlatıyoruz
            string path = context.Request.Path.Value ?? "";

            if (!path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase) &&
                !path.Contains(".") &&
                !path.StartsWith("/uploads", StringComparison.OrdinalIgnoreCase))
            {
                string ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
                string userAgent = context.Request.Headers["User-Agent"].ToString() ?? "Unknown";

                try
                {
                    // Task.Run tuzağından kurtulduk, güvenli scope içinde direkt await ediyoruz
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var scopedVisitorService = scope.ServiceProvider.GetRequiredService<IVisitorLogService>();
                        await scopedVisitorService.LogVisitAsync(ipAddress, userAgent);
                    }
                }
                catch
                {
                    // Loglama sırasında bir hata olursa (DB yoğunluğu vs.) ana siteyi asla etkilemesin kalkanı
                }
            }
        }
    }
}
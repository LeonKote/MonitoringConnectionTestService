using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MonitoringConnectionLib.Interfaces;
using MonitoringConnectionLib.Prometheus;
using Prometheus;
using Serilog;

namespace MonitoringConnectionLib
{
    /// <summary>
    /// Расширения для DI / упрощённой регистрации.
    /// </summary>
    public static class MonitoringExtensions
    {
        /// <summary>
        /// Регистрация реестра метрик в контейнере DI.
        /// Затем из сервисов/контроллеров можно inject-ить IMetricsRegistry.
        /// </summary>
        public static WebApplicationBuilder AddMonitoringMetrics(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IMetricsRegistry, PrometheusMetricsRegistry>();
            return builder;
        }

        public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .Enrich.WithMachineName()
                .WriteTo.Console()
                .WriteTo.Http("http://logstash:8080", null)
                .CreateLogger();

            builder.Host.UseSerilog();
            return builder;
        }

        public static WebApplication UseMonitoringEndpoints(this WebApplication app)
        {
            app.UseHttpMetrics();
            app.MapMetrics();
            return app;
        }
    }
}

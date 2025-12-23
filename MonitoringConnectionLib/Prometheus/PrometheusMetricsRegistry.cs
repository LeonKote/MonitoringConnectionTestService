using MonitoringConnectionLib.Interfaces;
using Prometheus;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using ICounter = MonitoringConnectionLib.Interfaces.ICounter;

namespace MonitoringConnectionLib.Prometheus
{
    /// <summary>
    /// Реестр метрик, кеширует созданные метрики и использует Metrics.CreateCounter.
    /// </summary>
    public class PrometheusMetricsRegistry : IMetricsRegistry
    {
        // Ключ — санитизированное имя метрики
        private readonly ConcurrentDictionary<string, ICounter> _counters = new();

        // Regex для разрешённых символов в имени метрики (заменяем запрещённые на '_')
        private static readonly Regex _invalidNameChars = new(@"[^a-zA-Z0-9_:]", RegexOptions.Compiled);

        public ICounter GetOrCreateCounter(string name, string help, params string[] labelNames)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Metric name required", nameof(name));
            var sanitized = SanitizeName(name);

            // double-check help
            help ??= string.Empty;

            return _counters.GetOrAdd(sanitized, _ =>
            {
                var config = new CounterConfiguration
                {
                    LabelNames = labelNames?.Length > 0 ? labelNames : null
                };

                // Создаёт и регистрирует метрику в глобальном реестре
                var counter = Metrics.CreateCounter(sanitized, help, config);
                return new PrometheusCounter(counter, labelNames);
            });
        }

        public ICounter? TryGetCounter(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            var sanitized = SanitizeName(name);
            return _counters.TryGetValue(sanitized, out var c) ? c : null;
        }

        private static string SanitizeName(string name)
        {
            // Prometheus metric name must match [a-zA-Z_:][a-zA-Z0-9_:]*
            // Простая санитизация: заменить недопустимые символы на '_', если имя начинается с неразрешённого — префикс.
            var s = _invalidNameChars.Replace(name, "_");
            if (!Regex.IsMatch(s, @"^[a-zA-Z_:]")) s = $"m_{s}";
            return s;
        }
    }
}

using Prometheus;
using ICounter = MonitoringConnectionLib.Interfaces.ICounter;

namespace MonitoringConnectionLib.Prometheus;

/// <summary>
/// Реализация ICounter, обёртка над Prometheus.Counter.
/// </summary>
internal class PrometheusCounter : ICounter
{
    private readonly Counter _counter;
    private readonly string[] _labelNames;

    public int LabelCount => _labelNames?.Length ?? 0;

    public PrometheusCounter(Counter counter, string[]? labelNames)
    {
        _counter = counter ?? throw new ArgumentNullException(nameof(counter));
        _labelNames = labelNames ?? Array.Empty<string>();
    }

    public void Inc(double value = 1.0, params string[] labelValues)
    {
        if ((_labelNames?.Length ?? 0) > 0)
        {
            if (labelValues == null || labelValues.Length != _labelNames!.Length)
                throw new ArgumentException($"Metric expects {_labelNames!.Length} label values, got {labelValues?.Length ?? 0}.");

            _counter.WithLabels(labelValues).Inc(value);
        }
        else
        {
            // no labels
            _counter.Inc(value);
        }
    }
}

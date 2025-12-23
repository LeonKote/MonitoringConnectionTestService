namespace MonitoringConnectionLib.Interfaces;

/// <summary>
/// Абстракция счётчика (не заставляет пользователей зависеть напрямую от Prometheus types).
/// Поддерживает инкремент с/без значений label'ов.
/// </summary>
public interface ICounter
{
    /// <summary>
    /// Увеличить счётчик на 1 (или на value).
    /// Если метрика имеет label'ы — нужно передать значения label'ов в same order as declared.
    /// </summary>
    void Inc(double value = 1.0, params string[] labelValues);

    /// <summary>
    /// Прочитать количество label'ов, ожидаемых этой метрикой.
    /// </summary>
    int LabelCount { get; }
}

namespace MonitoringConnectionLib.Interfaces;

/// <summary>
/// Регистратор/реестр метрик. Позволяет получить/создать счётчики централизованно.
/// </summary>
public interface IMetricsRegistry
{
    /// <summary>
    /// Зарегистрировать новый счётчик или вернуть существующий.
    /// </summary>
    ICounter GetOrCreateCounter(string name, string help, params string[] labelNames);

    /// <summary>
    /// Попытаться получить уже существующий счётчик (без создания).
    /// Возвращает null если не найден.
    /// </summary>
    ICounter? TryGetCounter(string name);
}

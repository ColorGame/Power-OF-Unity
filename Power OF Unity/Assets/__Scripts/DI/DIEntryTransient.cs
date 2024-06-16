using System;

/// <summary>
/// Временная фабрика. При запросе, каждый раз создает новый экземпляр.
/// </summary>
/// <remarks>
/// Наследует DIEntry.
/// Фабрика не создает экземпляры мгновенно, а только, когда вы запрашиваете классы, вызывая Resolve().
/// </remarks>

public sealed class DIEntryTransient<T> : DIEntry<T>
{
    public DIEntryTransient(DIContainer diContainer, Func<DIContainer, T> factory) : base(diContainer, factory) { }

    public override T Resolve()
    {
        return Factory(DiContainer); // Выполняет переданный делегат и возвращает тип Т
    }
}

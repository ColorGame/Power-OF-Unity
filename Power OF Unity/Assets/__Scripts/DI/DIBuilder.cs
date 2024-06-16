/// <summary>
/// Строитель -создает фабрики по конфигурации.
/// </summary>
public sealed class DIBuilder<T> // sealed запрещает другим классам наследовать от этого класса
{
    private readonly DIEntry<T> _entry;

    public DIBuilder(DIEntry<T> entry)
    {
        _entry = entry;
    }

    /// <summary>
    /// Принудительное создание фабрики.(что бы использовать полученный экземпляр дальше в коде)
    /// </summary>   
    public T CreateInstance()
    {
        return _entry.Resolve();
    }
}

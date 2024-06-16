
/// <summary>
/// Интерфейс ТОЧКА ВХОДА, для внедрения зависимостей 
/// Всем реализующим объектам поставить в инспекторе ТЕГ - "EntryPoint"
/// </summary>
public interface IEntryPoint
{
    /// <summary>
    /// Согласно дизайну выполняется после Awake() и OnEnable()
    /// </summary>   
    void Process(DIContainer container);
}


/// <summary>
/// Интерфейс ТОЧКА ВХОДА, для внедрения зависимостей 
/// Всем реализующим объектам поставить в инспекторе ТЕГ - "EntryPoint"
/// </summary>
public interface IEntryPoint
{
    /// <summary>
    /// Внедрение зависимости. Согласно дизайну выполняется после Awake() и OnEnable()
    /// </summary>   
    void Inject(DIContainer container);  
}

using System;
using System.Collections.Generic;

/// <summary>
/// Контейнер для внедрения зависимостей.
/// Контейнер должен использоваться только в EntryPoint и не передаваться в классы
/// </summary>
/// <remarks>
/// Все зависимости (имеется в виду фабрики) регистрируются в DIContainer. Контейнеры могут наследовать один от другого с помощью composition,
/// это означает, что вы можете создать дерево зависимостей, в котором дочерние контейнеры знают о родительском и могут запрашивать экземпляры у него, 
/// но родительские контейнеры не знают о дочерних. Удобно создавать один контейнер для всей игры, который работает как контекст проекта, 
/// и разные контейнеры для каждой сцены вашей игры. Когда вы уничтожаете сцену, вы уничтожаете ее содержимое, поэтому все зависимости этой сцены тоже уничтожаются.
/// </remarks>
public sealed class DIContainer // https://github.com/vavilichev/Lukomor
{
    // readonly указывает на то, что присвоение значения полю может происходить только при объявлении или в конструкторе этого класса.//readonly не позволяет вам изменять сам объект-список, а не его внутренний массив.
    private readonly Dictionary<(string, Type), DIEntry> _factoriesDic = new(); //Словарь ФАБРИК. Ключ - это тег(если создается несколько Singleton) и тип, Значение - фабрика
    private readonly HashSet<(string, Type)> _cachedKeysForResolving = new(); // Кэшированные Ключи Для Разрешения циклических зависимостей

    private readonly DIContainer _parentDiContainer;

    public DIContainer(DIContainer parentDiContainer = null)
    {
        _parentDiContainer = parentDiContainer;
    }


    /// <summary>
    /// Зарегистрировать фабрику, которая производит singleton (не статический, конечно, просто экземпляр с флагом) и кэширует его, а также использует кэшированное значение при каждом запуске Resolve().
    /// Регистрация не создает экземпляры мгновенно. Контейнер создает экземпляры, когда вы запрашиваете классы, вызывая Resolve().
    /// Вы можете создать экземпляр с регистрацией, используя метод CreateInstance() в классе DIBuilder. Каждая регистрация возвращает экземпляр DIBuilder, поэтому вы можете сделать это с регистрацией.
    /// </summary>
    /// <remarks>
    /// DIContainer хранит фабрики (factories), поэтому вам необходимо зарегистрировать фабрики в Словарь ФАБРИК(_factoriesDic). Factory - это делегат, который получает ссылку на экземпляр DIContainer 
    /// (вы можете получить другие экземпляры из этого контейнера) и возвращает экземпляр запрошенного типа.
    /// </remarks>
    public DIBuilder<T> RegisterSingleton<T>(Func<DIContainer, T> factory)
    {
        return RegisterSingleton("", factory); // с пустым тегом
    }

    /// <summary>
    /// С помощью системы тегов можно создать уникальные экземпляры одного и того же типа для различных нужд 
    /// </summary>    
    public DIBuilder<T> RegisterSingleton<T>(string tag, Func<DIContainer, T> factory)
    {
        var key = (tag, typeof(T));

        return RegisterSingleton(key, factory);
    }

    /// <summary>
    /// Зарегистрировать фабрику(factory) для многократного Resolve, чтобы получать новый экземпляр каждый раз, когда вы выполняете Resolve().
    /// </summary>
    /// <remarks>
    /// </remarks>    
    public DIBuilder<T> Register<T>(Func<DIContainer, T> factory)
    {
        return Register("", factory);
    }

    /// <summary>
    /// Создание временных вариантов с тегами
    /// </summary>   
    public DIBuilder<T> Register<T>(string tag, Func<DIContainer, T> factory)
    {
        var key = (tag, typeof(T));

        return Register(key, factory);
    }

    public T Resolve<T>(string tag = "")
    {
        var type = typeof(T);
        var key = (tag, type);

        if (_cachedKeysForResolving.Contains(key))
        {
            throw new Exception($"Cyclic dependencies. Key: {key}");
        }

        _cachedKeysForResolving.Add(key);

        T result;

        if (!_factoriesDic.ContainsKey(key))
        {
            if (_parentDiContainer == null)
            {
                throw new Exception($"There is no factory registered for key: {key}");
            }

            result = _parentDiContainer.Resolve<T>(tag);
        }
        else
        {
            result = _factoriesDic[key].Resolve<T>();
        }

        _cachedKeysForResolving.Remove(key);

        return result;
    }

    private DIBuilder<T> RegisterSingleton<T>((string, Type) key, Func<DIContainer, T> factory)
    {
        if (_factoriesDic.ContainsKey(key))
        {
            throw new Exception("Already has factory entry for key: " + key);
        }

        var diEntry = new DIEntrySingleton<T>(this, factory);

        _factoriesDic[key] = diEntry;

        return new DIBuilder<T>(diEntry);
    }

    private DIBuilder<T> Register<T>((string, Type) key, Func<DIContainer, T> factory)
    {
        if (_factoriesDic.ContainsKey(key))
        {
            throw new Exception("Already has factory entry for type: " + key.Item2.Name);
        }

        var diEntry = new DIEntryTransient<T>(this, factory);

        _factoriesDic[key] = diEntry;

        return new DIBuilder<T>(diEntry);
    }
}

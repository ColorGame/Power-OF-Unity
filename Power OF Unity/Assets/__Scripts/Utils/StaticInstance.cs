using UnityEngine;

/// <summary>
/// Статический экземпляр похож на синглтон, но вместо уничтожения любых новых
/// экземпляров он переопределяет текущий экземпляр. Это удобно для сброса состояния
/// и избавляет вас от необходимости делать это вручную
/// </summary>
public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    protected virtual void Awake() => Instance = this as T;

    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
}

/// <summary>
/// Это преобразует статический экземпляр в базовый синглтон. Это уничтожит все созданные новые
/// версии, оставив исходный экземпляр нетронутым
/// </summary>
public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Там больше, чем один Singleton" + transform + " - " + Instance);
            Destroy(gameObject);
        }
        base.Awake(); //Выполним присваивание в базовом методе
    }
}

/// <summary>
/// Наконец, у нас есть постоянная версия singleton. Это сохранится при загрузке scene.
/// Идеально подходит для системных классов, которым требуются постоянные данные с сохранением состояния. 
/// Или аудиоисточников, где музыка воспроизводится через загрузочные экраны и т.д
/// </summary>
public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}


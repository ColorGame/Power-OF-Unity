using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Начальный загрузчик
/// </summary>
/// <remarks>
/// До Awake() на загруженной сцене создает PersistentEntryPoint синголтон с основными системами
/// </remarks>
public class Bootstrapper
{
    private static Bootstrapper _instance;
    private DIContainer _rootContainer;
    /* private Coroutines _coroutines;
     private UIRootView _uiRoot;*/
    private ScenesService _scenesService;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]//Используйте этот атрибут для получения обратного вызова при запуске среды выполнения и загрузке первой сцены.
                                                                              //Используйте различные параметры для RuntimeInitializeLoadType, чтобы контролировать, когда метод вызывается в последовательности запуска.
                                                                              //BeforeSceneLoad. Здесь загружаются объекты сцены, но функция Awake() еще не была вызвана. Здесь все объекты считаются неактивными.                                                                         
                                                                              //AfterSceneLoad происходит после Awake() и OnEnable(). Здесь объекты сцены считаются полностью загруженными и настроенными. Активные объекты можно найти с помощью FindObjectsByType .
    private static void AutoStartGame()
    {
        _instance = new Bootstrapper();
        _instance.RunGame();
    }

    private void RunGame() // Запуск игры
    {
        if (!System.Enum.TryParse(SceneManager.GetActiveScene().name, out SceneName activeSceneName)) // Попробуем преобразовать имя текущей сцены в Enum SceneName
        {
            return; // Если не удалость преобразовать то сцена НЕ ЗАРЕГЕСТРИРОВАННА в проекте-Выходим и ничего не делаем.           
        }

        // Здесь можно установить БАЗОВЫЕ НАСТРОЙКИ
        //Application.targetFrameRate = 60; // Установим для мобилок FPS = 60 т.к по умолчанию 30
        //Screen.sleepTimeout = SleepTimeout.NeverSleep; // отключим сон у телефона

        /* _coroutines = new GameObject("[COROUTINE]").AddComponent<Coroutines>(); // Создадим объект и прикрепим скрипт
         Object.DontDestroyOnLoad(_coroutines.gameObject);

         var prefabUIRoot = Resources.Load<UIRootView>("UIRoot"); // Создадим UI 
         _uiRoot = Object.Instantiate(prefabUIRoot);
         Object.DontDestroyOnLoad(_uiRoot.gameObject);*/

        SceneManager.sceneLoaded += SceneManager_OnSceneLoaded; // Подпишемся СЦЕНА ЗАНРУЖЕНА для настройки EntryPoint при переходе на другую сцену. SceneManager.sceneLoaded вызывается после Awake() и OnEnable() объектов.

        _rootContainer = new DIContainer();
        _scenesService = _rootContainer.RegisterSingleton(_ => new ScenesService()).CreateInstance();

        var PersistentEntryPoint = Object.Instantiate(Resources.Load<PersistentEntryPoint>("PersistentEntryPoint"));// Создать префаб PersistentEntryPoint     
        Object.DontDestroyOnLoad(PersistentEntryPoint.gameObject);
        PersistentEntryPoint.Process(_rootContainer);

        if (activeSceneName == SceneName.Bootstrap)
        {
            _scenesService.Load(SceneName.MainMenu); // Это строка запуститься только ели мы стартовали со сцены Bootstrap           
        }       
    }

    private void SceneManager_OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (System.Enum.TryParse(SceneManager.GetActiveScene().name, out SceneName activeSceneName)) // Попробуем преобразовать имя текущей сцены в Enum SceneName
        {      
            if (activeSceneName != SceneName.Bootstrap)
            {
                FindEntryPoint(activeSceneName);
            }
            else
            {
                return; // Если это Bootstrap то выходим
            }
        }
        else
        {
            _scenesService.Load(SceneName.MainMenu); // Если сцена не зарегестрированна в проекте(не удалость преобразовать) -перейдем в главное меню (это когда с зарегестр. сцены перехожу на незарегестр. сцену)
        }
    }

    private void FindEntryPoint(SceneName sceneName)
    {        
        GameObject gameObject = GameObject.FindWithTag("EntryPoint");
        IEntryPoint entryPoint = gameObject.GetComponent<IEntryPoint>();     
        var childContainer = new DIContainer(_rootContainer); // Создадим дочерний контейнер и передадим корневой.
        entryPoint.Process(childContainer);// Передадим созданный контейнер в точку входа преданной нам сцены         
    }
}
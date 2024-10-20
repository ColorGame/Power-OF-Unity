using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Начальный загрузчик
/// </summary>
/// <remarks>
/// При воспроизведении подгружает дополнительно(Additive) сцену Startup 
/// </remarks>
public class Bootstrapper
{
    private static Bootstrapper _instance;
    private DIContainer _rootContainer;

    //private Coroutines _coroutines;
    //private UIRootView _uiRoot;
    //private ScenesService _scenesService;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]//Используйте этот атрибут для получения обратного вызова при запуске среды выполнения и загрузке первой сцены.
                                                                                    //Используйте различные параметры для RuntimeInitializeLoadType, чтобы контролировать, когда метод вызывается в последовательности запуска.
                                                                                    //AfterAssembliesLoaded. Обратный вызов вызывается, когда все сборки загружены и предварительно загруженные ресурсы инициализированы. На этом этапе объект первой сцены ещё не загружен.                                                                                    
                                                                                    //BeforeSceneLoad. Здесь загружаются объекты сцены, но функция Awake() еще не была вызвана. Здесь все объекты считаются неактивными.                                                                         
                                                                                    //AfterSceneLoad происходит после Awake() и OnEnable(). Здесь объекты сцены считаются полностью загруженными и настроенными. Активные объекты можно найти с помощью FindObjectsByType .
    private static void AutoStartGame()
    {
        _instance = new Bootstrapper();
        _instance.RunGame().Forget();
    }

    private async UniTask RunGame() // Запуск игры
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (!Enum.TryParse(activeScene.name, out SceneName activeSceneName)) // Попробуем преобразовать имя текущей сцены в Enum SceneName
        {
            return; // Если не удалость преобразовать то сцена НЕ ЗАРЕГЕСТРИРОВАННА в проекте-Выходим и ничего не делаем.           
        }

        // Здесь можно установить БАЗОВЫЕ НАСТРОЙКИ
        //Application.targetFrameRate = 60; // Установим для мобилок FPS = 60 т.к по умолчанию 30
        //Screen.sleepTimeout = SleepTimeout.NeverSleep; // отключим сон у телефона
            
        _rootContainer = new DIContainer();

        //Создаем экземпляр LoadingScreen и регистрируем в DI контейнере
        var LoadingScreenProvider = _rootContainer.RegisterSingleton(c => new LoadingScreenProvider()).CreateInstance();
        await LoadingScreenProvider.Load();       

        //Загружаю Неразрушаемую точку входа
        var persistentEntryPointAsset = new LocalAssetLoader();
        IEntryPoint persistentEntryPoint = await persistentEntryPointAsset.Load<IEntryPoint>(AssetsConstants.PersistentEntryPoint);
        persistentEntryPoint.Inject(_rootContainer);

        //После загрузки активной сцены, инициализирую ее точку входа.
        while (activeScene.isLoaded == false)
        {
            await UniTask.Yield();
        }

        //Выгружаю экран загрузки
        await UniTask.Delay(TimeSpan.FromSeconds(3f));
        LoadingScreenProvider.Unload();

        FindEntryPointAndInject(activeScene);

        SceneManager.sceneLoaded += SceneManager_OnSceneLoaded; // Подпишемся СЦЕНА ЗАГРУЖЕНА для настройки EntryPoint при переходе на другую сцену. SceneManager.sceneLoaded вызывается после Awake() и OnEnable() объектов (!!!бывает сбои запкскается позже Start().)
                
    }
    private void SceneManager_OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindEntryPointAndInject(scene);
    }
    
    /// <summary>
    /// Поиск точки входа в сцену и внедряем зависимость
    /// </summary>
    private void FindEntryPointAndInject(Scene scene)
    {
        GameObject[] gameObjectArray = scene.GetRootGameObjects();
        foreach (GameObject gameObject in gameObjectArray)
        {
            if (gameObject.TryGetComponent(out IEntryPoint entryPoint))
            {
                var childContainer = new DIContainer(_rootContainer); // Создадим дочерний контейнер и передадим корневой.
                entryPoint.Inject(childContainer);// Передадим созданный контейнер в точку входа преданной нам сцены
                return;
            }
        }
    }
}
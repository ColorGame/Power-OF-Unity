using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Обслуживание сцен
/// </summary>
public class ScenesService
{
    public ScenesService(DIContainer rootContainer, LoadingScreenProvider loadingScreenProvider)
    {
        _rootContainer = rootContainer;
        _loadingScreenProvider = loadingScreenProvider;
    }

    private DIContainer _rootContainer;
    private LoadingScreenProvider _loadingScreenProvider;

    public string GetActiveSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
    public Scene GetScene(SceneName sceneName)
    {
        return SceneManager.GetSceneByBuildIndex((int)sceneName);
    }

    public void Load(SceneName scene)
    {
        SceneManager.LoadScene((int)scene);
    }

    public async UniTask<Scene> LoadSceneAsync(SceneName sceneName, LoadSceneMode loadSceneMode)
    {
        var scene = SceneManager.LoadSceneAsync((int)sceneName, loadSceneMode);
        while (scene.isDone == false) //Если Операция не завершена
        {
            await UniTask.Yield();// как в кроутине "yield return null". Подождите до следующего обновления, чтобы продолжить
        }
        return GetScene(sceneName);
    }

    public async UniTask UnloadScene(SceneName sceneName)
    {
        var scene = SceneManager.UnloadSceneAsync((int)sceneName);
        while (scene.isDone == false) //Если Операция не завершена
        {
            await UniTask.Yield();// как в кроутине "yield return null". Подождите до следующего обновления, чтобы продолжить
        }
    }
    /// <summary>
    /// Загрузить сцену через загрузочный экран
    /// </summary>
    /// <remarks>
    /// Сцена будет загружена в режиме Single
    /// </remarks>
    public async UniTask LoadSceneByLoadingScreen(SceneName sceneName)
    {
        await _loadingScreenProvider.Load();
        _loadingScreenProvider.DontDestroyOnLoad();

        Scene scene =  await LoadSceneAsync(sceneName, LoadSceneMode.Single);
        FindEntryPointInSceneAndInject(scene);

        await UniTask.Delay(TimeSpan.FromSeconds(2f));
        _loadingScreenProvider.Unload();
        TryFindStartSceneAndActivate(scene);
    }

    /// <summary>
    /// Поиск точки входа в активной сцену и внедряем зависимость
    /// </summary>
    public void FindEntryPointInSceneAndInject(Scene scene)
    {
        IEntryPoint entryPoint = GetComponentFromRoot<IEntryPoint>(scene);
       // IEntryPoint entryPoint = GameObject.FindGameObjectWithTag("EntryPoint").GetComponent<IEntryPoint>();
        var childContainer = new DIContainer(_rootContainer); // Создадим дочерний контейнер и передадим корневой.
        entryPoint.Inject(childContainer);// Передадим созданный контейнер в точку входа преданной нам сцены       
    }

    /// <summary>
    /// Поробуем найти старта сцены и активировать ее
    /// </summary>
    /// <remarks>
    /// Не у каждой сцены есть метод StartScene
    /// </remarks>
    public bool TryFindStartSceneAndActivate(Scene scene)
    {
        IStartScene startScene = GetComponentFromRoot<IStartScene>(scene);
        if (startScene != null)
        {
            startScene.StartScene();
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// Получить компонент типа Т из в корневых игровых объектах сцены .
    /// </summary>
    public static T GetComponentFromRoot<T>(Scene scene)
    {
        var rootObjects = scene.GetRootGameObjects();

        T result = default;
        foreach (var go in rootObjects)
        {
            if (go.TryGetComponent(out result))
            {
                break;
            }
        }

        return result;
    }
}

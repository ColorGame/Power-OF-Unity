using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
/// <summary>
/// Обслуживание сцен
/// </summary>
public class ScenesService
{   
    public ScenesService(LoadingScreenProvider loadingScreenProvider)
    {
        _loadingScreenProvider = loadingScreenProvider;
    }

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

    public async UniTask LoadSceneAsync(SceneName sceneName, LoadSceneMode loadSceneMode)
    {
        var scene = SceneManager.LoadSceneAsync((int)sceneName, loadSceneMode);
        while (scene.isDone == false) //Если Операция не завершена
        {
            await UniTask.Yield();// как в кроутине "yield return null". Подождите до следующего обновления, чтобы продолжить
        }        
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
    /// Сцена будет зашружена в режиме Single
    /// </remarks>
    public async UniTask LoadSceneByLoadingScreen(SceneName sceneName)
    {
        await _loadingScreenProvider.Load();
        _loadingScreenProvider.DontDestroyOnLoad();

        await LoadSceneAsync(sceneName,LoadSceneMode.Single);

       // await UniTask.Delay(TimeSpan.FromSeconds(2f));
        _loadingScreenProvider.Unload();

    }
}

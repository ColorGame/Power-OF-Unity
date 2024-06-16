using System.Collections;
using System.ComponentModel;
using UnityEngine.SceneManagement;
/// <summary>
/// Обслуживание сцен
/// </summary>
public class ScenesService 
{
    //private UIRootView _rootView;

   /* public void Init(UIRootView rootView)
    {
        _rootView = rootView;
    }*/

    public string GetActiveSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public void Load(SceneName scene) 
    {
        
        SceneManager.LoadScene(scene.ToString()); //Загружает сцену по ее имени или индексу в настройках сборки.
                                                  //Примечание: В большинстве случаев, чтобы избежать пауз или сбоев в производительности во время загрузки, вам следует использовать асинхронную версию этой команды, которая является: LoadSceneAsync.
        
    }

    public IEnumerator LoadAsync (SceneName scene)
    {
      //  _rootView.ShowLoadingScreen();

        // В момент когда загружена новая сцена старая еще до конца не уничтожена, и во избежании конфликтов в новую будем заходить через пустую сцену Bootstrap
       // yield return SceneManager.LoadSceneAsync(SceneName.Bootstrap.ToString()); 
        yield return SceneManager.LoadSceneAsync(scene.ToString());

        //_rootView.HideLoadingScreen();
    }
}

using System.Collections;
using UnityEngine.SceneManagement;

public static class SceneLoader //статический класс - загрузчик сцен
{
    public static void Load(SceneName scene) 
    {
        SceneManager.LoadScene(scene.ToString()); //Загружает сцену по ее имени или индексу в настройках сборки.
                                                  //Примечание: В большинстве случаев, чтобы избежать пауз или сбоев в производительности во время загрузки, вам следует использовать асинхронную версию этой команды, которая является: LoadSceneAsync.
    }

    public static IEnumerator LoadAsync (SceneName scene)
    {
        yield return SceneManager.LoadSceneAsync(scene.ToString());
    }
}

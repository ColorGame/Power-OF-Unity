using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// ��������� ���������
/// </summary>
/// <remarks>
/// ��� ��������������� ���������� �������������(Additive) ����� Startup 
/// </remarks>
public class Bootstrapper
{
    private static Bootstrapper _instance;
    private DIContainer _rootContainer;

    //private Coroutines _coroutines;
    //private UIRootView _uiRoot;
    //private ScenesService _scenesService;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]//����������� ���� ������� ��� ��������� ��������� ������ ��� ������� ����� ���������� � �������� ������ �����.
                                                                                    //����������� ��������� ��������� ��� RuntimeInitializeLoadType, ����� ��������������, ����� ����� ���������� � ������������������ �������.
                                                                                    //AfterAssembliesLoaded. �������� ����� ����������, ����� ��� ������ ��������� � �������������� ����������� ������� ����������������. �� ���� ����� ������ ������ ����� ��� �� ��������.                                                                                    
                                                                                    //BeforeSceneLoad. ����� ����������� ������� �����, �� ������� Awake() ��� �� ���� �������. ����� ��� ������� ��������� �����������.                                                                         
                                                                                    //AfterSceneLoad ���������� ����� Awake() � OnEnable(). ����� ������� ����� ��������� ��������� ������������ � ������������. �������� ������� ����� ����� � ������� FindObjectsByType .
    private static void AutoStartGame()
    {
        _instance = new Bootstrapper();
        _instance.RunGame().Forget();
    }

    private async UniTask RunGame() // ������ ����
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (!Enum.TryParse(activeScene.name, out SceneName activeSceneName)) // ��������� ������������� ��� ������� ����� � Enum SceneName
        {
            return; // ���� �� �������� ������������� �� ����� �� ����������������� � �������-������� � ������ �� ������.           
        }

        // ����� ����� ���������� ������� ���������
        //Application.targetFrameRate = 60; // ��������� ��� ������� FPS = 60 �.� �� ��������� 30
        //Screen.sleepTimeout = SleepTimeout.NeverSleep; // �������� ��� � ��������
            
        _rootContainer = new DIContainer();

        //������� ��������� LoadingScreen � ������������ � DI ����������
        var LoadingScreenProvider = _rootContainer.RegisterSingleton(c => new LoadingScreenProvider()).CreateInstance();
        await LoadingScreenProvider.Load();       

        //�������� ������������� ����� �����
        var persistentEntryPointAsset = new LocalAssetLoader();
        IEntryPoint persistentEntryPoint = await persistentEntryPointAsset.Load<IEntryPoint>(AssetsConstants.PersistentEntryPoint);
        persistentEntryPoint.Inject(_rootContainer);

        //����� �������� �������� �����, ������������� �� ����� �����.
        while (activeScene.isLoaded == false)
        {
            await UniTask.Yield();
        }

        //�������� ����� ��������
        await UniTask.Delay(TimeSpan.FromSeconds(3f));
        LoadingScreenProvider.Unload();

        FindEntryPointAndInject(activeScene);

        SceneManager.sceneLoaded += SceneManager_OnSceneLoaded; // ���������� ����� ��������� ��� ��������� EntryPoint ��� �������� �� ������ �����. SceneManager.sceneLoaded ���������� ����� Awake() � OnEnable() �������� (!!!������ ���� ����������� ����� Start().)
                
    }
    private void SceneManager_OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindEntryPointAndInject(scene);
    }
    
    /// <summary>
    /// ����� ����� ����� � ����� � �������� �����������
    /// </summary>
    private void FindEntryPointAndInject(Scene scene)
    {
        GameObject[] gameObjectArray = scene.GetRootGameObjects();
        foreach (GameObject gameObject in gameObjectArray)
        {
            if (gameObject.TryGetComponent(out IEntryPoint entryPoint))
            {
                var childContainer = new DIContainer(_rootContainer); // �������� �������� ��������� � ��������� ��������.
                entryPoint.Inject(childContainer);// ��������� ��������� ��������� � ����� ����� ��������� ��� �����
                return;
            }
        }
    }
}
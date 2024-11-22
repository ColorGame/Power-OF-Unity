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
        var loadingScreenProvider = _rootContainer.RegisterSingleton(c => new LoadingScreenProvider()).CreateInstance();
        await loadingScreenProvider.Load();

        //������� ��������� ScenesService � ������������ � DI ����������
        var scenesService = _rootContainer.RegisterSingleton(c => new ScenesService(_rootContainer,loadingScreenProvider)).CreateInstance();

        //�������� ������������� ����� �����
        var persistentEntryPointAsset = new LocalAssetLoader();
        IEntryPoint persistentEntryPoint = await persistentEntryPointAsset.Load<IEntryPoint>(AssetsConstants.PersistentEntryPoint);
        persistentEntryPoint.Inject(_rootContainer);

        //����� �������� �������� �����, ������������� �� ����� �����.
        while (activeScene.isLoaded == false)
        {
            await UniTask.Yield();
        }
        scenesService.FindEntryPointInSceneAndInject(activeScene);

        //�������� ����� ��������
        await UniTask.Delay(TimeSpan.FromSeconds(3f));
        loadingScreenProvider.Unload();        

        scenesService.TryFindStartSceneAndActivate(activeScene);
    } 
}
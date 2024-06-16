using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// ��������� ���������
/// </summary>
/// <remarks>
/// �� Awake() �� ����������� ����� ������� PersistentEntryPoint ��������� � ��������� ���������
/// </remarks>
public class Bootstrapper
{
    private static Bootstrapper _instance;
    private DIContainer _rootContainer;
    /* private Coroutines _coroutines;
     private UIRootView _uiRoot;*/
    private ScenesService _scenesService;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]//����������� ���� ������� ��� ��������� ��������� ������ ��� ������� ����� ���������� � �������� ������ �����.
                                                                              //����������� ��������� ��������� ��� RuntimeInitializeLoadType, ����� ��������������, ����� ����� ���������� � ������������������ �������.
                                                                              //BeforeSceneLoad. ����� ����������� ������� �����, �� ������� Awake() ��� �� ���� �������. ����� ��� ������� ��������� �����������.                                                                         
                                                                              //AfterSceneLoad ���������� ����� Awake() � OnEnable(). ����� ������� ����� ��������� ��������� ������������ � ������������. �������� ������� ����� ����� � ������� FindObjectsByType .
    private static void AutoStartGame()
    {
        _instance = new Bootstrapper();
        _instance.RunGame();
    }

    private void RunGame() // ������ ����
    {
        if (!System.Enum.TryParse(SceneManager.GetActiveScene().name, out SceneName activeSceneName)) // ��������� ������������� ��� ������� ����� � Enum SceneName
        {
            return; // ���� �� �������� ������������� �� ����� �� ����������������� � �������-������� � ������ �� ������.           
        }

        // ����� ����� ���������� ������� ���������
        //Application.targetFrameRate = 60; // ��������� ��� ������� FPS = 60 �.� �� ��������� 30
        //Screen.sleepTimeout = SleepTimeout.NeverSleep; // �������� ��� � ��������

        /* _coroutines = new GameObject("[COROUTINE]").AddComponent<Coroutines>(); // �������� ������ � ��������� ������
         Object.DontDestroyOnLoad(_coroutines.gameObject);

         var prefabUIRoot = Resources.Load<UIRootView>("UIRoot"); // �������� UI 
         _uiRoot = Object.Instantiate(prefabUIRoot);
         Object.DontDestroyOnLoad(_uiRoot.gameObject);*/

        SceneManager.sceneLoaded += SceneManager_OnSceneLoaded; // ���������� ����� ��������� ��� ��������� EntryPoint ��� �������� �� ������ �����. SceneManager.sceneLoaded ���������� ����� Awake() � OnEnable() ��������.

        _rootContainer = new DIContainer();
        _scenesService = _rootContainer.RegisterSingleton(_ => new ScenesService()).CreateInstance();

        var PersistentEntryPoint = Object.Instantiate(Resources.Load<PersistentEntryPoint>("PersistentEntryPoint"));// ������� ������ PersistentEntryPoint     
        Object.DontDestroyOnLoad(PersistentEntryPoint.gameObject);
        PersistentEntryPoint.Process(_rootContainer);

        if (activeSceneName == SceneName.Bootstrap)
        {
            _scenesService.Load(SceneName.MainMenu); // ��� ������ ����������� ������ ��� �� ���������� �� ����� Bootstrap           
        }       
    }

    private void SceneManager_OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (System.Enum.TryParse(SceneManager.GetActiveScene().name, out SceneName activeSceneName)) // ��������� ������������� ��� ������� ����� � Enum SceneName
        {      
            if (activeSceneName != SceneName.Bootstrap)
            {
                FindEntryPoint(activeSceneName);
            }
            else
            {
                return; // ���� ��� Bootstrap �� �������
            }
        }
        else
        {
            _scenesService.Load(SceneName.MainMenu); // ���� ����� �� ����������������� � �������(�� �������� �������������) -�������� � ������� ���� (��� ����� � ���������. ����� �������� �� �����������. �����)
        }
    }

    private void FindEntryPoint(SceneName sceneName)
    {        
        GameObject gameObject = GameObject.FindWithTag("EntryPoint");
        IEntryPoint entryPoint = gameObject.GetComponent<IEntryPoint>();     
        var childContainer = new DIContainer(_rootContainer); // �������� �������� ��������� � ��������� ��������.
        entryPoint.Process(childContainer);// ��������� ��������� ��������� � ����� ����� ��������� ��� �����         
    }
}
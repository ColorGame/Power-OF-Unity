using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Главное меню
/// </summary>
public class MainMenuUI : MonoBehaviour 
{
    [SerializeField] private Animator _animator; //Аниматор для меню
    [SerializeField] private Button _resumeGameButton; // продолжить
    [SerializeField] private Button _startGameButton; // старт новой
    [SerializeField] private Button _loadGameButton; // загрузить
    [SerializeField] private Button _optionGameButton; // настройки
    [SerializeField] private Button _quitGameButton; // выйти
    
    private OptionsSubMenuUIProvider _optionsSubMenuUIProvider;
    private LoadGameSubMenuUIProvider _loadGameSubMenuUIProvider;
    private ScenesService _scenesService;
    private HashAnimationName _hashAnimationName;

    public void Init(OptionsSubMenuUIProvider optionsSubMenuUIProvider, LoadGameSubMenuUIProvider loadGameSubMenuUIProvider, ScenesService scenesService, HashAnimationName hashAnimationName)
    {
        _optionsSubMenuUIProvider = optionsSubMenuUIProvider;
        _loadGameSubMenuUIProvider = loadGameSubMenuUIProvider;
        _scenesService = scenesService;
        _hashAnimationName = hashAnimationName;

        Setup();
    }

    private void Setup()
    {
        _resumeGameButton.onClick.AddListener(() => { Debug.Log("ЗАГЛУШКА_ПРОДОЛЖИТЬ"); });
        _loadGameButton.onClick.AddListener(() => { _loadGameSubMenuUIProvider.LoadAndToggleVisible().Forget();  });
        _optionGameButton.onClick.AddListener(() => { _optionsSubMenuUIProvider.LoadAndToggleVisible().Forget(); });
        _startGameButton.onClick.AddListener(() => { _scenesService.LoadSceneByLoadingScreen(SceneName.UnitSetup).Forget(); });
        _quitGameButton.onClick.AddListener(() => { Application.Quit(); });// Кнопка будет работать тоько после сборки  kd
    }
    public void StartAnimation()
    {
        _animator.enabled = true;
        _animator.CrossFade(_hashAnimationName.MaiMenuOpen, 0);        
    }


}

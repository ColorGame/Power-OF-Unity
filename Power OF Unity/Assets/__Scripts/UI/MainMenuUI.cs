using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Главное меню
/// </summary>
public class MainMenuUI : MonoBehaviour 
{
    [SerializeField] private Button _resumeGameButton; // продолжить
    [SerializeField] private Button _startGameButton; // старт новой
    [SerializeField] private Button _loadGameButton; // загрузить
    [SerializeField] private Button _optionGameButton; // настройки
    [SerializeField] private Button _quitGameButton; // выйти

    private OptionsSubMenuUI _optionsMenuUI;
    private ScenesService _scenesService;
    private LoadGameSubMenuUI _loadGameSubMenuUI;

    public void Init(OptionsSubMenuUI optionsMenuUI, ScenesService scenesService, LoadGameSubMenuUI loadGameSubMenuUI)
    {
        _optionsMenuUI = optionsMenuUI;
        _scenesService = scenesService;
        _loadGameSubMenuUI = loadGameSubMenuUI;

        Setup();
    }

    private void Setup()
    {
        _resumeGameButton.onClick.AddListener(() => { Debug.Log("ЗАГЛУШКА_ПРОДОЛЖИТЬ"); });
        _loadGameButton.onClick.AddListener(() => { _loadGameSubMenuUI.ToggleVisible();  });
        _optionGameButton.onClick.AddListener(() => { _optionsMenuUI.ToggleVisible(); });
        _startGameButton.onClick.AddListener(() => { _scenesService.Load(SceneName.UnitSetupMenu); });
        _quitGameButton.onClick.AddListener(() => { Application.Quit(); });// Кнопка будет работать тоько после сборки  kd
    }
}

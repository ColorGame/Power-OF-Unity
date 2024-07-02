using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour // Главное меню
{
    [SerializeField] private Button _resumeGameButton;
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _loadGameButton;
    [SerializeField] private Button _setupGameButton;
    [SerializeField] private Button _quitGameButton;

    private OptionsMenuUI _optionsMenuUI;
    private ScenesService _scenesService;

    public void Init(OptionsMenuUI optionsMenuUI, ScenesService scenesService)
    {
        _optionsMenuUI = optionsMenuUI;
        _scenesService = scenesService;

        Setup();
    }

    private void Setup()
    {
        _resumeGameButton.onClick.AddListener(() => { Debug.Log("ЗАГЛУШКА"); });
        _loadGameButton.onClick.AddListener(() => { Debug.Log("ЗАГЛУШКА"); });
        _setupGameButton.onClick.AddListener(() => { _optionsMenuUI.ToggleVisible(); });
        _startGameButton.onClick.AddListener(() => { _scenesService.Load(SceneName.UnitSetupMenu); });
        _quitGameButton.onClick.AddListener(() => { Application.Quit(); });// Кнопка будет работать тоько после сборки  kd
    }
}

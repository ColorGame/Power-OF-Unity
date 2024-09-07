using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Игровое меню
/// </summary>
/// <remarks>
/// Создается и инициализируется в каждой сцене
/// </remarks>
public class GameMenuUI : ToggleVisibleAnimatioMenuUI
{
    [SerializeField] private Button _resumeGameButton; // продолжить
    [SerializeField] private Button _saveGameButton; // сохранить
    [SerializeField] private Button _loadGameButton; // загрузить
    [SerializeField] private Button _optionGameButton; // настройки
    [SerializeField] private Button _quitGameButton; // выйти


    private OptionsSubMenuUI _optionsSubMenuUI;
    private QuitGameSubMenuUI _quitGameSubMenuUI;
    private SaveGameSubMenuUI _saveGameSubMenuUI;   
    private LoadGameSubMenuUI _loadGameSubMenuUI;


    public void Init(GameInput gameInput, HashAnimationName hashAnimationName, OptionsSubMenuUI optionsSubMenuUI, QuitGameSubMenuUI quitGameSubMenuUI, SaveGameSubMenuUI saveGameSubMenuUI, LoadGameSubMenuUI loadGameSubMenuUI)
    {
        _gameInput = gameInput;
        _hashAnimationName = hashAnimationName;
        _optionsSubMenuUI = optionsSubMenuUI;
        _quitGameSubMenuUI = quitGameSubMenuUI;
        _saveGameSubMenuUI = saveGameSubMenuUI;
        _loadGameSubMenuUI = loadGameSubMenuUI;

        Setup();
        SetAnimationOpenClose();
    }

    private void Setup()
    {             
        _resumeGameButton.onClick.AddListener(() => {ToggleVisible(); }); 

        _saveGameButton.onClick.AddListener(() => 
        {
            UnsubscribeAlternativeToggleVisible();
            _saveGameSubMenuUI.ToggleVisible(SubscribeAlternativeToggleVisible); 
        }); 
        _loadGameButton.onClick.AddListener(() => 
        {
            UnsubscribeAlternativeToggleVisible();
            _loadGameSubMenuUI.ToggleVisible(SubscribeAlternativeToggleVisible); 
        });
        _optionGameButton.onClick.AddListener(() => 
        {
            UnsubscribeAlternativeToggleVisible();
            _optionsSubMenuUI.ToggleVisible(SubscribeAlternativeToggleVisible);
        }); 
        _quitGameButton.onClick.AddListener(() => 
        {
            UnsubscribeAlternativeToggleVisible();
            _quitGameSubMenuUI.ToggleVisible(SubscribeAlternativeToggleVisible);
        }); 
    }

    protected override void SetAnimationOpenClose()
    {
        _animationOpen = _hashAnimationName.GameMenuOpen;
        _animationClose = _hashAnimationName.GameMenuClose;
    }
}

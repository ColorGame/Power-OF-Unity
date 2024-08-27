using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Под меню настроики СОХРАНЕНИЯ... 
/// </summary>
/// <remarks>
/// Создается и инициализируется в каждой сцене
/// </remarks>
public class SaveGameSubMenuUI : ToggleVisibleAnimatioSubscribeMenuUI
{
    [SerializeField] private Button _saveGameButton;    //Сохранить игру
    [SerializeField] private Button _quitButton;        //Выйти из меню

    public void Init(GameInput gameInput)
    {
        _gameInput = gameInput;

        Setup();
    }

    private void Setup()
    {
        _saveGameButton.onClick.AddListener(() => { Debug.Log("ЗАГЛУШКА_СОХРАНИТЬ"); });
        _quitButton.onClick.AddListener(() => { ToggleVisible(); });
    }

    protected override void SetAnimationOpenClose()
    {
        _animationOpen = _animBase.SaveGameSubMenuOpen;
        _animationClose = _animBase.SaveGameSubMenuClose;
    }
}

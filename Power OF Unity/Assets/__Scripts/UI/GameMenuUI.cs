using System;

/// <summary>
/// Игровое меню
/// </summary>
/// <remarks>
/// Создается и инициализируется в каждой сцене
/// </remarks>
public class GameMenuUI : ToggleVisibleAnimatioSubscribeMenuUI
{

    private OptionsSubMenuUI _optionsSubMenuUI;
    private QuitGameSubMenuUI _quitGameSubMenuUI;
    private SaveGameSubMenuUI _saveGameSubMenuUI;   
    private LoadGameSubMenuUI _loadGameSubMenuUI;

    public void Init(GameInput gameInput, OptionsSubMenuUI optionsSubMenuUI, QuitGameSubMenuUI quitGameSubMenuUI, SaveGameSubMenuUI saveGameSubMenuUI, LoadGameSubMenuUI loadGameSubMenuUI)
    {
        _gameInput = gameInput;
        _optionsSubMenuUI = optionsSubMenuUI;
        _quitGameSubMenuUI = quitGameSubMenuUI;
        _saveGameSubMenuUI = saveGameSubMenuUI;
        _loadGameSubMenuUI = loadGameSubMenuUI;

        Setup();
    }

    private void Setup()
    {
        throw new NotImplementedException();
    }

    protected override void SetAnimationOpenClose()
    {
        throw new System.NotImplementedException();
    }
}

using UnityEngine;

/// <summary>
/// Под меню настроики СОХРАНЕНИЯ... 
/// </summary>
/// <remarks>
/// Создается и инициализируется в каждой сцене
/// </remarks>
public class SaveGameSubMenuUI : ToggleVisibleAnimatioSubscribeMenuUI
{
    public void Init(GameInput gameInput)
    {
        _gameInput = gameInput;

        Setup();
    }

    private void Setup()
    {

    }

    protected override void SetAnimationOpenClose()
    {

    }
}

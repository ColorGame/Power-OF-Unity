using UnityEngine;

/// <summary>
/// Под меню настроики ЗАГРУЗКИ... 
/// </summary>
/// <remarks>
/// Создается и инициализируется в PersistentEntryPoint
/// </remarks>
public class LoadGameSubMenuUI : ToggleVisibleAnimatioSubscribeMenuUI
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

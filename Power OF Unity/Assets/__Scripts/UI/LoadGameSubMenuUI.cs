using UnityEngine;

/// <summary>
/// ��� ���� ��������� ��������... 
/// </summary>
/// <remarks>
/// ��������� � ���������������� � PersistentEntryPoint
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

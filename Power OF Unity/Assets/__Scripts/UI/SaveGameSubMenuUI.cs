using UnityEngine;

/// <summary>
/// ��� ���� ��������� ����������... 
/// </summary>
/// <remarks>
/// ��������� � ���������������� � ������ �����
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

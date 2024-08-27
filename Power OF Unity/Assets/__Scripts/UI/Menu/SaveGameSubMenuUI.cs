using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��� ���� ��������� ����������... 
/// </summary>
/// <remarks>
/// ��������� � ���������������� � ������ �����
/// </remarks>
public class SaveGameSubMenuUI : ToggleVisibleAnimatioSubscribeMenuUI
{
    [SerializeField] private Button _saveGameButton;    //��������� ����
    [SerializeField] private Button _quitButton;        //����� �� ����

    public void Init(GameInput gameInput)
    {
        _gameInput = gameInput;

        Setup();
    }

    private void Setup()
    {
        _saveGameButton.onClick.AddListener(() => { Debug.Log("��������_���������"); });
        _quitButton.onClick.AddListener(() => { ToggleVisible(); });
    }

    protected override void SetAnimationOpenClose()
    {
        _animationOpen = _animBase.SaveGameSubMenuOpen;
        _animationClose = _animBase.SaveGameSubMenuClose;
    }
}

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��� ���� ��������� ����������... 
/// </summary>
/// <remarks>
/// ��������� � ���������������� � ������ �����
/// </remarks>
public class SaveGameSubMenuUI : ToggleVisibleAnimatioMenuUI
{
    [SerializeField] private Button _saveGameButton;    //��������� ����
    [SerializeField] private Button _quitButton;        //����� �� ����

    public void Init(GameInput gameInput, HashAnimationName hashAnimationName)
    {
        _gameInput = gameInput;
        _hashAnimationName = hashAnimationName;

        Setup();
        SetAnimationOpenClose();
    }

    private void Setup()
    {
        _saveGameButton.onClick.AddListener(() => { Debug.Log("��������_���������"); });
        _quitButton.onClick.AddListener(() => { ToggleVisible(); });
    }

    protected override void SetAnimationOpenClose()
    {
        _animationOpen = _hashAnimationName.SaveGameSubMenuOpen;
        _animationClose = _hashAnimationName.SaveGameSubMenuClose;
    }
}

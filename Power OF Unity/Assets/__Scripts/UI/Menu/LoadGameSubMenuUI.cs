using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��� ���� ��������� ��������... 
/// </summary>
/// <remarks>
/// ��������� � ���������������� � PersistentEntryPoint
/// </remarks>
public class LoadGameSubMenuUI : ToggleVisibleAnimatioMenuUI
{
    [SerializeField] private Button _loadGameButton;    //��������� ����
    [SerializeField] private Button _deleteSaveButton;  //������� ����������
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
        _loadGameButton.onClick.AddListener(() => { Debug.Log("��������_���������"); });
        _deleteSaveButton.onClick.AddListener(() => { Debug.Log("��������_�������"); }); // ������� �������������� ���� �� �������� (��/���)
        _quitButton.onClick.AddListener(() => { ToggleVisible(); });
    }

    protected override void SetAnimationOpenClose()
    {
        _animationOpen = _hashAnimationName.LoadGameSubMenuOpen;
        _animationClose = _hashAnimationName.LoadGameSubMenuClose;
    }
}

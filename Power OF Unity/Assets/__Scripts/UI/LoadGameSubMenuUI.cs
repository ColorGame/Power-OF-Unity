using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��� ���� ��������� ��������... 
/// </summary>
/// <remarks>
/// ��������� � ���������������� � PersistentEntryPoint
/// </remarks>
public class LoadGameSubMenuUI : ToggleVisibleAnimatioSubscribeMenuUI
{
    [SerializeField] private Button _loadGameButton;    //��������� ����
    [SerializeField] private Button _deleteSaveButton;  //������� ����������
    [SerializeField] private Button _quitButton;        //����� �� ����
    public void Init(GameInput gameInput)
    {
        _gameInput = gameInput;

        Setup();
    }

    private void Setup()
    {
        _loadGameButton.onClick.AddListener(() => { Debug.Log("��������_���������"); });
        _deleteSaveButton.onClick.AddListener(() => { Debug.Log("��������_�������"); }); // ������� �������������� ���� �� �������� (��/���)
        _quitButton.onClick.AddListener(() => { ToggleVisible(); });
    }

    protected override void SetAnimationOpenClose()
    {
        _animationOpen = _animBase.LoadGameSubMenuOpen;
        _animationClose = _animBase.LoadGameSubMenuClose;
    }
}

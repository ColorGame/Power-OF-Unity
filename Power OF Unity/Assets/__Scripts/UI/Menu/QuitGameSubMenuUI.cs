using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��� ���� ������ �� �������� ����
/// </summary>
/// <remarks>
/// ��������� � ���������������� � ������ �����
/// </remarks>
public class QuitGameSubMenuUI : ToggleVisibleAnimatioSubscribeMenuUI
{
    [SerializeField] private Button _mainMenuButton; //����� � ������� ����
    [SerializeField] private Button _desktopButton;  //����� �� ���� �� ������� ����
    [SerializeField] private Button _cancelButton;   //������

    private ScenesService _scenesService;

    public void Init(GameInput gameInput, ScenesService scenesService)
    {
        _gameInput = gameInput;
        _scenesService = scenesService;

        Setup();       
    }

    private void Setup()
    {
        _mainMenuButton.onClick.AddListener(() => 
        {
            ToggleVisible();
            _scenesService.Load(SceneName.MainMenu); 
        }); 
        _desktopButton.onClick.AddListener(() => { Application.Quit(); }); 
        _cancelButton.onClick.AddListener(() => { ToggleVisible(); }); 
    }

    protected override void SetAnimationOpenClose()
    {
        _animationOpen = _animBase.QuitGameSubMenuOpen;
        _animationClose = _animBase.QuitGameSubMenuClose;
    }
}

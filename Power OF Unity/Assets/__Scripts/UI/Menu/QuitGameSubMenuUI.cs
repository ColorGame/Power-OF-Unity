using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��� ���� ������ �� �������� ����
/// </summary>
/// <remarks>
/// ��������� � ���������������� � ������ �����
/// </remarks>
public class QuitGameSubMenuUI : ToggleVisibleAnimatioMenuUI
{
    [SerializeField] private Button _mainMenuButton; //����� � ������� ����
    [SerializeField] private Button _desktopButton;  //����� �� ���� �� ������� ����
    [SerializeField] private Button _cancelButton;   //������

    private ScenesService _scenesService;

    public void Init(GameInput gameInput, ScenesService scenesService, HashAnimationName hashAnimationName)
    {
        _gameInput = gameInput;
        _scenesService = scenesService;
        _hashAnimationName = hashAnimationName;

        Setup();
        SetAnimationOpenClose();
    }

    private void Setup()
    {
        _mainMenuButton.onClick.AddListener(() => 
        {
            SetCloseDelegate(LoadMainMenu);
            ToggleVisible(); 
        });
        _desktopButton.onClick.AddListener(() => { Application.Quit(); });
        _cancelButton.onClick.AddListener(() => { ToggleVisible(); });
    }

    protected override void SetAnimationOpenClose()
    {
        _animationOpen = _hashAnimationName.QuitGameSubMenuOpen;
        _animationClose = _hashAnimationName.QuitGameSubMenuClose;
    }

    private void LoadMainMenu()
    {  
        _scenesService.LoadSceneByLoadingScreen(SceneName.MainMenu).Forget();
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������� ����
/// </summary>
/// <remarks>
/// ��������� � ���������������� � ������ �����
/// </remarks>
public class GameMenuUI : ToggleVisibleAnimatioSubscribeMenuUI
{
    [SerializeField] private Button _resumeGameButton; // ����������
    [SerializeField] private Button _saveGameButton; // ���������
    [SerializeField] private Button _loadGameButton; // ���������
    [SerializeField] private Button _optionGameButton; // ���������
    [SerializeField] private Button _quitGameButton; // �����


    private OptionsSubMenuUI _optionsSubMenuUI;
    private QuitGameSubMenuUI _quitGameSubMenuUI;
    private SaveGameSubMenuUI _saveGameSubMenuUI;   
    private LoadGameSubMenuUI _loadGameSubMenuUI;

    public void Init(GameInput gameInput, OptionsSubMenuUI optionsSubMenuUI, QuitGameSubMenuUI quitGameSubMenuUI, SaveGameSubMenuUI saveGameSubMenuUI, LoadGameSubMenuUI loadGameSubMenuUI)
    {
        _gameInput = gameInput;
        _optionsSubMenuUI = optionsSubMenuUI;
        _quitGameSubMenuUI = quitGameSubMenuUI;
        _saveGameSubMenuUI = saveGameSubMenuUI;
        _loadGameSubMenuUI = loadGameSubMenuUI;

        Setup();
    }

    private void Setup()
    {             
        _resumeGameButton.onClick.AddListener(() => {ToggleVisible(); }); 

        _saveGameButton.onClick.AddListener(() => 
        {
            UnsubscribeAlternativeToggleVisible();
            _saveGameSubMenuUI.ToggleVisible(SubscribeAlternativeToggleVisible); 
        }); 
        _loadGameButton.onClick.AddListener(() => 
        {
            UnsubscribeAlternativeToggleVisible();
            _loadGameSubMenuUI.ToggleVisible(SubscribeAlternativeToggleVisible); 
        });
        _optionGameButton.onClick.AddListener(() => 
        {
            UnsubscribeAlternativeToggleVisible();
            _optionsSubMenuUI.ToggleVisible(SubscribeAlternativeToggleVisible);
        }); 
        _quitGameButton.onClick.AddListener(() => 
        {
            UnsubscribeAlternativeToggleVisible();
            _quitGameSubMenuUI.ToggleVisible(SubscribeAlternativeToggleVisible);
        }); 
    }

    protected override void SetAnimationOpenClose()
    {
        _animationOpen = _animBase.GameMenuOpen;
        _animationClose = _animBase.GameMenuClose;
    }
}

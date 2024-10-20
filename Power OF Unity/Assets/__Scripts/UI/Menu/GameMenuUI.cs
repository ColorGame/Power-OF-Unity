using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������� ����
/// </summary>
/// <remarks>
/// ��������� � ���������������� � ������ �����
/// </remarks>
public class GameMenuUI : ToggleVisibleAnimatioMenuUI
{
    [SerializeField] private Button _resumeGameButton; // ����������
    [SerializeField] private Button _saveGameButton; // ���������
    [SerializeField] private Button _loadGameButton; // ���������
    [SerializeField] private Button _optionGameButton; // ���������
    [SerializeField] private Button _quitGameButton; // �����


    private OptionsSubMenuUIProvider _optionsSubMenuUIProvider;
    private QuitGameSubMenuUIProvider _quitGameSubMenuUIProvider;
    private SaveGameSubMenuUIProvider _saveGameSubMenuUIProvider;   
    private LoadGameSubMenuUIProvider _loadGameSubMenuUIProvider;


    public void Init(GameInput gameInput, HashAnimationName hashAnimationName, OptionsSubMenuUIProvider optionsSubMenuUIProvider, QuitGameSubMenuUIProvider quitGameSubMenuUIProvider, SaveGameSubMenuUIProvider saveGameSubMenuUIProvider, LoadGameSubMenuUIProvider loadGameSubMenuUIProvider)
    {
        _gameInput = gameInput;
        _hashAnimationName = hashAnimationName;
        _optionsSubMenuUIProvider = optionsSubMenuUIProvider;
        _quitGameSubMenuUIProvider = quitGameSubMenuUIProvider;
        _saveGameSubMenuUIProvider = saveGameSubMenuUIProvider;
        _loadGameSubMenuUIProvider = loadGameSubMenuUIProvider;

        Setup();
        SetAnimationOpenClose();
    }

    private void Setup()
    {             
        _resumeGameButton.onClick.AddListener(() => {ToggleVisible(); }); 

        // ��������� ������� (�������� �� �������������� ������������ ����� ����). ��� ���������� ���������� ����, ���� ������� ����� ������, � �������� �������������.
        _saveGameButton.onClick.AddListener(() => 
        {
            UnsubscribeAlternativeToggleVisible();
            _saveGameSubMenuUIProvider.LoadAndToggleVisible(SubscribeAlternativeToggleVisible).Forget(); 
        }); 
        _loadGameButton.onClick.AddListener(() => 
        {
            UnsubscribeAlternativeToggleVisible();
            _loadGameSubMenuUIProvider.LoadAndToggleVisible(SubscribeAlternativeToggleVisible).Forget(); 
        });
        _optionGameButton.onClick.AddListener(() => 
        {
            UnsubscribeAlternativeToggleVisible();
            _optionsSubMenuUIProvider.LoadAndToggleVisible(SubscribeAlternativeToggleVisible).Forget();
        }); 
        _quitGameButton.onClick.AddListener(() => 
        {
            UnsubscribeAlternativeToggleVisible();
            _quitGameSubMenuUIProvider.LoadAndToggleVisible(SubscribeAlternativeToggleVisible).Forget();
        }); 
    }

    protected override void SetAnimationOpenClose()
    {
        _animationOpen = _hashAnimationName.GameMenuOpen;
        _animationClose = _hashAnimationName.GameMenuClose;
    }
}

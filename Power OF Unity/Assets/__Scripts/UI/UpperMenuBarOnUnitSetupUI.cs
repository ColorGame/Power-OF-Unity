using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������� ������ ���� � ����� �������� �����
/// </summary>
public class UpperMenuBarOnUnitSetupUI : MonoBehaviour
{
    [Header("������ ��������� �������� ����")]
    [SerializeField] private Button _gameMenuButton;
    [Header("����� ���������� �����")]
    [SerializeField] private TextMeshProUGUI _coinCountText;
    [Header("������ ������� ����������� ������")]
    [SerializeField] private Button _unitManagerButtonButton;
    [SerializeField] private Button _weaponButton;
    [SerializeField] private Button _armorButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _missionButton;
    [Header("������� ������� ����� ���/����.\n��� ������������ �������")]
    [SerializeField] private Canvas _canvasSelectUnitAndPortfolioScreenSpace;
    [SerializeField] private Canvas _canvasInventoryScreenSpace;
    [SerializeField] private Transform _unitSpawnerOnInventoryMenu;


    private GameInput _gameInput;
    private GameMenuUI _gameMenuUI;

    public void Init(GameInput gameInput, GameMenuUI gameMenuUI)
    {
        _gameInput = gameInput;
        _gameMenuUI = gameMenuUI;

        Setup();
    }

    private void Setup()
    {
        _gameMenuButton.onClick.AddListener(() =>
        {
            UnsubscribeAlternativeToggleVisible();
            _gameMenuUI.ToggleVisible(SubscribeAlternativeToggleVisible);
        });

        _unitManagerButtonButton.onClick.AddListener(() =>
        {
            UnsubscribeAlternativeToggleVisible();
            _gameMenuUI.ToggleVisible(SubscribeAlternativeToggleVisible);
        });


        SubscribeAlternativeToggleVisible();
    }

    /// <summary>
    /// ����������� �� �������������� ������������ ��������� ���� (������ ��� ESC)
    /// </summary>
    protected void SubscribeAlternativeToggleVisible()
    {
        _gameInput.OnMenuAlternate += GameInput_OnMenuAlternate;
    }
    /// <summary>
    /// ���������� �� ��������������� ������������ ��������� ����
    /// </summary>
    protected void UnsubscribeAlternativeToggleVisible()
    {
        _gameInput.OnMenuAlternate -= GameInput_OnMenuAlternate;
    }
    private void GameInput_OnMenuAlternate(object sender, System.EventArgs e)
    {
        UnsubscribeAlternativeToggleVisible();
        _gameMenuUI.ToggleVisible(SubscribeAlternativeToggleVisible);
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ������ - ������  �����, � ���� ����������� ������
/// </summary>
public class UnitSelectAtManagementButtonUI : MonoBehaviour
{
    [SerializeField] private Button _unitSelectButton;  // ������ ��� ������  �����
    [SerializeField] private Image _selectedImage; // ����������� ���������� ������ 
    [Header("��� ������")]
    [SerializeField] private TextMeshProUGUI _numberUnitText; // ����� � ������
    [SerializeField] private TextMeshProUGUI _nameUnitText; // ��� �����
    [Header("�������������� �����")]
    [SerializeField] private TextMeshProUGUI _healthNumberText;
    [SerializeField] private TextMeshProUGUI _actionPointsNumberText;
    [SerializeField] private TextMeshProUGUI _powerNumberText;
    [SerializeField] private TextMeshProUGUI _accuracyNumberText;
    [Header("��������� ����� � ���� ����")]
    [SerializeField] private Transform _priceContainer;
    [SerializeField] private TextMeshProUGUI _priceText;
    [Header("��������� ������ � ���������� ������")]
    [SerializeField] private Transform _statusContainer;
    [SerializeField] private Button _barrackToggleButton;  // ������ ��� ���������� ����� �� ����
    [SerializeField] private Image _barrackButtonSelectedImage; // ����������� ���������� ������ 
    [SerializeField] private Image _barrackButtonIcon; // ����������� ���������� ������ 
    [SerializeField] private Button _missionToggleButton;  // ������ ��� ���������� ����� �� ������
    [SerializeField] private Image _missionButtonSelectedImage; // ����������� ���������� ������ 
    [SerializeField] private Image _missionButtonIcon; // ����������� ���������� ������ 


    private UnitManager _unitManager;
    private Unit _unit;   

    public void Init(Unit unit, UnitManager unitManager, int index)
    {
        _unitManager = unitManager;
        _unit = unit;


        _nameUnitText.text = unit.GetUnitTypeSO<UnitTypeSO>().GetName(); // ������� ��� � ��������� �����
        _numberUnitText.text = index.ToString();
        _healthNumberText.text = unit.GetHealthSystem().GetHealthFull().ToString();
        _actionPointsNumberText.text = unit.GetActionPointsSystem().GetActionPointsCountFull().ToString();
        _powerNumberText.text = unit.GetUnitPowerSystem().GetPower().ToString();
        _accuracyNumberText.text = unit.GetUnitAccuracySystem().GetAccuracy().ToString();

        _unitSelectButton.onClick.AddListener(() =>
        {
            _unitManager.SetSelectedUnit(unit);
        });

        if (_unitManager.GetUnitFriendList().Contains(unit)) // ���� ��� ���� � ������ ���� ������
        {
            Destroy(_priceContainer.gameObject);
            SetToggleButton();
            UpdateVisualToggleLocationButton();
        }
        else// � ��������� ������ - ��� ���� ��� �����
        {
            Destroy(_statusContainer.gameObject);
            _priceText.text = unit.GetUnitTypeSO<UnitFriendSO>().GetPriceHiring().ToString();
        }

        _unitManager.OnSelectedUnitChanged += UnitManager_OnSelectedUnitChanged;                     
    }

    private void SetToggleButton()
    {
        _barrackToggleButton.onClick.AddListener(() =>
        {
            ChangeLocation(Unit.Location.Barrack);
        });

        _missionToggleButton.onClick.AddListener(() =>
        {
            ChangeLocation(Unit.Location.Mission);
        });
    }
    /// <summary>
    /// �������� �������. 
    /// </summary>
    private void ChangeLocation(Unit.Location newLocation)
    {
        if (_unit.GetLocation() != newLocation) // ���� �� �� � ���������� ������� ��
        {
            _unitManager.ChangeLocation(_unit, newLocation);
            UpdateVisualToggleLocationButton();
        }
    }
    /// <summary>
    /// �������� ������ ������ ������������ �������
    /// </summary>
    private void UpdateVisualToggleLocationButton()
    {
        bool isBarrackLocation = _unit.GetLocation() == Unit.Location.Barrack;        
        _barrackButtonSelectedImage.enabled = (isBarrackLocation);
        _barrackButtonIcon.enabled = (isBarrackLocation);

        bool isMissionLocation = _unit.GetLocation() == Unit.Location.Mission;
        _missionButtonSelectedImage.enabled = (isMissionLocation);
        _missionButtonIcon.enabled = (isMissionLocation);
    }

    private void UnitManager_OnSelectedUnitChanged(object sender, Unit newSelectedUnit)
    {
        UpdateSelectedVisual(newSelectedUnit);
    }

    public void UpdateSelectedVisual(Unit selectedUnit) // (���������� �������) ��������� � ���������� ������������ ������.
    {
        _selectedImage.enabled = (selectedUnit != null && selectedUnit == _unit);   // �������� ��������� ������ ���� ��������� ���� != null � ��� ��� ����      
    }

    private void OnDestroy()
    {
        if (_unitManager != null)
            _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;
    }

   
}

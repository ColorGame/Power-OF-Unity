using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Кнопка - выбора  Юнита, в окне менеджмента юнитов
/// </summary>
public class UnitSelectAtManagementButtonUI : MonoBehaviour
{
    [SerializeField] private Button _unitSelectButton;  // Кнопка для выбора  Юнита
    [SerializeField] private Image _selectedImage; // Изображение выделенной кнопки 
    [Header("Имя иномер")]
    [SerializeField] private TextMeshProUGUI _numberUnitText; // Номер в списке
    [SerializeField] private TextMeshProUGUI _nameUnitText; // Имя юнита
    [Header("Характеристики юнита")]
    [SerializeField] private TextMeshProUGUI _healthNumberText;
    [SerializeField] private TextMeshProUGUI _actionPointsNumberText;
    [SerializeField] private TextMeshProUGUI _powerNumberText;
    [SerializeField] private TextMeshProUGUI _accuracyNumberText;
    [Header("Контейнер ПРАЙС и текс цены")]
    [SerializeField] private Transform _priceContainer;
    [SerializeField] private TextMeshProUGUI _priceText;
    [Header("Контейнер СТАТУС и внутринние кнопки")]
    [SerializeField] private Transform _statusContainer;
    [SerializeField] private Button _barrackToggleButton;  // Кнопка для переброски юнита на базу
    [SerializeField] private Image _barrackButtonSelectedImage; // Изображение выделенной кнопки 
    [SerializeField] private Image _barrackButtonIcon; // Изображение выделенной кнопки 
    [SerializeField] private Button _missionToggleButton;  // Кнопка для переброски юнита на миссию
    [SerializeField] private Image _missionButtonSelectedImage; // Изображение выделенной кнопки 
    [SerializeField] private Image _missionButtonIcon; // Изображение выделенной кнопки 


    private UnitManager _unitManager;
    private Unit _unit;   

    public void Init(Unit unit, UnitManager unitManager, int index)
    {
        _unitManager = unitManager;
        _unit = unit;


        _nameUnitText.text = unit.GetUnitTypeSO<UnitTypeSO>().GetName(); // Зададим имя с ЗАГЛАВНОЙ БУКВЫ
        _numberUnitText.text = index.ToString();
        _healthNumberText.text = unit.GetHealthSystem().GetHealthFull().ToString();
        _actionPointsNumberText.text = unit.GetActionPointsSystem().GetActionPointsCountFull().ToString();
        _powerNumberText.text = unit.GetUnitPowerSystem().GetPower().ToString();
        _accuracyNumberText.text = unit.GetUnitAccuracySystem().GetAccuracy().ToString();

        _unitSelectButton.onClick.AddListener(() =>
        {
            _unitManager.SetSelectedUnit(unit);
        });

        if (_unitManager.GetUnitFriendList().Contains(unit)) // Если это юнит в списке моих юнитов
        {
            Destroy(_priceContainer.gameObject);
            SetToggleButton();
            UpdateVisualToggleLocationButton();
        }
        else// В противном слючаи - это юнит для найма
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
    /// Изменить локацию. 
    /// </summary>
    private void ChangeLocation(Unit.Location newLocation)
    {
        if (_unit.GetLocation() != newLocation) // Если мы не в переданной локации то
        {
            _unitManager.ChangeLocation(_unit, newLocation);
            UpdateVisualToggleLocationButton();
        }
    }
    /// <summary>
    /// Обновить визуал кнопок переключения локации
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

    public void UpdateSelectedVisual(Unit selectedUnit) // (Обновление визуала) Включение и выключение визуализации выбора.
    {
        _selectedImage.enabled = (selectedUnit != null && selectedUnit == _unit);   // Включить выделение кнопки если выбранный юнит != null и это наш юнит      
    }

    private void OnDestroy()
    {
        if (_unitManager != null)
            _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;
    }

   
}

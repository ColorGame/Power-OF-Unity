using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Кнопка - выбора  Юнита, для настройки экипировки
/// </summary>
public class UnitSelectAtEquipmentButtonUI : MonoBehaviour, IToggleActivity
{
    [SerializeField] private Image _selectedImage; // Изображение выделенной кнопки 
    [SerializeField] private TextMeshProUGUI _numberUnitText; // Номер в списке
    [SerializeField] private TextMeshProUGUI _nameUnitText; // Имя юнита

    private Button _unitSelectAtEquipmentButton;  // Кнопка выбора  Юнита
    private UnitManager _unitManager;
    private Unit _unit;

    public void Init(Unit unit, UnitManager unitManager, int index)
    {
        _unitManager = unitManager;
        _unit = unit;

        _nameUnitText.text = unit.GetUnitTypeSO<UnitTypeSO>().GetName(); // Зададим имя с ЗАГЛАВНОЙ БУКВЫ
        _numberUnitText.text = index.ToString();

        _unitSelectAtEquipmentButton = GetComponent<Button>();
        _unitSelectAtEquipmentButton.onClick.AddListener(() =>
        {
            _unitManager.SetSelectedUnit(unit);
        });
       

        Unit selectedUnit = _unitManager.GetSelectedUnit(); // Выделенный Югит
        UpdateSelectedVisual(selectedUnit);
    }

    public void SetActive(bool active)
    {
        if (active)
        {
            _unitManager.OnSelectedUnitChanged += UnitManager_OnSelectedUnitChanged;
        }
        else
        {
            _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;
        }
    }

    private void UnitManager_OnSelectedUnitChanged(object sender, Unit selectedUnit)
    {
        UpdateSelectedVisual(selectedUnit);
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

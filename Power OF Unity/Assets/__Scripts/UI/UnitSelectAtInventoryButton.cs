using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Кнопка - выбора  Юнита, для настройки инвентаря
/// </summary>
public class UnitSelectAtInventoryButton : MonoBehaviour
{
    [SerializeField] private Image _selectedImage; // Изображение выделенной кнопки 
    [SerializeField] private TextMeshProUGUI _numberUnitText; // Номер в списке
    [SerializeField] private TextMeshProUGUI _nameUnitText; // Имя юнита

    private Button _unitSelectAtInventoryButton;  // Кнопка для включения панели Юнитов на МИССИИ
    private UnitInventorySystem _unitInventorySystem;
    private Unit _unit;

    public void Init(Unit unit, UnitInventorySystem unitInventorySystem  , int index)
    {
        _unitInventorySystem = unitInventorySystem;
        _unit = unit;

        _nameUnitText.text = unit.GetUnitTypeSO<UnitTypeSO>().GetName(); // Зададим имя с ЗАГЛАВНОЙ БУКВЫ
        _numberUnitText.text = index.ToString();

        _unitSelectAtInventoryButton = GetComponent<Button>();
        _unitSelectAtInventoryButton.onClick.AddListener(() => 
        {
            _unitInventorySystem.SetSelectedUnit(unit);
        });

        _unitInventorySystem.OnSelectedUnitChanged += UnitInventorySystem_OnSelectedUnitChanged;

        Unit selectedUnit = _unitInventorySystem.GetSelectedUnit(); // Выделенный Югит
        UpdateSelectedVisual(selectedUnit);
    }

    private void UnitInventorySystem_OnSelectedUnitChanged(object sender, Unit selectedUnit)
    {
        UpdateSelectedVisual(selectedUnit);
    }

    public void UpdateSelectedVisual(Unit selectedUnit) // (Обновление визуала) Включение и выключение визуализации выбора.
    {       
        _selectedImage.enabled = (selectedUnit == _unit);   // Включить выделение кнопки если это наш юнит        
    }
}

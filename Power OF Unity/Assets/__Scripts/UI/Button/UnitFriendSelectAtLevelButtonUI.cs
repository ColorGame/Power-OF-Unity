using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Кнопка - выбора дружественного юнита на игровом уровне.
/// </summary>
public class UnitSelectAtLevelButtonUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _nameUnitText; // Имя юнита
    [SerializeField] private TextMeshProUGUI _actionPointsText; // Очки действия
    [SerializeField] private Image _selectedButtonVisualUI; // Будем включать и выкл. GameObject что бы скрыть или показать рамку кнопки // В инспекторе надо закинуть рамку
    [SerializeField] private Image _healthBarImage; // в инспекторе закинуть шкалу здоровья "Bar"
    [SerializeField] private Image _backgroundImage; // в инспекторе закинуть шкалу здоровья "Bar"
    [SerializeField] private Image _actionImage; // в инспекторе закинуть шкалу здоровья "Bar"

    private Button _button; // Сама кнопка
    private Unit _unit;
    private UnitActionSystem _unitActionSystem;
    private Color _nameUnitTextColor;
    private Color _actionPointsTextColor;
    private Color _healthBarImageColor;
    private Color _backgroundImageColor;
    private Color _actionImageColor;


    public void Init(Unit unit, UnitActionSystem unitActionSystem, CameraFollow cameraFollow)
    {
        _unit = unit;
        _unitActionSystem = unitActionSystem;

        _nameUnitText.text = _unit.GetUnitTypeSO().GetName().ToUpper(); // Зададим имя с ЗАГЛАВНОЙ БУКВЫ
        _actionPointsText.text = _unit.GetActionPointsSystem().GetActionPointsCount().ToString();
        _healthBarImage.fillAmount = _unit.GetHealthSystem().GetHealthNormalized();

        // Сохраним цвета элементов кнопки
        _nameUnitTextColor = _nameUnitText.color;
        _actionPointsTextColor = _actionPointsText.color;
        _healthBarImageColor = _healthBarImage.color;
        _backgroundImageColor = _backgroundImage.color;
        _actionImageColor = _actionImage.color;

        _button = GetComponent<Button>();
        // т.к. кнопки создаются динамически то и события настраиваем в скрипте а не в инспекторе
        //Добавим событие при нажатии на нашу кнопку// AddListener() в аргумент должен получить делегат- ссылку на функцию. Функцию будем объявлять АНАНИМНО через лямбду () => {...} 
        _button.onClick.AddListener(() =>
        {
            _unitActionSystem.SetSelectedUnit(_unit, _unit.GetAction<MoveAction>()); // Сделаем юнита выделенным и активируем движение
            cameraFollow.SetTargetUnit(_unit);
        });

        _unit.GetHealthSystem().OnDamageAndHealing += Unit_OnDamageAndHealing; // Если у этого юнита измениться здоровье то обновим
        _unit.GetActionPointsSystem().OnActionPointsChanged += Unit_OnActionPointsChanged; // Если у этого юнита измениться очки действия то обновим
    }

    private void Unit_OnActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void Unit_OnDamageAndHealing(object sender, EventArgs e)
    {
        UpdateHealthBar();
    }

    public void UpdateSelectedVisual(Unit selectedUnit) // (Обновление визуала) Включение и выключение визуализации выбора.(вызывается событием при выборе кнопки базового действия)
    {       
        _selectedButtonVisualUI.enabled = (selectedUnit == _unit);   // Включить рамку если это наш юнит // Если не совподает то получим false и рамка отключиться       
    }
    private void UpdateActionPoints()
    {
        _actionPointsText.text = _unit.GetActionPointsSystem().GetActionPointsCount().ToString();
    }
    private void UpdateHealthBar()
    {
        _healthBarImage.fillAmount = _unit.GetHealthSystem().GetHealthNormalized();
    }

    //3//{ Третий способ скрыть кнопки когда занят действием
    public void HandleStateButton(bool isBusy) // Обработать состояние кнопки
    {
        if (isBusy) // Если занят
        {
            InteractableDesabled(); // Отключить взаимодействие
        }
        else
        {
            InteractableEnable(); // Включить взаимодействие
        }
    }

    private void InteractableEnable() // Включить взаимодействие
    {
        _button.interactable = true;
        // Восстановим оригинальные цвета
        _nameUnitText.color = _nameUnitTextColor;
        _actionPointsText.color = _actionPointsTextColor;
        _healthBarImage.color = _healthBarImageColor;
        _backgroundImage.color = _backgroundImageColor;
        _actionImage.color = _actionImageColor;

        Unit selectedUnit = _unitActionSystem.GetSelectedUnit(); // Выделенный Югит
        UpdateSelectedVisual(selectedUnit); // Обновим отображение рамки кнопки в зависимости от выбранного юнита
    }

    private void InteractableDesabled() // Отключить взаимодействие // Кнопка становиться не активная и меняет цвет(Настраивается в инспекторе color  Desabled)
    {
        _button.interactable = false;

        Color nameUnitTextColor = _nameUnitTextColor; // Сохраним в локальную переменную цвет текста
        Color actionPointsTextColor = _actionPointsTextColor;
        Color healthBarImageColor = _healthBarImageColor;
        Color backgroundImageColor = _backgroundImageColor;
        Color actionImageColor = _actionImageColor;


        nameUnitTextColor.a = 0.1f; // Изменим значение альфа канала
        actionPointsTextColor.a = 0.1f;
        healthBarImageColor.a = 0.1f;
        backgroundImageColor.a = 0.1f;
        actionImageColor.a = 0.1f;

        _nameUnitText.color = nameUnitTextColor; // Изменим текущий цвет текса (сдел прозрачным)
        _actionPointsText.color = actionPointsTextColor;
        _healthBarImage.color = healthBarImageColor;
        _backgroundImage.color = backgroundImageColor;
        _actionImage.color = actionImageColor;

        _selectedButtonVisualUI.enabled = (false); //Отключим рамку
    }   
    //3//}

    private void OnDestroy()
    {
        if (_unit != null)
        {
            _unit.GetHealthSystem().OnDamageAndHealing -= Unit_OnDamageAndHealing;
            _unit.GetActionPointsSystem().OnActionPointsChanged -= Unit_OnActionPointsChanged;
        }
    }
}

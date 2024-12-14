using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Система управления КНОПКАМИ ДЕЙСТВИЙ
/// </summary>
/// <remarks>
/// Настраивать кнопки при выборе юнита. 
/// </remarks>
public class ActionButtonSystemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _actionPointsText; // Ссылка на текст очков над кнопками
    [SerializeField] private Image _actionPointImage; // Картинка головы

    [SerializeField] private ActionButtonUI _mainWeaponButton; // Кнопка основного оружия
    [SerializeField] private ActionButtonUI _otherWeaponButton; // Кнопка дополнительного оружия
    [SerializeField] private ActionButtonUI _moveActionButton; // Кнопка перемещения
    [SerializeField] private Transform _grenadeButtonContainer; // Контейнер для кнопок гранат
    [SerializeField] private Button _unitEquipmentButton; // Кнопка экипировка юнита

    private Dictionary<GrenadeType, int> _grenadeTypeCountDict = new();// Словарь КЛЮЧ - тип гранаты, ЗНАЧЕНИЕ - количество

    private TooltipUI _tooltipUI;
    private UnitActionSystem _unitActionSystem;
    private TurnSystem _turnSystem;

    private List<ActionButtonUI> _actionButtonUIList; // Список кнопок действий  
    private Unit _selectedUnit;

    public void Init(UnitActionSystem unitActionSystem, TurnSystem turnSystem, TooltipUI tooltipUI)
    {
        _actionButtonUIList = new List<ActionButtonUI>(); // Создадим экземпляр списка       

        _unitActionSystem = unitActionSystem;
        _turnSystem = turnSystem;
        _tooltipUI = tooltipUI;

        _unitActionSystem.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged; //Выбранный Юнит Изменен
        _unitActionSystem.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged; //Выбранное Действие Изменено  
        _unitActionSystem.OnBusyChanged += UnitActionSystem_OnBusyChanged; // Занятость Изменена        
        _turnSystem.OnTurnChanged += TurnSystem_OnTurnChanged; // Изменен номер хода подписываемся       

        SetupEventSelectedUnit(_unitActionSystem.GetSelectedUnit()); // Настройка Event у выбранного Юнита
        CreateUnitActionButtons();// Создать Кнопки для Действий Юнита        
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateButtonVisibility();
    }

    private void UpdateButtonVisibility() // Обновление визуализации кнопок в зависимости от того ЧЕЙ ХОД (прятать во время врага)
    {
        bool isBusy = !_turnSystem.IsPlayerTurn(); // Занято когда ходит враг (НЕ Я)

        foreach (ActionButtonUI actionButtonUI in _actionButtonUIList) // В цикле обработаем состояние кнопок
        {
            actionButtonUI.HandleStateButton(isBusy);
        }

        _actionPointsText.enabled = _turnSystem.IsPlayerTurn(); // Показываем только во время МОЕГО ХОДА
        _actionPointImage.enabled = _turnSystem.IsPlayerTurn();
    }

    // скрыть кнопки когда занят действием
    private void UnitActionSystem_OnBusyChanged(object sender, UnitActionSystem.OnUnitSystemEventArgs e)
    {
        if (e.selectedAction is GrappleAction grappleAction) // Если выполняется Комбо Сделаем проверку состояний комбо
        {
            switch (grappleAction.GetState())
            {
                case GrappleAction.State.ComboSearchEnemy: // Если ишу врага то кнопки должны быть скрытыми
                case GrappleAction.State.ComboStart:
                    return; // выходим и игнорируем код ниже
            }
        }

        foreach (ActionButtonUI actionButtonUI in _actionButtonUIList) // В цикле обработаем состояние кнопок
        {
            actionButtonUI.HandleStateButton(e.isBusy);
        }
    }

    private void CreateUnitActionButtons() // Создать Кнопки для Действий Юнита 
    {
        _actionButtonUIList.Clear(); // Очистим сисок кнопок     

        _moveActionButton.SetMoveAction(_selectedUnit.GetAction<MoveAction>(), _unitActionSystem);
        _actionButtonUIList.Add(_moveActionButton);

        PlacedObjectTypeWithActionSO mainWeaponSO = _selectedUnit.GetUnitEquipment().GetPlacedObjectMainWeaponSlot(); // У выбранного юнита, в слоте ОСНОВНОГО ОРУЖИЯ, получим тип объекта
        if (mainWeaponSO != null)
        {
            BaseAction baseAction = mainWeaponSO.GetAction(_selectedUnit); // Получим у выбранного юнита, Базовое Действие для данного типа PlacedObjectTypeWithActionSO
            _mainWeaponButton.SetBaseActionAndplacedObjectTypeWithActionSO(baseAction, mainWeaponSO, _unitActionSystem);
            _actionButtonUIList.Add(_mainWeaponButton); // Добавим в список полученный компонент ActionButtonUI
        }
        else
        {
            _mainWeaponButton.InteractableDesabled();
        }

        PlacedObjectTypeWithActionSO otherWeaponSO = _selectedUnit.GetUnitEquipment().GetPlacedObjectOtherWeaponSlot(); // У выбранного юнита, в слоте ОСНОВНОГО ОРУЖИЯ, получим тип объекта
        if (otherWeaponSO != null)
        {
            BaseAction baseAction = otherWeaponSO.GetAction(_selectedUnit); // Получим Базовое Действие для данного типа PlacedObjectTypeWithActionSO
            _otherWeaponButton.SetBaseActionAndplacedObjectTypeWithActionSO(baseAction, otherWeaponSO, _unitActionSystem);
            _actionButtonUIList.Add(_otherWeaponButton); // Добавим в список полученный компонент ActionButtonUI
        }
        else
        {
            _otherWeaponButton.InteractableDesabled();
        }


        foreach (Transform buttonTransform in _grenadeButtonContainer) // Очистим контейнер с кнопками
        {
            Destroy(buttonTransform.gameObject); // Удалим игровой объект прикрипленный к Transform
        }      
               

        foreach (GrenadeTypeSO grenadeTypeSO in _selectedUnit.GetUnitEquipment().GetGrenadeTypeSOList())// Переберем гранаты в багаже и заполним словарь
        {
            GrenadeType grenadeType = grenadeTypeSO.GetGrenadeType();

            if (!_grenadeTypeCountDict.ContainsKey(grenadeType))
            {
                _grenadeTypeCountDict.Add(grenadeType, 1);
            }
            else
            {
                _grenadeTypeCountDict[grenadeType] += 1;
            }

            
        }

        // создадим кнопку для данного типа гранаты и передадим количество

            ///  ActionButtonUI grenadeFragButton = Instantiate<ActionButtonUI>(GameAssets.Instance.actionButtonUI, _grenadeButtonContainer);

        /* foreach (BaseAction baseAction in _selectedUnit.GetBaseActionsArray()) // В цикле переберем массив базовых действий у выбранного юнита
         {
             Transform actionButtonTransform = Instantiate(GameAssets.Instance.actionButtonUI, _grenadeButtonContainer); // Для каждого baseAction создадим префаб кнопки и назначим родителя - Контейнер для кнопок
             ActionButtonUI actionButtonUI = actionButtonTransform.CreateInstanceClass<ActionButtonUI>(); // У кнопки найдем компонент ActionButtonUI
             actionButtonUI.SetBaseAction(baseAction, _unitActionSystem); //Назвать и Присвоить базовое действие (нашей кнопке)

             MouseEnterExitEventsUI mouseEnterExitEvents = actionButtonTransform.CreateInstanceClass<MouseEnterExitEventsUI>(); // Найдем на кнопке компонент - События входа и выхода мышью 
             mouseEnterExitEvents.OnMouseEnter += (object sender, EventArgs e) => // Подпишемся на событие - ПРИ ВХОДЕ мыши на кнопку. Функцию будем объявлять АНАНИМНО через лямбду () => {...} 
             {
                 _tooltipUI.ShowAnchoredShortTooltip(baseAction.GetToolTip(), (RectTransform)actionButtonTransform); // При наведении на кнопку покажем подсказку и передадим текст
             };
             mouseEnterExitEvents.OnMouseExit += (object sender, EventArgs e) => // Подпишемся на событие - ПРИ ВЫХОДЕ мыши из кнопки.
             {
                 _tooltipUI.Hide(); // При отведении мыши скроем подсказку
             };

             _actionButtonUIList.Add(actionButtonUI); // Добавим в список полученный компонент ActionButtonUI
         }*/
    }

    private void SelectedUnit_OnActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, Unit selectedUnit)
    {
        SetupEventSelectedUnit(selectedUnit);   // Настройка Event у выбранного Юнита
        CreateUnitActionButtons();  // Создать Кнопки для Действий Юнита 
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    private void SetupEventSelectedUnit(Unit newSelectedUnit) // Настройка Event у выбранного Юнита
    {
        if (_selectedUnit != null) //Если есть выбранный юнит то отпишемся от него.  (при первом запуске выбранный юнит не назначен и нет подписки)
        {
            _selectedUnit.GetActionPointsSystem().OnActionPointsChanged -= SelectedUnit_OnActionPointsChanged;
        }
        //Получим нового выбранного юнита и подпишемся на него        
        _selectedUnit = newSelectedUnit;
        _selectedUnit.GetActionPointsSystem().OnActionPointsChanged += SelectedUnit_OnActionPointsChanged; // У выбранного юнита изменились очки действия
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs empty)
    {
        UpdateSelectedVisual();
    }


    private void UpdateSelectedVisual() //Обнавление визуализации выбора( при выборе кнопки включим рамку)
    {
        foreach (ActionButtonUI actionButtonUI in _actionButtonUIList)
        {
            actionButtonUI.UpdateSelectedVisual();
        }
    }

    private void UpdateActionPoints() // Обнавление очков действий (над кнопками действий)
    {
        _actionPointsText.text = " " + _selectedUnit.GetActionPointsSystem().GetActionPointsCount(); //Изменим текст добавив в него количество очков
    }

}

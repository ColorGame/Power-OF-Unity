using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; 


/// <summary>
/// Система управления КНОПКАМИ ДЕЙСТВИЙ
/// </summary>
/// <remarks>
/// Динамически создавать кнопки при выборе юнита. 
/// </remarks>
public class ActionButtonSystemUI : MonoBehaviour 
{    
    [SerializeField] private Transform _actionButtonContainerTransform; // В инспекторе назначить  Контейнер для кнопок( находиться в сцене в Canvas)   
    [SerializeField] private TextMeshProUGUI _actionPointsText; // Ссылка на текст очков над кнопками
    [SerializeField] private Image _actionPointImage; // Картинка головы

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

        SetupEventSelectedUnit(); // Настройка Event у выбранного Юнита
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
       
        _actionPointsText.gameObject.SetActive(_turnSystem.IsPlayerTurn()); // Показываем только во время МОЕГО ХОДА
        _actionPointImage.gameObject.SetActive(_turnSystem.IsPlayerTurn());
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
        foreach (Transform buttonTransform in _actionButtonContainerTransform) // Очистим контейнер с кнопками
        {
            Destroy(buttonTransform.gameObject); // Удалим игровой объект прикрипленный к Transform
        }

        _actionButtonUIList.Clear(); // Очистим сисок кнопок     

        foreach (BaseAction baseAction in _selectedUnit.GetBaseActionsList()) // В цикле переберем массив базовых действий у выбранного юнита
        {
            Transform actionButtonTransform = Instantiate(GameAssets.Instance.actionButtonUIPrefab, _actionButtonContainerTransform); // Для каждого baseAction создадим префаб кнопки и назначим родителя - Контейнер для кнопок
            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>(); // У кнопки найдем компонент ActionButtonUI
            actionButtonUI.SetBaseAction(baseAction, _unitActionSystem); //Назвать и Присвоить базовое действие (нашей кнопке)

            MouseEnterExitEventsUI mouseEnterExitEvents = actionButtonTransform.GetComponent<MouseEnterExitEventsUI>(); // Найдем на кнопке компонент - События входа и выхода мышью 
            mouseEnterExitEvents.OnMouseEnter += (object sender, EventArgs e) => // Подпишемся на событие - ПРИ ВХОДЕ мыши на кнопку. Функцию будем объявлять АНАНИМНО через лямбду () => {...} 
            {
                _tooltipUI.ShowAnchoredTooltip(baseAction.GetToolTip(), (RectTransform)actionButtonTransform); // При наведении на кнопку покажем подсказку и передадим текст
            };
            mouseEnterExitEvents.OnMouseExit += (object sender, EventArgs e) => // Подпишемся на событие - ПРИ ВЫХОДЕ мыши из кнопки.
            {
                _tooltipUI.Hide(); // При отведении мыши скроем подсказку
            };

            _actionButtonUIList.Add(actionButtonUI); // Добавим в список полученный компонент ActionButtonUI
        }
    }

    private void SelectedUnit_OnActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e) 
    {
        SetupEventSelectedUnit();   // Настройка Event у выбранного Юнита
        CreateUnitActionButtons();  // Создать Кнопки для Действий Юнита 
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    private void SetupEventSelectedUnit() // Настройка Event у выбранного Юнита
    {
        if (_selectedUnit != null) //Если есть выбранный юнит то отпишемся от него.  (при первом запуске выбранный юнит не назначен и нет подписки)
        {
            _selectedUnit.GetActionPointsSystem().OnActionPointsChanged -= SelectedUnit_OnActionPointsChanged;
        }
        //Получим нового выбранного юнита и подпишемся на него
        Unit newSelectedUnit = _unitActionSystem.GetSelectedUnit();
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

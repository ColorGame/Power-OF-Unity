using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnitActionSystem;

/// <summary>
/// Система кнопок - выбора ЮНИТА на игровом уровне.
/// </summary>
/// <remarks>
/// Динамически создавать кнопки и обновлять из визуал. 
/// </remarks>
public class UnitSelectAtLevelButtonsSystemUI : MonoBehaviour
{    
    [SerializeField] private Transform _enemyUnitButonContainerTransform; // В инспекторе назначить  Контейнер для кнопок( находиться в сцене в Canvas)
    [SerializeField] private Transform _friendlyUnitButonContainerTransform; // В инспекторе назначить  Контейнер для кнопок( находиться в сцене в Canvas)

    private List<UnitSelectAtLevelButtonUI> _friendlyUnitButtonList; // Ключ - юнит. Значение - кнопка для этого юнита
    private List<UnitEnemySelectAtLevelButtonUI> _enemyUnitButonUIList; // Список кнопок Вражеских Юнитов   
    private UnitManager _unitManager;
    private TurnSystem _turnSystem;
    private UnitActionSystem _unitActionSystem;
    private CameraFollow _cameraFollow;

    public void Init(UnitManager unitManager, TurnSystem turnSystem, UnitActionSystem unitActionSystem, CameraFollow cameraFollow)
    {
        _unitManager = unitManager;
        _turnSystem = turnSystem;
        _unitActionSystem = unitActionSystem;
        _cameraFollow = cameraFollow;

        Setup();
    }

    private void Awake()
    {
        _friendlyUnitButtonList = new List<UnitSelectAtLevelButtonUI>();
        _enemyUnitButonUIList = new List<UnitEnemySelectAtLevelButtonUI>();     
    }

    private void Setup()
    {
        _unitManager.OnAnyUnitDeadAndRemoveList += UnitManager_OnAnyUnitDeadAndRemoveList;// Событие Любой Юнит Умер И Удален из Списка
        _unitManager.OnAnyEnemyUnitSpawnedAndAddList += UnitManager_OnAnyEnemyUnitSpawnedAndAddList;// Любой вражеский юнит ражден и добавлен в Списка

        _turnSystem.OnTurnChanged += TurnSystem_OnTurnChanged; // Изменен номер хода подписываемся на Event // Будет выполняться (изминение текста очков действий).
        _unitActionSystem.OnBusyChanged += UnitActionSystem_OnBusyChanged; // Занятость Изменена
        _unitActionSystem.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged; //Выбранный Юнит Изменен       

        CreateEnemyUnitButtons(); // Создать Кнопки для  Юнитов
        CreateFriendlyUnitButtons();
        UpdateSelectedVisual(_unitActionSystem.GetSelectedUnit()); 
    }

    private void UnitManager_OnAnyEnemyUnitSpawnedAndAddList(object sender, EventArgs e)
    {
        CreateEnemyUnitButtons();
    }

    private void UnitManager_OnAnyUnitDeadAndRemoveList(object sender, EventArgs e)
    {
        CreateEnemyUnitButtons();
        CreateFriendlyUnitButtons();
    }  

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateButtonVisibility();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, Unit selectedUnit) //sender - отправитель // Подписка должна иметь туже сигнатуру что и функция отправителя OnSelectedUnitChanged
    {
        UpdateSelectedVisual(selectedUnit);
    }

    private void UnitActionSystem_OnBusyChanged(object sender, BusyChangedEventArgs e)
    {
        if (e.selectedAction is GrappleAction comboAction) // Если выполняется Комбо Сделаем проверку состояний комбо
        {
            switch (comboAction.GetState())
            {
                case GrappleAction.State.ComboSearchEnemy: // Если ишу врага то кнопки должны быть скрытыми
                case GrappleAction.State.ComboStart:
                    return; // выходим и игнорируем код ниже
            }
        }

        foreach (UnitSelectAtLevelButtonUI friendlyUnitButonUI in _friendlyUnitButtonList)// Переберем коллекцию
        {
            friendlyUnitButonUI.HandleStateButton(e.isBusy);
        }
    }
     
    private void CreateEnemyUnitButtons() // Создать Кнопки для Врагов Юнитов
    {
        foreach (Transform buttonTransform in _enemyUnitButonContainerTransform) // Очистим контейнер с кнопками
        {
            Destroy(buttonTransform.gameObject); // Удалим игровой объект прикрипленный к Transform
        }

        _enemyUnitButonUIList.Clear(); // Очистим сисок кнопок

        foreach (Unit unit in _unitManager.GetUnitEnemyList())// Переберем вражеских юнитов
        {
            if (unit.GetTransform().gameObject.activeSelf) // Если юнит активный то 
            {
                UnitEnemySelectAtLevelButtonUI unitEnemySelectAtLevelButton = Instantiate(GameAssetsSO.Instance.unitEnemySelectAtLevelButton, _enemyUnitButonContainerTransform); // Для каждого ЮНИТА создадим префаб кнопки и назначим родителя - Контейнер для кнопок               
                unitEnemySelectAtLevelButton.Init(unit, _cameraFollow);//Назвать и Присвоить

                _enemyUnitButonUIList.Add(unitEnemySelectAtLevelButton);// Добавим в список нашу кнопку
            }
        }
    }

    private void CreateFriendlyUnitButtons() // Создать Кнопки для Дружественныйх Юнитов
    {
        foreach (Transform buttonTransform in _friendlyUnitButonContainerTransform) // Очистим контейнер с кнопками
        {
            Destroy(buttonTransform.gameObject); // Удалим игровой объект прикрипленный к Transform
        }

        _friendlyUnitButtonList.Clear();

        foreach (Unit unit in _unitManager.GetUnitList())// Переберем дружественных юнитов
        {
            UnitSelectAtLevelButtonUI UnitSelectAtLevelButton = Instantiate(GameAssetsSO.Instance.unitFriendSelectAtLevelButton, _friendlyUnitButonContainerTransform); // Для каждого ЮНИТА создадим префаб кнопки и назначим родителя - Контейнер для кнопок
            UnitSelectAtLevelButton.Init(unit, _unitActionSystem,_cameraFollow);//Назвать и Присвоить

            _friendlyUnitButtonList.Add(UnitSelectAtLevelButton);     
        }
    }

    private void UpdateSelectedVisual(Unit selectedUnit) //Обнавление визуализации выбора( при выборе кнопки включим рамку)
    {
        foreach (UnitSelectAtLevelButtonUI friendlyUnitButonUI in _friendlyUnitButtonList) // Переберем коллекцию
        {
            friendlyUnitButonUI.UpdateSelectedVisual(selectedUnit);
        }
    }
    private void UpdateButtonVisibility() // Обновление визуализации кнопок в зависимости от того ЧЕЙ ХОД (прятать во время хода врага)
    {
        bool isBusy = !_turnSystem.IsPlayerTurn(); // Занято когда ходит враг (НЕ Я)

        foreach (UnitSelectAtLevelButtonUI friendlyUnitButonUI in _friendlyUnitButtonList) // Переберем коллекцию
        {
            friendlyUnitButonUI.HandleStateButton(isBusy);
        }
    }

    private void OnDestroy()
    {
        _unitManager.OnAnyUnitDeadAndRemoveList -= UnitManager_OnAnyUnitDeadAndRemoveList;
        _unitManager.OnAnyEnemyUnitSpawnedAndAddList -= UnitManager_OnAnyEnemyUnitSpawnedAndAddList;
        _turnSystem.OnTurnChanged -= TurnSystem_OnTurnChanged; 
        _unitActionSystem.OnBusyChanged -= UnitActionSystem_OnBusyChanged; 
        _unitActionSystem.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;   
    }
}

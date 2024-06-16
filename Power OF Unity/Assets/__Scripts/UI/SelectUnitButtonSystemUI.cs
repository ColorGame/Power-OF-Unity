using System;
using System.Collections.Generic;
using UnityEngine;
using static UnitActionSystem;

/// <summary>
/// Система управления КНОПКАМИ ВЫБОРА ЮНИТОВ
/// </summary>
/// <remarks>
/// Динамически создавать кнопки и обновлять из визуал. 
/// </remarks>
public class SelectUnitButtonSystemUI : MonoBehaviour
{
    [SerializeField] private Transform _enemyUnitButonUIPrefab; // В инспекторе закинем префаб Кнопки
    [SerializeField] private Transform _enemyUnitButonContainerTransform; // В инспекторе назначить  Контейнер для кнопок( находиться в сцене в Canvas)
    [SerializeField] private Transform _friendlyUnitButonPrefab; // В инспекторе закинем префаб Кнопки
    [SerializeField] private Transform _friendlyUnitButonContainerTransform; // В инспекторе назначить  Контейнер для кнопок( находиться в сцене в Canvas)

    private Dictionary<Unit, SelectUnitFriendlyButtonUI> _friendlyUnitButtonDictionary; // Ключ - юнит. Значение - кнопка для этого юнита
    private List<SelectUnitEnemyButtonUI> _enemyUnitButonUIList; // Список кнопок Вражеских Юнитов   
    private UnitManager _unitManager;
    private TurnSystem _turnSystem;
    private UnitActionSystem _unitActionSystem;

    public void Init(UnitManager unitManager, TurnSystem turnSystem, UnitActionSystem unitActionSystem)
    {
        _unitManager = unitManager;
        _turnSystem = turnSystem;
        _unitActionSystem = unitActionSystem;
    }

    private void Awake()
    {
        _friendlyUnitButtonDictionary = new Dictionary<Unit, SelectUnitFriendlyButtonUI>();
        _enemyUnitButonUIList = new List<SelectUnitEnemyButtonUI>();     
    }

    private void Start()
    {
        _unitManager.OnAnyUnitDeadAndRemoveList += UnitManager_OnAnyUnitDeadAndRemoveList;// Событие Любой Юнит Умер И Удален из Списка
        _unitManager.OnAnyEnemyUnitSpawnedAndAddList += UnitManager_OnAnyEnemyUnitSpawnedAndAddList;// Любой вражеский юнит ражден и добавлен в Списка

        _turnSystem.OnTurnChanged += TurnSystem_OnTurnChanged; // Изменен номер хода подписываемся на Event // Будет выполняться (изминение текста очков действий).
        _unitActionSystem.OnBusyChanged += UnitActionSystem_OnBusyChanged; // Занятость Изменена
        _unitActionSystem.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged; //Выбранный Юнит Изменен       

        CreateEnemyUnitButtons(); // Создать Кнопки для  Юнитов
        CreateFriendlyUnitButtons();
        UpdateSelectedVisual();
    }

    private void UnitManager_OnAnyEnemyUnitSpawnedAndAddList(object sender, EventArgs e)
    {
        CreateEnemyUnitButtons();
    }

    private void UnitManager_OnAnyUnitDeadAndRemoveList(object sender, EventArgs e)
    {
        CreateEnemyUnitButtons();
    }  

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateButtonVisibility();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e) //sender - отправитель // Подписка должна иметь туже сигнатуру что и функция отправителя OnSelectedUnitChanged
    {
        UpdateSelectedVisual();
    }

    private void UnitActionSystem_OnBusyChanged(object sender, OnUnitSystemEventArgs e)
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

        foreach (SelectUnitFriendlyButtonUI friendlyUnitButonUI in _friendlyUnitButtonDictionary.Values)// Переберем коллекцию, значения из словаря
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
                Transform actionButtonTransform = Instantiate(_enemyUnitButonUIPrefab, _enemyUnitButonContainerTransform); // Для каждого ЮНИТА создадим префаб кнопки и назначим родителя - Контейнер для кнопок
                SelectUnitEnemyButtonUI enemyUnitButonUI = actionButtonTransform.GetComponent<SelectUnitEnemyButtonUI>();// У кнопки найдем компонент SelectUnitEnemyButtonUI
                enemyUnitButonUI.Init(unit);//Назвать и Присвоить


                _enemyUnitButonUIList.Add(enemyUnitButonUI);// Добавим в список нашу кнопку
            }
        }
    }

    private void CreateFriendlyUnitButtons() // Создать Кнопки для Дружественныйх Юнитов
    {
        foreach (Transform buttonTransform in _friendlyUnitButonContainerTransform) // Очистим контейнер с кнопками
        {
            Destroy(buttonTransform.gameObject); // Удалим игровой объект прикрипленный к Transform
        }

        _friendlyUnitButtonDictionary.Clear();

        foreach (Unit unit in _unitManager.GetUnitFriendList())// Переберем дружественных юнитов
        {
            Transform actionButtonTransform = Instantiate(_friendlyUnitButonPrefab, _friendlyUnitButonContainerTransform); // Для каждого ЮНИТА создадим префаб кнопки и назначим родителя - Контейнер для кнопок
            SelectUnitFriendlyButtonUI selectUnitFriendlyButtonUI = actionButtonTransform.GetComponent<SelectUnitFriendlyButtonUI>();// У кнопки найдем компонент SelectUnitFriendlyButtonUI
            selectUnitFriendlyButtonUI.Init(unit, _unitActionSystem);//Назвать и Присвоить

            _friendlyUnitButtonDictionary[unit] = selectUnitFriendlyButtonUI; // Ключ - ЮНИТ. Значение - SelectUnitFriendlyButtonUI        
        }
    }

    private void UpdateSelectedVisual() //Обнавление визуализации выбора( при выборе кнопки включим рамку)
    {
        foreach (SelectUnitFriendlyButtonUI friendlyUnitButonUI in _friendlyUnitButtonDictionary.Values) // Переберем коллекцию, значения из словаря
        {
            friendlyUnitButonUI.UpdateSelectedVisual();
        }
    }
    private void UpdateButtonVisibility() // Обновление визуализации кнопок в зависимости от того ЧЕЙ ХОД (прятать во время хода врага)
    {
        bool isBusy = !_turnSystem.IsPlayerTurn(); // Занято когда ходит враг (НЕ Я)

        foreach (SelectUnitFriendlyButtonUI friendlyUnitButonUI in _friendlyUnitButtonDictionary.Values) // Переберем коллекцию, значения из словаря
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

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Основной класс Юнита без MonoBehaviour
/// </summary>
/// <remarks>
/// Инициализирует подсистемы юнита
/// </remarks>
public class Unit
{
    public Unit(UnitTypeSO unitTypeSO, SoundManager soundManager)
    {
        _healthSystem = new Health(_unitTypeSO.GetBasicHealth(), soundManager);
        _actionPointsSystem = new ActionPoints(this);
        _baseActionsList = new List<BaseAction>();

        switch (unitTypeSO)
        {
            case UnitFriendSO unitFriendSO:
                _unitTypeSO = unitFriendSO;
                _unitAmorType = unitFriendSO.GetUnitArmorType();
                _isEnemy = false;
                break;
            case UnitEnemySO unitEnemySO:
                _unitTypeSO = unitEnemySO;
                _baseActionsList = unitEnemySO.GetBaseActionsList();
                _isEnemy = true;
                break;
        }

        _unitInventory = new UnitInventory(this);
        _unitEquipment = new UnitEquipment(this);
                
        _soundManager = soundManager;
    }


    public static event EventHandler OnAnyEnemyUnitSpawned; // Событие Любой Вражеский Рожденный(созданный) Юнит
    public static event EventHandler OnAnyUnitDead; // Событие Любой Юнит Умер    

    // Частные случаи
    private bool _isEnemy;
    private GridPositionXZ _gridPosition;

    private UnitTypeSO _unitTypeSO;
    private Health _healthSystem;
    private ActionPoints _actionPointsSystem;
    private UnitInventory _unitInventory;
    private UnitEquipment _unitEquipment;
    private List<BaseAction> _baseActionsList; // Список базовых действий // Будем использовать при создании кнопок   
    private UnitArmorType _unitAmorType; //Тип брони юнита        
    private Transform _unitCoreTransform;

    private LevelGrid _levelGrid;
    private TurnSystem _turnSystem;
    private SoundManager _soundManager;
    private UnitActionSystem _unitActionSystem;

    /// <summary>
    /// Настройка ЮНИТА при спанвне на Уровня. (Настроим transform и gridPosition ...)
    /// </summary>   
    public void SetupForSpawn( LevelGrid levelGrid, TurnSystem turnSystem, Transform unitCoreTransform, CameraFollow cameraFollow, UnitActionSystem unitActionSystem)
    {
        // Когда Unit спавниться, настроим его положение в сетке и добовим к GridObjectUnitXZ(объектам сетки) в данной ячейки         
        
        _levelGrid = levelGrid;
        _turnSystem = turnSystem;
        _unitActionSystem = unitActionSystem;
        _gridPosition = _levelGrid.GetGridPosition(unitCoreTransform.position); //Получим сеточную позицию в месте спавна
        _levelGrid.AddUnitAtGridPosition(_gridPosition, this); //Добавим юнита в нашу сетку
        _unitCoreTransform = unitCoreTransform;

        MoveAction moveAction = unitCoreTransform.GetComponent<MoveAction>();
        UnitWorldUI unitWorldUI = unitCoreTransform.GetComponentInChildren<UnitWorldUI>(true);
        LookAtCamera lookAtCamera = unitCoreTransform.GetComponentInChildren<LookAtCamera>(true);
        HandleAnimationEvents handleAnimationEvents = unitCoreTransform.GetComponentInChildren<HandleAnimationEvents>(true);
        UnitRagdollSpawner unitRagdollSpawner = unitCoreTransform.GetComponentInChildren<UnitRagdollSpawner>(true);

        _baseActionsList.Add(moveAction); // Добавим передвижение в базовые действия
        moveAction.SetupForSpawn(this, unitActionSystem, _unitTypeSO.GetBasicMoveDistance());
        unitWorldUI.SetupForSpawn(this, unitActionSystem, _turnSystem);
        lookAtCamera.SetupForSpawn(cameraFollow);
        handleAnimationEvents.SetupForSpawn(this);
        unitRagdollSpawner.SetupForSpawn(this);
        _healthSystem.SetupForSpawn();
        _actionPointsSystem.SetupForSpawn(_turnSystem);

        _healthSystem.OnDead += HealthSystem_OnDead; // подписываемся на Event. Будет выполняться при смерти юнита       

        if (_isEnemy)
            OnAnyEnemyUnitSpawned?.Invoke(this, EventArgs.Empty); // Запустим событие Любой Рожденный(созданный) Вражеский Юнит. Событие статичное поэтому будет выполняться для всех вражеских созданных Юнитов
    }

    /// <summary>
    ///  Настройка ЮНИТА при смерти удалении или выходе из игровой сцены.
    /// </summary>
    private void SetupOnDestroyAndQuit()
    {
        _levelGrid.GetGridPosition(GetTransformPosition());
        _levelGrid.RemoveUnitAtGridPosition(_gridPosition, this);
        _actionPointsSystem.SetupOnDestroyAndQuit();

        _healthSystem.OnDead -= HealthSystem_OnDead;        
    }

    public void AddBaseActionsList(BaseAction baseAction)
    {
        _baseActionsList.Add(baseAction);
    }
    public void RemoveBaseActionsList(BaseAction baseAction)
    {
        _baseActionsList.Remove(baseAction);
    }

    public void UpdateGridPosition()
    {
        GridPositionXZ newGridPosition = _levelGrid.GetGridPosition(GetTransformPosition()); //Получим новую позицию юнита на сетке.
        if (newGridPosition != _gridPosition) // Если новая позиция на сетке отличается от последней то ...
        {
            // Изменем положение юнита на сетке
            GridPositionXZ oldGridPosition = _gridPosition; // Сохраним старую позицию что бы передать в event
            _gridPosition = newGridPosition; //Обновим позицию - Новая позиция становиться текущей

            _levelGrid.UnitMovedGridPosition(this, oldGridPosition, newGridPosition); //в UnitMovedGridPosition запускаем Событие. Поэтому эту строку поместим в КОНЦЕЦ . Иначе мы запускаем событие сетка обнавляется а юнит еще не перемещен
        }
    }

    public T GetAction<T>() where T : BaseAction //Фунцкция для получения любого типа базового действия // Создадим метод с GENERICS и ограничим типами в  BaseAction
    {
        foreach (BaseAction baseAction in _baseActionsList) // Переберем Массив базовых действий
        {
            if (baseAction is T) // если T совподает с каким нибудь baseAction то ...
            {
                return baseAction as T; // Вернем это базовое действие КАК Т // (T)baseAction; - еще один метод записи
            }
        }
        return null; // Если нет совпадений то вернем ноль
    }

    public GridPositionXZ GetGridPosition() // Получить сеточную позицию
    {
        return _gridPosition;
    }

    public Vector3 GetWorldPosition() // Получить мировую позицию
    {
        return GetTransformPosition();
    }

    public List<BaseAction> GetBaseActionsList() // Получить Список базовых действий
    {
        return _baseActionsList;
    }

    public bool IsEnemy() // Раскроем поле
    {
        return _isEnemy;
    }

    private void HealthSystem_OnDead(object sender, EventArgs e) // Будет выполняться при смерти юнита
    {
        SetupOnDestroyAndQuit();
        UnityEngine.Object.Destroy(_unitCoreTransform.gameObject); // Уничтожим игровой объект к которому прикриплен данный скрипт

        // Вслучае смерти активного Юнита надо предать ход следующему юниту это происходит в UnitActionSystem    

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty); // Запустим событие Любой Мертвый Юнит. Событие статичное поэтому будет выполняться для любого мертвого Юнита      
    }




    public Health GetHealthSystem() { return _healthSystem; }
    public ActionPoints GetActionPointsSystem() { return _actionPointsSystem; }
    public UnitInventory GetUnitInventory() { return _unitInventory; }
    public UnitEquipment GetUnitEquipment() { return _unitEquipment; }
    public Vector3 GetTransformPosition() { return _unitCoreTransform.position; }
    public void SetTransformPosition(Vector3 position) {  _unitCoreTransform.position = position; }
    public Transform GetTransform() { return _unitCoreTransform; }
    public void SetTransformForward(Vector3 forward) {  _unitCoreTransform.forward = forward; }
    public T GetUnitTypeSO<T>() where T : UnitTypeSO //Фунцкция для получения любого наследника от UnitTypeSO
    {
        if (_unitTypeSO is T unit_T_SO)
        {
            return unit_T_SO;
        }
        return null; // Если нет совпадений то вернем ноль
    }
    public UnitArmorType GetUnitArmorType() { return _unitAmorType; }
    public TurnSystem GetTurnSystem() { return _turnSystem; }
    public SoundManager GetSoundManager() { return _soundManager; }
    public UnitActionSystem GetUnitActionSystem() { return _unitActionSystem; } 
    public LevelGrid GetLevelGrid() { return _levelGrid;}

}


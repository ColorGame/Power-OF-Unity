using System;
using UnityEngine;

/// <summary>
/// ОЧКИ ДЕЙСТВИЯ юнита
/// </summary>
public class UnitActionPoints
{
    /// <summary>
    /// ОЧКИ ДЕЙСТВИЯ юнита
    /// </summary>
    public UnitActionPoints(Unit unit, int actionPoints)
    {
        _unit = unit;
        _actionPoints = actionPoints;
        _actionPointsFull = _actionPoints;
    }

    public event EventHandler OnActionPointsChanged;  // изменении очков действий.         


    private int _actionPoints; // Очки действия
    private int _actionPointsFull; //Полное количество очков действия
    private float _penaltyStunPercent;  // Штрафной процент оглушения (будем применять в след ход)
    private bool _stunned = false; // Оглушенный(по умолчанию ложь)
    private Unit _unit;

    /*private int _startStunTurnNumber; //Номер очереди (хода) при старте события оглушения
   private int _durationStunEffectTurnNumber; // Продолжительность оглушающего эффекта Количество ходов*/

    private TurnSystem _turnSystem;

    public void SetupForSpawn()
    {
        _turnSystem = _unit.GetTurnSystem();
        _turnSystem.OnTurnChanged += TurnSystem_OnTurnChanged; // Подпиш. на событие Ход Изменен    
    }

    /// <summary>
    ///  Настройка ЮНИТА при смерти или удалении.
    /// </summary>
    public void SetupOnDestroyAndQuit()
    {
        _turnSystem.OnTurnChanged -= TurnSystem_OnTurnChanged;
    }
    /// <summary>
    /// ПОПРОБУЕМ Потратить Очки Действия, Чтобы Выполнить Действие
    /// </summary>
    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction) // Этот метод выполняет вместе два нижних метода
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointCost());
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Мы МОЖЕМ Потратить Очки Действия, Чтобы Выполнить Действие ? 
    /// </summary>
    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (_actionPoints >= baseAction.GetActionPointCost()) // Если очков действия хватает то...
        {
            return true; // Можем выполнить действие
        }
        else
        {
            return false; // Увы очков не хватает
        }

        /*// Альтернативная запись кода выше
        return _actionPoints >= baseAction.GetActionPointCost();*/
    }
    /// <summary>
    /// Потратить очки действий (amount- количество которое надо потратить)
    /// </summary>
    public void SpendActionPoints(int amount)
    {
        _actionPoints -= amount;

        OnActionPointsChanged?.Invoke(this, EventArgs.Empty); // запускаем событие ПОСЛЕ обнавления очков действий.(для // РЕШЕНИЕ // 2 //в ActionButtonSystemUI)
    }

    /// <summary>
    /// Ход изменен Сбросим очки действий до полного
    /// </summary>
    public void TurnSystem_OnTurnChanged(object sender, EventArgs empty)
    {
        if ((_unit.GetIsEnemy() && !_turnSystem.IsPlayerTurn()) || // Если это враг И его очередь (НЕ очередь игрока) ИЛИ это НЕ враг(игрок) и очередь игрока то...
            (!_unit.GetIsEnemy() && _turnSystem.IsPlayerTurn()))
        {
            _actionPoints = _actionPointsFull;

            if (_penaltyStunPercent != 0)
            {
                _actionPoints -= Mathf.RoundToInt(_actionPoints * _penaltyStunPercent); // Применим штраф
                _penaltyStunPercent = 0;
                SetStunned(false); // Отключим оглушение
            }

            //  БОлее сложная распространяется на след ход
            /*int passedTurnNumber = _turnSystem.GetTurnNumber() - _startStunTurnNumber;// прошло ходов от начала Оглушения
            if (passedTurnNumber <= _durationStunEffectTurnNumber) // Если ходов прошло меньше или равно длительности ОГЛУШЕНИЯ (Значит оглушение еще действует)
            {
                _actionPoints -= Mathf.RoundToInt(_actionPoints * _penaltyStunPercent); // Применим штраф
                _penaltyStunPercent = _penaltyStunPercent *0.3f; // Уменьшим штраф оставим 30% от изначального (это надо Если оглушение длиться несколько ходов)
            }
            if (passedTurnNumber > _durationStunEffectTurnNumber) //Если ходов прошло больше продолжительности ОГЛУШЕНИЯ
            {
                SetStunned(false); // Отключим оглушение
                _penaltyStunPercent = 0; // И обнулим штраф
            }  */

            OnActionPointsChanged?.Invoke(this, EventArgs.Empty); // запускаем событие ПОСЛЕ обнавления очков действий.(для // РЕШЕНИЕ // 2 //в ActionButtonSystemUI)
        }
    }
    /// <summary>
    /// Оглушить на stunPercent(процент оглушения)
    /// </summary>
    public void Stun(float stunPercent)
    {
        SetStunned(true);
        _penaltyStunPercent = stunPercent; // Установим Процента Оглушения

        /*// БОлее сложная распространяется на след ход
        _startStunTurnNumber = _turnSystem.GetTurnNumber(); // Получим стартовый номер хода              
        if (_actionPoints > 0) // Если очков хода больше нуля
        {
            _durationStunEffectTurnNumber = 1; //Нужно НАСТРОИТЬ// Оглушение будет длиться весь следующий ход
        }
        if(_actionPoints<=0) // Если очков хода нету
        {
            _durationStunEffectTurnNumber = 3; //Нужно НАСТРОИТЬ// Оглушение будет длиться следующие 3 хода (через ход врага)
        }*/
        OnActionPointsChanged?.Invoke(this, EventArgs.Empty); // запускаем событие ПОСЛЕ обнавления очков действий.
    }
    /// <summary>
    /// Получить количество очков действия
    /// </summary>
    public int GetActionPointsCount() { return _actionPoints; }
    public int GetActionPointsCountFull() { return _actionPointsFull; }

    public bool GetStunned() { return _stunned; }
    private void SetStunned(bool stunned) { _stunned = stunned; }
}

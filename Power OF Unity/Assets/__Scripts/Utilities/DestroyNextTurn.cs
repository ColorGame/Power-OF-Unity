using UnityEngine;

/// <summary>
/// Удаление или отключение переданного объекта через заданное количество ходов
/// </summary>
public class DestroyNextTurn 
{
    public enum State
    {
        Destroy,
        SetActive
    }

    private readonly int _turnAmountToDestroy = 2; // Количество ходов до уничтожения
    private readonly State _state; // ВЫбрать уничтожить или отключить
    private readonly TurnSystem _turnSystem;
    private readonly GameObject _gameObject;

    private int _startTurnNumber; // Номер очереди (хода) при старте 
    private int _currentTurnNumber; // Текущий номер очереди (хода) 

    /// <summary>
    /// Удаление или отключение переданного объекта через заданное количество ходов
    /// </summary>
    public DestroyNextTurn(GameObject gameObject, int turnAmountToDestroy, State state, TurnSystem turnSystem)
    {
        _gameObject = gameObject;
        _turnAmountToDestroy = turnAmountToDestroy;
        _state = state;
        _turnSystem = turnSystem;

        _startTurnNumber = _turnSystem.GetTurnNumber(); // Получим номер хода

        _turnSystem.OnTurnChanged += TurnSystem_OnTurnChanged; // Подпиш. на событие Ход Изменен
    }      

    private void TurnSystem_OnTurnChanged(object sender, System.EventArgs e)
    {
        _currentTurnNumber = _turnSystem.GetTurnNumber(); // Получим ТЕКУЩИЙ номер хода;

        if (_currentTurnNumber - _startTurnNumber == _turnAmountToDestroy)
        {
            // через _turnAmountToDestroy хода уничтожим  или отключим

            switch (_state)
            {
                case State.Destroy:
                    OnDestroy();
                    break;
                case State.SetActive:
                    _gameObject.SetActive(false);
                    break;
            }

        }
    }

    private void OnDestroy()
    {
        Object.Destroy(_gameObject);
        _turnSystem.OnTurnChanged -= TurnSystem_OnTurnChanged;
    }
}

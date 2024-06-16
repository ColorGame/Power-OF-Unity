using System;
/// <summary>
/// Система очереди (ходов) . Обрабатывает логику ходов
/// </summary>
public class TurnSystem
{   
    public event EventHandler OnTurnChanged; // Ход Изменен (когда меняется ход мы запустим событие Event)

    private int _turnNumber = 1; // Номер очереди (хода)
    private bool _isPlayerTurn = true; // Ход игрока, по умолчанию true т.к. он ходит первым

    public void NextTurn() // Следующая очередь (ход) ВЫЗЫВАЕМ ПРИ НАЖАТИИ НА Button "END TURN"
    {
        _turnNumber++; // Увеличиваем число на еденицу
        _isPlayerTurn = !_isPlayerTurn; // перевернем ход игрока. Каждый второй ход будет активна для игрока

        OnTurnChanged?.Invoke(this, EventArgs.Empty); 
    }

    public  int GetTurnNumber() //Вернуть Номер хода
    {
        return _turnNumber;
    }

    public  bool IsPlayerTurn()
    {
        return _isPlayerTurn;
    }
}

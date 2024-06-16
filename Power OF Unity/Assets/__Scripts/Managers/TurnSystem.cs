using System;
/// <summary>
/// ������� ������� (�����) . ������������ ������ �����
/// </summary>
public class TurnSystem
{   
    public event EventHandler OnTurnChanged; // ��� ������� (����� �������� ��� �� �������� ������� Event)

    private int _turnNumber = 1; // ����� ������� (����)
    private bool _isPlayerTurn = true; // ��� ������, �� ��������� true �.�. �� ����� ������

    public void NextTurn() // ��������� ������� (���) �������� ��� ������� �� Button "END TURN"
    {
        _turnNumber++; // ����������� ����� �� �������
        _isPlayerTurn = !_isPlayerTurn; // ���������� ��� ������. ������ ������ ��� ����� ������� ��� ������

        OnTurnChanged?.Invoke(this, EventArgs.Empty); 
    }

    public  int GetTurnNumber() //������� ����� ����
    {
        return _turnNumber;
    }

    public  bool IsPlayerTurn()
    {
        return _isPlayerTurn;
    }
}

using UnityEngine;

/// <summary>
/// �������� ��� ���������� ����������� ������� ����� �������� ���������� �����
/// </summary>
public class DestroyNextTurn 
{
    public enum State
    {
        Destroy,
        SetActive
    }

    private readonly int _turnAmountToDestroy = 2; // ���������� ����� �� �����������
    private readonly State _state; // ������� ���������� ��� ���������
    private readonly TurnSystem _turnSystem;
    private readonly GameObject _gameObject;

    private int _startTurnNumber; // ����� ������� (����) ��� ������ 
    private int _currentTurnNumber; // ������� ����� ������� (����) 

    /// <summary>
    /// �������� ��� ���������� ����������� ������� ����� �������� ���������� �����
    /// </summary>
    public DestroyNextTurn(GameObject gameObject, int turnAmountToDestroy, State state, TurnSystem turnSystem)
    {
        _gameObject = gameObject;
        _turnAmountToDestroy = turnAmountToDestroy;
        _state = state;
        _turnSystem = turnSystem;

        _startTurnNumber = _turnSystem.GetTurnNumber(); // ������� ����� ����

        _turnSystem.OnTurnChanged += TurnSystem_OnTurnChanged; // ������. �� ������� ��� �������
    }      

    private void TurnSystem_OnTurnChanged(object sender, System.EventArgs e)
    {
        _currentTurnNumber = _turnSystem.GetTurnNumber(); // ������� ������� ����� ����;

        if (_currentTurnNumber - _startTurnNumber == _turnAmountToDestroy)
        {
            // ����� _turnAmountToDestroy ���� ���������  ��� ��������

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

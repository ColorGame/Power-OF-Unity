using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour    //������� �������� ���� ���� ����� ����������� ������ ������ 
                                                    // �� ����� ��������� ����������� �������� ������� ��������� ���� �����. ���-�� �������� �� ������� ��������� BaseAction ������� ���
                                                    // abstract - �� ��������� ������� Instance (���������) ����� ������.
{
    public static event EventHandler OnAnyActionStart; // static - ���������� ��� event ����� ������������ ��� ����� ������ �� �������� �� ���� ������� � ��� �������� ������. ������� ��� ������������� ����� ������� ��������� �� ����� ������ �� �����-���� ���������� �������, ��� ����� �������� ������ � ������� ����� �����, ������� ����� ��������� ���� � �� �� ������� ��� ������ �������. 
                                                       // �� �������� ������� Event ���  ������� ������ ��������.
    public static event EventHandler OnAnyActionCompleted; // �� �������� ������� Event ���  ���������� ������ ��������.

    protected Unit _unit; // ���� �� ������� ����� ��������� ��������
    protected bool _isActive; // ������� ����������. ��� �� ��������� ����������� ���������� ���������� ��������

    protected SoundManager _soundManager;
    protected UnitActionSystem _unitActionSystem;
    protected LevelGrid _levelGrid;
    protected TurnSystem _turnSystem;

    //���� ������������ ���������� ������� Action ������ - //public delegate void ActionCompleteDelegate(); //���������� �������� // ��������� ������� ������� �� ��������� �������� � ���������� �������
    protected Action _onActionComplete; //(�� ���������� ��������)// �������� ������� � ������������ ���� - using System;
                                        //�������� ��� ������� ��� ������������ ���������� (� ��� ����� ��������� ������ ������� �� ���������).
                                        //Action- ���������� �������. ���� ��� �������. ������� Func<>. 
                                        //�������� ��������. ����� ���������� ������� � ������� �� �������� ��������, ����� �����, � ������������ ����� ����, ����������� ����������� �������.
                                        //�������� ��������. ����� �������� �������� ������� �� ������� ������

    public void Init(Unit unit)
    {
        _unit = unit;

        _soundManager = _unit.GetSoundManager();
        _unitActionSystem = _unit.GetUnitActionSystem();
        _levelGrid = _unit.GetLevelGrid();
        _turnSystem = _unit.GetTurnSystem();
    }
    protected virtual void Awake() // protected virtual- ���������� ��� ����� �������������� � �������� �������
    {        
        
    }

    protected virtual void Start()
    {
        
    }

    public abstract string GetActionName(); // ������� ��� �������� // abstract - ��������� ������������� � ������ ��������� � � ������� ������ ����� ������ ����.
   
    public abstract string GetToolTip(); // ������� ����������� ���������

    public abstract int GetMaxActionDistance(); // ������� ��������� ��������
    public abstract void TakeAction(GridPositionXZ gridPosition, Action onActionComplete); //Generic ��������� �������� (�����������) � �������� �������� �������� ������� ��� �������� � ������� onActionComplete (��� ���������� ��������, � ����� ������ ��� ClearBusy() // �������� ��������� ��� ����� ��������� - ������������ ������ UI ) 

    public abstract List<GridPositionXZ> GetValidActionGridPositionList(); //�������� ������ ���������� �������� ������� ��� ��������

    public virtual bool IsValidActionGridPosition(GridPositionXZ gridPosition) //(���������) �������� �� �������� ������� ���������� ��� �������� //������� virtual- ���� ������������ �������������� ��� ������
    {
        List<GridPositionXZ> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition); // ���� ���� _gridPositioAnchor ����������� � ����� ���������� �������, �� ��������  ������
    }

    public virtual int GetActionPointCost() // �������� ������ ����� �� �������� (��������� ��������) //������� virtual- ���� ������������ �������������� ��� ������ 
    {
        return 1; // �� ��������� ��������� ���� �������� =1
    }

    protected void ActionStart(Action onActionComplete) //����� �������� � �������� �������� ������� onActionComplete (��� ���������� ��������) // � ����� ���� �� ������������ ������� � ������ TakeAction() � �� ���� ��������� �������
    {
        _isActive = true;
        _onActionComplete = onActionComplete; // �������� ���������� �������

        OnAnyActionStart?.Invoke(this, EventArgs.Empty); // �������� �������  // ����� ������ ���� ��������� ����� ���� ��� �������� �������� � ��� ��������� //�������� ����� ����� ActionStart() ���������� ����� ���� ��� ��� ���� �������� ��������� ��� ����� �������� ���� ����� � ����� �������
    }

    protected void ActionComplete() //�������� ���������
    {
        _isActive = false;
        _onActionComplete(); // ������� ���������� �������.(� ����� ������ ��� ClearBusy() // �������� ��������� ��� ����� ��������� - ������������ ������ UI  

        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty); // �������� �������
    }

    public Unit GetUnit() // �������� �������� Unit 
    {
        return _unit;
    }

    public EnemyAIAction GetBestEnemyAIAction() // ������� ������ �������� ���������� �� ��� ���������� ��������
    {
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>(); // ������ �������� ���������� ��

        List<GridPositionXZ> validActionGridPositionList = GetValidActionGridPositionList(); //�������� ������ ���������� �������� ������� ��� ��������

        foreach (GridPositionXZ gridPosition in validActionGridPositionList) // ��������� ��� ������ ����� �� ������������ ����������� ������
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition); // ����������� �������� ���������� �� ��� ����� ���������� �������� � ���� ������� �����
            enemyAIActionList.Add(enemyAIAction); // ������� ������������� �������� � ������ (��� ������� �������� ����� ���� ������)
        }

        if (enemyAIActionList.Count > 0) // �������� ��� ������ �� ������
        {
            enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue); //�����������. ��������� ������� ������� �������� ����� �������� ����������(������� �������� ����� ������) // (Comparison<T> ������� - ������������ �����, ������������ ��� ������� ������ ����. ������ �������� -�������� ����� �����, ������� ���������� ������������� �������� ����������(������ � ������) a � b),  

            // ��� ��������� // ������ ��������� ������ �������� ��� ����� (https://stackoverflow.com/questions/74245814/can-someone-help-me-understand-anonymous-functions-in-c)
            /*int EnemyAIActionComparison(EnemyAIAction a, EnemyAIAction b)
            {
                if (a.actionValue > b.actionValue) return +1;
                if (a.actionValue < b.actionValue) return -1;
                return 0; // ��� �����
            }
            //��������� �� ����� ��������, ��������� ����� ���������(����� �������� ������ ����), �� ����� �� ������ ��������
            int EnemyAIActionComparison(EnemyAIAction x, EnemyAIAction y)
            {
                return x.actionValue - y.actionValue;
            }*/
            // ������ ������� ������ � ���������� ��������� actionValue
            return enemyAIActionList[0]; // ������ ����� ���, ��� ����� ������� ������
        }
        else
        {
            // ��� ��������� �������� �� �����
            return null;
        }

    }

    public abstract EnemyAIAction GetEnemyAIAction(GridPositionXZ gridPosition); //�������� �������� ���������� �� � ������ �������� ��� _gridPositioAnchor // abstract - ��������� ������������� � ������ ��������� � � ������� ������ ����� ������ ����.
}

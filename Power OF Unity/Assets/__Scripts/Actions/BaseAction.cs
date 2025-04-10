using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������� ��������. ����������� ���� 
/// </summary>
/// <remarks>
/// ��� ������� ������� ������ ���� ������������ � Unit.
/// </remarks>
public abstract class BaseAction : MonoBehaviour, ISetupForSpawn
{
    public static event EventHandler OnAnyActionStart; //������ ������ ��������.
    public static event EventHandler OnAnyActionCompleted; //���������� ������ ��������.

    protected bool _isActive; // ������� ����������. ��� �� ��������� ����������� ���������� ���������� ��������
    protected Unit _unit;
    protected LevelGrid _levelGrid;
    protected PlacedObjectTypeWithActionSO _placedObjectTypeWithActionSO;

    //���� ������������ ���������� ������� Action ������ - //public delegate void ActionCompleteDelegate(); //���������� �������� // ��������� ������� ������� �� ��������� �������� � ���������� �������
    protected Action _onActionComplete; //(�� ���������� ��������)// �������� ������� � ������������ ���� - using System;
                                        //�������� ��� ������� ��� ������������ ���������� (� ��� ����� ��������� ������ ������� �� ���������).
                                        //Action- ���������� �������. ���� ��� �������. ������� Func<>. 
                                        //�������� ��������. ����� ���������� ������� � ������� �� �������� ��������, ����� �����, � ������������ ����� ����, ����������� ����������� �������.
                                        //�������� ��������. ����� �������� �������� ������� �� ������� ������

    public virtual void SetupForSpawn(Unit unit)
    {
        _unit = unit;
        _levelGrid = _unit.GetLevelGrid();
    }
    protected virtual void Awake() // protected virtual- ���������� ��� ����� �������������� � �������� �������
    {

    }

    protected virtual void Start()
    {

    }
    /// <summary>
    /// ��������� PlacedObjectTypeWithActionSO ��� ������� �������� (����� ��� ��������� �����).
    /// </summary>  
    /// <remarks>
    /// ��� MoveAction �� ����� null
    /// </remarks>
    public virtual void SetPlacedObjectTypeWithActionSO(PlacedObjectTypeWithActionSO placedObjectTypeWithActionSO)  
    {
        _placedObjectTypeWithActionSO = placedObjectTypeWithActionSO;
    }
    public abstract string GetActionName(); // ������� ��� �������� // abstract - ��������� ������������� � ������ ��������� � � ������� ������ ����� ������ ����.

    public abstract string GetToolTip(); // ������� ����������� ���������

    public abstract int GetMaxActionDistance(); // ������� ��������� ��������
    public abstract void TakeAction(GridPositionXZ gridPosition, Action onActionComplete); //Generic ��������� �������� (�����������) � �������� �������� �������� ������� ��� �������� � ������� onActionComplete (��� ���������� ��������, � ����� ������ ��� ClearBusy() // �������� ��������� ��� ����� ��������� - ������������ ������ UI ) 
    
    /// <summary>
    /// �������� ������ ���������� �������� ������� ��� ��������
    /// </summary>
    public abstract List<GridPositionXZ> GetValidActionGridPositionList(); 

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

    public Unit GetUnit() { return _unit; }
    public PlacedObjectTypeWithActionSO GetPlacedObjectTypeWithActionSO() { return _placedObjectTypeWithActionSO; }


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

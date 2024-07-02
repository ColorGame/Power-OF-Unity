using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction // �������� ��������������
{
    public static event EventHandler OnAnyInteractActionComplete; // ����� �������������� ���������

    private int _maxInteractDistance = 1; // ��������� ��������������
    private GridPositionXZ _targetGridPosition;

    private void Update()
    {
        if (!_isActive) // ���� �� ������� �� ...
        {
            return; // ������� � ���������� ��� ����
        }

        Vector3 targetDirection = (_unit.GetLevelGrid().GetWorldPosition(_targetGridPosition) - transform.position).normalized; // ����������� � ������� �������, ��������� ������
        float rotateSpeed = 10f; //����� ���������//

        transform.forward = Vector3.Slerp(transform.forward, targetDirection, Time.deltaTime * rotateSpeed); // ������ �����.

        //��� �� �� �� ������ ��������� � ����� ��������� ����� �� ���������� ��������� �������� - ������� �������� ��� ��������
        //����� �������� ������ ������� ����� ������� �� ����� ����� ����� �� ��� ����������� ������
        //ActionComplete(); //�������� ���������

    }

    public override string GetActionName() // ������� ��� ��� ������
    {
        return "��������������";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPositionXZ gridPosition) //�������� �������� ���������� ��  ��� ���������� ��� �������� �������// ������������� ����������� ������� ����� //EnemyAIAction ������ � ������ ���������� ������� �������, ���� ������ - ��������� ������ ������ � ����������� �� ��������� ����� ������� ��� �����
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0
        };
    }

    public override List<GridPositionXZ> GetValidActionGridPositionList() // �������� ������ ���������� �������� ������� ��� �������� // ������������� ������� �������  
    {
        List<GridPositionXZ> validGridPositionList = new List<GridPositionXZ>();

        GridPositionXZ unitGridPosition = _unit.GetGridPosition(); // ������� ������� � ����� �����

        for (int x = -_maxInteractDistance; x <= _maxInteractDistance; x++) // ���� ��� ����� ����� ������� � ������������ unitGridPosition, ������� ��������� ���������� �������� � �������� ������� _maxInteractDistance
        {
            for (int z = -_maxInteractDistance; z <= _maxInteractDistance; z++)
            {
                GridPositionXZ offsetGridPosition = new GridPositionXZ(x, z, 0);  // ��������� �������� �������. ��� ������� ���������(0,0, 0-����) �������� ��� ���� 
                GridPositionXZ testGridPosition = unitGridPosition + offsetGridPosition;  // ����������� �������� �������

                if (!_unit.GetLevelGrid().IsValidGridPosition(testGridPosition)) // �������� �������� �� testGridPosition ���������� �������� �������� ���� ��� �� ��������� � ���� �����
                {
                    continue;
                }

                /*//�������� ����������� �������� ������� �� ������� �����
                DoorInteract door = _unit.GetLevelGrid().GetDoorAtGridPosition(testGridPosition);

                if (door == null)
                {
                    // � ���� ������� ����� ��� �����
                    continue;
                }*/
                // �������� ��������� �������������� ��� �� �� ����� ���������������� �� ������ � ������
                IInteractable interactable = _unit.GetLevelGrid().GetInteractableAtGridPosition(testGridPosition);

                if (interactable == null)
                {
                    // � ���� ������� ����� ��� ������� ��������������
                    continue;
                }


                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPositionXZ gridPosition, Action onActionComplete) // ������������� TakeAction (��������� �������� (�����������). (������� onActionComplete - �� ���������� ��������). � ����� ������ �������� �������� ������� ClearBusy - �������� ���������
    {
        IInteractable interactable = _unit.GetLevelGrid().GetInteractableAtGridPosition(gridPosition); // ������� IInteractable(��������� ��������������) �� ���������� �������� ������� // ��� ��� ������� ����� ������ �� ������� (�����, �����, ������...) - ��� �� �� ���������� ���� ���������
        interactable.Interact(OnInteractComplete); //���������� �������������� � ���������� IInteractable(��������� ��������������) � ��������� ������ - ��� ���������� �������������� (���� ������� ����� �������� ���� �����)
        _targetGridPosition = gridPosition;
        ActionStart(onActionComplete); // ������� ������� ������� ����� �������� // �������� ���� ����� � ����� ����� ���� �������� �.�. � ���� ������ ���� EVENT � �� ������ ����������� ����� ���� ��������
    }

    private void OnInteractComplete() //��� ���������� ��������������
    {

        OnAnyInteractActionComplete?.Invoke(this, EventArgs.Empty);
        ActionComplete(); //�������� ���������
                          //��� �� �� �� ������ ��������� � ����� ��������� ����� �� ���������� ��������� �������� - ������� �������� ��� ��������
                          //����� �������� ������ ������� ����� ������� �� ����� ����� ����� �� ��� ����������� ������

    }

    public override int GetMaxActionDistance()
    {
        return _maxInteractDistance;
    }

    public override string GetToolTip()
    {
        return "���� - " + GetActionPointCost() + "\n" +
                "��������� - " + GetMaxActionDistance() + "\n" +
                "����� ��������� �����, � <color=#00ff00> ������� �����</color>  ";
    }
}



using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


// ���� ���� ����� � �� ������ ����������� ����� ������. �������� ���, ������� � Project Settings/ Script Execution Order � �������� ���� Deafault Time
public class UnitActionSystem : MonoBehaviour // ������� �������� ����� (��������� ������ �������� �����)
{
    public event EventHandler<Unit> OnSelectedUnitChanged; // ��������� ���� ������� (����� ���������� ��������� ���� �� �������� ������� Event) <Unit>-����� ��������� ����
    public event EventHandler OnSelectedActionChanged; // ��������� �������� �������� (����� �������� �������� �������� � ����� ������ �� �������� ������� Event)  
    public event EventHandler OnGameOver; // ����� ����

    public event EventHandler<OnUnitSystemEventArgs> OnBusyChanged; // ��������� �������� (����� �������� �������� _isBusy, �� �������� ������� Event, � �������� �� � ���������) � <> -generic ���� ��� ����� ������ ����������

    public class OnUnitSystemEventArgs : EventArgs // �������� ����� �������, ����� � ��������� ������� �������� ������ ������
    {
        public bool isBusy;
        public BaseAction selectedAction; // ��������� ��������
    }


    private LayerMask _unitLayerMask; // ����� ���� ������ (�������� � ����������) ���� ������� Units
    private Unit _selectedUnit; // ��������� ���� (�� ���������).���� ������� ������������� ����� ������� ����� ���������� ���������� �����

    private GameInput _gameInput;
    private BaseAction _selectedAction; // ��������� ��������// ����� ���������� � Button   
    private bool _isBusy; // ����� (������� ���������� ��� ���������� ������������� ��������)
    private UnitManager _unitManager;
    private TurnSystem _turnSystem;
    private LevelGrid _levelGrid;
    private MouseOnGameGrid _mouseOnGameGrid;

    public void Init(GameInput gameInput, UnitManager unitManager, TurnSystem turnSystem, LevelGrid levelGrid, MouseOnGameGrid mouseOnGameGrid)
    {
        _gameInput = gameInput;
        _unitManager = unitManager;
        _turnSystem = turnSystem;
        _levelGrid = levelGrid;
        _mouseOnGameGrid = mouseOnGameGrid;

        Setup();
    }

    private void Setup()
    {
        _unitLayerMask = LayerMask.GetMask("Units");
        _selectedUnit = _unitManager.GetUnitOnMissionList()[0];
        SetSelectedUnit(_selectedUnit, _selectedUnit.GetAction<MoveAction>()); // ���������(����������) ���������� �����, ���������� ��������� ��������,   // ��� ������ � _targetUnit ���������� ���� �� ��������� 


        // ���� ������ � ������ ��� ��� ��������� �����
        _gameInput.OnClickAction += GameInput_OnClickAction; // ���������� �� ������� ���� �� ���� ��� ��������
        _unitManager.OnAnyUnitDeadAndRemoveList += UnitManager_OnAnyUnitDeadAndRemoveList; //���������� �� ������� ����� ���� ���� � ������ �� ������
        _turnSystem.OnTurnChanged += TurnSystem_OnTurnChanged; // ���������� ��� �������
    }


    private void OnDisable()
    {
        _unitManager.OnAnyUnitDeadAndRemoveList -= UnitManager_OnAnyUnitDeadAndRemoveList; //���������� �� ������� ����� ���� ���� � ������ �� ������
        _turnSystem.OnTurnChanged -= TurnSystem_OnTurnChanged; // ���������� ��� �������
        _gameInput.OnClickAction -= GameInput_OnClickAction; // ���������� �� ������� ���� �� ���� ��� ��������
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (_turnSystem.IsPlayerTurn()) // ���� ��� ������ ��
        {
            List<Unit> myUnitList = _unitManager.GetUnitList(); // ������ ������ ���� ������
            if (myUnitList.Count > 0) // ���� ���� ����� �� �������� ��������� ������� �� ������ �����
            {
                SetSelectedUnit(myUnitList[0], myUnitList[0].GetAction<MoveAction>());
            }
        };
    }

    private void UnitManager_OnAnyUnitDeadAndRemoveList(object sender, EventArgs e)
    {
        if (_selectedUnit.GetHealthSystem().IsDead()) // ���� ���������� ���� ������� �� ...
        {
            List<Unit> friendlyUnitList = _unitManager.GetUnitList(); // ������ ������ ������������� ������
            if (friendlyUnitList.Count > 0) // ���� ���� ����� �� �������� ��������� ������� �� ������ �����
            {
                SetSelectedUnit(friendlyUnitList[0], friendlyUnitList[0].GetAction<MoveAction>());
            }
            else // ���� ��� ������ � ����� �� �����
            {
                OnGameOver?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private void GameInput_OnClickAction(object sender, EventArgs e)
    {
        if (_isBusy) // ���� ����� ... �� ���������� ����������
        {
            return;
        }

        if (!_turnSystem.IsPlayerTurn()) // ��������� ��� ������� ������ ���� ��� �� ���������� ����������
        {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())  // ��������, ������� �� ��������� ���� �� �������� ����������������� ����������  
                                                            // ���������� � ����� ������� �������. (current - ���������� ������� ������� �������.) (IsPointerOverGameObject() -��������� ��������� (����) �� ������� ������)
        {
            return; // ���� ��������� ���� ��� �������(UI), ����� ������������� ����� , ��� �� �� ����� �������� �� ������, ���� �� ����� � ����� ����� ������� ���������� ��� �������
        }
        if (TryHandleUnitSelection()) // ������� ��������� ������ �����
        {
            return; //���� �� ������� ����� �� TryHandleUnitSelection() ������ true. ����� ������������� �����, ����� �� ����� �������� �� �����, �������� ����� �������, ���������� ��������� ���� �� ��� � ����� ����� �����
        }

        HandleSelectedAction(); // ���������� ��������� ��������      
    }

    public void HandleSelectedAction() // ���������� ��������� ��������
    {
        GridPositionXZ mouseGridPosition = _levelGrid.GetGridPosition(_mouseOnGameGrid.GetPositionOnlyHitVisible()); // ����������� ������� ���� �� �������� � ��������.

        if (!_selectedAction.IsValidActionGridPosition(mouseGridPosition)) // ��������� ��� ������ ���������� ��������, �������� ������� ���� �� ������������ �������� . ���� �� ��������� ��...
        {
            return; // ���������� ����������  //���������� ! � return; �������� �������� ������ if()
        }

        if (!_selectedUnit.GetActionPointsSystem().TrySpendActionPointsToTakeAction(_selectedAction)) // ��� ���������� ����� ��������� ��������� ���� ��������, ����� ��������� ��������� ��������. ���� �� ����� ��...
        {
            return; // ���������� ����������
        }

        SetBusy(); // ���������� �������
        _selectedAction.TakeAction(mouseGridPosition, ClearBusy); //� ���������� �������� ������� ����� "��������� �������� (�����������)" � ��������� � ������� ������� ClearBusy
    }

    private void SetBusy() // ���������� �������
    {
        _isBusy = true;
        OnBusyChanged?.Invoke(this, new OnUnitSystemEventArgs // ������� ����� ��������� ������ OnUnitSystemEventArgs
        {
            isBusy = _isBusy,
            selectedAction = _selectedAction,
        });
    }

    private void ClearBusy() // �������� ��������� ��� ����� ���������
    {
        _isBusy = false;
        OnBusyChanged?.Invoke(this, new OnUnitSystemEventArgs // ������� ����� ��������� ������ OnUnitSystemEventArgs
        {
            isBusy = _isBusy,
            selectedAction = _selectedAction,
        });
    }

    private bool TryHandleUnitSelection() // ������� ��������� ������ �����
    {
        if (_selectedAction is GrappleAction comboAction) //���� ��������� �������� GrappleAction ��
        {
            switch (comboAction.GetState()) // ������� ��������� ����� 
            {
                case GrappleAction.State.ComboSearchEnemy:
                case GrappleAction.State.ComboStart:
                    // � ���� ������� ������ �������� ������� �����
                    return false;
            }
        }

        Ray ray = Camera.main.ScreenPointToRay(_gameInput.GetMouseScreenPoint()); // ��� �� ������ � ����� ��� ���������� ������ ����
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, _unitLayerMask)) // ������ true ���� �� ���-�� �������. �.�. ������� ����� �������������� �� ����������� ����� ������ �� ������
        {   // �������� ���� �� �� ������� � ������� �� ������ ���������  <Unit>
            if (raycastHit.transform.TryGetComponent(out UnitCoreView unitCore)) // ������������ TryGetComponent ����� CreateInstanceClass � ��� ��� �� ���� ������ ������� ��������. TryGetComponent - ���������� true, ���� ��������� < > ������. ���������� ��������� ���������� ����, ���� �� ����������.
            {
                Unit unit = unitCore.GetUnit();
                if (unit == _selectedUnit) // ������ �������� ��������� �������� �� ���������� ����� ��� ���������� _selectedAction (�������� ������ ���������� ����� �� �������� ������� ���������� �� ���) ���� ��� ������ ������ �� ������ ���������� _selectedAction �� ������ ����� ������� �����.
                {
                    // ���� ���� ��� ������
                    return false;
                }

                if (unit.GetType() != _selectedUnit.GetType()) // ���� ��� ����� �� ����� (�� ���� �� ���������)
                {
                    // ��� ���� ��� �������� �� ����
                    return false;
                }
                SetSelectedUnit(unit, unit.GetAction<MoveAction>()); // ������ (����) � ������� ����� ��� ����������� ���������.
                return true;
            }
        }
        // �� ���� ������� �����
        return false;
    }

    public void SetSelectedUnit(Unit unit, BaseAction baseAction) // ���������(����������) ���������� �����,� ���������� ������� ��������, � ��������� �������   
    {
        _selectedUnit = unit; // �������� ���������� � ���� ����� ����������� ��������� ������.

        SetSelectedAction(baseAction); // ������� ��������� "MoveAction"  ������ ���������� ����� (�� ��������� ��� ������ ������� ��������� ����� MoveAction). �������� � ���������� _selectedAction ����� ������� SetSelectedAction()

        OnSelectedUnitChanged?.Invoke(this, _selectedUnit); // "?"- ��������� ��� !=0. Invoke ������� (this-������ �� ������ ������� ��������� ������� "�����������" � ����� UnitSelectedVisual � ActionButtonSystemUI ����� ��� ������������ "������������" ��� ����� ��� ����� ������ �� _targetUnit)
    }

    public void SetSelectedAction(BaseAction baseAction) //���������� ��������� ��������, � ��������� �������  
    {
        _selectedAction = baseAction;
       
        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty); // "?"- ��������� ��� !=0. Invoke ������� (this-������ �� ������ ������� ��������� ������� "�����������" � ����� ActionButtonSystemUI  LevelGridVisual ����� ��� ������������ "������������")
    }
    public BaseAction GetSelectedAction() { return _selectedAction; }// ������� ��������� ��������
    public Unit GetSelectedUnit() { return _selectedUnit; }



}

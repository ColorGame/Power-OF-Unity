using System;
using System.Collections.Generic;
using UnityEngine;
using static UnitActionSystem;

/// <summary>
/// ������� ������ - ������ ����� �� ������� ������.
/// </summary>
/// <remarks>
/// ����������� ��������� ������ � ��������� �� ������. 
/// </remarks>
public class UnitSelectAtLevelButtonsSystemUI : MonoBehaviour
{    
    [SerializeField] private Transform _enemyUnitButonContainerTransform; // � ���������� ���������  ��������� ��� ������( ���������� � ����� � Canvas)
    [SerializeField] private Transform _friendlyUnitButonContainerTransform; // � ���������� ���������  ��������� ��� ������( ���������� � ����� � Canvas)

    private List<UnitFriendSelectAtLevelButtonUI> _friendlyUnitButtonList; // ���� - ����. �������� - ������ ��� ����� �����
    private List<UnitEnemySelectAtLevelButtonUI> _enemyUnitButonUIList; // ������ ������ ��������� ������   
    private UnitManager _unitManager;
    private TurnSystem _turnSystem;
    private UnitActionSystem _unitActionSystem;
    private CameraFollow _cameraFollow;

    public void Init(UnitManager unitManager, TurnSystem turnSystem, UnitActionSystem unitActionSystem, CameraFollow cameraFollow)
    {
        _unitManager = unitManager;
        _turnSystem = turnSystem;
        _unitActionSystem = unitActionSystem;
        _cameraFollow = cameraFollow;
    }

    private void Awake()
    {
        _friendlyUnitButtonList = new List<UnitFriendSelectAtLevelButtonUI>();
        _enemyUnitButonUIList = new List<UnitEnemySelectAtLevelButtonUI>();     
    }

    private void Start()
    {
        _unitManager.OnAnyUnitDeadAndRemoveList += UnitManager_OnAnyUnitDeadAndRemoveList;// ������� ����� ���� ���� � ������ �� ������
        _unitManager.OnAnyEnemyUnitSpawnedAndAddList += UnitManager_OnAnyEnemyUnitSpawnedAndAddList;// ����� ��������� ���� ������ � �������� � ������

        _turnSystem.OnTurnChanged += TurnSystem_OnTurnChanged; // ������� ����� ���� ������������� �� Event // ����� ����������� (��������� ������ ����� ��������).
        _unitActionSystem.OnBusyChanged += UnitActionSystem_OnBusyChanged; // ��������� ��������
        _unitActionSystem.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged; //��������� ���� �������       

        CreateEnemyUnitButtons(); // ������� ������ ���  ������
        CreateFriendlyUnitButtons();
        UpdateSelectedVisual(_unitActionSystem.GetSelectedUnit()); 
    }

    private void UnitManager_OnAnyEnemyUnitSpawnedAndAddList(object sender, EventArgs e)
    {
        CreateEnemyUnitButtons();
    }

    private void UnitManager_OnAnyUnitDeadAndRemoveList(object sender, EventArgs e)
    {
        CreateEnemyUnitButtons();
        CreateFriendlyUnitButtons();
    }  

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateButtonVisibility();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, Unit selectedUnit) //sender - ����������� // �������� ������ ����� ���� ��������� ��� � ������� ����������� OnSelectedUnitChanged
    {
        UpdateSelectedVisual(selectedUnit);
    }

    private void UnitActionSystem_OnBusyChanged(object sender, OnUnitSystemEventArgs e)
    {
        if (e.selectedAction is GrappleAction comboAction) // ���� ����������� ����� ������� �������� ��������� �����
        {
            switch (comboAction.GetState())
            {
                case GrappleAction.State.ComboSearchEnemy: // ���� ��� ����� �� ������ ������ ���� ��������
                case GrappleAction.State.ComboStart:
                    return; // ������� � ���������� ��� ����
            }
        }

        foreach (UnitFriendSelectAtLevelButtonUI friendlyUnitButonUI in _friendlyUnitButtonList)// ��������� ���������
        {
            friendlyUnitButonUI.HandleStateButton(e.isBusy);
        }
    }
     
    private void CreateEnemyUnitButtons() // ������� ������ ��� ������ ������
    {
        foreach (Transform buttonTransform in _enemyUnitButonContainerTransform) // ������� ��������� � ��������
        {
            Destroy(buttonTransform.gameObject); // ������ ������� ������ ������������� � Transform
        }

        _enemyUnitButonUIList.Clear(); // ������� ����� ������

        foreach (Unit unit in _unitManager.GetUnitEnemyList())// ��������� ��������� ������
        {
            if (unit.GetTransform().gameObject.activeSelf) // ���� ���� �������� �� 
            {
                UnitEnemySelectAtLevelButtonUI unitEnemySelectAtLevelButton = Instantiate(GameAssets.Instance.unitEnemySelectAtLevelButton, _enemyUnitButonContainerTransform); // ��� ������� ����� �������� ������ ������ � �������� �������� - ��������� ��� ������               
                unitEnemySelectAtLevelButton.Init(unit, _cameraFollow);//������� � ���������

                _enemyUnitButonUIList.Add(unitEnemySelectAtLevelButton);// ������� � ������ ���� ������
            }
        }
    }

    private void CreateFriendlyUnitButtons() // ������� ������ ��� �������������� ������
    {
        foreach (Transform buttonTransform in _friendlyUnitButonContainerTransform) // ������� ��������� � ��������
        {
            Destroy(buttonTransform.gameObject); // ������ ������� ������ ������������� � Transform
        }

        _friendlyUnitButtonList.Clear();

        foreach (Unit unit in _unitManager.GetUnitFriendList())// ��������� ������������� ������
        {
            UnitFriendSelectAtLevelButtonUI UnitFriendSelectAtLevelButton = Instantiate(GameAssets.Instance.unitFriendSelectAtLevelButton, _friendlyUnitButonContainerTransform); // ��� ������� ����� �������� ������ ������ � �������� �������� - ��������� ��� ������
            UnitFriendSelectAtLevelButton.Init(unit, _unitActionSystem,_cameraFollow);//������� � ���������

            _friendlyUnitButtonList.Add(UnitFriendSelectAtLevelButton);     
        }
    }

    private void UpdateSelectedVisual(Unit selectedUnit) //���������� ������������ ������( ��� ������ ������ ������� �����)
    {
        foreach (UnitFriendSelectAtLevelButtonUI friendlyUnitButonUI in _friendlyUnitButtonList) // ��������� ���������
        {
            friendlyUnitButonUI.UpdateSelectedVisual(selectedUnit);
        }
    }
    private void UpdateButtonVisibility() // ���������� ������������ ������ � ����������� �� ���� ��� ��� (������� �� ����� ���� �����)
    {
        bool isBusy = !_turnSystem.IsPlayerTurn(); // ������ ����� ����� ���� (�� �)

        foreach (UnitFriendSelectAtLevelButtonUI friendlyUnitButonUI in _friendlyUnitButtonList) // ��������� ���������
        {
            friendlyUnitButonUI.HandleStateButton(isBusy);
        }
    }

    private void OnDestroy()
    {
        _unitManager.OnAnyUnitDeadAndRemoveList -= UnitManager_OnAnyUnitDeadAndRemoveList;
        _unitManager.OnAnyEnemyUnitSpawnedAndAddList -= UnitManager_OnAnyEnemyUnitSpawnedAndAddList;
        _turnSystem.OnTurnChanged -= TurnSystem_OnTurnChanged; 
        _unitActionSystem.OnBusyChanged -= UnitActionSystem_OnBusyChanged; 
        _unitActionSystem.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;   
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using static UnitActionSystem;

public class SelectUnitButtonUI : MonoBehaviour
{
    [SerializeField] private Transform _enemyUnitButonUIPrefab; // � ���������� ������� ������ ������
    [SerializeField] private Transform _enemyUnitButonContainerTransform; // � ���������� ���������  ��������� ��� ������( ���������� � ����� � Canvas)
    [SerializeField] private Transform _friendlyUnitButonPrefab; // � ���������� ������� ������ ������
    [SerializeField] private Transform _friendlyUnitButonContainerTransform; // � ���������� ���������  ��������� ��� ������( ���������� � ����� � Canvas)

    private Dictionary<Unit, FriendlyUnitButtonUI> _friendlyUnitButtonDictionary; // ���� - ����. �������� - ������ ��� ����� �����
    private List<EnemyUnitButtonUI> _enemyUnitButonUIList; // ������ ������ ��������� ������   
    private UnitManager _unitManager;
    private TurnSystem _turnSystem;
    private UnitActionSystem _unitActionSystem;

    public void Initialize(UnitManager unitManager, TurnSystem turnSystem, UnitActionSystem unitActionSystem)
    {
        _unitManager = unitManager;
        _turnSystem = turnSystem;
        _unitActionSystem = unitActionSystem;
    }

    private void Awake()
    {
        _friendlyUnitButtonDictionary = new Dictionary<Unit, FriendlyUnitButtonUI>();
        _enemyUnitButonUIList = new List<EnemyUnitButtonUI>();     
    }

    private void Start()
    {
        _unitManager.OnAnyUnitDeadAndRemoveList += UnitManager_OnAnyUnitDeadAndRemoveList;// ������� ����� ���� ���� � ������ �� ������
        _unitManager.OnAnyEnemyUnitSpawnedAndAddList += UnitManager_OnAnyEnemyUnitSpawnedAndAddList;// ����� ��������� ���� ������ � �������� � ������

        _turnSystem.OnTurnChanged += TurnSystem_OnTurnChanged; // ������� ����� ���� ������������� �� Event // ����� ����������� (��������� ������ ����� ��������).
        _unitActionSystem.OnBusyChanged += UnitActionSystem_OnBusyChanged; // ��������� ��������
        _unitActionSystem.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged; //��������� ���� �������
        Unit.OnAnyFriendlyChangeHealth += Unit_OnAnyFriendlyChangeHealth; //� ������ �������������� ����� ���������� ��������   
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged; //��������� ����� �������� � ������(Any)

        CreateEnemyUnitButtons(); // ������� ������ ���  ������
        CreateFriendlyUnitButtons();
        UpdateSelectedVisual();
    }

    private void UnitManager_OnAnyEnemyUnitSpawnedAndAddList(object sender, EventArgs e)
    {
        CreateEnemyUnitButtons();
    }

    private void UnitManager_OnAnyUnitDeadAndRemoveList(object sender, EventArgs e)
    {
        CreateEnemyUnitButtons();
    }

    private void Unit_OnAnyFriendlyChangeHealth(object sender, EventArgs e)
    {
        FriendlyUnitButtonUI friendlyUnitButonUI = _friendlyUnitButtonDictionary[sender as Unit];
        friendlyUnitButonUI.UpdateHealthBar();
    }
   

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateButtonVisibility();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e) //sender - ����������� // �������� ������ ����� ���� ��������� ��� � ������� ����������� OnSelectedUnitChanged
    {
        UpdateSelectedVisual();
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

        foreach (FriendlyUnitButtonUI friendlyUnitButonUI in _friendlyUnitButtonDictionary.Values)// ��������� ���������, �������� �� �������
        {
            friendlyUnitButonUI.HandleStateButton(e.isBusy);
        }
    }

    // �������� // ����� ���������� ������. ����� ����� � ������ Unit � ���������� ������ ����� � ���� ������, ������� ���� � ���� �������. ��� ����������� ����� ��� ������ ����������, ����� ����� ���������� ������ � ���������� ��� �� ���������� ���� �������� "0" � �� ����� �� "2".
    // ������� 1 //- �������� ������� ���������� ������� UnitActionSystemUI , ������� � Project Settings/ Script Execution Order � �������� ���� Deafault Time � �����
    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs empty) //��������� ��������� ����� �������� � ������(Any) ����� � �� ������ � ����������. ������� ��.
    {
        FriendlyUnitButtonUI friendlyUnitButonUI = _friendlyUnitButtonDictionary[sender as Unit];
        friendlyUnitButonUI.UpdateActionPoints();// ���������� ����� ��������         
    }
    

    private void CreateEnemyUnitButtons() // ������� ������ ��� ������ ������
    {
        foreach (Transform buttonTransform in _enemyUnitButonContainerTransform) // ������� ��������� � ��������
        {
            Destroy(buttonTransform.gameObject); // ������ ������� ������ ������������� � Transform
        }

        _enemyUnitButonUIList.Clear(); // ������� ����� ������

        foreach (Unit unit in _unitManager.GetEnemyUnitList())// ��������� ��������� ������
        {
            if (unit.gameObject.activeSelf) // ���� ���� �������� �� 
            {
                Transform actionButtonTransform = Instantiate(_enemyUnitButonUIPrefab, _enemyUnitButonContainerTransform); // ��� ������� ����� �������� ������ ������ � �������� �������� - ��������� ��� ������
                EnemyUnitButtonUI enemyUnitButonUI = actionButtonTransform.GetComponent<EnemyUnitButtonUI>();// � ������ ������ ��������� EnemyUnitButtonUI
                enemyUnitButonUI.SetUnit(unit);//������� � ���������


                _enemyUnitButonUIList.Add(enemyUnitButonUI);// ������� � ������ ���� ������
            }
        }
    }

    private void CreateFriendlyUnitButtons() // ������� ������ ��� �������������� ������
    {
        foreach (Transform buttonTransform in _friendlyUnitButonContainerTransform) // ������� ��������� � ��������
        {
            Destroy(buttonTransform.gameObject); // ������ ������� ������ ������������� � Transform
        }

        _friendlyUnitButtonDictionary.Clear();

        foreach (Unit unit in _unitManager.GetMyUnitList())// ��������� ������������� ������
        {
            Transform actionButtonTransform = Instantiate(_friendlyUnitButonPrefab, _friendlyUnitButonContainerTransform); // ��� ������� ����� �������� ������ ������ � �������� �������� - ��������� ��� ������
            FriendlyUnitButtonUI friendlyUnitButonUI = actionButtonTransform.GetComponent<FriendlyUnitButtonUI>();// � ������ ������ ��������� FriendlyUnitButtonUI
            friendlyUnitButonUI.SetUnit(unit);//������� � ���������

            _friendlyUnitButtonDictionary[unit] = friendlyUnitButonUI; // ���� - ����. �������� - FriendlyUnitButtonUI        
        }
    }

    private void UpdateSelectedVisual() //���������� ������������ ������( ��� ������ ������ ������� �����)
    {
        foreach (FriendlyUnitButtonUI friendlyUnitButonUI in _friendlyUnitButtonDictionary.Values) // ��������� ���������, �������� �� �������
        {
            friendlyUnitButonUI.UpdateSelectedVisual();
        }
    }
    private void UpdateButtonVisibility() // ���������� ������������ ������ � ����������� �� ���� ��� ��� (������� �� ����� ���� �����)
    {
        bool isBusy = !TurnSystem.Instance.IsPlayerTurn(); // ������ ����� ����� ���� (�� �)

        foreach (FriendlyUnitButtonUI friendlyUnitButonUI in _friendlyUnitButtonDictionary.Values) // ��������� ���������, �������� �� �������
        {
            friendlyUnitButonUI.HandleStateButton(isBusy);
        }
    }
}

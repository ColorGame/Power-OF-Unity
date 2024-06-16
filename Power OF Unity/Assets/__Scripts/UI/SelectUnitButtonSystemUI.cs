using System;
using System.Collections.Generic;
using UnityEngine;
using static UnitActionSystem;

/// <summary>
/// ������� ���������� �������� ������ ������
/// </summary>
/// <remarks>
/// ����������� ��������� ������ � ��������� �� ������. 
/// </remarks>
public class SelectUnitButtonSystemUI : MonoBehaviour
{
    [SerializeField] private Transform _enemyUnitButonUIPrefab; // � ���������� ������� ������ ������
    [SerializeField] private Transform _enemyUnitButonContainerTransform; // � ���������� ���������  ��������� ��� ������( ���������� � ����� � Canvas)
    [SerializeField] private Transform _friendlyUnitButonPrefab; // � ���������� ������� ������ ������
    [SerializeField] private Transform _friendlyUnitButonContainerTransform; // � ���������� ���������  ��������� ��� ������( ���������� � ����� � Canvas)

    private Dictionary<Unit, SelectUnitFriendlyButtonUI> _friendlyUnitButtonDictionary; // ���� - ����. �������� - ������ ��� ����� �����
    private List<SelectUnitEnemyButtonUI> _enemyUnitButonUIList; // ������ ������ ��������� ������   
    private UnitManager _unitManager;
    private TurnSystem _turnSystem;
    private UnitActionSystem _unitActionSystem;

    public void Init(UnitManager unitManager, TurnSystem turnSystem, UnitActionSystem unitActionSystem)
    {
        _unitManager = unitManager;
        _turnSystem = turnSystem;
        _unitActionSystem = unitActionSystem;
    }

    private void Awake()
    {
        _friendlyUnitButtonDictionary = new Dictionary<Unit, SelectUnitFriendlyButtonUI>();
        _enemyUnitButonUIList = new List<SelectUnitEnemyButtonUI>();     
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

        foreach (SelectUnitFriendlyButtonUI friendlyUnitButonUI in _friendlyUnitButtonDictionary.Values)// ��������� ���������, �������� �� �������
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
                Transform actionButtonTransform = Instantiate(_enemyUnitButonUIPrefab, _enemyUnitButonContainerTransform); // ��� ������� ����� �������� ������ ������ � �������� �������� - ��������� ��� ������
                SelectUnitEnemyButtonUI enemyUnitButonUI = actionButtonTransform.GetComponent<SelectUnitEnemyButtonUI>();// � ������ ������ ��������� SelectUnitEnemyButtonUI
                enemyUnitButonUI.Init(unit);//������� � ���������


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

        foreach (Unit unit in _unitManager.GetUnitFriendList())// ��������� ������������� ������
        {
            Transform actionButtonTransform = Instantiate(_friendlyUnitButonPrefab, _friendlyUnitButonContainerTransform); // ��� ������� ����� �������� ������ ������ � �������� �������� - ��������� ��� ������
            SelectUnitFriendlyButtonUI selectUnitFriendlyButtonUI = actionButtonTransform.GetComponent<SelectUnitFriendlyButtonUI>();// � ������ ������ ��������� SelectUnitFriendlyButtonUI
            selectUnitFriendlyButtonUI.Init(unit, _unitActionSystem);//������� � ���������

            _friendlyUnitButtonDictionary[unit] = selectUnitFriendlyButtonUI; // ���� - ����. �������� - SelectUnitFriendlyButtonUI        
        }
    }

    private void UpdateSelectedVisual() //���������� ������������ ������( ��� ������ ������ ������� �����)
    {
        foreach (SelectUnitFriendlyButtonUI friendlyUnitButonUI in _friendlyUnitButtonDictionary.Values) // ��������� ���������, �������� �� �������
        {
            friendlyUnitButonUI.UpdateSelectedVisual();
        }
    }
    private void UpdateButtonVisibility() // ���������� ������������ ������ � ����������� �� ���� ��� ��� (������� �� ����� ���� �����)
    {
        bool isBusy = !_turnSystem.IsPlayerTurn(); // ������ ����� ����� ���� (�� �)

        foreach (SelectUnitFriendlyButtonUI friendlyUnitButonUI in _friendlyUnitButtonDictionary.Values) // ��������� ���������, �������� �� �������
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

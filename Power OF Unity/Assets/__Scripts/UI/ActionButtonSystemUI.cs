using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; 


/// <summary>
/// ������� ���������� �������� ��������
/// </summary>
/// <remarks>
/// ����������� ��������� ������ ��� ������ �����. 
/// </remarks>
public class ActionButtonSystemUI : MonoBehaviour 
{    
    [SerializeField] private Transform _actionButtonContainerTransform; // � ���������� ���������  ��������� ��� ������( ���������� � ����� � Canvas)   
    [SerializeField] private TextMeshProUGUI _actionPointsText; // ������ �� ����� ����� ��� ��������
    [SerializeField] private Image _actionPointImage; // �������� ������

    private TooltipUI _tooltipUI;
    private UnitActionSystem _unitActionSystem;
    private TurnSystem _turnSystem;

    private List<ActionButtonUI> _actionButtonUIList; // ������ ������ ��������  
    private Unit _selectedUnit;

    public void Init(UnitActionSystem unitActionSystem, TurnSystem turnSystem, TooltipUI tooltipUI)
    {
        _actionButtonUIList = new List<ActionButtonUI>(); // �������� ��������� ������       

        _unitActionSystem = unitActionSystem;
        _turnSystem = turnSystem;
        _tooltipUI = tooltipUI;

        _unitActionSystem.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged; //��������� ���� �������
        _unitActionSystem.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged; //��������� �������� ��������  
        _unitActionSystem.OnBusyChanged += UnitActionSystem_OnBusyChanged; // ��������� ��������        
        _turnSystem.OnTurnChanged += TurnSystem_OnTurnChanged; // ������� ����� ���� �������������       

        SetupEventSelectedUnit(); // ��������� Event � ���������� �����
        CreateUnitActionButtons();// ������� ������ ��� �������� �����        
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateButtonVisibility();
    }   

    private void UpdateButtonVisibility() // ���������� ������������ ������ � ����������� �� ���� ��� ��� (������� �� ����� �����)
    {
        bool isBusy = !_turnSystem.IsPlayerTurn(); // ������ ����� ����� ���� (�� �)

        foreach (ActionButtonUI actionButtonUI in _actionButtonUIList) // � ����� ���������� ��������� ������
        {
            actionButtonUI.HandleStateButton(isBusy);
        }
       
        _actionPointsText.gameObject.SetActive(_turnSystem.IsPlayerTurn()); // ���������� ������ �� ����� ����� ����
        _actionPointImage.gameObject.SetActive(_turnSystem.IsPlayerTurn());
    }

    // ������ ������ ����� ����� ���������
    private void UnitActionSystem_OnBusyChanged(object sender, UnitActionSystem.OnUnitSystemEventArgs e)
    {
        if (e.selectedAction is GrappleAction grappleAction) // ���� ����������� ����� ������� �������� ��������� �����
        {
            switch (grappleAction.GetState())
            {
                case GrappleAction.State.ComboSearchEnemy: // ���� ��� ����� �� ������ ������ ���� ��������
                case GrappleAction.State.ComboStart:
                    return; // ������� � ���������� ��� ����
            }
        }

        foreach (ActionButtonUI actionButtonUI in _actionButtonUIList) // � ����� ���������� ��������� ������
        {
            actionButtonUI.HandleStateButton(e.isBusy);
        }       
    } 

    private void CreateUnitActionButtons() // ������� ������ ��� �������� ����� 
    {
        foreach (Transform buttonTransform in _actionButtonContainerTransform) // ������� ��������� � ��������
        {
            Destroy(buttonTransform.gameObject); // ������ ������� ������ ������������� � Transform
        }

        _actionButtonUIList.Clear(); // ������� ����� ������     

        foreach (BaseAction baseAction in _selectedUnit.GetBaseActionsList()) // � ����� ��������� ������ ������� �������� � ���������� �����
        {
            Transform actionButtonTransform = Instantiate(GameAssets.Instance.actionButtonUIPrefab, _actionButtonContainerTransform); // ��� ������� baseAction �������� ������ ������ � �������� �������� - ��������� ��� ������
            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>(); // � ������ ������ ��������� ActionButtonUI
            actionButtonUI.SetBaseAction(baseAction, _unitActionSystem); //������� � ��������� ������� �������� (����� ������)

            MouseEnterExitEventsUI mouseEnterExitEvents = actionButtonTransform.GetComponent<MouseEnterExitEventsUI>(); // ������ �� ������ ��������� - ������� ����� � ������ ����� 
            mouseEnterExitEvents.OnMouseEnter += (object sender, EventArgs e) => // ���������� �� ������� - ��� ����� ���� �� ������. ������� ����� ��������� �������� ����� ������ () => {...} 
            {
                _tooltipUI.ShowAnchoredTooltip(baseAction.GetToolTip(), (RectTransform)actionButtonTransform); // ��� ��������� �� ������ ������� ��������� � ��������� �����
            };
            mouseEnterExitEvents.OnMouseExit += (object sender, EventArgs e) => // ���������� �� ������� - ��� ������ ���� �� ������.
            {
                _tooltipUI.Hide(); // ��� ��������� ���� ������ ���������
            };

            _actionButtonUIList.Add(actionButtonUI); // ������� � ������ ���������� ��������� ActionButtonUI
        }
    }

    private void SelectedUnit_OnActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e) 
    {
        SetupEventSelectedUnit();   // ��������� Event � ���������� �����
        CreateUnitActionButtons();  // ������� ������ ��� �������� ����� 
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    private void SetupEventSelectedUnit() // ��������� Event � ���������� �����
    {
        if (_selectedUnit != null) //���� ���� ��������� ���� �� ��������� �� ����.  (��� ������ ������� ��������� ���� �� �������� � ��� ��������)
        {
            _selectedUnit.GetActionPointsSystem().OnActionPointsChanged -= SelectedUnit_OnActionPointsChanged;
        }
        //������� ������ ���������� ����� � ���������� �� ����
        Unit newSelectedUnit = _unitActionSystem.GetSelectedUnit();
        _selectedUnit = newSelectedUnit;
        _selectedUnit.GetActionPointsSystem().OnActionPointsChanged += SelectedUnit_OnActionPointsChanged; // � ���������� ����� ���������� ���� ��������
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs empty)
    {
        UpdateSelectedVisual();
    }

   
    private void UpdateSelectedVisual() //���������� ������������ ������( ��� ������ ������ ������� �����)
    {
        foreach (ActionButtonUI actionButtonUI in _actionButtonUIList)
        {
            actionButtonUI.UpdateSelectedVisual();
        }        
    }

    private void UpdateActionPoints() // ���������� ����� �������� (��� �������� ��������)
    {       
        _actionPointsText.text = " " + _selectedUnit.GetActionPointsSystem().GetActionPointsCount(); //������� ����� ������� � ���� ���������� �����
    }       
    
}

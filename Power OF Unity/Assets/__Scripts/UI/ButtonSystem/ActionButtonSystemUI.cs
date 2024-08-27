using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// ������� ���������� �������� ��������
/// </summary>
/// <remarks>
/// ����������� ������ ��� ������ �����. 
/// </remarks>
public class ActionButtonSystemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _actionPointsText; // ������ �� ����� ����� ��� ��������
    [SerializeField] private Image _actionPointImage; // �������� ������

    [SerializeField] private ActionButtonUI _mainWeaponButton; // ������ ��������� ������
    [SerializeField] private ActionButtonUI _otherWeaponButton; // ������ ��������������� ������
    [SerializeField] private ActionButtonUI _moveActionButton; // ������ �����������
    [SerializeField] private Transform _grenadeButtonContainer; // ��������� ��� ������ ������
    [SerializeField] private Button _unitEquipmentButton; // ������ ���������� �����


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

        SetupEventSelectedUnit(_unitActionSystem.GetSelectedUnit()); // ��������� Event � ���������� �����
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

        _actionPointsText.enabled = _turnSystem.IsPlayerTurn(); // ���������� ������ �� ����� ����� ����
        _actionPointImage.enabled = _turnSystem.IsPlayerTurn();
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
        _actionButtonUIList.Clear(); // ������� ����� ������     

        _moveActionButton.SetMoveAction(_selectedUnit.GetAction<MoveAction>(), _unitActionSystem);
        _actionButtonUIList.Add(_moveActionButton);

        PlacedObjectTypeWithActionSO mainWeaponplacedObjectTypeWithActionSO = _selectedUnit.GetUnitEquipment().GetPlacedObjectMainWeaponSlot(); // � ���������� �����, � ����� ��������� ������, ������� ��� �������
        if (mainWeaponplacedObjectTypeWithActionSO != null)
        {
            BaseAction baseAction = mainWeaponplacedObjectTypeWithActionSO.GetAction(_selectedUnit); // ������� � ���������� �����, ������� �������� ��� ������� ���� PlacedObjectTypeWithActionSO
            _mainWeaponButton.SetBaseActionAndplacedObjectTypeWithActionSO(baseAction, mainWeaponplacedObjectTypeWithActionSO, _unitActionSystem);
            _actionButtonUIList.Add(_mainWeaponButton); // ������� � ������ ���������� ��������� ActionButtonUI
        }
        else
        {
            _mainWeaponButton.InteractableDesabled();
        }

        PlacedObjectTypeWithActionSO otherWeaponplacedObjectTypeWithActionSO = _selectedUnit.GetUnitEquipment().GetPlacedObjectOtherWeaponSlot(); // � ���������� �����, � ����� ��������� ������, ������� ��� �������
        if (otherWeaponplacedObjectTypeWithActionSO != null)
        {
            BaseAction baseAction = otherWeaponplacedObjectTypeWithActionSO.GetAction(_selectedUnit); // ������� ������� �������� ��� ������� ���� PlacedObjectTypeWithActionSO
            _otherWeaponButton.SetBaseActionAndplacedObjectTypeWithActionSO(baseAction, otherWeaponplacedObjectTypeWithActionSO, _unitActionSystem);
            _actionButtonUIList.Add(_otherWeaponButton); // ������� � ������ ���������� ��������� ActionButtonUI
        }
        else
        {
            _otherWeaponButton.InteractableDesabled();
        }


        foreach (Transform buttonTransform in _grenadeButtonContainer) // ������� ��������� � ��������
        {
            Destroy(buttonTransform.gameObject); // ������ ������� ������ ������������� � Transform
        }

        List<GrenadeTypeSO> GrenadeFragList = new List<GrenadeTypeSO>();
        List<GrenadeTypeSO> GrenadeSmokeList = new List<GrenadeTypeSO>();
        List<GrenadeTypeSO> GrenadeElectroshockList = new List<GrenadeTypeSO>();
        List<GrenadeTypeSO> GrenadePlasmaList = new List<GrenadeTypeSO>();

        foreach (GrenadeTypeSO grenadeTypeSO in _selectedUnit.GetUnitEquipment().GetGrenadeTypeSOList()) // ��������� ������� � ������ � ����������� �� �������
        {
            PlacedObjectType placedObjectType = grenadeTypeSO.GetPlacedObjectType();
            switch (placedObjectType)
            {
                case PlacedObjectType.GrenadeFrag:
                    GrenadeFragList.Add(grenadeTypeSO);
                    break;
                case PlacedObjectType.GrenadeSmoke:
                    GrenadeSmokeList.Add(grenadeTypeSO);
                    break;
                case PlacedObjectType.GrenadeStun:
                    GrenadeElectroshockList.Add(grenadeTypeSO);
                    break;
                case PlacedObjectType.GrenadePlasma:
                    GrenadePlasmaList.Add(grenadeTypeSO);
                    break;
            }
        }



        if (GrenadeFragList.Count != 0)
        {
            // �������� ������ ��� ������� ���� ������� � ��������� ���������� GrenadeFragList.Count ������� ����
            ///  ActionButtonUI grenadeFragButton = Instantiate<ActionButtonUI>(GameAssets.Instance.actionButtonUI, _grenadeButtonContainer);
        }

        /* foreach (BaseAction baseAction in _selectedUnit.GetBaseActionsArray()) // � ����� ��������� ������ ������� �������� � ���������� �����
         {
             Transform actionButtonTransform = Instantiate(GameAssets.Instance.actionButtonUI, _grenadeButtonContainer); // ��� ������� baseAction �������� ������ ������ � �������� �������� - ��������� ��� ������
             ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>(); // � ������ ������ ��������� ActionButtonUI
             actionButtonUI.SetBaseAction(baseAction, _unitActionSystem); //������� � ��������� ������� �������� (����� ������)

             MouseEnterExitEventsUI mouseEnterExitEvents = actionButtonTransform.GetComponent<MouseEnterExitEventsUI>(); // ������ �� ������ ��������� - ������� ����� � ������ ����� 
             mouseEnterExitEvents.OnMouseEnter += (object sender, EventArgs e) => // ���������� �� ������� - ��� ����� ���� �� ������. ������� ����� ��������� �������� ����� ������ () => {...} 
             {
                 _tooltipUI.ShowAnchoredShortTooltip(baseAction.GetToolTip(), (RectTransform)actionButtonTransform); // ��� ��������� �� ������ ������� ��������� � ��������� �����
             };
             mouseEnterExitEvents.OnMouseExit += (object sender, EventArgs e) => // ���������� �� ������� - ��� ������ ���� �� ������.
             {
                 _tooltipUI.Hide(); // ��� ��������� ���� ������ ���������
             };

             _actionButtonUIList.Add(actionButtonUI); // ������� � ������ ���������� ��������� ActionButtonUI
         }*/
    }

    private void SelectedUnit_OnActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, Unit selectedUnit)
    {
        SetupEventSelectedUnit(selectedUnit);   // ��������� Event � ���������� �����
        CreateUnitActionButtons();  // ������� ������ ��� �������� ����� 
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    private void SetupEventSelectedUnit(Unit newSelectedUnit) // ��������� Event � ���������� �����
    {
        if (_selectedUnit != null) //���� ���� ��������� ���� �� ��������� �� ����.  (��� ������ ������� ��������� ���� �� �������� � ��� ��������)
        {
            _selectedUnit.GetActionPointsSystem().OnActionPointsChanged -= SelectedUnit_OnActionPointsChanged;
        }
        //������� ������ ���������� ����� � ���������� �� ����        
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

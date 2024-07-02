//#define HEX_GRID_SYSTEM //������������ �������� ������� //  � C# ��������� ��� �������� �������������, ����������� ������� �� ��������public enum����� ��������� ���� ��������� ������������. 
//��� ��������� ���������� ������� ������������� ������ ��������� ����� �� ����������� � ��������� ��� � ��� �������� �����, ��� ��� ����������. 

using Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnitActionSystem;


// ������� LevelGridVisual ��� ������� ����� ������� �� ���������, ��������� �� �����, ����� ���������� ������� ����������� ����� ����� ����������.
// (Project Settings/ Script Execution Order � �������� ���������� LevelGridVisual ���� Default Time)
public class LevelGridVisual : MonoBehaviour //�������� ������� ������������  ������������ ��������� ����� �� ����� 
{
    [Serializable] // ����� ��������� ��������� ����� ������������ � ����������
    public struct GridVisualTypeMaterial    //������ ����� ��� ��������� // �������� ��������� ����� � ��������� ������. ������ � �������� ��������� ������������ ��� ���� ������ �������� ����������� ����� ������ � C#
    {                                       //� ������ ��������� ��������� ��������� ����� � ����������
        public GridVisualType gridVisualType;
        public Material materialGrid;
    }

    public enum GridVisualType //���������� ��������� �����
    {
        White,
        Blue,
        BlueSoft,
        Red,
        RedSoft,
        Yellow,
        YellowSoft,
        Green,
        GreenSoft,
    }


    [SerializeField] private List<GridVisualTypeMaterial> _gridVisualTypeMaterialListQuad; // ������ ��� ��������� ����������� ��������� ����� ������� (������ �� ���������� ���� ������) ����������� ��������� ����� // � ���������� ��� ������ ��������� ���������� ��������������� �������� �����
    [SerializeField] private List<GridVisualTypeMaterial> _gridVisualTypeMaterialListHex; // ������ ��� ��������� ����������� ��������� ����� ������������ (������ �� ���������� ���� ������) ����������� ��������� ����� // � ���������� ��� ������ ��������� ���������� ��������������� �������� �����


    private List<GridPositionXZ> _validGridPositionForGrenadeActionList; // ����� ���������� -������ ���������� �������� ������� ��� �������� ������� 
    private List<GridPositionXZ> _validGridPositionForComboActionList; // ����� ���������� -������ ���������� �������� ������� ��� �������� �����

    private LevelGridVisualSingle[,,] _gridSystemVisualSingleArray; // ���������� ������    

    private UnitActionSystem _unitActionSystem;
    private LevelGrid _levelGrid;
    private MouseOnGameGrid _mouseOnGameGrid;

    // ��� ������� ������������ ����� (����������� ������ ��� ������)
    // private LevelGridVisualSingle _lastSelectedGridSystemVisualSingle; // ��������� ��������� �������� ������� ������������ �������

    public void Init(UnitActionSystem unitActionSystem, LevelGrid levelGrid, MouseOnGameGrid mouseOnGameGrid)
    {
        _unitActionSystem = unitActionSystem;
        _levelGrid = levelGrid;
        _mouseOnGameGrid = mouseOnGameGrid;
    }

    private void Start()
    {
        _gridSystemVisualSingleArray = new LevelGridVisualSingle[ // ������� ������ ������������� �������� _widthX �� _heightY  � FloorAmount
            _levelGrid.GetWidth(),
            _levelGrid.GetHeight(),
            _levelGrid.GetFloorAmount()
        ];

        for (int x = 0; x < _levelGrid.GetWidth(); x++)
        {
            for (int z = 0; z < _levelGrid.GetHeight(); z++)
            {
                for (int floor = 0; floor < _levelGrid.GetFloorAmount(); floor++)  // ��������� ��� �����
                {
                    GridPositionXZ gridPosition = new GridPositionXZ(x, z, floor);

                    Transform gridSystemVisualSingleTransform = Instantiate(GameAssets.Instance.levelGridSystemVisualSinglePrefab, _levelGrid.GetWorldPosition(gridPosition), Quaternion.identity); // �������� ��� ������ � ������ ������� �����

                    _gridSystemVisualSingleArray[x, z, floor] = gridSystemVisualSingleTransform.GetComponent<LevelGridVisualSingle>(); // ��������� ��������� LevelGridVisualSingle � ���������� ������ ��� x,y,_floor ��� ����� ������� �������.
                }
            }
        }

        _unitActionSystem.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged; // ���������� �� ������� ��������� �������� �������� (����� �������� �������� �������� � ����� ������ �� �������� ������� Event)
        _unitActionSystem.OnBusyChanged += Instance_OnBusyChanged; //���������� �� ������� ��������� �������� 
        _mouseOnGameGrid.OnMouseGridPositionChanged += MouseOnGameGrid_OnMouseGridPositionChanged;// ���������� �� ������� �������� ������� ���� �������� ��� ��������� � ���������� ����� �����. �������� ��������� �������
        MoveAction.OnAnyUnitPathComplete += MoveAction_OnAnyUnitPathComplete; //���������� � ������ ����� �������� ����       

        UpdateGridVisual();


        // ��������� ��� ����� ��� ������� HEX(�������������� ������ � ������ MouseOnGameGrid_OnMouseGridPositionChanged)
        /* for (int x = 0; x < _levelGrid.GetWidth(); x++)
         {
             for (int y = 0; y < _levelGrid.GetHeight(); y++)
             {
                 _gridSystemVisualSingleArray[x, y].
                     ShowTooltipsFollowMouse(GetGridVisualTypeMaterial(GridVisualType.White));
             }
         }*/

    }

    private void MoveAction_OnAnyUnitPathComplete(object sender, Unit unit)
    {
        if (_unitActionSystem.GetSelectedUnit() == unit) // ���� ���� �������� � ����������� ����� �� ������� ������������ (����� �� ��������� �� ����� ������ ������)
        {
            UpdateGridVisual();
        }
    }
    private void Instance_OnBusyChanged(object sender, OnUnitSystemEventArgs e)
    {
        if (e.selectedAction is not MoveAction) // ���� ��������� �������� �� �� ����� �������� �� �������. MoveAction ��� �������� �� OnBusyChanged � � ��� ����������� ����.
                                                // ����� ������� ���������� ������� OnAnyUnitPathComplete ��� � ����� ���������
        {
            UpdateGridVisual();
        }
    }
    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }



    private void MouseOnGameGrid_OnMouseGridPositionChanged(object sender, MouseOnGameGrid.OnMouseGridPositionChangedEventArgs e)
    {
        // ��� ��������� ��������� ���� ����� ��������� ����������� ����� ��� ��������, ������� ���������� ������ �������� �������

        BaseAction selectedAction = _unitActionSystem.GetSelectedAction(); // ������� ��������� ��������

        switch (selectedAction) // ������������� ��������� ������� � ����������� �� ���������� ��������
        {
            case GrenadeFragmentationAction grenadeFragmentationAction:// �� ����� ������� �������

                UpdateVisualDamageCircleGrenade(grenadeFragmentationAction, e);
                break;

            case GrenadeStunAction grenadeStunAction:// �� ����� ������� �������

                UpdateVisualDamageCircleGrenade(grenadeStunAction, e);
                break;

            case GrenadeSmokeAction grenadeSmokeAction:

                UpdateVisualDamageQuadGrenade(grenadeSmokeAction, e);
                break;

            case GrappleAction comboAction:
                if (comboAction.GetState() == GrappleAction.State.ComboStart)
                {
                    UpdateVisualSelectedQuadComboAction(comboAction, e);
                }
                break;
        }
    }

    private void UpdateVisualSelectedQuadComboAction(GrappleAction comboAction, MouseOnGameGrid.OnMouseGridPositionChangedEventArgs e)
    {
        _gridSystemVisualSingleArray[e.lastMouseGridPosition.x, e.lastMouseGridPosition.z, e.lastMouseGridPosition.floor].HideQuadGrenade(); // ������ ������� �� ���������� ������
        GridPositionXZ mouseGridPosition = e.newMouseGridPosition; // �������� ������� ����

        if (_validGridPositionForComboActionList.Contains(mouseGridPosition)) // ���� �������� ������� ���� ������ � ���������� �������� �� ...
        {
            _gridSystemVisualSingleArray[mouseGridPosition.x, mouseGridPosition.z, mouseGridPosition.floor].ShowQuadGrenade(GetGridVisualTypeMaterial(GridVisualType.Red)); // ������� ��� ����� �������� ������� ������� �������� ����� (���� ���� ����������� ������������ �����)
        }
    }

    private void UpdateVisualDamageQuadGrenade(GrenadeAction grenadeAction, MouseOnGameGrid.OnMouseGridPositionChangedEventArgs e)
    {
        _gridSystemVisualSingleArray[e.lastMouseGridPosition.x, e.lastMouseGridPosition.z, e.lastMouseGridPosition.floor].HideQuadGrenade(); // ������ ������� �� ���������� ������
        GridPositionXZ mouseGridPosition = e.newMouseGridPosition; // �������� ������� ����

        if (_validGridPositionForGrenadeActionList.Contains(mouseGridPosition)) // ���� �������� ������� ���� ������ � ���������� �������� �� ...
        {
            float damageRadiusInWorldPosition = grenadeAction.GetDamageRadiusInWorldPosition();
            _gridSystemVisualSingleArray[mouseGridPosition.x, mouseGridPosition.z, mouseGridPosition.floor].ShowQuadGrenade(GetGridVisualTypeMaterial(GridVisualType.RedSoft), damageRadiusInWorldPosition); // ������� ��� ����� �������� ������� ������� ��������� �� ������� � ��������� ������  � ���� ���������
        }
    }

    private void UpdateVisualDamageCircleGrenade(GrenadeAction grenadeAction, MouseOnGameGrid.OnMouseGridPositionChangedEventArgs e)
    {
        _gridSystemVisualSingleArray[e.lastMouseGridPosition.x, e.lastMouseGridPosition.z, e.lastMouseGridPosition.floor].Hide�ircleGrenade(); // ������ ���� �� ���������� ������
        GridPositionXZ mouseGridPosition = e.newMouseGridPosition; // �������� ������� ����

        if (_validGridPositionForGrenadeActionList.Contains(mouseGridPosition)) // ���� �������� ������� ���� ������ � ���������� �������� �� ...
        {
            float damageRadiusInWorldPosition = grenadeAction.GetDamageRadiusInWorldPosition();
            _gridSystemVisualSingleArray[mouseGridPosition.x, mouseGridPosition.z, mouseGridPosition.floor].Show�ircleGrenade(damageRadiusInWorldPosition); // ������� ��� ����� �������� ������� ���� ��������� �� ������� � ��������� ������ �����
        }
    }

    private void HideAllGridPosition() // ������ ��� ������� �����
    {
        for (int x = 0; x < _levelGrid.GetWidth(); x++)
        {
            for (int z = 0; z < _levelGrid.GetHeight(); z++)
            {
                for (int floor = 0; floor < _levelGrid.GetFloorAmount(); floor++)  // ��������� ��� �����
                {
                    _gridSystemVisualSingleArray[x, z, floor].Hide();
                }
            }
        }
    }

    private void ShowGridPositionRange(GridPositionXZ gridPosition, int range, GridVisualType gridVisualType, bool showFigureRhombus) // �������� ��������� �������� �������� ������� ��� �������� (� ��������� �������� �������� �������, ������ ��������, ��� ��������� ������� �����, ������� ���������� ���� ���� ���������� � ���� ����� �� �������� true, ���� � ���� �������� �� - false )
    {
        // �� �������� ��� � ShootAction � ������ "public override List<GridPositionXZ> GetValidActionGridPositionList()"

        List<GridPositionXZ> gridPositionList = new List<GridPositionXZ>();

        for (int x = -range; x <= range; x++)  // ���� ��� ����� ����� ������� � ������������ unitGridPosition, ������� ��������� ���������� �������� � �������� ������� range
        {
            for (int z = -range; z <= range; z++)
            {
                GridPositionXZ offsetGridPosition = new GridPositionXZ(x, z, 0); // ��������� �������� �������. ��� ������� ���������(0,0) �������� ��� ���� 
                GridPositionXZ testGridPosition = gridPosition + offsetGridPosition; // ����������� �������� �������

                if (!_levelGrid.IsValidGridPosition(testGridPosition)) // �������� �������� �� testGridPosition ���������� �������� �������� ���� ��� �� ��������� � ���� �����
                {
                    continue; // continue ���������� ��������� ���������� � ��������� �������� ����� 'for' ��������� ��� ����
                }

                LevelGridNode levelGridNode = _levelGrid.GetGridNode(testGridPosition);

                //���� � ���� ������� ��� ���� ���� ������ ��� GridPositionXZ ����� � �������  
                if (levelGridNode == null)
                {
                    continue; // ��������� ��� �������
                }


                /*//�������� �������� ������� ������� ����� � �������
                if (PathfindingMonkey.Instance.GetGridPositionInAirList().Contains(testGridPosition))
                {
                    continue;
                }*/

                if (showFigureRhombus)
                {
                    // ��� ��������� �������� ������� ���� � �� �������
                    int testDistance = Mathf.Abs(x) + Mathf.Abs(z); // ����� ���� ������������� ��������� �������� �������
                    if (testDistance > range) //������� ������ �� ����� � ���� ����� // ���� ���� � (0,0) �� ������ � ������������ (5,4) ��� �� ������� �������� 5+4>7
                    {
                        continue;
                    }
                }
                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType); // ������� ��������� �������� ��������
    }

    public void ShowGridPositionList(List<GridPositionXZ> gridPositionlist, GridVisualType gridVisualType)  //������� ������ GridPositionXZ (� ��������� ���������� ������ GridPositionXZ � ��������� ������������ ����� gridVisualType)
    {
        foreach (GridPositionXZ gridPosition in gridPositionlist) // � ����� ��������� ������ � �������(�������) ������ �� ������� ������� ��� ��������
        {
            _gridSystemVisualSingleArray[gridPosition.x, gridPosition.z, gridPosition.floor].
                Show(GetGridVisualTypeMaterial(gridVisualType)); // � �������� ShowTooltipsFollowMouse �������� �������� � ����������� �� ����������� ��� �������
        }
    }

    public void UpdateGridVisual() // ���������� ������� �����
    {
        HideAllGridPosition(); // ������ ��� ������� �����

        Unit selectedUnit = _unitActionSystem.GetSelectedUnit(); //������� ���������� �����

        BaseAction selectedAction = _unitActionSystem.GetSelectedAction(); // ������� ��������� ��������

        GridVisualType gridVisualType;  // �������� ����� ���� GridVisualType

        switch (selectedAction) // ������������� ��������� ������� ����� � ����������� �� ���������� ��������
        {
            default: // ���� ���� ����� ����������� �� ��������� ���� ��� ��������������� selectedAction
            case MoveAction moveAction: // �� ����� ������ -�����
                gridVisualType = GridVisualType.White;
                break;

            case VisionAction spinAction: // �� ����� �������� -�������
                gridVisualType = GridVisualType.Blue;
                break;

            case HealAction healAction: // �� ����� ������� -�������
                gridVisualType = GridVisualType.Green;
                ShowGridPositionRange(selectedUnit.GetGridPosition(), healAction.GetMaxActionDistance(), GridVisualType.GreenSoft, false); // ������� �������� 
                break;

            case ShootAction shootAction: // �� ����� �������� -�������
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetMaxActionDistance(), GridVisualType.RedSoft, true); // ������� �������� �������� ����-true
                break;

            case GrenadeAction grenadeAction:// �� ����� ������� ������� -������
                gridVisualType = GridVisualType.Yellow;
                _validGridPositionForGrenadeActionList = grenadeAction.GetValidActionGridPositionList(); //�������� -������ ���������� �������� ������� ��� �������� �������                                 
                break;

            case SwordAction swordAction: // �� ����� ����� ����� -�������
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRange(selectedUnit.GetGridPosition(), swordAction.GetMaxActionDistance(), GridVisualType.RedSoft, false); // ������� �������� �����
                break;

            case InteractAction interactAction: // �� ����� �������������� -�������
                gridVisualType = GridVisualType.Blue;
                ShowGridPositionRange(selectedUnit.GetGridPosition(), interactAction.GetMaxActionDistance(), GridVisualType.BlueSoft, false); // ������� �������� 
                break;

            case GrappleAction comboAction: // �� ����� ������ ����� �������� -�������    

                GrappleAction.State statecomboAction = comboAction.GetState(); // ������� ��������� ����� 
                switch (statecomboAction)
                {
                    default:
                    case GrappleAction.State.ComboSearchPartner: // ���� ���� �������� ��� �����
                        gridVisualType = GridVisualType.Green;
                        ShowGridPositionRange(selectedUnit.GetGridPosition(), comboAction.GetMaxActionDistance(), GridVisualType.GreenSoft, false); // ������� �������� 
                        break;

                    case GrappleAction.State.ComboSearchEnemy: // ���� ���� ����� ��
                        gridVisualType = GridVisualType.Red;
                        ShowGridPositionRange(selectedUnit.GetGridPosition(), comboAction.GetMaxActionDistance(), GridVisualType.RedSoft, true); // ������� ��������  ����-true
                        break;

                    case GrappleAction.State.ComboStart: // ������ ���� ���� ����������
                        gridVisualType = GridVisualType.RedSoft;
                        _validGridPositionForComboActionList = selectedAction.GetValidActionGridPositionList();
                        break;

                }
                break;

            case SpotterFireAction spotterFireAction: // ������������� ���� -���������
                gridVisualType = GridVisualType.Green;
                ShowGridPositionRange(selectedUnit.GetGridPosition(), spotterFireAction.GetMaxActionDistance(), GridVisualType.GreenSoft, false); // ������� �������� 
                break;
        }

        ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType); // �������(�������) ������ �� ������� ������� ��� �������� (� �������� �������� ������ ���������� ������� ����� ���������� ��������, � ��� ��������� ������������ ������� ��� ����� switch)
    }



#if HEX_GRID_SYSTEM // ���� ������������ �������� �������

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType) //(������� �������� � ����������� �� ���������) �������� ��� ��������� ��� �������� ������������ � ����������� �� ����������� � �������� ��������� �������� ������������
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in _gridVisualTypeMaterialListHex) // � ����� ��������� ������ ��� ��������� ����������� ��������� ����� 
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType) // ����  ��������� �����(gridVisualType) ��������� � ���������� ��� ��������� �� ..
            {
                return gridVisualTypeMaterial.materialGrid; // ������ �������� ��������������� ������� ��������� �����
            }
        }

        Debug.LogError("�� ���� ����� GridVisualTypeMaterial ��� GridVisualType " + gridVisualType); // ���� �� ������ ����������� ������ ������
        return null;
    }


#else//� ��������� ������ �������������

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType) //(������� �������� � ����������� �� ���������) �������� ��� ��������� ��� �������� ������������ � ����������� �� ����������� � �������� ��������� �������� ������������
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in _gridVisualTypeMaterialListQuad) // � ����� ��������� ������ ��� ��������� ����������� ��������� ����� 
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType) // ����  ��������� �����(gridVisualType) ��������� � ���������� ��� ��������� �� ..
            {
                return gridVisualTypeMaterial.materialGrid; // ������ �������� ��������������� ������� ��������� �����
            }
        }

        Debug.LogError("�� ���� ����� GridVisualTypeMaterial ��� GridVisualType " + gridVisualType); // ���� �� ������ ����������� ������ ������
        return null;
    }
#endif
}

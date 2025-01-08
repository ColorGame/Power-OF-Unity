using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������������ ���������� � ����� ����� 
/// </summary>
/// <remarks>�������� � ����� PlacedObject</remarks>
public class ArmorGridVisual : MonoBehaviour
{
    [Serializable] // ����� ��������� ��������� ����� ������������ � ����������
    public struct GridVisualTypeColor    //������ ����� ��� ��������� // �������� ��������� ����� � ��������� ������. ������ � �������� ��������� ������������ ��� ���� ������ �������� ����������� ����� ������ � C#
    {                                       //� ������ ��������� ��������� ��������� ����� � ����������
        public GridVisualType gridVisualType;
        public Color colorlGrid;
    }

    public enum GridVisualType //���������� ��������� �����
    {
        Ocean,
        Orange,
    }

    [SerializeField] private List<GridVisualTypeColor> _gridVisualColorList; // ������ ������ ��� ������������ �����
    [SerializeField] private EquipmentGridVisualSingleSetColor _armorHeadGridVisual;
    [SerializeField] private EquipmentGridVisualSingleSetColor _armorBodyGridVisual;


    private PickUpDropPlacedObject _pickUpDropPlacedObject;
    private EquipmentGrid _equipmentGrid;
    private UnitEquipmentSystem _unitEquipmentSystem;
    private Canvas _canvasArmorGrid;


    public void Init(PickUpDropPlacedObject pickUpDrop, EquipmentGrid equipmentGrid, UnitEquipmentSystem unitEquipmentSystem)
    {
        _pickUpDropPlacedObject = pickUpDrop;
        _equipmentGrid = equipmentGrid;
        _unitEquipmentSystem = unitEquipmentSystem;

        _canvasArmorGrid = _equipmentGrid.GetCanvasArmorGrid();
    }

    public void SetActive(bool active)
    {
        if (active)
        {
            // �������� ����� ������ ���� ������� ������ ����� 
            if (_equipmentGrid.GetStateGrid() == EquipmentGrid.GridState.ArmorGrid)
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }
        else
        {
            Disable();
        }
    }

    private void Enable()
    {
        _canvasArmorGrid.enabled = true;

        _equipmentGrid.OnAddInEquipmentGrid += EquipmentGrid_OnAddEquipmentGrid;
        _equipmentGrid.OnRemoveFromEquipmentGridAndHung += EquipmentGrid_OnRemoveFromEquipmentGridAndHung;
        _equipmentGrid.OnRemoveFromEquipmentGridAndMoveStartPosition += EquipmentGrid_OnRemoveFromEquipmentGridAndMoveStartPosition;

        _pickUpDropPlacedObject.OnGrabbedObjectGridPositionChanged += PickUpDropManager_OnGrabbedObjectGridPositionChanged;
        _pickUpDropPlacedObject.OnGrabbedObjectGridExits += PickUpDropManager_OnGrabbedObjectGridExits;

        _unitEquipmentSystem.OnEquipmentGridsCleared += UnitEquipmentSystem_OnEquipmentGridsCleared;       
    }

    private void Disable()
    {
        _canvasArmorGrid.enabled = false;

        _equipmentGrid.OnAddInEquipmentGrid -= EquipmentGrid_OnAddEquipmentGrid;
        _equipmentGrid.OnRemoveFromEquipmentGridAndHung -= EquipmentGrid_OnRemoveFromEquipmentGridAndHung;
        _equipmentGrid.OnRemoveFromEquipmentGridAndMoveStartPosition += EquipmentGrid_OnRemoveFromEquipmentGridAndMoveStartPosition;

        _pickUpDropPlacedObject.OnGrabbedObjectGridPositionChanged -= PickUpDropManager_OnGrabbedObjectGridPositionChanged;
        _pickUpDropPlacedObject.OnGrabbedObjectGridExits -= PickUpDropManager_OnGrabbedObjectGridExits;

        _unitEquipmentSystem.OnEquipmentGridsCleared -= UnitEquipmentSystem_OnEquipmentGridsCleared;       
    }

    /// <summary>
    /// ��������� ������
    /// </summary>    
    private void UnitEquipmentSystem_OnEquipmentGridsCleared(object sender, EventArgs e)
    {
        SetDefoltState(); // ���������� ��������� ���������        
    }
    /// <summary>
    /// ���������� ������ ������� �����
    /// </summary>
    private void PickUpDropManager_OnGrabbedObjectGridExits(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    /// <summary>
    /// ������� ������������ ������� �� ����� ����������
    /// </summary>
    private void PickUpDropManager_OnGrabbedObjectGridPositionChanged(object sender, PlacedObjectGridParameters e)
    {
       // UpdateVisual();
        ShowPossibleGridPositions(e.slot, e.placedObject, e.gridPositioAnchor, GridVisualType.Orange); //�������� ��������� �������� �������
    }
    /// <summary>
    /// ������ ������ �� ����� � ����� ��� ���
    /// </summary>
    private void EquipmentGrid_OnRemoveFromEquipmentGridAndHung(object sender, PlacedObject placedObject)
    {
        SetIsBusyAndMaterial(placedObject, false, GridVisualType.Orange);
    }

    /// <summary>
    /// ������ ������ �� ����� � �������� � ��������� �������
    /// </summary>
    private void EquipmentGrid_OnRemoveFromEquipmentGridAndMoveStartPosition(object sender, PlacedObject placedObject)
    {
        SetIsBusyAndMaterial(placedObject, false, GridVisualType.Ocean);
    }
    /// <summary>
    /// ������ �������� � ����� 
    /// </summary>
    private void EquipmentGrid_OnAddEquipmentGrid(object sender, PlacedObject placedObject)
    {
        SetIsBusyAndMaterial(placedObject, true);
    }
    /// <summary>
    /// �������� ������
    /// </summary>
    private void UpdateVisual()
    {
        SetVisualTypeOnFreeGridPosition(GridVisualType.Ocean);
    }
    /// <summary>
    /// ���������� ��������� ���������
    /// </summary>
    private void SetDefoltState()
    {
        _armorHeadGridVisual.SetIsBusyAndColor(false, GetColorTypeMaterial(GridVisualType.Ocean));
        _armorBodyGridVisual.SetIsBusyAndColor(false, GetColorTypeMaterial(GridVisualType.Ocean));
    }
    /// <summary>
    /// ���������� ��������� � ��� ������������
    /// </summary>
    private void SetIsBusyAndMaterial(PlacedObject placedObject, bool isBusy, GridVisualType gridVisualType = 0)
    {        
        EquipmentSlot equipmentSlot = placedObject.GetGridSystemXY().GetGridSlot(); // ���� �����

        switch (equipmentSlot)
        {
            case EquipmentSlot.HeadArmorSlot:
                _armorHeadGridVisual.SetIsBusyAndColor(isBusy, GetColorTypeMaterial(gridVisualType));
                break;
            case EquipmentSlot.BodyArmorSlot:
                _armorBodyGridVisual.SetIsBusyAndColor(isBusy, GetColorTypeMaterial(gridVisualType));
                break;
        }
    }

    /// <summary>
    /// �������� ��������� �������� �������
    /// </summary>
    private void ShowPossibleGridPositions(EquipmentSlot equipmentSlot, PlacedObject placedObject, Vector2Int gridPositioAnchor, GridVisualType gridVisualType)
    {       
        switch (equipmentSlot)
        {
            case EquipmentSlot.HeadArmorSlot:
                if (!_armorHeadGridVisual.GetIsBusy())// ���� ����� �� ������
                {
                    _armorHeadGridVisual.Show(GetColorTypeMaterial(gridVisualType));
                }
                break;
            case EquipmentSlot.BodyArmorSlot:
                if (!_armorBodyGridVisual.GetIsBusy())// ���� ����� �� ������
                {
                    _armorBodyGridVisual.Show(GetColorTypeMaterial(gridVisualType));
                }
                break;
        }
    }
    /// <summary>
    /// ���������� ���������� ��� ������������ �� ��������� �������� �������
    /// </summary>
    private void SetVisualTypeOnFreeGridPosition(GridVisualType gridVisualType)
    {
        if (!_armorHeadGridVisual.GetIsBusy())// ���� ����� �� ������
        {
            _armorHeadGridVisual.Show(GetColorTypeMaterial(gridVisualType));
        }
        if (!_armorBodyGridVisual.GetIsBusy())// ���� ����� �� ������
        {
            _armorBodyGridVisual.Show(GetColorTypeMaterial(gridVisualType));
        }
    }

    /// <summary>
    /// (������� ���� � ����������� �� ���������) �������� ���� ��� �������� ������������ � ����������� �� ����������� � �������� ��������� �������� ������������
    /// </summary>
    private Color GetColorTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeColor gridVisualTypeMaterial in _gridVisualColorList) // � ����� ��������� ������ ��� ��������� ����������� ��������� ����� 
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType) // ����  ��������� �����(gridVisualType) ��������� � ���������� ��� ��������� �� ..
            {
                return gridVisualTypeMaterial.colorlGrid; // ������ �������� ��������������� ������� ��������� �����
            }
        }

        Debug.LogError("�� ���� ����� GridVisualTypeColor ��� GridVisualType " + gridVisualType); // ���� �� ������ ����������� ������ ������
        return new Color(0, 0, 0, 0);
    }

    private void OnDestroy()
    {
        _unitEquipmentSystem = null; // ������� ������ ����� ����� ���������
    }
}

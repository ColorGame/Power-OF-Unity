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


    public void Init(PickUpDropPlacedObject pickUpDrop, EquipmentGrid equipmentGrid, UnitEquipmentSystem unitEquipmentSystem)
    {
        _pickUpDropPlacedObject = pickUpDrop;
        _equipmentGrid = equipmentGrid;
        _unitEquipmentSystem = unitEquipmentSystem;
    }



    public void SetActive(bool active)
    {
        if (active)
        {
            _pickUpDropPlacedObject.OnAddPlacedObjectAtEquipmentGrid += OnAddPlacedObjectAtGrid;
            _pickUpDropPlacedObject.OnRemovePlacedObjectAtEquipmentGrid += PickUpDropSystem_OnRemovePlacedObjectAtGrid;
            _pickUpDropPlacedObject.OnGrabbedObjectGridExits += PickUpDropManager_OnGrabbedObjectGridExits;

            _unitEquipmentSystem.OnEquipmentGridsCleared += UnitEquipmentSystem_OnEquipmentGridsCleared;
            _unitEquipmentSystem.OnAddPlacedObjectAtEquipmentGrid += OnAddPlacedObjectAtGrid;
        }
        else
        {
            _pickUpDropPlacedObject.OnAddPlacedObjectAtEquipmentGrid -= OnAddPlacedObjectAtGrid;
            _pickUpDropPlacedObject.OnRemovePlacedObjectAtEquipmentGrid -= PickUpDropSystem_OnRemovePlacedObjectAtGrid;
            _pickUpDropPlacedObject.OnGrabbedObjectGridExits -= PickUpDropManager_OnGrabbedObjectGridExits;

            _unitEquipmentSystem.OnEquipmentGridsCleared -= UnitEquipmentSystem_OnEquipmentGridsCleared;
            _unitEquipmentSystem.OnAddPlacedObjectAtEquipmentGrid -= OnAddPlacedObjectAtGrid;
        }
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
    /// ������ ������ �� ����� � ����� ��� ���
    /// </summary>
    private void PickUpDropSystem_OnRemovePlacedObjectAtGrid(object sender, PlacedObject placedObject)
    {
        SetIsBusyAndMaterial(placedObject, false, GridVisualType.Orange);
    }
    /// <summary>
    /// ������ �������� � ����� 
    /// </summary>
    private void OnAddPlacedObjectAtGrid(object sender, PlacedObject placedObject)
    {
        SetIsBusyAndMaterial(placedObject, true);
    }
    /// <summary>
    /// �������� ������
    /// </summary>
    private void UpdateVisual()
    {        
        if (!_armorHeadGridVisual.GetIsBusy())// ���� ����� �� ������
        {
            _armorHeadGridVisual.Show(GetColorTypeMaterial(GridVisualType.Ocean));
        }
        if (!_armorBodyGridVisual.GetIsBusy())// ���� ����� �� ������
        {
            _armorBodyGridVisual.Show(GetColorTypeMaterial(GridVisualType.Ocean));
        }
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
    /// ���������� ��������� � ��������
    /// </summary>
    private void SetIsBusyAndMaterial(PlacedObject placedObject, bool isBusy, GridVisualType gridVisualType = 0)
    {
        GridSystemXY<GridObjectEquipmentXY> gridSystemXY = placedObject.GetGridSystemXY(); // �������� ������� ������� �������� ������
        List<Vector2Int> OccupiesGridPositionList = placedObject.GetOccupiesGridPositionList(); // ������ ���������� �������� �������
        EquipmentSlot equipmentSlot = gridSystemXY.GetGridSlot(); // ���� �����

        switch (equipmentSlot)
        {
            case EquipmentSlot.ArmorHeadSlot:
                _armorHeadGridVisual.SetIsBusyAndColor(isBusy, GetColorTypeMaterial(gridVisualType));
                break;
            case EquipmentSlot.ArmorBodySlot:
                _armorBodyGridVisual.SetIsBusyAndColor(isBusy, GetColorTypeMaterial(gridVisualType));
                break;
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

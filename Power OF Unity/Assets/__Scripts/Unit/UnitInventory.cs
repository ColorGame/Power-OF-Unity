using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��������� ����� (��������� ��� ���������, �������� ������� �����). 
/// </summary>
public class UnitInventory
{
    public UnitInventory(Unit unit)
    {
        _unit = unit;
        _placedObjectList = new List<PlacedObjectParameters>(); 
    }

    public event EventHandler<PlacedObject> OnAddPlacedObjectList; // ������� �������� ������� � ���������
    public event EventHandler<PlacedObject> OnRemovePlacedObjectList; // ������� ������ ������� �� ���������

    private Unit _unit;
    private List<PlacedObjectParameters> _placedObjectList; // ������ ��������� ���������


    /// <summary>
    /// �������� ���������� ������ � ������ "����������� �������� � ����� ���������".
    /// </summary>  
    public void AddPlacedObjectList(PlacedObject placedObject)
    {       
        _placedObjectList.Add(placedObject.GetPlacedObjectParameters());
        foreach (BaseAction baseAction in placedObject.GetPlacedObjectTypeSO().GetBaseActionsArray())
        {
            _unit.AddBaseActionsList(baseAction);
        }
        OnAddPlacedObjectList?.Invoke(this, placedObject);
    }
    /// <summary>
    /// ������� ���������� ������ �� ������� "����������� �������� � ����� ���������".
    /// </summary>  
    public void RemovePlacedObjectList(PlacedObject placedObject)
    {
        //_placedObjectList.Remove(placedObject);

        OnRemovePlacedObjectList?.Invoke(this, placedObject);
    }


    /// <summary>
    /// ����������� ������ � ����� ���������
    /// </summary>
    [Serializable]
    public struct PlacedObjectOnInventoryGrid
    {
        public GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY; //����� �� ������� �������� ����������� ������
        public Vector2Int gridPositioAnchor; // �������� ������� ����� ������������ �������
        public PlacedObjectTypeSO placedObjectTypeSO;
    }
    //private List<PlacedObjectOnInventoryGrid> _placedObjectOnInventoryGrid; // ������ ����������� �������� � ����� ���������.
}

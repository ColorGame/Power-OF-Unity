using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��������� ����� (��������� ��� ���������, �������� ������� �����). 
/// </summary>
/// <remarks>
/// ��������� ������ ���� ���������� � �����
/// </remarks>
public class UnitInventory : MonoBehaviour // ��������� �����
{   

   

    [Header("��������� ������ ��� �����\n(������ ������ ������� ������� ����)")]
    [SerializeField] private List<PlacedObject> _placedObjectList; // ������ ����������� �������� � ����� ���������.
   

   
    private Unit _unit;
    private UnitEquipment _unitEquipment;

    private void Awake()
    {
        _unit = GetComponent<Unit>();
        _unitEquipment = GetComponent<UnitEquipment>();
    }

    private void Start()
    {
        if (_unit.IsEnemy()) // ���� ��� ���� ���� �� ��������� ��� ��������� �� ������
        {
            foreach (PlacedObject placedObject in _placedObjectList)
            {
                _unitEquipment.EquipArmor(placedObject);
            }
        }
    }

    /// <summary>
    /// �������� ���������� ������ � ������ "����������� �������� � ����� ���������".
    /// </summary>  
    public void AddPlacedObjectList(PlacedObject placedObject)
    {
        _placedObjectList.Add(placedObject);       
    }
    /// <summary>
    /// ������� ���������� ������ �� ������� "����������� �������� � ����� ���������".
    /// </summary>  
    public void RemovePlacedObjectList(PlacedObject placedObject)
    {
        _placedObjectList.Remove(placedObject);     
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

using UnityEngine;


/// <summary>
/// ������ ����� (������ ��������� ������������ ������� PlacedObject) 
/// </summary>
/// <remarks>
/// GridObjectEquipmentXY ��������� � ������ ������ �����. �������� ��������� ��� �������� ��������
/// </remarks>
public class GridObjectEquipmentXY 
{
    private GridSystemXY<GridObjectEquipmentXY> _gridSystem; // �������� ������� .� �������� ������� ��� GridObjectUnitXZ// ������� �������� ������� ������� ������� ���� ������ (��� ���������� �������� ��� ����� 2-�� �����)
    private Vector2Int _gridPosition; // ��������� ������� � �����
    private PlacedObject _placedObject; // ����������� ������.
    private IInteractable _interactable; // IInteractable(��������� ��������������) (����� ������ GridObjectUnitXZ ��� � ����) ��������� ��������� ����������������� � ����� �������� (�����, �����, ������...) - ������� ��������� ���� ���������

    public GridObjectEquipmentXY(GridSystemXY<GridObjectEquipmentXY> gridSystem, Vector2Int gridPosition) // ����������� 
    {
        _gridSystem = gridSystem;
        _gridPosition = gridPosition;
        _placedObject = null;
    }

    public override string ToString() // ������������� ToString(). ����� ��� ���������� ������� � ����� � ����� � ���� ������ (����� ����� ��������� �������� ������������ ������)
    {                
        return _gridPosition.ToString() + "\n" +"!-"+ _placedObject;        
    }

    public void AddPlacedObject(PlacedObject placedObject) // ������� ����������� ������ 
    {
        _placedObject = placedObject;
    }

    public PlacedObject GetPlacedObject() // �������� ����������� ������ 
    {
        return _placedObject;
    }

    public void RemovePlacedObject() // ������ ����������� ������ 
    {
        _placedObject = null;
    } 

    public bool HasPlacedObject()
    {
        return _placedObject != null;
    }


}


using UnityEngine;

/// <summary>
/// ��������� ����� ���������� �� ��� ��������� (������� � ������� ������������� �������). 
/// </summary>
/// <remarks>
/// ��������� ������ ���� ���������� � �����
/// </remarks>
public class UnitEquipment 
{
    public UnitEquipment(Unit unit)
    {
        _unit = unit;
    }

    private Unit _unit;
    private Transform _rightHandTransform = null;
    private Transform _leftHandTransform = null;

    public void SetupForSpawn()
    {
        // ����������� �� ������� ����� ������� ����� ��� ��������� ���������� rightHandTransform � leftHandTransform �.�. ��� ������ ����� ������ �������
        if (_unit.IsEnemy())
        {
            PlacedObjectTypeSO mainPlacedObjectTypeSO =  _unit.GetUnitTypeSO<UnitEnemySO>().GetMainPlacedObjectTypeSO(); // ������� �������� ������ ��� ���������� �����
            // �������
            PlacedObject placedObject = PlacedObject.CreateInWorld(_rightHandTransform.position, mainPlacedObjectTypeSO, _unit.GetTransform());
            EquipWeapon(placedObject);
        }
    }


    /// <summary>
    ///  ����������� �������, ����� � �������� ���������� ���� ������
    /// </summary>
    public void EquipWeapon(PlacedObject placedObject)
    {
        //�������� �� PlacedObject  SO � �� ���� �� ����� ���� � ������

    }

    /// <summary>
    ///  ����������� ������, ����� � �������� ���������� ���� ������
    /// </summary>
    public void EquipArmor()
    {

    }

    public void FreeHands() // ���������� ����
    {
       /* if (InventorySlot.MainWeaponSlot == placedObject.GetGridSystemXY().GetGridSlot())//���� ������ �� ����� ��������� ������
        {
           // ������ ����������
        }*/
    }
}

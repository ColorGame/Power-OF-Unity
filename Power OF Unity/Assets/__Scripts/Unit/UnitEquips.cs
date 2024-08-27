using UnityEngine;

/// <summary>
/// ��������� ����� ���������� �� ��� ���������� (������� � ������� ������������� �������). 
/// </summary>
public class UnitEquips 
{
    /// <summary>
    /// �������� ����� ���������� �� ��� ���������� (������� � ������� ������������� �������). 
    /// </summary>
    public UnitEquips(Unit unit)
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
            PlacedObjectTypeWithActionSO mainplacedObjectTypeWithActionSO =  _unit.GetUnitTypeSO<UnitEnemySO>().GetMainplacedObjectTypeWithActionSO(); // ������� �������� ������ ��� ���������� �����
            // �������
          //  PlacedObject placedObject = PlacedObject.CreateInWorld(_rightHandTransform.position, mainplacedObjectTypeWithActionSO, _unit.GetTransform(), _unit.Get);
         //   EquipWeapon(placedObject);
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
       /* if (EquipmentSlot.MainWeaponSlot == placedObject.GetActiveGridSystemXY().GetGridSlot())//���� ������ �� ����� ��������� ������
        {
           // ������ ����������
        }*/
    }
}

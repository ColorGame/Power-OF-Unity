using UnityEngine;

/// <summary>
/// ��������� ����� ���������� �� ��� ��������� (������� � ������� ������������� �������). 
/// </summary>
/// <remarks>
/// ��������� ������ ���� ���������� � �����
/// </remarks>
public class UnitEquipment : MonoBehaviour
{
    private Transform _rightHandTransform = null;
    private Transform _leftHandTransform = null;

    private void Start()
    {
        // ����������� �� ������� ����� ������� ����� ��� ��������� ���������� rightHandTransform � leftHandTransform �.�. ��� ������ ����� ������ �������
    }


    /// <summary>
    ///  ����������� ������, ����� � �������� ���������� ���� ������
    /// </summary>
    public void EquipArmor(PlacedObject placedObject)
    {
        //�������� �� PlacedObject  SO � �� ���� �� ����� ���� � ������
    }
}

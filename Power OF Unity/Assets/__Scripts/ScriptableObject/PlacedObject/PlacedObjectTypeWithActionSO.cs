
using UnityEngine;

/// <summary>
/// ����������� ������ c ��������� (ACTION)
/// </summary>
/// <remarks>
/// abstract - ������ ������� ��������� ������� ������.
/// �� �������� ��, ������ �����, ������ ������������ ��������, ����� ��� "SwordTypeSO"  "ShootingWeaponTypeSO" ��� "GrenadeTypeSO" "HealItemTypeSO".
/// </remarks>

public abstract class PlacedObjectTypeWithActionSO : PlacedObjectTypeSO
{
    [Header("������ ������������ ������� 3D(��� �������� � ����)")]
    [SerializeField] private Transform _prefab3D;
    /// <summary>
    /// �������� ������� �������� ��� ������� PlacedObjectTypeWithActionSO
    /// </summary>
    /// <remarks>
    /// � �������� �������� ����� � �������� ����� �������� BaseAction
    /// </remarks>
    public abstract BaseAction GetAction(Unit unit);

    protected Transform GetPrefab3D() {  return _prefab3D; }
}

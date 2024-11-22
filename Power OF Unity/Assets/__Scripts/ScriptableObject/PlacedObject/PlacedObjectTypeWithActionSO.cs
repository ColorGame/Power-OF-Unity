
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
    [SerializeField] private AnimatorOverrideController _animatorOverrideController = null;
    /// <summary>
    /// �������� ������� �������� ��� ������� PlacedObjectTypeWithActionSO
    /// </summary>
    /// <remarks>
    /// � �������� �������� ����� � �������� ����� �������� BaseAction
    /// </remarks>
    public abstract BaseAction GetAction(Unit unit);

    public AnimatorOverrideController GetAnimatorOverrideController() {  return _animatorOverrideController; }
    protected Transform GetPrefab3D() {  return _prefab3D; }


}

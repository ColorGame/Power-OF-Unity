
using System;
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
    [SerializeField] protected GameObject _prefab3D;
    [Header("��������� ��������������� ��������")]
    [SerializeField] private AnimatorOverrideController _animatorOverrideController = null;
    
   
    /// <summary>
    /// �������� ������� �������� ��� ������� PlacedObjectTypeWithActionSO
    /// </summary>
    /// <remarks>
    /// � �������� �������� ����� � �������� ����� �������� BaseAction
    /// </remarks>
    public abstract BaseAction GetAction(Unit unit);   
    public AnimatorOverrideController GetAnimatorOverrideController() { return _animatorOverrideController; }
    public GameObject GetPrefab3D() { return _prefab3D; }



    protected void Search3DPrefab(string nameFail, GameObject[] prefab3DArray)
    {
        int prefabDeleteLastCharName = 3; // ���������� �������� ��� �������� � ����� ������� (����� ������ _3D )
      

        foreach (GameObject go in prefab3DArray)
        {
            string prefabName = go.name.Remove(go.name.Length - prefabDeleteLastCharName); // ������� ��� ������� ��� ��������� 2 ��������
            if (nameFail.Equals(prefabName, StringComparison.OrdinalIgnoreCase)) // ������� ��� SO � ��������� ��� ����� ��������
            {
                _prefab3D = go;
            }
        }
       

        if (_prefab3D == null)
        {
            Debug.Log($"�� ������� ��������� (_prefab3D). ������� ��� {name}");
        }
        
    }
}

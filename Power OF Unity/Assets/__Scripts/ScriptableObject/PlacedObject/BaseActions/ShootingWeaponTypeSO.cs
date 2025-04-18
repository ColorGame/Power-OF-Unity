using System;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/PlacedObjectType_WithAction/ShootingWeaponType")]
public class ShootingWeaponTypeSO : PlacedObjectTypeWithActionSO //���������� ������ ������ - ������ ���� SO (��������� ����� ������������ �������)
{
    [Header("��� ������������ �������")]
    [SerializeField] ShootingWeaponType _shootingWeaponType;
    [Header("��� ������ ��� ����� ���� (��� ����������� � �����)")]
    [SerializeField] private bool _isOneHand = false;
    [Header("���������� ��������� �� ���� ��������")]
    [SerializeField] private int numberShotInOneAction = 3;
    [Header("�������� ����� ����������")]
    [SerializeField] private float delayShot = 0.2f;
    [Header("��������� �������� � �������")]
    [SerializeField] private int maxShootDistance = 7;
    [Header("�������� �����")]
    [SerializeField] private int shootDamage = 6;
    [Header("������� ���������� ��������� ��������")]
    [SerializeField] private float percentageShootDistanceIncrease = 0.5f;
    [Header("������� ���������� ����� �� �������� ")]
    [SerializeField] private float percentageShootDamageIncrease = 0.5f;
    public override BaseAction GetAction(Unit unit)
    {
        return unit.GetAction<ShootAction>();
    }

    /// <summary>
    /// �������� ����������� ��������� ��� ������� ������������ ������� 
    /// </summary>
    public override PlacedObjectTooltip GetPlacedObjectTooltip()
    {
        return PlacedObjectTypeBaseStatsSO.Instance.GetTooltipPlacedObject(this, _shootingWeaponType);
    }

    public int GetNumberShotInOneAction() { return numberShotInOneAction; }
    public float GetDelayShot() { return delayShot; }
    public int GetMaxShootDistance() { return maxShootDistance; }
    public float GetShootDamage() { return shootDamage; }
    public float GetPercentageShootDistanceIncrease() { return percentageShootDistanceIncrease; }
    public float GetPercentageShootDamageIncrease() { return percentageShootDamageIncrease; }
    /// <summary>
    /// ��� ������ ��� ����� ���� (��� ����������� � �����)
    /// </summary>
    public bool GetIsOneHand() { return _isOneHand; }
    public ShootingWeaponType GetShootingWeaponType()
    {
        return _shootingWeaponType;
    }


    [ContextMenu("��������������")]
    protected override void AutoCompletion()
    {
        _shootingWeaponType = SheetProcessor.ParseEnum<ShootingWeaponType>(name);

        Search2DPrefabAndVisual(
          name,
          PlacedObjectGeneralListForAutoCompletionSO.Instance.Shooting2DArray);

        Search3DPrefab(name,
           PlacedObjectGeneralListForAutoCompletionSO.Instance.ShootingPrefab3DArray);

        if (name.Contains("Pistol") || name.Contains("Revolver"))
            _isOneHand = true;
        else
            _isOneHand = false;

        if (_isOneHand)
            _canPlacedOnSlotArray = new EquipmentSlot[] { EquipmentSlot.MainWeaponSlot, EquipmentSlot.OtherWeaponsSlot, EquipmentSlot.BagSlot };
        else
            _canPlacedOnSlotArray = new EquipmentSlot[] { EquipmentSlot.MainWeaponSlot, EquipmentSlot.BagSlot };

        base.AutoCompletion();
    }
}

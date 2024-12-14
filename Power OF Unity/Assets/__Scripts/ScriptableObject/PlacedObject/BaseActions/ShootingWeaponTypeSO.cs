using System;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/ShootingWeaponType")]
public class ShootingWeaponTypeSO : PlacedObjectTypeWithActionSO //Стреляющее оружие Оружие - объект типа SO (наследует класс Размещенного объекта)
{
    [Header("Тип размещаемого объекта")]
    [SerializeField] ShootingWeaponType _shootingWeaponType;
    [Header("Это оружие для одной руки (для отображения с ЩИТОМ)")]
    [SerializeField] private bool _isOneHand= false;
    [Header("Количество выстрелов за одно действие")]
    [SerializeField] private int numberShotInOneAction = 3;
    [Header("Задержка между выстрелами")]
    [SerializeField] private float delayShot= 0.2f;
    [Header("Дистанция выстрела в клетках")]
    [SerializeField] private int maxShootDistance = 7;
    [Header("Величина урона")]
    [SerializeField] private int shootDamage = 6;
    [Header("Процент увеличения дальности выстрела")]
    [SerializeField] private float percentageShootDistanceIncrease = 0.5f;
    [Header("Процент увеличения урона от выстрела ")]
    [SerializeField] private float percentageShootDamageIncrease = 0.5f;
    public override BaseAction GetAction(Unit unit)
    {
        return unit.GetAction<ShootAction>();
    }

    /// <summary>
    /// Получить всплывающую подсказку для данного размещенного объекта 
    /// </summary>
    public override PlacedObjectTooltip GetPlacedObjectTooltip()
    {
        return PlacedObjectTypeBaseStatsSO.Instance.GetTooltipPlacedObject(this, _shootingWeaponType);
    }

    public int GetNumberShotInOneAction() { return numberShotInOneAction; }
    public float GetDelayShot() {  return delayShot; }
    public int GetMaxShootDistance() { return maxShootDistance; }
    public float GetShootDamage() {  return shootDamage; }
    public float GetPercentageShootDistanceIncrease() {  return percentageShootDistanceIncrease; }
    public float GetPercentageShootDamageIncrease() {  return percentageShootDamageIncrease; }
    /// <summary>
    /// Это оружие для одной руки (для отображения с ЩИТОМ)
    /// </summary>
    public bool GetIsOneHand() { return _isOneHand; }
    public ShootingWeaponType GetShootingWeaponType()
    {
        return _shootingWeaponType;
    }
}

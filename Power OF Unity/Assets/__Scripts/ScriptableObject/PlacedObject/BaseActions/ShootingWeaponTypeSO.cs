using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/ShootingWeaponType")]
public class ShootingWeaponTypeSO : PlacedObjectTypeWithActionSO //Стреляющее оружие Оружие - объект типа SO (наследует класс Размещенного объекта)
{
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

    /* public override string GetToolTip()
     {
         return""+ GetName();
     }*/

    public int GetNumberShotInOneAction() { return numberShotInOneAction; }
    public float GetDelayShot() {  return delayShot; }
    public int GetMaxShootDistance() { return maxShootDistance; }
    public float GetShootDamage() {  return shootDamage; }
    public float GetPercentageShootDistanceIncrease() {  return percentageShootDistanceIncrease; }
    public float GetPercentageShootDamageIncrease() {  return percentageShootDamageIncrease; }


}

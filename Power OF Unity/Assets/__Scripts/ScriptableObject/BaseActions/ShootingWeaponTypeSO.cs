using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/ShootingWeaponType")]
public class ShootingWeaponTypeSO : PlacedObjectTypeSO //Стреляющее оружие Оружие - объект типа SO (наследует класс Размещенного объекта)
{   
   
    [SerializeField] private int numberShotInOneAction = 3; // Количество выстрелов за одно действие
    [SerializeField] private float delayShot= 0.2f; //задержка между выстрелами
    [SerializeField] private int maxShootDistance = 7;// Дистанция выстрела 
    [SerializeField] private int shootDamage = 6; // Величина уронв
    [SerializeField] private float percentageShootDistanceIncrease = 0.5f;// Процент увеличения дальности выстрела 
    [SerializeField] private float percentageShootDamageIncrease = 0.5f;//Процент увеличения урона от выстрела 
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

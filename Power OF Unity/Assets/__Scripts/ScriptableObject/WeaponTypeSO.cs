using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/WeaponType")]
public class WeaponTypeSO : PlacedObjectTypeSO // Оружие - объект типа SO (наследует класс Размещенного объекта)
{
   


   
    public int damage; // Величина урона
    public int numberShotInOneAction; // Количество выстрелов за одно действие
    public float delayShot; //задержка между выстрелами
    
    public override string GetToolTip()
    {
        return""+ GetName();
    }

  


}

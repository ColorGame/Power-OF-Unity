using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/WeaponType")]
public class WeaponTypeSO : PlacedObjectTypeSO // Оружие - объект типа SO (наследует класс Размещенного объекта)
{

    [SerializeField] private int _shootDistance; // Дистанция выстрела 
    [SerializeField] private int damage; // Величина урона
    [SerializeField] private int numberShotInOneAction; // Количество выстрелов за одно действие
    [SerializeField] private float delayShot; //задержка между выстрелами
    
   /* public override string GetToolTip()
    {
        return""+ GetName();
    }*/

  


}

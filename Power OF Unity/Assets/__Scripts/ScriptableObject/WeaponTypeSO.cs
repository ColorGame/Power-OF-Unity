using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/WeaponType")]
public class WeaponTypeSO : PlacedObjectTypeSO // Оружие - объект типа SO (наследует класс Размещенного объекта)
{
    public enum TypeWeapon
    {
        GrappleGun,
        Pistol1,
        Pistol2Machine,
        Pistol3Machine,
        Pistol4Laser,
        Revolver,
        Rifle1Small,
        Rifle2Small,
        Rifle3Base,
        Rifle4Laser,
        Rifle5PlasmaSmall,
        Rifle6PlasmaSmall,
        Rifle7PlasmaBase,
        Shotgun1,
        Shotgun2Plasma,
        Sniper1,
        Sniper2Plasma,
        Sword1,
        Sword2Double,
        Sword3Stun
    }


    public TypeWeapon typeWeapon;
    public int damage; // Величина урона
    public int numberShotInOneAction; // Количество выстрелов за одно действие
    public float delayShot; //задержка между выстрелами
    
    public override string GetToolTip()
    {
        return""+ nameString;
    }

  


}

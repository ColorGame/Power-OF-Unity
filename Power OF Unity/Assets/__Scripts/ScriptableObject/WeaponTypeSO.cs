using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/WeaponType")]
public class WeaponTypeSO : PlacedObjectTypeSO // ������ - ������ ���� SO (��������� ����� ������������ �������)
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
    public int damage; // �������� �����
    public int numberShotInOneAction; // ���������� ��������� �� ���� ��������
    public float delayShot; //�������� ����� ����������
    
    public override string GetToolTip()
    {
        return""+ nameString;
    }

  


}

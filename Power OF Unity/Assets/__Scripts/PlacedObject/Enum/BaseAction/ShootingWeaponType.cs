
/// <summary>
/// ���� ����������� ������.<br/>
/// ���������� 3-������ � 3-��������� 1=(1A) 2=(1B) 3=(2A) 4=(2B) 5=(3A) 6=(3B).<br/>
/// �������� ������ 1-Base 2-Laser 3-Plasma.<br/>
/// Weapon_(���������� �����)_(�����)_(���������� ����� ���������)_���_�����������<br/>
/// Weapon_1_1A_Pistol_Base = 1 - ������������� �������� �������� �� �������� � ����� ������ WarehouseManager
/// </summary>
public enum ShootingWeaponType
{
    None = 0,                           //���*���
    //Pistol (���������� �����=1)       //2*2       ��� ����
    Weapon_1_1A_Pistol_Base = 1,             
    Weapon_1_1B_Pistol_BaseMod = 2,      

    Weapon_1_2A_Pistol_Laser = 3,            
    Weapon_1_2B_Pistol_LaserMod = 4,
    //Revolver (���������� �����=2)     //3*2       ��� ���� ���� ���������� � ��������� �� ��� ���������� (������ ��� ������)
    Weapon_2_1A_Revolver_Base = 5,
    Weapon_2_2A_Revolver_Laser = 6,
    //SMG (���������� �����=3)          //3*2       ������ ��� ��� ��� ������. �� ���� ����������� �����������
    Weapon_3_1A_SMG_Base = 7,
    Weapon_3_1B_SMG_BaseMod = 8,

    Weapon_3_2A_SMG_Laser = 9,
    Weapon_3_2B_SMG_LaserMod = 10,

    Weapon_3_3A_SMG_Plasma = 11,
   
    //Rifle (���������� �����=5)        //4*2       ��� ����
    Weapon_4_1A_1_Rifle_BaseSmall = 12,         
    Weapon_4_1A_2_Rifle_Base = 13,
    Weapon_4_1B_Rifle_BaseMod = 14,

    Weapon_4_2A_1_Rifle_LaserSmall = 15,
    Weapon_4_2A_2_Rifle_Laser = 16,
    Weapon_4_2B_Rifle_LaserMod = 17,

    Weapon_4_3A_1_Rifle_PlasmaSmall = 18,    
    Weapon_4_3A_2_Rifle_Plasma = 19,
    Weapon_4_3B_Rifle_PlasmaMod = 20,     

    //Shotgun (���������� �����=4)      //4*2       ��� ����
    Weapon_5_1A_Shotgun_Base = 21,
    Weapon_5_2A_Shotgun_Laser = 22,

    Weapon_5_3A_Shotgun_Plasma = 23,
    Weapon_5_3B_Shotgun_PlasmaMod = 24,

    //Sniper (���������� �����=6)       //5*2       ��� ����
    Weapon_6_1A_Sniper_Base = 25,
    Weapon_6_1B_Sniper_BaseMod = 26,

    Weapon_6_2A_Sniper_Laser = 27,
    Weapon_6_2B_Sniper_LaserMod = 28,

    Weapon_6_3A_Sniper_Plasma = 29,
    Weapon_6_3B_Sniper_PlasmaMod = 30,

}

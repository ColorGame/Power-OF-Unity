
/// <summary>
/// ��� ����������� ��������
/// </summary>
public enum PlacedObjectType
{    
    _______WEAPON_______ = 0,
    NoneWeapon = 1,                 //��� ��������
    GrapplingHookGun = 2,           //�������� � ���������� ������
    SyringeGun = 3,                 //�����-��������

    GrenadeFrag = 4,
    GrenadeSmoke = 5,
    GrenadeStun = 6,
    GrenadePlasma = 7,

    Pistol1Base = 8,                //��������
    Pistol2Machine = 9,             //��������-�������
    Pistol3MachineWithButt = 10,    //��������-������� � ���������. ������ ��������� � ������ ������ ��� ������ ���������
    Pistol4Laser = 11,              //�������� ��������
    RevolverLaser = 12,             //�������� ���������. ������� ��������� ������� - ������ ��������� ���������. ������ ���� � ���������
   
    Rifle1Small = 13,               //�������� ���������
    Rifle2Base = 14,                //��������
    Rifle3LaserSmall = 15,          //�������� �������� ���������
    Rifle4LaserBase = 16,           //�������� ��������    
    
    Shotgun = 17,                   //��������
    SniperRifle = 18,               //����������� ��������

    LaserShotgun = 19,              //�������� ��������
    LaserSniperRifle = 20,          //�������� ����������� ��������

    PlasmaRifleSmall = 21,          //��������� ���������� ��������
    PlasmaRifle = 22,               //���������� ��������
    PlasmaShotgun = 23,             //���������� ��������
    PlasmaSniperRifle = 24,         //���������� ����������� ��������

    Sword = 25,
    SwordDouble = 26,
    SwordStun = 27,

    _______ITEM_______ = 28,

    Binoculars = 29,                //�������
    Medipack = 30,                  //����������� �����
    CombatShield = 31,              //������ ��������� ��� Combat Assault Shield

    NightVisionCamera = 32,         //������ ������� �������    

    _______ARMOR_______ = 33,

    HeadArmorMilitary = 34,         //���������� ������� ����
    HeadArmorJunker = 35,           //��������� ������� ����
    HeadArmorSpaceNoFace = 36,      //����������� ���� ����������� ��� ����
    HeadArmorBigCoverClear = 37,    //���������� ������ ��� �������� �������
    HeadArmorBigCover = 38,         //������ ��� �������� �������

    BodyArmorMilitary = 39,         //������� ����������
    BodyArmorMilitaryMod = 40,      //���������� ������� ���������� 
    BodyArmorSpace = 41,            //����������� ����������
    BodyArmorSpaceMod = 42,         //����������� ��� ������������ ���������� (����������)
    BodyArmorBigExoskeleton = 43,   //���������� ������� ����������

}




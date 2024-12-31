
/// <summary>
/// Типы стреляющего оружия
/// </summary>
public enum ShootingWeaponType
{
    None = 0,                       //шир*выс
    //Pistol                        //2*2       Доп оруж
    Pistol_1A_Base = 1,             
    Pistol_1B_BaseMod = 2,          
    Pistol_1C_BaseMachine = 3,      

    Pistol_2A_Laser = 4,            
    Pistol_2B_LaserMod = 5,
    //Revolver                      //3*2       Доп оруж Урон сопостовим с автоматом ну или дробовиком (мощное доп оружие)
    Revolver_1A_Laser = 6,          
    Revolver_1B_LaserMod = 7,          
    //Rifle                         //4*2       Осн оруж
    Rifle_1A_BaseSmall = 8,         
    Rifle_1B_Base = 9,

    Rifle_2A_LaserSmall = 10,
    Rifle_2B_LaserBase = 11,
    Rifle_2C_LaserBaseMode = 12,

    Rifle_3A_PlasmaSmall = 13,
    Rifle_3B_PlasmaSmallMod = 14,
    Rifle_3C_PlasmaBase = 15,
    Rifle_3D_PlasmaBaseMod = 16,
    //Shotgun                       //4*2       Осн оруж
    Shotgun_1A_Base = 17,
    Shotgun_2A_Laser = 18,

    Shotgun_3A_Plasma = 19,
    Shotgun_3B_PlasmaMod = 20,
    //SMG                           //3*2       Нельзя исп как доп оружие. Но дает модификацию проворности
    SMG_1A_Base = 21,
    SMG_1B_BaseMod = 22,

    SMG_2A_Laser = 23,
    SMG_2B_LaserMod = 24,

    SMG_3A_Plasma = 25,
    //Sniper                        //5*2       Осн оруж
    Sniper_1A_Base = 26,

    Sniper_2A_Laser = 27,
    Sniper_2B_LaserMod = 28,

    Sniper_3A_Plasma = 29,
    Sniper_3B_PlasmaMod = 30,

}


/// <summary>
/// Тип размещяемых объектов
/// </summary>
public enum PlacedObjectType
{    
    _______WEAPON_______ = 0,
    NoneWeapon = 1,                 //Нет предмета
    GrapplingHookGun = 2,           //пистолет с абордажным крюком
    SyringeGun = 3,                 //Шприц-пистолет

    GrenadeFrag = 4,
    GrenadeSmoke = 5,
    GrenadeStun = 6,
    GrenadePlasma = 7,

    Pistol1Base = 8,                //Пистолет
    Pistol2Machine = 9,             //Пистолет-пулемет
    Pistol3MachineWithButt = 10,    //Пистолет-пулемет с прикладом. Больше дальность и меньше тряски при ручном наведении
    Pistol4Laser = 11,              //Лазерный пистолет
    RevolverLaser = 12,             //Лазерный револьвер. Мощьный одиночный выстрел - замена лазерному пистолету. Болеше урон и дальность
   
    Rifle1Small = 13,               //Винтовка Маленькая
    Rifle2Base = 14,                //Винтовка
    Rifle3LaserSmall = 15,          //Лазерная винтовка маленькая
    Rifle4LaserBase = 16,           //Лазерная винтовка    
    
    Shotgun = 17,                   //Дробовик
    SniperRifle = 18,               //Снайперская винтовка

    LaserShotgun = 19,              //Лазерный дробовик
    LaserSniperRifle = 20,          //Лазерная снайперская винтовка

    PlasmaRifleSmall = 21,          //Маленькая плазменная винтовка
    PlasmaRifle = 22,               //Плазменная винтовка
    PlasmaShotgun = 23,             //Плазменный дробовик
    PlasmaSniperRifle = 24,         //Плазменная снайперская винтовка

    Sword = 25,
    SwordDouble = 26,
    SwordStun = 27,

    _______ITEM_______ = 28,

    Binoculars = 29,                //Бинокль
    Medipack = 30,                  //Медицинский пакет
    CombatShield = 31,              //Боевой Штурмовой щит Combat Assault Shield

    NightVisionCamera = 32,         //Камера ночного видения    

    _______ARMOR_______ = 33,

    HeadArmorMilitary = 34,         //Стандартый военный шлем
    HeadArmorJunker = 35,           //Улучшеный военный шлем
    HeadArmorSpaceNoFace = 36,      //Космический шлем закрывающий все лицо
    HeadArmorBigCoverClear = 37,    //Прозрачная крышка для большого солдата
    HeadArmorBigCover = 38,         //Крышка для большого солдата

    BodyArmorMilitary = 39,         //Военный бронежилет
    BodyArmorMilitaryMod = 40,      //Улучшенный военный бронежилет 
    BodyArmorSpace = 41,            //Космический бронежилет
    BodyArmorSpaceMod = 42,         //Модификации для космического бронежилет (дополнение)
    BodyArmorBigExoskeleton = 43,   //Бронежилет Большой экзоскелет

}




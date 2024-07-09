
/// <summary>
/// Тип размещяемых объектов
/// </summary>
public enum PlacedObjectType
{
    None,                   //Нет предмета
    GrapplingHookGun,       //пистолет с абордажным крюком
    SyringeGun,             //Шприц-пистолет

    GrenadeFrag,
    GrenadeSmoke,
    GrenadeStun,
    GrenadePlasma,

    Pistol1Base,             //Пистолет
    Pistol2Machine,          //Пистолет-пулемет
    Pistol3MachineWithButt,  //Пистолет-пулемет с прикладом. Больше дальность и меньше тряски при ручном наведении
    Pistol4Laser,            //Лазерный пистолет
    RevolverLaser,           //Лазерный револьвер. Мощьный одиночный выстрел - замена лазерному пистолету. Болеше урон и дальность
   
    Rifle1Small,             //Винтовка Маленькая
    Rifle2Base,              //Винтовка
    Rifle3LaserSmall,        //Лазерная винтовка маленькая
    Rifle4LaserBase,         //Лазерная винтовка
    
    
    Shotgun,                //Дробовик
    SniperRifle,            //Снайперская винтовка

    LaserShotgun,           //Лазерный дробовик
    LaserSniperRifle,       //Лазерная снайперская винтовка

    PlasmaRifleSmall,       //Маленькая плазменная винтовка
    PlasmaRifle,            //Плазменная винтовка
    PlasmaShotgun,          //Плазменный дробовик
    PlasmaSniperRifle,      //Плазменная снайперская винтовка

    Sword,
    SwordDouble,
    SwordStun,


    

    Binoculars,             // Бинокль
    Medipack,               //Медицинский пакет
    CombatShield,           //Боевой Штурмовой щит Combat Assault Shield

    NightVisionCamera,      //Камера ночного видения
}




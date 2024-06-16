
/// <summary>
/// Тип размещяемых объектов
/// </summary>
public enum PlacedObjectType
{
    None,                   //Нет предмета
    GrapplingHookGun,       //пистолет с абордажным крюком
    SyringeGun,             //Шприц-пистолет

    Pistol,                 //Пистолет
    Shotgun,                //Дробовик
    MachinePistol,          //Пистолет-пулемет
    MachinePistolWithButt,  //Пистолет-пулемет с прикладом. Больше дальность и меньше тряски при ручном наведении
    RifleSmall,             //Винтовка Маленькая
    Rifle,                  //Винтовка
    SniperRifle,            //Снайперская винтовка

    LaserPistol,            //Лазерный пистолет
    LaserRevolver,          //Лазерный револьвер. Мощьный одиночный выстрел - замена лазерному пистолету. Болеше урон и дальность
    LaserShotgun,           //Лазерный дробовик
    LaserRifleSmall,        //Лазерная винтовка маленькая
    LaserRifle,             //Лазерная винтовка
    LaserSniperRifle,       //Лазерная снайперская винтовка

    PlasmaRifleSmall,       //Маленькая плазменная винтовка
    PlasmaRifle,            //Плазменная винтовка
    PlasmaShotgun,          //Плазменный дробовик
    PlasmaSniperRifle,      //Плазменная снайперская винтовка

    Sword,
    SwordDouble,
    SwordStun,


    FragGrenade,
    SmokeGrenade,
    ElectroshockGrenade,
    PlasmaGrenade,

    Binoculars,             // Бинокль
    Medipack,               //Медицинский пакет
    CombatShield,           //Боевой Штурмовой щит Combat Assault Shield

    NightVisionCamera,      //Камера ночного видения
}




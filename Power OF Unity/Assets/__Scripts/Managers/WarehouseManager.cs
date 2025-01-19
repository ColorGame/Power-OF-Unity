using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

/// <summary>
/// Управление ресурсами (монеты, оружие броня...)
/// </summary>
/// <remarks>
/// Предметы экипировки на юните не учитываются.
/// </remarks>
public class WarehouseManager
{
    public WarehouseManager(TooltipUI tooltipUI)
    {
        Init(tooltipUI);
    }

    public event EventHandler<PlacedObjectTypeAndCount> OnChangCountPlacedObject; // Изменено количество размещенных объектов
    public event EventHandler<uint> OnChangCoinCount; // Изменено количество монет

    private uint _coin;
    private Dictionary<PlacedObjectTypeSO, uint> _allPlacedObjectCountDictionary = null;

    private TooltipUI _tooltipUI;

    private void Init(TooltipUI tooltipUI)
    {
        _tooltipUI = tooltipUI;
        InitPlacedObjectList(firstStart: true);
    }

    private void InitPlacedObjectList(bool firstStart)
    {
        if (firstStart)
        {
            ResourcesBasicListSO resourcesBasicListSO = Resources.Load<ResourcesBasicListSO>(typeof(ResourcesBasicListSO).Name); // Загружает ресурс запрошенного типа, хранящийся по адресу path(путь) в папке Resources(эту папку я создал в папке ScriptableObjects).
                                                                                                                                 // Что бы не ошибиться в имени пойдем другим путем. Создадим экземпляр BuildingTypeListSO (список будет один) и назавем также как и класс, потом для поиска SO будем извлекать имя класса которое совпадает с именем экземпляра
            _coin = resourcesBasicListSO.GetCoin();           
            _allPlacedObjectCountDictionary = GetSortedDictionaryByKeys(resourcesBasicListSO.GetAllPlacedObjectCountDictionary());
        }
        else
        {
            // Реализовать загрузку
        }
    }

    /// <summary>
    /// Добавить монет
    /// </summary>
    public void PlusCoin(uint coin)
    {
        _coin += coin;
        OnChangCoinCount?.Invoke(this, _coin);
    }
    /// <summary>
    /// Попробую убавить(потратить) монеты
    /// </summary>
    public bool TryMinusCoins(uint coin)
    {
        if (_coin >= coin)
        {
            _coin -= coin;
            OnChangCoinCount?.Invoke(this, _coin);
            return true;
        }
        else { return false; }
    }
    /// <summary>
    /// Попробуем купить 
    /// </summary>
    /// <remarks>
    /// В аргумент передать сумму покупки и словарь объектов покупки (Keys-Объект, Values-Количесвто)
    /// </remarks>
    public bool TryBuy(uint sumBuy, Dictionary<PlacedObjectTypeSO, uint> buyObjectCountDictionary)
    {
        if (TryMinusCoins(sumBuy))
        {
            foreach (PlacedObjectTypeSO placedObjectTypeSO in buyObjectCountDictionary.Keys)
            {
                PlusCountPlacedObject(placedObjectTypeSO, buyObjectCountDictionary[placedObjectTypeSO]);
            }
            return true;
        }
        else { return false; }
    }
    /// <summary>
    /// Продать (проверка, на достаточность объектов для продажи, ДОЛЖНА происходит на стороне запросившего класса)
    /// </summary>
    /// /// <remarks>
    /// В аргумент передать сумму продажи и словарь объектов для продажи (Keys-Объект, Values-Количесвто)
    /// </remarks>
    public void Sell(uint sumSell, Dictionary<PlacedObjectTypeSO, uint> sellObjectCountDictionary)
    {
        PlusCoin(sumSell);
        foreach (PlacedObjectTypeSO placedObjectTypeSO in sellObjectCountDictionary.Keys)
        {
            MinusCountPlacedObjectType(placedObjectTypeSO, sellObjectCountDictionary[placedObjectTypeSO]);
        }
    }

    /// <summary>
    /// Добавить количество переданного PlacedObjectTypeSO (в аргумент можно передать число>=0)
    /// </summary>
    /// <remarks>
    /// Если этого типа предмета нету в словаре, то добавим его (при открытии нового предмета).
    /// </remarks>
    public void PlusCountPlacedObject(PlacedObjectTypeSO placedObjectTypeSO, uint number = 1)
    {
        if (!_allPlacedObjectCountDictionary.ContainsKey(placedObjectTypeSO)) // Если этого типа нет в списке ключей (когда открываем новый предмет)
        {
            _allPlacedObjectCountDictionary.Add(placedObjectTypeSO, number); // Добавим в словарь новый тип placedObject //_allPlacedObjectCountDictionary[placedObjectTypeSO] = number; // Альтернативная запись

            //При добавлении нового предмета надо отсортировать словарь 
            _allPlacedObjectCountDictionary = GetSortedDictionaryByKeys(_allPlacedObjectCountDictionary);
        }
        else
        {
            _allPlacedObjectCountDictionary[placedObjectTypeSO] += number; // Добавим к текущему количеству
        }
        OnChangCountPlacedObject?.Invoke(this, new PlacedObjectTypeAndCount(placedObjectTypeSO, _allPlacedObjectCountDictionary[placedObjectTypeSO]));
    }

    /// <summary>
    /// Попробую уменьшить ,на переданное количество, PlacedObjectTypeSO 
    /// </summary>
    /// <remarks>(в аргумент можно передать число>=0)</remarks>
    public bool TryMinusCountPlacedObjectType(PlacedObjectTypeSO placedObjectTypeSO, uint number = 1)
    {
        if (!_allPlacedObjectCountDictionary.ContainsKey(placedObjectTypeSO)) // Если этого типа нет в словаре
        {
            return false;
        }

        if (number > _allPlacedObjectCountDictionary[placedObjectTypeSO]) // Если отнимае больше чем у нас есть то 
        {
            return false;
        }

        MinusCountPlacedObjectType(placedObjectTypeSO, number);
        return true;
    }

    private void MinusCountPlacedObjectType(PlacedObjectTypeSO placedObjectTypeSO, uint number)
    {
        _allPlacedObjectCountDictionary[placedObjectTypeSO] -= number;
        OnChangCountPlacedObject?.Invoke(this, new PlacedObjectTypeAndCount(placedObjectTypeSO, _allPlacedObjectCountDictionary[placedObjectTypeSO]));
    }
    /// <summary>
    /// Получить отсортированный словарь по ключам.<br/>
    /// Здесь можем отслеживать иерархию сортировки по типам<br/>
    /// Метод жестко закодирован, при появление нового типа PlacedObjectTypeSO надо дописывать вручную
    /// </summary>
   /* private Dictionary<PlacedObjectTypeSO, uint> GetSortedDictionaryByKeys(Dictionary<PlacedObjectTypeSO, uint> dic)
    {
        Dictionary<PlacedObjectTypeSO, uint> sortedDic = new();

        //Создадим списки всех типов
        List<BodyArmorTypeSO> bodyArmorTypeSOList = new();
        List<HeadArmorTypeSO> headArmorTypeSOList = new();
        List<CombatDroneTypeSO> combatDroneTypeSOList = new();
        List<GrappleTypeSO> grappleTypeSOList = new();
        List<GrenadeTypeSO> grenadeTypeSOList = new();
        List<HealItemTypeSO> healItemTypeSOList = new();
        List<ShieldItemTypeSO> shieldItemTypeSOList = new();
        List<ShootingWeaponTypeSO> shootingWeaponTypeSOList = new();
        List<SpotterFireItemTypeSO> spotterFireItemTypeSOList = new();
        List<SwordTypeSO> swordTypeSOList = new();


        //переберем ключи в словаре  и отсортируем по типам
        foreach (PlacedObjectTypeSO placedObjectTypeSO in dic.Keys)
        {
            switch (placedObjectTypeSO)
            {
                case BodyArmorTypeSO bodyArmorTypeSO:
                    bodyArmorTypeSOList.Add(bodyArmorTypeSO); break;
                case HeadArmorTypeSO headArmorTypeSO:
                    headArmorTypeSOList.Add(headArmorTypeSO); break;
                case CombatDroneTypeSO combatDroneTypeSO:
                    combatDroneTypeSOList.Add(combatDroneTypeSO); break;
                case GrappleTypeSO grappleTypeSO:
                    grappleTypeSOList.Add(grappleTypeSO); break;
                case GrenadeTypeSO grenadeTypeSO:
                    grenadeTypeSOList.Add(grenadeTypeSO); break;
                case HealItemTypeSO healItemTypeSO:
                    healItemTypeSOList.Add(healItemTypeSO); break;
                case ShieldItemTypeSO shieldItemTypeSO:
                    shieldItemTypeSOList.Add(shieldItemTypeSO); break;
                case ShootingWeaponTypeSO shootingWeaponTypeSO:
                    shootingWeaponTypeSOList.Add(shootingWeaponTypeSO); break;
                case SpotterFireItemTypeSO spotterFireItemTypeSO:
                    spotterFireItemTypeSOList.Add(spotterFireItemTypeSO); break;
                case SwordTypeSO swordTypeSO:
                    swordTypeSOList.Add(swordTypeSO); break;
            }
        }

        // Отсортируем по имени внутри каждого списка
        bodyArmorTypeSOList.Sort(SortName);
        headArmorTypeSOList.Sort(SortName);
        combatDroneTypeSOList.Sort(SortName);
        grappleTypeSOList.Sort(SortName);
        grenadeTypeSOList.Sort(SortName);
        healItemTypeSOList.Sort(SortName);
        shieldItemTypeSOList.Sort(SortName);
        shootingWeaponTypeSOList.Sort(SortName);
        spotterFireItemTypeSOList.Sort(SortName);
        swordTypeSOList.Sort(SortName);

        // Создадим общий список где можем отслеживать иерархию сортировки по типам
        List<PlacedObjectTypeSO> placedObjectTypeSOList = new();
        placedObjectTypeSOList.AddRange(grappleTypeSOList);             //1
        placedObjectTypeSOList.AddRange(shootingWeaponTypeSOList);      //2
        placedObjectTypeSOList.AddRange(swordTypeSOList);               //3
        placedObjectTypeSOList.AddRange(grenadeTypeSOList);             //4
        placedObjectTypeSOList.AddRange(healItemTypeSOList);            //5
        placedObjectTypeSOList.AddRange(shieldItemTypeSOList);          //6
        placedObjectTypeSOList.AddRange(spotterFireItemTypeSOList);     //7
        placedObjectTypeSOList.AddRange(combatDroneTypeSOList);         //8
        placedObjectTypeSOList.AddRange(headArmorTypeSOList);           //9
        placedObjectTypeSOList.AddRange(bodyArmorTypeSOList);           //10

        //Заполним итоговый СЛОВАРЬ
        foreach (PlacedObjectTypeSO placedObjectTypeSO in placedObjectTypeSOList)
        {
            sortedDic[placedObjectTypeSO] = dic[placedObjectTypeSO];
        }
        return sortedDic;
    }*/
    private Dictionary<PlacedObjectTypeSO, uint> GetSortedDictionaryByKeys(Dictionary<PlacedObjectTypeSO, uint> dic) // Более сложный в понимании но производительность увеличена в4 раза
    {
        Dictionary<PlacedObjectTypeSO, uint> sortedDic = new();

        // Количество предметов определенного типа (Уберем дефолтное состояние None = 0)
        int countGrappleTypeSO = Enum.GetNames(typeof(GrappleType)).Length - 1;
        int countShootingWeaponTypeSO = Enum.GetNames(typeof(ShootingWeaponType)).Length - 1;
        int countSwordTypeSO = Enum.GetNames(typeof(SwordType)).Length - 1;
        int countGrenadeTypeSO = Enum.GetNames(typeof(GrenadeType)).Length - 1;
        int countHealItemTypeSO = Enum.GetNames(typeof(HealItemType)).Length - 1;
        int countShieldItemTypeSO = Enum.GetNames(typeof(ShieldItemType)).Length - 1;
        int countSpotterFireItemTypeSO = Enum.GetNames(typeof(SpotterFireItemType)).Length - 1;
        int countCombatDroneTypeSO = Enum.GetNames(typeof(CombatDroneType)).Length - 1;
        int countHeadArmorTypeSO = Enum.GetNames(typeof(HeadArmorType)).Length - 1;
        int countBodyArmorTypeSO = Enum.GetNames(typeof(BodyArmorType)).Length - 1; // Уберем дефолтное состояние = 0

        // Инициализируем массив всех PlacedObjectType
        PlacedObjectTypeSO[] placedObjectTypeSOArray = new PlacedObjectTypeSO[(
            countGrappleTypeSO +
            countShootingWeaponTypeSO +
            countSwordTypeSO +
            countGrenadeTypeSO +
            countHealItemTypeSO +
            countShieldItemTypeSO +
            countSpotterFireItemTypeSO +
            countCombatDroneTypeSO +
            countHeadArmorTypeSO +
            countBodyArmorTypeSO
            )];
        //переберем ключи в словаре  и отсортируем по типам
        int numberInEnum = 0;// Порядковый номер внутри ENUM (не индекс)
        int previousCount = 0; // Количество предыдущих элементов
        foreach (PlacedObjectTypeSO placedObjectTypeSO in dic.Keys)
        {
            switch (placedObjectTypeSO)
            {
                case GrappleTypeSO grappleTypeSO:                           //1 Индекс от 0 до countGrappleTypeSO(не включая)
                    numberInEnum = (int)grappleTypeSO.GetGrappleType();
                    previousCount = 0; 
                    break;

                case ShootingWeaponTypeSO shootingWeaponTypeSO:             //2 Индекс от countGrappleTypeSO до countShootingWeaponTypeSO(не включая)
                    numberInEnum = (int)shootingWeaponTypeSO.GetShootingWeaponType();
                    previousCount = countGrappleTypeSO;
                    break;

                case SwordTypeSO swordTypeSO:                               //3 Индекс от countShootingWeaponTypeSO до countSwordTypeSO(не включая)
                    numberInEnum = (int)swordTypeSO.GetSwordType();
                    previousCount =
                        countGrappleTypeSO +
                        countShootingWeaponTypeSO;
                    break;

                case GrenadeTypeSO grenadeTypeSO:                           //4 Индекс от countSwordTypeSO до countGrenadeTypeSO(не включая)
                    numberInEnum = (int)grenadeTypeSO.GetGrenadeType();
                    previousCount =
                        countGrappleTypeSO +
                        countShootingWeaponTypeSO +
                        countSwordTypeSO;
                    break;

                case HealItemTypeSO healItemTypeSO:                         //5 Индекс от countGrenadeTypeSO до countHealItemTypeSO(не включая)
                    numberInEnum = (int)healItemTypeSO.GetHealItemType();
                    previousCount =
                       countGrappleTypeSO +
                        countShootingWeaponTypeSO +
                        countSwordTypeSO +
                        countGrenadeTypeSO;
                    break;

                case ShieldItemTypeSO shieldItemTypeSO:                     //6 Индекс от countHealItemTypeSO до countShieldItemTypeSO(не включая)
                    numberInEnum = (int)shieldItemTypeSO.GetShieldItemType();
                    previousCount =
                        countGrappleTypeSO +
                        countShootingWeaponTypeSO +
                        countSwordTypeSO +
                        countGrenadeTypeSO +
                        countHealItemTypeSO;
                    break;
                case SpotterFireItemTypeSO spotterFireItemTypeSO:            //7 Индекс от countShieldItemTypeSO до countSpotterFireItemTypeSO(не включая)
                    numberInEnum = (int)spotterFireItemTypeSO.GetSpotterFireItemType();
                    previousCount =
                        countGrappleTypeSO +
                        countShootingWeaponTypeSO +
                        countSwordTypeSO +
                        countGrenadeTypeSO +
                        countHealItemTypeSO +
                        countShieldItemTypeSO;
                    break;
                case CombatDroneTypeSO combatDroneTypeSO:                   //8 Индекс от countSpotterFireItemTypeSO до countCombatDroneTypeSO(не включая)
                    numberInEnum = (int)combatDroneTypeSO.GetCombatDroneType();
                    previousCount =
                        countGrappleTypeSO +
                        countShootingWeaponTypeSO +
                        countSwordTypeSO +
                        countGrenadeTypeSO +
                        countHealItemTypeSO +
                        countShieldItemTypeSO +
                        countSpotterFireItemTypeSO;
                    break;

                case HeadArmorTypeSO headArmorTypeSO:                       //9 Индекс от countCombatDroneTypeSO до countHeadArmorTypeSO(не включая)
                    numberInEnum = (int)headArmorTypeSO.GetHeadArmorType();
                    previousCount =
                         countGrappleTypeSO +
                         countShootingWeaponTypeSO +
                         countSwordTypeSO +
                         countGrenadeTypeSO +
                         countHealItemTypeSO +
                         countShieldItemTypeSO +
                         countSpotterFireItemTypeSO +
                         countCombatDroneTypeSO;
                    break;

                case BodyArmorTypeSO bodyArmorTypeSO:                       //10 Индекс от countHeadArmorTypeSO до countBodyArmorTypeSO(не включая)
                    numberInEnum = (int)bodyArmorTypeSO.GetBodyArmorType();
                    previousCount =
                        countGrappleTypeSO +
                        countShootingWeaponTypeSO +
                        countSwordTypeSO +
                        countGrenadeTypeSO +
                        countHealItemTypeSO +
                        countShieldItemTypeSO +
                        countSpotterFireItemTypeSO +
                        countCombatDroneTypeSO +
                        countHeadArmorTypeSO;
                    break;
            }
            placedObjectTypeSOArray[previousCount + numberInEnum - 1] = placedObjectTypeSO; // отнимем 1 чтобы получить индекс
        }

        //Заполним итоговый СЛОВАРЬ
        foreach (PlacedObjectTypeSO placedObjectTypeSO in placedObjectTypeSOArray)
        {
            sortedDic[placedObjectTypeSO] = dic[placedObjectTypeSO];
        }
        return sortedDic;
    }


    private int SortName(PlacedObjectTypeSO x1, PlacedObjectTypeSO x2)
    {
        return x1.name.CompareTo(x2.name);
    }

    public uint GetCountCoin() { return _coin; }
    /// <summary>
    /// Получить список всех PlacedObjectTypeSO хранящихся на складе.<br/>
    /// Возвращается ОТСОРТИРОВАННЫЙ по имени список
    /// </summary>
    public IEnumerable<PlacedObjectTypeSO> GetAllPlacedObjectTypeSOList() { return _allPlacedObjectCountDictionary.Keys; }
    public uint GetCountPlacedObject(PlacedObjectTypeSO placedObjectTypeSO) { return _allPlacedObjectCountDictionary[placedObjectTypeSO]; } // Проверять не надо т.к. по ирархии вызовов этот метод вызывается в цикле перебора имеющихся ключей
}

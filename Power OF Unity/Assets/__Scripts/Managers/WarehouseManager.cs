using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

/// <summary>
/// ���������� ��������� (������, ������ �����...)
/// </summary>
/// <remarks>
/// �������� ���������� �� ����� �� �����������.
/// </remarks>
public class WarehouseManager
{
    public WarehouseManager(TooltipUI tooltipUI)
    {
        Init(tooltipUI);
    }

    public event EventHandler<PlacedObjectTypeAndCount> OnChangCountPlacedObject; // �������� ���������� ����������� ��������
    public event EventHandler<uint> OnChangCoinCount; // �������� ���������� �����

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
            ResourcesBasicListSO resourcesBasicListSO = Resources.Load<ResourcesBasicListSO>(typeof(ResourcesBasicListSO).Name); // ��������� ������ ������������ ����, ���������� �� ������ path(����) � ����� Resources(��� ����� � ������ � ����� ScriptableObjects).
                                                                                                                                 // ��� �� �� ��������� � ����� ������ ������ �����. �������� ��������� BuildingTypeListSO (������ ����� ����) � ������� ����� ��� � �����, ����� ��� ������ SO ����� ��������� ��� ������ ������� ��������� � ������ ����������
            _coin = resourcesBasicListSO.GetCoin();           
            _allPlacedObjectCountDictionary = GetSortedDictionaryByKeys(resourcesBasicListSO.GetAllPlacedObjectCountDictionary());
        }
        else
        {
            // ����������� ��������
        }
    }

    /// <summary>
    /// �������� �����
    /// </summary>
    public void PlusCoin(uint coin)
    {
        _coin += coin;
        OnChangCoinCount?.Invoke(this, _coin);
    }
    /// <summary>
    /// �������� �������(���������) ������
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
    /// ��������� ������ 
    /// </summary>
    /// <remarks>
    /// � �������� �������� ����� ������� � ������� �������� ������� (Keys-������, Values-����������)
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
    /// ������� (��������, �� ������������� �������� ��� �������, ������ ���������� �� ������� ������������ ������)
    /// </summary>
    /// /// <remarks>
    /// � �������� �������� ����� ������� � ������� �������� ��� ������� (Keys-������, Values-����������)
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
    /// �������� ���������� ����������� PlacedObjectTypeSO (� �������� ����� �������� �����>=0)
    /// </summary>
    /// <remarks>
    /// ���� ����� ���� �������� ���� � �������, �� ������� ��� (��� �������� ������ ��������).
    /// </remarks>
    public void PlusCountPlacedObject(PlacedObjectTypeSO placedObjectTypeSO, uint number = 1)
    {
        if (!_allPlacedObjectCountDictionary.ContainsKey(placedObjectTypeSO)) // ���� ����� ���� ��� � ������ ������ (����� ��������� ����� �������)
        {
            _allPlacedObjectCountDictionary.Add(placedObjectTypeSO, number); // ������� � ������� ����� ��� placedObject //_allPlacedObjectCountDictionary[placedObjectTypeSO] = number; // �������������� ������

            //��� ���������� ������ �������� ���� ������������� ������� 
            _allPlacedObjectCountDictionary = GetSortedDictionaryByKeys(_allPlacedObjectCountDictionary);
        }
        else
        {
            _allPlacedObjectCountDictionary[placedObjectTypeSO] += number; // ������� � �������� ����������
        }
        OnChangCountPlacedObject?.Invoke(this, new PlacedObjectTypeAndCount(placedObjectTypeSO, _allPlacedObjectCountDictionary[placedObjectTypeSO]));
    }

    /// <summary>
    /// �������� ��������� ,�� ���������� ����������, PlacedObjectTypeSO 
    /// </summary>
    /// <remarks>(� �������� ����� �������� �����>=0)</remarks>
    public bool TryMinusCountPlacedObjectType(PlacedObjectTypeSO placedObjectTypeSO, uint number = 1)
    {
        if (!_allPlacedObjectCountDictionary.ContainsKey(placedObjectTypeSO)) // ���� ����� ���� ��� � �������
        {
            return false;
        }

        if (number > _allPlacedObjectCountDictionary[placedObjectTypeSO]) // ���� ������� ������ ��� � ��� ���� �� 
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
    /// �������� ��������������� ������� �� ������.<br/>
    /// ����� ����� ����������� �������� ���������� �� �����<br/>
    /// ����� ������ �����������, ��� ��������� ������ ���� PlacedObjectTypeSO ���� ���������� �������
    /// </summary>
   /* private Dictionary<PlacedObjectTypeSO, uint> GetSortedDictionaryByKeys(Dictionary<PlacedObjectTypeSO, uint> dic)
    {
        Dictionary<PlacedObjectTypeSO, uint> sortedDic = new();

        //�������� ������ ���� �����
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


        //��������� ����� � �������  � ����������� �� �����
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

        // ����������� �� ����� ������ ������� ������
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

        // �������� ����� ������ ��� ����� ����������� �������� ���������� �� �����
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

        //�������� �������� �������
        foreach (PlacedObjectTypeSO placedObjectTypeSO in placedObjectTypeSOList)
        {
            sortedDic[placedObjectTypeSO] = dic[placedObjectTypeSO];
        }
        return sortedDic;
    }*/
    private Dictionary<PlacedObjectTypeSO, uint> GetSortedDictionaryByKeys(Dictionary<PlacedObjectTypeSO, uint> dic) // ����� ������� � ��������� �� ������������������ ��������� �4 ����
    {
        Dictionary<PlacedObjectTypeSO, uint> sortedDic = new();

        // ���������� ��������� ������������� ���� (������ ��������� ��������� None = 0)
        int countGrappleTypeSO = Enum.GetNames(typeof(GrappleType)).Length - 1;
        int countShootingWeaponTypeSO = Enum.GetNames(typeof(ShootingWeaponType)).Length - 1;
        int countSwordTypeSO = Enum.GetNames(typeof(SwordType)).Length - 1;
        int countGrenadeTypeSO = Enum.GetNames(typeof(GrenadeType)).Length - 1;
        int countHealItemTypeSO = Enum.GetNames(typeof(HealItemType)).Length - 1;
        int countShieldItemTypeSO = Enum.GetNames(typeof(ShieldItemType)).Length - 1;
        int countSpotterFireItemTypeSO = Enum.GetNames(typeof(SpotterFireItemType)).Length - 1;
        int countCombatDroneTypeSO = Enum.GetNames(typeof(CombatDroneType)).Length - 1;
        int countHeadArmorTypeSO = Enum.GetNames(typeof(HeadArmorType)).Length - 1;
        int countBodyArmorTypeSO = Enum.GetNames(typeof(BodyArmorType)).Length - 1; // ������ ��������� ��������� = 0

        // �������������� ������ ���� PlacedObjectType
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
        //��������� ����� � �������  � ����������� �� �����
        int numberInEnum = 0;// ���������� ����� ������ ENUM (�� ������)
        int previousCount = 0; // ���������� ���������� ���������
        foreach (PlacedObjectTypeSO placedObjectTypeSO in dic.Keys)
        {
            switch (placedObjectTypeSO)
            {
                case GrappleTypeSO grappleTypeSO:                           //1 ������ �� 0 �� countGrappleTypeSO(�� �������)
                    numberInEnum = (int)grappleTypeSO.GetGrappleType();
                    previousCount = 0; 
                    break;

                case ShootingWeaponTypeSO shootingWeaponTypeSO:             //2 ������ �� countGrappleTypeSO �� countShootingWeaponTypeSO(�� �������)
                    numberInEnum = (int)shootingWeaponTypeSO.GetShootingWeaponType();
                    previousCount = countGrappleTypeSO;
                    break;

                case SwordTypeSO swordTypeSO:                               //3 ������ �� countShootingWeaponTypeSO �� countSwordTypeSO(�� �������)
                    numberInEnum = (int)swordTypeSO.GetSwordType();
                    previousCount =
                        countGrappleTypeSO +
                        countShootingWeaponTypeSO;
                    break;

                case GrenadeTypeSO grenadeTypeSO:                           //4 ������ �� countSwordTypeSO �� countGrenadeTypeSO(�� �������)
                    numberInEnum = (int)grenadeTypeSO.GetGrenadeType();
                    previousCount =
                        countGrappleTypeSO +
                        countShootingWeaponTypeSO +
                        countSwordTypeSO;
                    break;

                case HealItemTypeSO healItemTypeSO:                         //5 ������ �� countGrenadeTypeSO �� countHealItemTypeSO(�� �������)
                    numberInEnum = (int)healItemTypeSO.GetHealItemType();
                    previousCount =
                       countGrappleTypeSO +
                        countShootingWeaponTypeSO +
                        countSwordTypeSO +
                        countGrenadeTypeSO;
                    break;

                case ShieldItemTypeSO shieldItemTypeSO:                     //6 ������ �� countHealItemTypeSO �� countShieldItemTypeSO(�� �������)
                    numberInEnum = (int)shieldItemTypeSO.GetShieldItemType();
                    previousCount =
                        countGrappleTypeSO +
                        countShootingWeaponTypeSO +
                        countSwordTypeSO +
                        countGrenadeTypeSO +
                        countHealItemTypeSO;
                    break;
                case SpotterFireItemTypeSO spotterFireItemTypeSO:            //7 ������ �� countShieldItemTypeSO �� countSpotterFireItemTypeSO(�� �������)
                    numberInEnum = (int)spotterFireItemTypeSO.GetSpotterFireItemType();
                    previousCount =
                        countGrappleTypeSO +
                        countShootingWeaponTypeSO +
                        countSwordTypeSO +
                        countGrenadeTypeSO +
                        countHealItemTypeSO +
                        countShieldItemTypeSO;
                    break;
                case CombatDroneTypeSO combatDroneTypeSO:                   //8 ������ �� countSpotterFireItemTypeSO �� countCombatDroneTypeSO(�� �������)
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

                case HeadArmorTypeSO headArmorTypeSO:                       //9 ������ �� countCombatDroneTypeSO �� countHeadArmorTypeSO(�� �������)
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

                case BodyArmorTypeSO bodyArmorTypeSO:                       //10 ������ �� countHeadArmorTypeSO �� countBodyArmorTypeSO(�� �������)
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
            placedObjectTypeSOArray[previousCount + numberInEnum - 1] = placedObjectTypeSO; // ������� 1 ����� �������� ������
        }

        //�������� �������� �������
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
    /// �������� ������ ���� PlacedObjectTypeSO ���������� �� ������.<br/>
    /// ������������ ��������������� �� ����� ������
    /// </summary>
    public IEnumerable<PlacedObjectTypeSO> GetAllPlacedObjectTypeSOList() { return _allPlacedObjectCountDictionary.Keys; }
    public uint GetCountPlacedObject(PlacedObjectTypeSO placedObjectTypeSO) { return _allPlacedObjectCountDictionary[placedObjectTypeSO]; } // ��������� �� ���� �.�. �� ������� ������� ���� ����� ���������� � ����� �������� ��������� ������
}

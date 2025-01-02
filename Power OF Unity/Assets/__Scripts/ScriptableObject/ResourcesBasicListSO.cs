using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Базовый список объектов размещения. БУДЕТ ВСЕГО ОДИН ЭКЗЕМПЛЯР (один список)
/// </summary>
[CreateAssetMenu(fileName = "ResourcesBasicListSO", menuName = "ScriptableObjects/Date/ResourcesBasicListSO")]
public class ResourcesBasicListSO : ScriptableObject
{
    [Header("Стартовое количество МОНЕТ")]
    [SerializeField] private uint _coin;
    [Header("Список предметов с ДЕЙСТВИЕМ(Action)")]
    [SerializeField] private List <PlacedObjectTypeAndCount> _placedObjectWithActionList; 
    [Header("Список предметов БРОНИ(Armor)")]
    [SerializeField] private List<PlacedObjectTypeAndCount> _placedObjectTypeArmorList; 

    private Dictionary<PlacedObjectTypeSO, uint> _allPlacedObjectCountDictionary = null;


   
    public uint GetCoin() {  return _coin; }

    public Dictionary<PlacedObjectTypeSO, uint> GetAllPlacedObjectCountDictionary()
    {
        if (_allPlacedObjectCountDictionary == null)
        {
            _allPlacedObjectCountDictionary = CompleteDictionary();
        }
        return _allPlacedObjectCountDictionary;
    }

    private Dictionary<PlacedObjectTypeSO, uint> CompleteDictionary()
    {
        Dictionary<PlacedObjectTypeSO, uint> allPlacedObjectCountDictionary = new Dictionary<PlacedObjectTypeSO, uint>();

        foreach (PlacedObjectTypeAndCount placedObjectTypeAndCount in _placedObjectWithActionList)
        {
            if(!allPlacedObjectCountDictionary.ContainsKey(placedObjectTypeAndCount.placedObjectTypeSO)) //Если этого типа нет в ключах словоря до добавим
            {
                allPlacedObjectCountDictionary[placedObjectTypeAndCount.placedObjectTypeSO] = placedObjectTypeAndCount.count;
            }
        }
        foreach (PlacedObjectTypeAndCount placedObjectTypeAndCount in _placedObjectTypeArmorList)
        {
            if (!allPlacedObjectCountDictionary.ContainsKey(placedObjectTypeAndCount.placedObjectTypeSO)) //Если этого типа нет в ключах словоря до добавим
            {
                allPlacedObjectCountDictionary[placedObjectTypeAndCount.placedObjectTypeSO] = placedObjectTypeAndCount.count;
            }
        }
        return allPlacedObjectCountDictionary;
    }

    public List<PlacedObjectTypeAndCount> GetPlacedObjectWithActionList()
    {
        // ИСКЛЮЧИМ ДУБЛИКАТЫ PlacedObjectTypeSO
        // Это сгруппирует список по свойству placedObjectTypeSO, затем выберите первую запись в группировке.
       
        return _placedObjectWithActionList.GroupBy(x=>x.placedObjectTypeSO).Select(x=>x.First()).ToList(); 
    }
    public List<PlacedObjectTypeAndCount> GetPlacedObjectArmorList()
    {
        return _placedObjectTypeArmorList.GroupBy(x => x.placedObjectTypeSO).Select(x => x.First()).ToList(); 
    }
}



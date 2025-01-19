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
    [SerializeField] private uint _coin =100000;
    [Header("Список предметов с ДЕЙСТВИЕМ(Action)")]
    [SerializeField] private List <PlacedObjectTypeAndCount> _placedObjectWithActionList= new(); 
    [Header("Список предметов БРОНИ(Armor)")]
    [SerializeField] private List<PlacedObjectTypeAndCount> _placedObjectTypeArmorList = new();
    [Header("Количество каждого предмета")]
    [SerializeField] private uint _count = 3;

    private Dictionary<PlacedObjectTypeSO, uint> _allPlacedObjectCountDictionary = null;

    [ContextMenu("АВТОЗАПОЛНЕНИЕ")]
    private void DownloadSheets() // Синхронизация данных
    {
        foreach(PlacedObjectTypeSO placedObjectType in PlacedObjectGeneralListForAutoCompletionSO.Instance.ArmorSOArray)
        {
            _placedObjectTypeArmorList.Add(new PlacedObjectTypeAndCount(placedObjectType, _count));
        }
        foreach (PlacedObjectTypeSO placedObjectType in PlacedObjectGeneralListForAutoCompletionSO.Instance.ActionSOArray)
        {
            _placedObjectWithActionList.Add(new PlacedObjectTypeAndCount(placedObjectType, _count));
        }


    }    

    public uint GetCoin() {  return _coin; }
    /// <summary>
    /// Получить словарь всех доступных PlacedObjectTypeSO и их количество<br/>
    /// Словарь НЕ ОТСОРТИРОВАН !!!
    /// </summary>
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
       // _placedObjectWithActionList.Sort((component1, component2) => component1.placedObjectTypeSO.name.CompareTo(component2.placedObjectTypeSO.name)); // Отсортируем по имени

        foreach (PlacedObjectTypeAndCount placedObjectTypeAndCount in _placedObjectWithActionList)
        {
            // Исключим повторение
            if(!allPlacedObjectCountDictionary.ContainsKey(placedObjectTypeAndCount.placedObjectTypeSO)) //Если этого типа нет в ключах словоря то добавим
            {
                allPlacedObjectCountDictionary[placedObjectTypeAndCount.placedObjectTypeSO] = placedObjectTypeAndCount.count;
            }
        }

       // _placedObjectTypeArmorList.Sort((component1, component2) => component1.placedObjectTypeSO.name.CompareTo(component2.placedObjectTypeSO.name));
        foreach (PlacedObjectTypeAndCount placedObjectTypeAndCount in _placedObjectTypeArmorList)
        {
            if (!allPlacedObjectCountDictionary.ContainsKey(placedObjectTypeAndCount.placedObjectTypeSO)) //Если этого типа нет в ключах словоря то добавим
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



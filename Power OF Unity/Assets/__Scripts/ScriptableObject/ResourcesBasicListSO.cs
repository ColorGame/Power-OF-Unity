using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Базовый список объектов размещения. БУДЕТ ВСЕГО ОДИН ЭКЗЕМПЛЯР (один список)
/// </summary>
[CreateAssetMenu(fileName = "ResourcesBasicListSO", menuName = "ScriptableObjects/ResourcesBasicListSO")]
public class ResourcesBasicListSO : ScriptableObject
{
    [Header("Стартовое количество МОНЕТ")]
    [SerializeField] private int _coin;
    [Header("Список предметов с ДЕЙСТВИЕМ(Action)")]
    [SerializeField] private List<PlacedObjectTypeAndCount> _placedObjectWithActionList; // Список Типов Размещяемых объектов c ДЕЙСТВИЕМ
    [Header("Список предметов БРОНИ(Armor)")]
    [SerializeField] private List<PlacedObjectTypeAndCount> _placedObjectTypeArmorList; // Список Размещяемых объектов типа БРОНЯ


    public int GetCoin() {  return _coin; }
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



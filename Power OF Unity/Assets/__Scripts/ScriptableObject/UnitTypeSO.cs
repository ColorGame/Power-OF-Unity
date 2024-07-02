using System;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// ScriptableObject, контейнер данных представляющий ЮНИТА для спавна. 
/// </summary>
/// <remarks>
/// abstract - НЕЛЬЗЯ создать экземпляр данного класса.
/// На практике вы, скорее всего, будете использовать подкласс, такой как "UnitFriendSO"  "UnitEnemySO".
/// </remarks>

public abstract class UnitTypeSO : ScriptableObject, ISerializationCallbackReceiver//ISerializationCallbackReceiver Интерфейс для получения обратных вызовов при сериализации и десериализации.Будем использовать для создания ID при сериализации
                                                                                   //Сериализацией называется процесс записи состояния объекта (с возможной последующей передачей) в поток. Десериализация это процесс обратный сериализации – из данных,
                                                                                   //которые находятся в потоке, мы можем восстановить состояние объекта и использовать этот объект в другом месте.
{
    [Header("Авт.сген. UUID для сохр/загр.\nОчистите это поле, если вы хотите сген-ть новое.")]
    [SerializeField] private string _unitID = null;
    [SerializeField] private string _name;   // Имя Юнита
    [SerializeField] private int _basicHealth; // Здоровье
    [SerializeField] private int _basicActionPoints = Constant.ACTION_POINTS_MAX; // Очки действия
    [Header("Дистанция движения.\nИзмеряется в узлах сетки, включает узел с самими Юнитом")]
    [SerializeField] private int _basicMoveDistance = Constant.MOVE_DISTSNCE_MAX;
    [Header("Префаб ядра юнита со скриптами")]
    [SerializeField] private Transform _unitCorePrefab;  

    public string GetName() { return _name; }
    public int GetBasicHealth() { return _basicHealth; }
    public int GetBasicActionPoints() { return _basicActionPoints; }
    public int GetBasicMoveDistance() { return _basicMoveDistance; }
    public Transform GetUnitCorePrefab() { return _unitCorePrefab; }
        
    /// <summary>
    /// Получить массив ДОПУСТИМЫХ базовых действий для этого юнита
    /// </summary>
    public BaseAction[] GetValidBaseActionArray()
    {
        return _unitCorePrefab.GetComponents<BaseAction>();
    }


    /* /// <summary>
     /// Получите экземпляр SO из его UUID.
     /// </summary>
     public static UnitTypeSO GetFromID(string itemID)
     {

     }*/


   

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        // Сгенерируйте и сохраните новый UUID, если он пуст или там просто пустые пробелы.
        if (string.IsNullOrWhiteSpace(_unitID))
        {
            _unitID = Guid.NewGuid().ToString();
        }
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        // Требуется ISerializationCallbackReceiver, но нам не нужно ничего с этим делать.
    }
}
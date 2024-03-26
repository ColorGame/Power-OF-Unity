
using System;
using System.Collections.Generic;
using UnityEngine;
using static PlacedObjectTypeBaseStatsSO;

[CreateAssetMenu(fileName = "GameAssets", menuName = "ScriptableObjects/GameAssets")]
public class GameAssets : ScriptableObject //Игровые Активы
{

    private static GameAssets _instance; // т.к. мы обрабатываем в ручную то не надо создавать его в сцене

    // Когда кто-то получит доступ к этому свойству оно запустит код (get) и при необходимости создаст экземпляр ИГРОВЫХ АКТИВОВ или вернет существующую
    public static GameAssets Instance
    {
        get //(расширим свойство get) Будем обрабатывать вручную а не через компилятор
        {
            if (_instance == null) // Если экземпляр нулевой то создадим его
            {
                _instance = Resources.Load<GameAssets>(typeof(GameAssets).Name); //В поле экземпляра - (_instance) установим ресурс запрошенного типа, хранящийся по адресу path(путь) в папке Resources(эту папку я создал в папке ScriptableObjects  и в папке Prefab).                                                                     
            }
            return _instance; //Вернем поле экземпляра (если оно не нулевое то вернем существующее)
        }
    }

    /// <summary>
    ///Структура: Тип вражеского юнита и ЕГО ПРЕФАБ
    /// </summary>
    [System.Serializable]
    public struct EnemyTypePrefabUnit
    {
        public EnemyUnitType enemyType;
        public Transform transformPrefab;

        public EnemyTypePrefabUnit(EnemyUnitType enemyType, Transform transformPrefab)
        {
            this.enemyType = enemyType;
            this.transformPrefab = transformPrefab;
        }
    }

    public Transform grenadeProjectilePrefab; // Префаб граната
    public Transform bulletProjectilePrefab; // Префаб пули
    public Transform placedObjectTypeButtonPrefab; // Кнопка выбора оружия
    public Transform levelGridSystemVisualSinglePrefab; // Префаб визуализации узла сетки
    public Transform inventoryGridSystemVisualSinglePrefab; // Префаб визуализации узла сетки Инвенторя
    [Header("ПРЕФАБЫ FX-система частиц")]
    public Transform grenadeExplosionFXPrefab; // Префаб частички взрыва гранаты //НЕЗАБУДЬ ПОСТАВИТЬ ГАЛОЧКУ У TRAIL самоуничтожение(Destroy) после проигрывания
    public Transform grenadeSmokeFXPrefab; // Префаб дыма от гранаты // Уничтожать дым будет скрипт прикрипленный к нему
    public Transform comboPartnerFXPrefab; // Префаб  частички взаимодействия
    public Transform electricityWhiteFXPrefab; // Префаб электромагнитное облако
    public Transform bulletHitFXPrefab; // Префаб частиц от поппадания пули
    public Transform healFXPrefab; // Префаб Частички исцеления
    public Transform spotterFireFXPrefab; // Префаб Частички волны при наблюдении

    [Header("Совместим Тип вражеского юнита и ЕГО ПРЕФАБ")]
    [SerializeField] private List<EnemyTypePrefabUnit> _enemyTypePrefabUnitList;

    /// <summary>
    /// Словарь (КЛЮЧ - Тип вражеского юнита; ЗНАЧЕНИЕ - ЕГО ПРЕФАБ)
    /// </summary>
    private Dictionary<EnemyUnitType, Transform> _enemyTypePrefabUnitDictionary = null;

    /// <summary>
    /// Заполнить первую часть таблицы ПРЕФАБЫ ВРАГА
    /// </summary>
    public void CompleteFirstPartTableEnemyPrefab()
    {
        if (_enemyTypePrefabUnitList.Count == 0)
        {
            _enemyTypePrefabUnitList = new List<EnemyTypePrefabUnit>();           
            foreach (EnemyUnitType enemyType  in Enum.GetValues(typeof(EnemyUnitType)))
            {
                _enemyTypePrefabUnitList.Add(new EnemyTypePrefabUnit(enemyType, null));
            }        
        }
        else{Debug.Log("ТАБЛИЦА -- Enemy Type Prefab Unit List -- уже заполнена!!!");}
    }

    public Transform GetEnemyPrefab(EnemyUnitType enemyType)
    {
        BuildLookup();
        return _enemyTypePrefabUnitDictionary[enemyType];
    }

    /// <summary>
    /// Поиск по сборке
    /// </summary>    
    private void BuildLookup()
    {
        if (_enemyTypePrefabUnitDictionary != null) return; // Выходим если Dictionary уже заполнен
        _enemyTypePrefabUnitDictionary = GetEnemyTypePrefabUnitDictionary();
    }

    private Dictionary<EnemyUnitType, Transform> GetEnemyTypePrefabUnitDictionary()
    {
        //Инициализируем возвращаемый словарь
        Dictionary<EnemyUnitType, Transform> enemyTypePrefabUnitDictionary = new Dictionary<EnemyUnitType, Transform>();

        for (int index = 0; index < _enemyTypePrefabUnitList.Count; index++)
        {
            EnemyTypePrefabUnit enemyTypePrefabUnit = _enemyTypePrefabUnitList[index];
            //Заполним словарь
            enemyTypePrefabUnitDictionary[enemyTypePrefabUnit.enemyType] = enemyTypePrefabUnit.transformPrefab;
        }
        return enemyTypePrefabUnitDictionary;
    }



}
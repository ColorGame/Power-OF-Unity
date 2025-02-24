using UnityEngine;

[CreateAssetMenu(fileName = "GameAssetsSO", menuName = "ScriptableObjects/Date/GameAssetsSO")]
public class GameAssetsSO : ScriptableObject //Игровые Активы
{

    private static GameAssetsSO _instance; // т.к. мы обрабатываем в ручную то не надо создавать его в сцене

    // Когда кто-то получит доступ к этому свойству оно запустит код (get) и при необходимости создаст экземпляр ИГРОВЫХ АКТИВОВ или вернет существующую
    public static GameAssetsSO Instance
    {
        get //(расширим свойство get) Будем обрабатывать вручную а не через компилятор
        {
            if (_instance == null) // Если экземпляр нулевой то создадим его
            {
                _instance = Resources.Load<GameAssetsSO>(typeof(GameAssetsSO).Name); //В поле экземпляра - (_instance) установим ресурс запрошенного типа, хранящийся по адресу path(путь) в папке Resources(эту папку я создал в папке ScriptableObjects  и в папке Prefab).                                                                     
            }
            return _instance; //Вернем поле экземпляра (если оно не нулевое то вернем существующее)
        }
    }
     
    public Transform grenadeProjectilePrefab; // Префаб граната
    public Transform bulletProjectilePrefab; // Префаб пули
    public Transform LevelGridVisualSinglePrefab; // Префаб визуализации узла сетки
    public EquipmentGridVisualSingle EquipmentGridInVisualSingleScreenSpacePrefab; // Префаб визуализации узла сетки Инвенторя
    public EquipmentGridVisualSingle EquipmentGridInVisualSingleWorldSpacePrefab; // Префаб визуализации узла сетки Инвенторя

    [Header("Кнопки - для динамического создания в UI")]
    public ActionButtonUI actionButtonUI; // Префаб кнопки выбора действия
    public PlacedObjectSelectButtonUI placedObjectSelectButton; // Кнопка выбора объекта  типа PlacedObject
    public PlacedObjectBuySellCountButtonUI placedObjectBuySellCountButton; //Кнопки изменения количества покупки и продажи объекта типа PlacedObject
    public UnitEnemySelectAtLevelButtonUI unitEnemySelectAtLevelButton; // Кнопка - выбора вражеского юнита на игровом уровне.
    public UnitSelectAtLevelButtonUI unitFriendSelectAtLevelButton; // Кнопка - выбора дружественного юнита на игровом уровне.
    public UnitSelectAtEquipmentButtonUI unitSelectAtEquipmentButton; // Кнопка - выбора  Юнита, для настройки экипировки
    public UnitSelectAtManagementButtonUI unitSelectAtManagementButton; // Кнопка - выбора  Юнита, в окне менеджмента юнитов

    [Header("ПРЕФАБЫ FX-система частиц")]
    public Transform grenadeExplosionFXPrefab; // Префаб частички взрыва гранаты //НЕЗАБУДЬ ПОСТАВИТЬ ГАЛОЧКУ У TRAIL самоуничтожение(Destroy) после проигрывания
    public Transform grenadeSmokeFXPrefab; // Префаб дыма от гранаты // Уничтожать дым будет скрипт прикрипленный к нему
    public Transform comboPartnerFXPrefab; // Префаб  частички взаимодействия
    public Transform electricityWhiteFXPrefab; // Префаб электромагнитное облако
    public Transform bulletHitFXPrefab; // Префаб частиц от поппадания пули
    public Transform healFXPrefab; // Префаб Частички исцеления
    public Transform spotterFireFXPrefab; // Префаб Частички волны при наблюдении
    
      



}
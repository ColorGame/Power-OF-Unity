using System;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.Rendering.DebugUI;


[CreateAssetMenu(fileName = "PlacedObjectTypeBaseStatsSO", menuName = "ScriptableObjects/Date/PlacedObjectTypeBaseStats")]

public class PlacedObjectTypeBaseStatsSO : ScriptableObject
{
    private static PlacedObjectTypeBaseStatsSO _instance; // т.к. мы обрабатываем в ручную то не надо создавать его в сцене

    // Когда кто-то получит доступ к этому свойству оно запустит код (get) и при необходимости создаст экземпляр ИГРОВЫХ АКТИВОВ или вернет существующую
    public static PlacedObjectTypeBaseStatsSO Instance
    {
        get //(расширим свойство get) Будем обрабатывать вручную а не через компилятор
        {
            if (_instance == null) // Если экземпляр нулевой то создадим его
            {
                _instance = Resources.Load<PlacedObjectTypeBaseStatsSO>(typeof(PlacedObjectTypeBaseStatsSO).Name); //В поле экземпляра - (_instance) установим ресурс запрошенного типа, хранящийся по адресу path(путь) в папке Resources(эту папку я создал в папке ScriptableObjects  и в папке Prefab).                                                                     
            }
            return _instance; //Вернем поле экземпляра (если оно не нулевое то вернем существующее)
        }
    }

    /// <summary>
    /// Список текста на разных языках, для данного типа размещенного объекта.<br/>Можно использовать за место КОРТЕЖЕЙ в списках
    /// </summary>
    [Serializable]
    public struct TextListPlacedObjectType
    {
        public PlacedObjectType placedObjectType;
        public string placedObjectSubtype;
        public List<string> textList;

        public TextListPlacedObjectType(PlacedObjectType placedObjectType, string placedObjectSubtype, List<string> stringList) //конструктор
        {
            this.placedObjectType = placedObjectType;
            this.placedObjectSubtype = placedObjectSubtype;
            this.textList = stringList;
        }
    }
    /// <summary>
    /// Структура для отображения в инспекторе ТИПА размещенного объекта и СПИСКА текста
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public struct TextListType<T> where T : Enum
    {
        public T placedObjectType;
        public List<string> textList;
        public TextListType(T placedObjectType, List<string> textList)
        {
            this.placedObjectType = placedObjectType;
            this.textList = textList;
        }
    }

    /// <summary>
    /// Индекс СТОЛБЦА в таблице где перечислены ТИПЫ (placedObjectType) размещенных объектов. 2й столбец(индекс=1)
    /// </summary>
    private const int PLACED_OBJECT_TYPE_COLUMN_INDEX = 1;
    /// <summary>
    /// Индекс СТОЛБЦА в таблице где перечислены ПОДТИПЫ (placedObjectType) размещенных объектов. 3й столбец(индекс=2)
    /// </summary>
    private const int PLACED_OBJECT_SUBTYPE_COLUMN_INDEX = 2;

    /// <summary>
    /// Начальный индекс СТРОКИ, в Гугл таблице, с которой мы будем заполнять наш лист.<br/>Будем переберать со 2й строки(индекс =1), 1-это шапка таблицы
    /// </summary>
    private const int START_ROW_INDEX = 1;

    /// <summary>
    /// Начальный индекс СТОЛБЦА, в Гугл таблице, с которой мы будем заполнять наш лист.<br/>Будем переберать с 4-ей колнки (индекс =3)  там храняться данные  
    /// </summary>
    private const int START_COLUMN_INDEX = 3;

    /// <summary>
    /// Индекс строки ЗАГОЛОВКА таблицы. 1-я строка (индекс=0)
    /// </summary>
    private const int TABLE_HEADER_ROW_INDEX = 0;

    /// <summary>
    /// Страницы нашей Гугл таблицы
    /// </summary>
    private enum TablePage
    {
        Name = 1,
        Descriptions = 2,
        Details = 3,
        SideEffects = 4,
    }

    /// <summary>
    /// Идентификатор таблицы в Google таблицах.
    /// Допустим, ваша таблица имеет следующий URL-адрес https://docs.google.com/spreadsheets/d/1ic4wt3ADm_Qta03j3m8dS_gJjr2x0XbFLZ75f6TvaY4/edit#gid=1901640271
    /// В этом случае идентификатор таблицы равен _sheetId = "1ic4wt3ADm_Qta03j3m8dS_gJjr2x0XbFLZ75f6TvaY4" и _pageNameId "1901640271" (the gid parameter).
    /// </summary>
    private string _sheetId = "1ic4wt3ADm_Qta03j3m8dS_gJjr2x0XbFLZ75f6TvaY4";
    /// <summary>
    /// Словарь (КЛЮЧ - Название страницы; ЗНАЧЕНИЕ - ID страницы)
    /// </summary>
    private readonly Dictionary<TablePage, int> _pageIDDictionary = new() { { TablePage.Name, 1901640271 }, { TablePage.Descriptions, 563410214 }, { TablePage.Details, 350745494 }, { TablePage.SideEffects, 2116600352 } };
    /// <summary>
    /// Список ИМЕН предметов на разных языках
    /// </summary>
    [Header("Список ИМЕН предметов на разных языках")]
    [SerializeField] private List<TextListPlacedObjectType> _namesPlacedObjectList;
    /// <summary>
    /// Список ОПИСАНИЙ  предметов на разных языках
    /// </summary>
    [Header("Список ОПИСАНИЙ  предметов на разных языках")]
    [SerializeField] private List<TextListPlacedObjectType> _descriptionsPlacedObjectList;
    /// <summary>
    /// Список ПОДРОБНОСТЕЙ  предметов на разных языках
    /// </summary>
    [Header("Список ПОДРОБНОСТЕЙ  предметов на разных языках")]
    [SerializeField] private List<TextListPlacedObjectType> _detailsPlacedObjectList;
    /// <summary>
    /// Список ПОБОЧНЫХ ЭФФЕКТОВ  предметов на разных языках
    /// </summary>
     [Header("Список ПОБОЧНЫХ ЭФФЕКТОВ  предметов на разных языках")]
    [SerializeField] private List<TextListPlacedObjectType> _sideEffectsPlacedObjectList;


    private Dictionary<BodyArmorType, List<PlacedObjectTooltip>> _bodyArmorTypeTooltipDict = null;
    private Dictionary<HeadArmorType, List<PlacedObjectTooltip>> _headArmorTypeTooltipDict = null;
    private Dictionary<CombatDroneType, List<PlacedObjectTooltip>> _combatDroneTypeTooltipDict = null;
    private Dictionary<GrappleType, List<PlacedObjectTooltip>> _grappleTypeTooltipDict = null;
    private Dictionary<GrenadeType, List<PlacedObjectTooltip>> _grenadeTypeTooltipDict = null;
    private Dictionary<HealItemType, List<PlacedObjectTooltip>> _healItemTypeTooltipDict = null;
    private Dictionary<ShieldItemType, List<PlacedObjectTooltip>> _shieldItemTypeTooltipDict = null;
    private Dictionary<ShootingWeaponType, List<PlacedObjectTooltip>> _shootingWeaponTypeTooltipDict = null;
    private Dictionary<SpotterFireItemType, List<PlacedObjectTooltip>> _spotterFireItemTypeTooltipDict = null;
    private Dictionary<SwordType, List<PlacedObjectTooltip>> _swordTypeTooltipDict = null;



    public PlacedObjectTooltip GetTooltipPlacedObject(PlacedObjectTypeSO placedObjectTypeSO, Enum enumPlacedObject, LanguageIndex languageIndex = LanguageIndex.Russian)
    {
        BuildLookup(placedObjectTypeSO);

        List<PlacedObjectTooltip> placedObjectTooltipList = new();

        switch (placedObjectTypeSO)
        {
            case BodyArmorTypeSO:
                placedObjectTooltipList = _bodyArmorTypeTooltipDict[(BodyArmorType)enumPlacedObject];
                break;
            case HeadArmorTypeSO:
                placedObjectTooltipList = _headArmorTypeTooltipDict[(HeadArmorType)enumPlacedObject];
                break;
            case CombatDroneTypeSO:
                placedObjectTooltipList = _combatDroneTypeTooltipDict[(CombatDroneType)enumPlacedObject];
                break;
            case GrappleTypeSO:
                placedObjectTooltipList = _grappleTypeTooltipDict[(GrappleType)enumPlacedObject];
                break;
            case GrenadeTypeSO:
                placedObjectTooltipList = _grenadeTypeTooltipDict[(GrenadeType)enumPlacedObject];
                break;
            case HealItemTypeSO:
                placedObjectTooltipList = _healItemTypeTooltipDict[(HealItemType)enumPlacedObject];
                break;
            case ShieldItemTypeSO:
                placedObjectTooltipList = _shieldItemTypeTooltipDict[(ShieldItemType)enumPlacedObject];
                break;
            case ShootingWeaponTypeSO:
                placedObjectTooltipList = _shootingWeaponTypeTooltipDict[(ShootingWeaponType)enumPlacedObject];
                break;
            case SpotterFireItemTypeSO:
                placedObjectTooltipList = _spotterFireItemTypeTooltipDict[(SpotterFireItemType)enumPlacedObject];
                break;
            case SwordTypeSO:
                placedObjectTooltipList = _swordTypeTooltipDict[(SwordType)enumPlacedObject];
                break;
        }
        return placedObjectTooltipList[(int)languageIndex]; // По индексу актуального языка вернем всплывающую подсказку
    }



    [ContextMenu("OpenGoogleSheet")]
    private void Open()
    {
        Application.OpenURL($"https://docs.google.com/spreadsheets/d/{_sheetId}/edit?gid={_pageIDDictionary[TablePage.Name]}#gid={_pageIDDictionary[TablePage.Name]}");
    }

    [ContextMenu("DownloadSheets")]
    private void DownloadSheets() // Синхронизация данных
    {
        // В кроутине заполним наши таблицы
        EditorCoroutineUtility.StartCoroutineOwnerless(DownloadRawCvsTable(_sheetId, _pageIDDictionary));// Этот метод запускает EditorCoroutine без объекта-владельца.EditorCoroutine выполняется до тех пор, пока не завершится или не будет отменен с помощью StopCoroutine(EditorCoroutine).
    }
    //  https://docs.google.com/spreadsheets/d/1ic4wt3ADm_Qta03j3m8dS_gJjr2x0XbFLZ75f6TvaY4/edit?usp=sharing
    private IEnumerator DownloadRawCvsTable(string sheetId, Dictionary<TablePage, int> pageTableID)
    {
        foreach (TablePage page in Enum.GetValues(typeof(TablePage)))
        {
            string actualUrl = $"https://docs.google.com/spreadsheets/d/{sheetId}/export?format=csv&gid={pageTableID[page]}";
            using (UnityWebRequest request = UnityWebRequest.Get(actualUrl)) // using- позволяет безопасно работать с фаловым потоком, и предотвратить утечку памяти(не надо каждый раз закрывать поток что бы не перепольнить память)
            {
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError ||
                    request.result == UnityWebRequest.Result.DataProcessingError)
                {
                    Debug.LogError(request.error);
                }
                else
                {
                    // Debug.Log($"Успешная загрузка {page}");
                    // Debug.Log(request.downloadHandler.text);
                    yield return CompleteTables(request.downloadHandler.text, page);//(STOP) Запустим другую кроутину 
                }
            }
        }               
        yield return null;
    }

    private IEnumerator CompleteTables(string rawCSVText, TablePage pages)//Заполнение таблиц
    {
        switch (pages)
        {
            case TablePage.Name:
                _namesPlacedObjectList = GetCompleteTable(rawCSVText);
                break;

            case TablePage.Descriptions:
                _descriptionsPlacedObjectList = GetCompleteTable(rawCSVText);
                break;
            case TablePage.Details:
                _detailsPlacedObjectList = GetCompleteTable(rawCSVText);
                break;
            case TablePage.SideEffects:
                _sideEffectsPlacedObjectList = GetCompleteTable(rawCSVText);
                break;
        }
        yield return null;// yield return null - это точка, в которой выполнение приостанавливается и возобновляется в следующем кадре.
    }

    private List<TextListPlacedObjectType> GetCompleteTable(string rawCSVText) //Заполнение таблицы  
    {
        // Инициализируем список который будем заполнять       
        List<TextListPlacedObjectType> textPlacedObjectList = new();

        string[,] gridDateArray = SheetProcessor.ProcessData(rawCSVText); // Получим преобразованный текст в двумерный массив

        if (!LanguageIndexInTableCorrectOrder(gridDateArray)) return null; //Вернем null  если не совпадает       

        // Переберем СТРОКИ       
        for (int rowIndex = START_ROW_INDEX; rowIndex < gridDateArray.GetLength(0); rowIndex++) //переберем строки // GetLength(0)- вернет длину первой части двумерного массива  (если string[4,5] - то получим 4 )
        {
            // Создадим экземпляр TextListPlacedObjectType и инициализируем его поля
            TextListPlacedObjectType textPlacedObjectType = new();

            // Получим тип размещаемого объекта в этой строке
            textPlacedObjectType.placedObjectType = SheetProcessor.ParseEnum<PlacedObjectType>(gridDateArray[rowIndex, PLACED_OBJECT_TYPE_COLUMN_INDEX]);// Преобразуем данные из ячейки в Enum типа<T>
            textPlacedObjectType.placedObjectSubtype = gridDateArray[rowIndex, PLACED_OBJECT_SUBTYPE_COLUMN_INDEX];

            // Переберем СТОЛБЦЫ
            List<string> textList = new();
            for (int columnIndex = START_COLUMN_INDEX; columnIndex < gridDateArray.GetLength(1); columnIndex++) // Перберем ячейки в данной строке // GetLength(1)- вернет длину второй части двумерного массива  (если string[4,5] - то получим 5 )
            {
                string text = gridDateArray[rowIndex, columnIndex];
                textList.Add(text);
            }
            textPlacedObjectType.textList = textList;

            textPlacedObjectList.Add(textPlacedObjectType);
        }
        return textPlacedObjectList;
    }

    /// <summary>
    /// Проверим - Индекс языков в таблице, идет в правильном порядке?
    /// </summary>
    ///  <returns>
    /// true - правильный порядок. false -порядок нарушен. 
    ///  </returns>
    private bool LanguageIndexInTableCorrectOrder(string[,] gridDateArray)
    {
        // Переберем столбцы для первой строки. Это ШАПКА таблицы            
        for (int columnIndex = START_COLUMN_INDEX; columnIndex < gridDateArray.GetLength(1); columnIndex++) // Перберем ячейки в строке // GetLength(1)- вернет длину второй части двумерного массива  (если string[4,5] - то получим 5 )
        {
            LanguageIndex testlanguageIndex = SheetProcessor.ParseEnum<LanguageIndex>(gridDateArray[TABLE_HEADER_ROW_INDEX, columnIndex]);// Преобразуем данные из ячейки в Enum типа<LanguageIndex>

            int сorrectIndex = columnIndex - START_COLUMN_INDEX; //Перечесление языков начинается с 3-ей колонки (для того чтобы первый язык имел 0-й индекс отнимим смещение)
            if ((int)testlanguageIndex != сorrectIndex) // Если индекс языка не совпадает с порядком (0,1,2,3...)
            {
                Debug.LogError($"Не совподает индексы для {testlanguageIndex} из загружаемой гугл таблицы");
                return false;
            }
        }
        return true;
    }


    /// <summary>
    /// Поиск по сборке
    /// </summary>    
    private void BuildLookup(PlacedObjectTypeSO placedObjectTypeSO)
    {
        switch (placedObjectTypeSO)
        {
            case BodyArmorTypeSO:
                if(_bodyArmorTypeTooltipDict!=null)return;
                _bodyArmorTypeTooltipDict = GetPlacedObjectTooltipDictionary<BodyArmorType>();
                break;
            case HeadArmorTypeSO:
                if(_headArmorTypeTooltipDict!=null) return;
                _headArmorTypeTooltipDict = GetPlacedObjectTooltipDictionary<HeadArmorType>();
                break;
            case CombatDroneTypeSO:
                if (_combatDroneTypeTooltipDict != null) return;
                _combatDroneTypeTooltipDict = GetPlacedObjectTooltipDictionary<CombatDroneType>();
                break;
            case GrappleTypeSO:
                if (_grappleTypeTooltipDict != null) return;
                _grappleTypeTooltipDict =GetPlacedObjectTooltipDictionary<GrappleType>(); 
                break;
            case GrenadeTypeSO:
                if (_grenadeTypeTooltipDict != null) return;
                _grenadeTypeTooltipDict = GetPlacedObjectTooltipDictionary<GrenadeType>();
                break;
            case HealItemTypeSO:
                if (_healItemTypeTooltipDict != null) return;
                _healItemTypeTooltipDict = GetPlacedObjectTooltipDictionary<HealItemType>();
                break;
            case ShieldItemTypeSO:
                if (_shieldItemTypeTooltipDict != null) return;
                _shieldItemTypeTooltipDict = GetPlacedObjectTooltipDictionary<ShieldItemType>();
                break;
            case ShootingWeaponTypeSO:
                if (_shootingWeaponTypeTooltipDict != null) return;
                _shootingWeaponTypeTooltipDict = GetPlacedObjectTooltipDictionary<ShootingWeaponType>();
                break;
            case SpotterFireItemTypeSO:
                if (_spotterFireItemTypeTooltipDict != null) return;
                _spotterFireItemTypeTooltipDict = GetPlacedObjectTooltipDictionary<SpotterFireItemType>();
                break;
            case SwordTypeSO:
                if (_grappleTypeTooltipDict != null) return;
                _swordTypeTooltipDict = GetPlacedObjectTooltipDictionary<SwordType>();
                break;
        }
    }

    /// <summary>
    /// Заполнить словарь, всплывающих подсказок для размещенного объекта, из имеющихся списков
    /// </summary>
    private Dictionary<T, List<PlacedObjectTooltip>> GetPlacedObjectTooltipDictionary<T>() where T : Enum
    {
        //Инициализируем возвращаемый словарь
        Dictionary<T, List<PlacedObjectTooltip>> placedObjectTypeTooltipDictionary = new();

        // Во всех списках последовательность PlacedObject и их количество совподают поэтому переберем любой из них (нам нужны только индексы).
        for (int indexPlacedObject = 0; indexPlacedObject < _namesPlacedObjectList.Count; indexPlacedObject++)
        {
            T placedObjectSubtype = default;
            PlacedObjectType placedObjectType = _namesPlacedObjectList[indexPlacedObject].placedObjectType;

            // Если запрашиваемый тип T отличается от типа под индексом [indexPlacedObject] то переходим к след объекту
            if (placedObjectSubtype is BodyArmorType && placedObjectType != PlacedObjectType.BodyArmorType)          
                continue; //Переходим к след циклу
            if (placedObjectSubtype is HeadArmorType && placedObjectType != PlacedObjectType.HeadArmorType)
                continue; //Переходим к след циклу
            if (placedObjectSubtype is CombatDroneType && placedObjectType != PlacedObjectType.CombatDroneType)
                continue; //Переходим к след циклу
            if (placedObjectSubtype is GrappleType && placedObjectType != PlacedObjectType.GrappleType)
                continue; //Переходим к след циклу
            if (placedObjectSubtype is GrenadeType && placedObjectType != PlacedObjectType.GrenadeType)
                continue; //Переходим к след циклу
            if (placedObjectSubtype is HealItemType && placedObjectType != PlacedObjectType.HealItemType)
                continue; //Переходим к след циклу
            if (placedObjectSubtype is ShieldItemType && placedObjectType != PlacedObjectType.ShieldItemType)
                continue; //Переходим к след циклу
            if (placedObjectSubtype is ShootingWeaponType && placedObjectType != PlacedObjectType.ShootingWeaponType)
                continue; //Переходим к след циклу
            if (placedObjectSubtype is SpotterFireItemType && placedObjectType != PlacedObjectType.SpotterFireItemType)
                continue; //Переходим к след циклу
            if (placedObjectSubtype is SwordType && placedObjectType != PlacedObjectType.SwordType)
                continue; //Переходим к след циклу
            
            // После проверок присвоим значение
            placedObjectSubtype = SheetProcessor.ParseEnum<T>(_namesPlacedObjectList[indexPlacedObject].placedObjectSubtype);
           
            // Инициализируем список который будем заполнять
            List<PlacedObjectTooltip> placedObjectTooltipList = new List<PlacedObjectTooltip>();
            //Переберем список текста на разных языках, для данного типа размещенного объекта 
            for (int indexString = 0; indexString < _namesPlacedObjectList[indexPlacedObject].textList.Count; indexString++)
            {
                string name = _namesPlacedObjectList[indexPlacedObject].textList[indexString];
                string descriptions = _descriptionsPlacedObjectList[indexPlacedObject].textList[indexString];
                string details = _detailsPlacedObjectList[indexPlacedObject].textList[indexString];
                string sideEffects = _sideEffectsPlacedObjectList[indexPlacedObject].textList[indexString];

                PlacedObjectTooltip placedObjectTooltip = new PlacedObjectTooltip(name, descriptions, details, sideEffects);
                placedObjectTooltipList.Add(placedObjectTooltip);
            }
            //Присвоим КЛЮЧ и ЗНАЧЕНИЕ      
            placedObjectTypeTooltipDictionary[placedObjectSubtype] = placedObjectTooltipList;
        }
        return placedObjectTypeTooltipDictionary;
    }

    /// <summary>
    /// Преобразуем СПИСОК в СЛОВАРЬ
    /// </summary>  
    private Dictionary<PlacedObjectType, List<string>> ListToDictionary(List<TextListPlacedObjectType> textPlacedObjectList)
    {
        Dictionary<PlacedObjectType, List<string>> newDictionary = new Dictionary<PlacedObjectType, List<string>>();

        foreach (TextListPlacedObjectType textPlacedObject in textPlacedObjectList)
        {
            newDictionary[textPlacedObject.placedObjectType] = textPlacedObject.textList;
        }
        return newDictionary;
    }
}


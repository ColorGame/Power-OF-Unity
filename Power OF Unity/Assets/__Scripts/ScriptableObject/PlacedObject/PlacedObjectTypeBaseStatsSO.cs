using System;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEngine.Networking;


[CreateAssetMenu(fileName = "PlacedObjectTypeBaseStatsSO", menuName = "ScriptableObjects/PlacedObjectTypeBaseStats")]

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
    /// Абстрактный текст на разных языках, для данного типа размещенного объекта 
    /// </summary>
    [System.Serializable]
    public struct TextPlacedObjectType
    {
        public PlacedObjectType placedObjectType;
        public List<string> stringList;

        public TextPlacedObjectType(PlacedObjectType placedObjectType, List<string> stringList) //конструктор
        {
            this.placedObjectType = placedObjectType;
            this.stringList = stringList;
        }
    }

    /// <summary>
    /// Индекс СТОЛБЦА в таблице где перечислены размещенные объекты - <see cref="PlacedObjectType"/>
    /// </summary>
    private const int PLACED_OBJECT_TYPE_COLUMN_INDEX = 1;

    /// <summary>
    /// Начальный индекс СТРОКИ, в Гугл таблице, с которой мы будем заполнять наш лист. Будем переберать со 2й строки, 1-это шапка таблицы
    /// </summary>
    private const int START_ROW_INDEX = 1;

    /// <summary>
    /// Начальный индекс СТОЛБЦА, в Гугл таблице, с которой мы будем заполнять наш лист. Будем переберать с 3-ей колнки (индекс =2)  там храняться данные  
    /// </summary>
    private const int START_COLUMN_INDEX = 2;

    /// <summary>
    /// Индекс строки ЗАГОЛОВКА таблицы. 1-я строка (индекс=0)
    /// </summary>
    private const int TABLE_HEADER_ROW_INDEX = 0;

    /// <summary>
    /// Страницы нашей Гугл таблицы
    /// </summary>
    private enum TablePage
    {
        Names,
        Descriptions,
        Details,
        SideEffects
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
    private readonly Dictionary<TablePage, int> _pageIDDictionary = new() { { TablePage.Names, 1901640271 }, { TablePage.Descriptions, 563410214 }, { TablePage.Details, 350745494 }, { TablePage.SideEffects, 2116600352 } };

    [SerializeField] private LanguageIndex _languageIndex;
        
    [Tooltip("Список имен предметов на разных языках)")]
    [SerializeField] private List<TextPlacedObjectType> _namesPlacedObjectList;

    [Tooltip("Список ОПИСАНИЙ  предметов на разных языках")]
    [SerializeField] private List<TextPlacedObjectType> _descriptionsPlacedObjectList;

    [Tooltip("Список ПОДРОБНОСТЕЙ  предметов на разных языках")]
    [SerializeField] private List<TextPlacedObjectType> _detailsPlacedObjectList;

    [Tooltip("Список ПОБОЧНЫХ ЭФФЕКТОВ  предметов на разных языках")]
    [SerializeField] private List<TextPlacedObjectType> _sideEffectsPlacedObjectList;

    /// <summary>
    /// Словарь (КЛЮЧ - Тип размещенного объекта; ЗНАЧЕНИЕ - Список всплывающих подсказок на разных языках)
    /// </summary>
    private Dictionary<PlacedObjectType, List<PlacedObjectTooltip>> _placedObjectTooltipDictionary = null;

    private Dictionary<BodyArmorType, List<PlacedObjectTooltip>> _bodyArmorTypeTooltipDict = null;
    private Dictionary<GrappleType, List<PlacedObjectTooltip>> _grappleTypeTooltipDict = null;
    private Dictionary<GrenadeType, List<PlacedObjectTooltip>> _grenadeTypeTooltipDict = null;
    private Dictionary<HeadArmorType, List<PlacedObjectTooltip>> _headArmorTypeTooltipDict = null;
    private Dictionary<HealItemType, List<PlacedObjectTooltip>> _healItemTypeTooltipDict = null;
    private Dictionary<ShieldItemType, List<PlacedObjectTooltip>> _shieldItemTypeTooltipDict = null;
    private Dictionary<ShootingWeaponType, List<PlacedObjectTooltip>> _shootingWeaponTypeTooltipDict = null;
    private Dictionary<SpotterFireItemType, List<PlacedObjectTooltip>> _spotterFireItemTypeTooltipDict = null;
    private Dictionary<SwordType, List<PlacedObjectTooltip>> _swordTypeTooltipDict = null;
    private Dictionary<VisionItemType, List<PlacedObjectTooltip>> _visionItemTypeTooltipDict = null;


    public PlacedObjectTooltip GetTooltipPlacedObject(PlacedObjectTypeSO placedObjectTypeSO, Enum enumPlacedObject)
    {
        BuildLookup();
        switch (placedObjectTypeSO)
        {
            default:
            case GrappleTypeSO:
                List<PlacedObjectTooltip> placedObjectTooltipList = _grappleTypeTooltipDict[(GrappleType)enumPlacedObject]; // Получим список всплывающих подсказок на разных языках для переданного PlacedObjectType
                return placedObjectTooltipList[(int)_languageIndex]; // По индексу актуального языка вернем всплывающую подсказку
               

        }
       
    }

    public PlacedObjectTooltip GetTooltipPlacedObject(PlacedObjectType placedObjectType)
    {
        BuildLookup();
        List<PlacedObjectTooltip> placedObjectTooltipList = _placedObjectTooltipDictionary[placedObjectType]; // Получим список всплывающих подсказок на разных языках для переданного PlacedObjectType
        return placedObjectTooltipList[(int)_languageIndex]; // По индексу актуального языка вернем всплывающую подсказку

    }

    public string GetNamePlacedObject(PlacedObjectType placedObjectType)
    {
        BuildLookup();
        List<PlacedObjectTooltip> placedObjectTooltipList = _placedObjectTooltipDictionary[placedObjectType]; // Получим список всплывающих подсказок на разных языках для переданного PlacedObjectType              
        return placedObjectTooltipList[(int)_languageIndex].name.ToString(); // По индексу актуального языка вернем строку
    }



    [ContextMenu("OpenGoogleSheet")]
    private void Open()
    {
        Application.OpenURL($"https://docs.google.com/spreadsheets/d/{_sheetId}/edit#gid={_pageIDDictionary[TablePage.Names]}");
    }

    [ContextMenu("DownloadSheets")]
    private void DownloadSheets() // Синхронизация данных
    {
        // В кроутине заполним наши таблицы
        EditorCoroutineUtility.StartCoroutineOwnerless(DownloadRawCvsTable(_sheetId, _pageIDDictionary));// Этот метод запускает EditorCoroutine без объекта-владельца.EditorCoroutine выполняется до тех пор, пока не завершится или не будет отменен с помощью StopCoroutine(EditorCoroutine).
    }

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
            case TablePage.Names:
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
        BuildLookup();
        yield return null;// yield return null - это точка, в которой выполнение приостанавливается и возобновляется в следующем кадре.
    }

    private List<TextPlacedObjectType> GetCompleteTable(string rawCSVText) //Заполнение таблицы  
    {
        // Инициализирум поля который будем заполнять       
        List<TextPlacedObjectType> textPlacedObjectList = new List<TextPlacedObjectType>();

        string[,] gridDateArray = SheetProcessor.ProcessData(rawCSVText); // Получим преобразованный текст в двумерный массив

        if (!LanguageIndexInTableCorrectOrder(gridDateArray)) return null; //Вернем null  если не совпадает              

        // Переберем СТРОКИ       
        for (int rowIndex = START_ROW_INDEX; rowIndex < gridDateArray.GetLength(0); rowIndex++) //переберем стоки // GetLength(0)- вернет длину первой части двумерного массива  (если string[4,5] - то получим 4 )
        {
            // Получим тип размещаемого объекта в этой строке
            PlacedObjectType placedObjectType = SheetProcessor.ParseEnum<PlacedObjectType>(gridDateArray[rowIndex, PLACED_OBJECT_TYPE_COLUMN_INDEX]);// Преобразуем данные из ячейки в Enum типа<PlacedObjectType>

            //Если список пустой,  а если нет то - Сравним с последним предметом в списке и если это другой предмет то 
            if (textPlacedObjectList.Count == 0 || placedObjectType != textPlacedObjectList[textPlacedObjectList.Count - 1].placedObjectType)
            {
                // Создадим экземпляр TextPlacedObjectType и инициализируем его поля
                TextPlacedObjectType textPlacedObjectType = new TextPlacedObjectType(placedObjectType, new List<string>());

                textPlacedObjectList.Add(textPlacedObjectType); // Добавим в список созданный экземпляр

                // Переберем СТОЛБЦЫ               
                for (int columnIndex = START_COLUMN_INDEX; columnIndex < gridDateArray.GetLength(1); columnIndex++) // Перберем ячейки в данной строке // GetLength(1)- вернет длину второй части двумерного массива  (если string[4,5] - то получим 5 )
                {
                    string text = gridDateArray[rowIndex, columnIndex];

                    textPlacedObjectType.stringList.Add(text);
                }
            }
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
        for (int columnIndex = START_COLUMN_INDEX; columnIndex < gridDateArray.GetLength(1); columnIndex++) // Перберем ячейки в данной строке // GetLength(1)- вернет длину второй части двумерного массива  (если string[4,5] - то получим 5 )
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
    private void BuildLookup()
    {
       if(_placedObjectTooltipDictionary!= null) return; // Выходим если Dictionary уже заполнен
        _placedObjectTooltipDictionary = GetPlacedObjectTooltipDictionary();
    }

    /// <summary>
    /// Получить словарь всплывающих подсказок для размещенного объекта
    /// </summary>
    private Dictionary<PlacedObjectType, List<PlacedObjectTooltip>> GetPlacedObjectTooltipDictionary()
    {
        //Инициализируем возвращаемый словарь
        Dictionary<PlacedObjectType, List<PlacedObjectTooltip>> placedObjectTooltipDictionary = new ();

        // Во всех списках последовательность PlacedObjectType и языков совподают поэтому переберем любой из них (нам нужны только индексы).
        for (int indexPlacedObject = 0; indexPlacedObject < _namesPlacedObjectList.Count; indexPlacedObject++)
        {
            PlacedObjectType placedObjectType = _namesPlacedObjectList[indexPlacedObject].placedObjectType;
            // Инициализируем список который будем заполнять
            List<PlacedObjectTooltip> placedObjectTooltipList = new List<PlacedObjectTooltip>();
            //Переберем список текста на разных языках, для данного типа размещенного объекта 
            for (int indexString = 0; indexString < _namesPlacedObjectList[indexPlacedObject].stringList.Count; indexString++)
            {
                string name = _namesPlacedObjectList[indexPlacedObject].stringList[indexString];
                string descriptions = _descriptionsPlacedObjectList[indexPlacedObject].stringList[indexString];
                string details = _detailsPlacedObjectList[indexPlacedObject].stringList[indexString];
                string sideEffects = _sideEffectsPlacedObjectList[indexPlacedObject].stringList[indexString];

                PlacedObjectTooltip placedObjectTooltip = new PlacedObjectTooltip(name, descriptions, details, sideEffects);
                placedObjectTooltipList.Add(placedObjectTooltip);
            }
            placedObjectTooltipDictionary[placedObjectType] = placedObjectTooltipList;
        }
        return placedObjectTooltipDictionary;
    }


    /// <summary>
    /// Преобразуем СПИСОК в СЛОВАРЬ
    /// </summary>  
    private Dictionary<PlacedObjectType, List<string>> ListToDictionary(List<TextPlacedObjectType> textPlacedObjectList)
    {
        Dictionary<PlacedObjectType, List<string>> newDictionary = new Dictionary<PlacedObjectType, List<string>>();

        foreach (TextPlacedObjectType textPlacedObject in textPlacedObjectList)
        {
            newDictionary[textPlacedObject.placedObjectType] = textPlacedObject.stringList;
        }
        return newDictionary;
    }


    

}

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
    private static PlacedObjectTypeBaseStatsSO _instance; // �.�. �� ������������ � ������ �� �� ���� ��������� ��� � �����

    // ����� ���-�� ������� ������ � ����� �������� ��� �������� ��� (get) � ��� ������������� ������� ��������� ������� ������� ��� ������ ������������
    public static PlacedObjectTypeBaseStatsSO Instance
    {
        get //(�������� �������� get) ����� ������������ ������� � �� ����� ����������
        {
            if (_instance == null) // ���� ��������� ������� �� �������� ���
            {
                _instance = Resources.Load<PlacedObjectTypeBaseStatsSO>(typeof(PlacedObjectTypeBaseStatsSO).Name); //� ���� ���������� - (_instance) ��������� ������ ������������ ����, ���������� �� ������ path(����) � ����� Resources(��� ����� � ������ � ����� ScriptableObjects  � � ����� Prefab).                                                                     
            }
            return _instance; //������ ���� ���������� (���� ��� �� ������� �� ������ ������������)
        }
    }

    /// <summary>
    /// ������ ������ �� ������ ������, ��� ������� ���� ������������ �������.<br/>����� ������������ �� ����� �������� � �������
    /// </summary>
    [Serializable]
    public struct TextListPlacedObjectType
    {
        public PlacedObjectType placedObjectType;
        public string placedObjectSubtype;
        public List<string> textList;

        public TextListPlacedObjectType(PlacedObjectType placedObjectType, string placedObjectSubtype, List<string> stringList) //�����������
        {
            this.placedObjectType = placedObjectType;
            this.placedObjectSubtype = placedObjectSubtype;
            this.textList = stringList;
        }
    }
    /// <summary>
    /// ��������� ��� ����������� � ���������� ���� ������������ ������� � ������ ������
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
    /// ������ ������� � ������� ��� ����������� ���� (placedObjectType) ����������� ��������. 2� �������(������=1)
    /// </summary>
    private const int PLACED_OBJECT_TYPE_COLUMN_INDEX = 1;
    /// <summary>
    /// ������ ������� � ������� ��� ����������� ������� (placedObjectType) ����������� ��������. 3� �������(������=2)
    /// </summary>
    private const int PLACED_OBJECT_SUBTYPE_COLUMN_INDEX = 2;

    /// <summary>
    /// ��������� ������ ������, � ���� �������, � ������� �� ����� ��������� ��� ����.<br/>����� ���������� �� 2� ������(������ =1), 1-��� ����� �������
    /// </summary>
    private const int START_ROW_INDEX = 1;

    /// <summary>
    /// ��������� ������ �������, � ���� �������, � ������� �� ����� ��������� ��� ����.<br/>����� ���������� � 4-�� ������ (������ =3)  ��� ��������� ������  
    /// </summary>
    private const int START_COLUMN_INDEX = 3;

    /// <summary>
    /// ������ ������ ��������� �������. 1-� ������ (������=0)
    /// </summary>
    private const int TABLE_HEADER_ROW_INDEX = 0;

    /// <summary>
    /// �������� ����� ���� �������
    /// </summary>
    private enum TablePage
    {
        Name = 1,
        Descriptions = 2,
        Details = 3,
        SideEffects = 4,
    }

    /// <summary>
    /// ������������� ������� � Google ��������.
    /// ��������, ���� ������� ����� ��������� URL-����� https://docs.google.com/spreadsheets/d/1ic4wt3ADm_Qta03j3m8dS_gJjr2x0XbFLZ75f6TvaY4/edit#gid=1901640271
    /// � ���� ������ ������������� ������� ����� _sheetId = "1ic4wt3ADm_Qta03j3m8dS_gJjr2x0XbFLZ75f6TvaY4" � _pageNameId "1901640271" (the gid parameter).
    /// </summary>
    private string _sheetId = "1ic4wt3ADm_Qta03j3m8dS_gJjr2x0XbFLZ75f6TvaY4";
    /// <summary>
    /// ������� (���� - �������� ��������; �������� - ID ��������)
    /// </summary>
    private readonly Dictionary<TablePage, int> _pageIDDictionary = new() { { TablePage.Name, 1901640271 }, { TablePage.Descriptions, 563410214 }, { TablePage.Details, 350745494 }, { TablePage.SideEffects, 2116600352 } };
    /// <summary>
    /// ������ ���� ��������� �� ������ ������
    /// </summary>
    [Header("������ ���� ��������� �� ������ ������")]
    [SerializeField] private List<TextListPlacedObjectType> _namesPlacedObjectList;
    /// <summary>
    /// ������ ��������  ��������� �� ������ ������
    /// </summary>
    [Header("������ ��������  ��������� �� ������ ������")]
    [SerializeField] private List<TextListPlacedObjectType> _descriptionsPlacedObjectList;
    /// <summary>
    /// ������ ������������  ��������� �� ������ ������
    /// </summary>
    [Header("������ ������������  ��������� �� ������ ������")]
    [SerializeField] private List<TextListPlacedObjectType> _detailsPlacedObjectList;
    /// <summary>
    /// ������ �������� ��������  ��������� �� ������ ������
    /// </summary>
     [Header("������ �������� ��������  ��������� �� ������ ������")]
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
        return placedObjectTooltipList[(int)languageIndex]; // �� ������� ����������� ����� ������ ����������� ���������
    }



    [ContextMenu("OpenGoogleSheet")]
    private void Open()
    {
        Application.OpenURL($"https://docs.google.com/spreadsheets/d/{_sheetId}/edit?gid={_pageIDDictionary[TablePage.Name]}#gid={_pageIDDictionary[TablePage.Name]}");
    }

    [ContextMenu("DownloadSheets")]
    private void DownloadSheets() // ������������� ������
    {
        // � �������� �������� ���� �������
        EditorCoroutineUtility.StartCoroutineOwnerless(DownloadRawCvsTable(_sheetId, _pageIDDictionary));// ���� ����� ��������� EditorCoroutine ��� �������-���������.EditorCoroutine ����������� �� ��� ���, ���� �� ���������� ��� �� ����� ������� � ������� StopCoroutine(EditorCoroutine).
    }
    //  https://docs.google.com/spreadsheets/d/1ic4wt3ADm_Qta03j3m8dS_gJjr2x0XbFLZ75f6TvaY4/edit?usp=sharing
    private IEnumerator DownloadRawCvsTable(string sheetId, Dictionary<TablePage, int> pageTableID)
    {
        foreach (TablePage page in Enum.GetValues(typeof(TablePage)))
        {
            string actualUrl = $"https://docs.google.com/spreadsheets/d/{sheetId}/export?format=csv&gid={pageTableID[page]}";
            using (UnityWebRequest request = UnityWebRequest.Get(actualUrl)) // using- ��������� ��������� �������� � ������� �������, � ������������� ������ ������(�� ���� ������ ��� ��������� ����� ��� �� �� ������������ ������)
            {
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError ||
                    request.result == UnityWebRequest.Result.DataProcessingError)
                {
                    Debug.LogError(request.error);
                }
                else
                {
                    // Debug.Log($"�������� �������� {page}");
                    // Debug.Log(request.downloadHandler.text);
                    yield return CompleteTables(request.downloadHandler.text, page);//(STOP) �������� ������ �������� 
                }
            }
        }               
        yield return null;
    }

    private IEnumerator CompleteTables(string rawCSVText, TablePage pages)//���������� ������
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
        yield return null;// yield return null - ��� �����, � ������� ���������� ������������������ � �������������� � ��������� �����.
    }

    private List<TextListPlacedObjectType> GetCompleteTable(string rawCSVText) //���������� �������  
    {
        // �������������� ������ ������� ����� ���������       
        List<TextListPlacedObjectType> textPlacedObjectList = new();

        string[,] gridDateArray = SheetProcessor.ProcessData(rawCSVText); // ������� ��������������� ����� � ��������� ������

        if (!LanguageIndexInTableCorrectOrder(gridDateArray)) return null; //������ null  ���� �� ���������       

        // ��������� ������       
        for (int rowIndex = START_ROW_INDEX; rowIndex < gridDateArray.GetLength(0); rowIndex++) //��������� ������ // GetLength(0)- ������ ����� ������ ����� ���������� �������  (���� string[4,5] - �� ������� 4 )
        {
            // �������� ��������� TextListPlacedObjectType � �������������� ��� ����
            TextListPlacedObjectType textPlacedObjectType = new();

            // ������� ��� ������������ ������� � ���� ������
            textPlacedObjectType.placedObjectType = SheetProcessor.ParseEnum<PlacedObjectType>(gridDateArray[rowIndex, PLACED_OBJECT_TYPE_COLUMN_INDEX]);// ����������� ������ �� ������ � Enum ����<T>
            textPlacedObjectType.placedObjectSubtype = gridDateArray[rowIndex, PLACED_OBJECT_SUBTYPE_COLUMN_INDEX];

            // ��������� �������
            List<string> textList = new();
            for (int columnIndex = START_COLUMN_INDEX; columnIndex < gridDateArray.GetLength(1); columnIndex++) // �������� ������ � ������ ������ // GetLength(1)- ������ ����� ������ ����� ���������� �������  (���� string[4,5] - �� ������� 5 )
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
    /// �������� - ������ ������ � �������, ���� � ���������� �������?
    /// </summary>
    ///  <returns>
    /// true - ���������� �������. false -������� �������. 
    ///  </returns>
    private bool LanguageIndexInTableCorrectOrder(string[,] gridDateArray)
    {
        // ��������� ������� ��� ������ ������. ��� ����� �������            
        for (int columnIndex = START_COLUMN_INDEX; columnIndex < gridDateArray.GetLength(1); columnIndex++) // �������� ������ � ������ // GetLength(1)- ������ ����� ������ ����� ���������� �������  (���� string[4,5] - �� ������� 5 )
        {
            LanguageIndex testlanguageIndex = SheetProcessor.ParseEnum<LanguageIndex>(gridDateArray[TABLE_HEADER_ROW_INDEX, columnIndex]);// ����������� ������ �� ������ � Enum ����<LanguageIndex>

            int �orrectIndex = columnIndex - START_COLUMN_INDEX; //������������ ������ ���������� � 3-�� ������� (��� ���� ����� ������ ���� ���� 0-� ������ ������� ��������)
            if ((int)testlanguageIndex != �orrectIndex) // ���� ������ ����� �� ��������� � �������� (0,1,2,3...)
            {
                Debug.LogError($"�� ��������� ������� ��� {testlanguageIndex} �� ����������� ���� �������");
                return false;
            }
        }
        return true;
    }


    /// <summary>
    /// ����� �� ������
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
    /// ��������� �������, ����������� ��������� ��� ������������ �������, �� ��������� �������
    /// </summary>
    private Dictionary<T, List<PlacedObjectTooltip>> GetPlacedObjectTooltipDictionary<T>() where T : Enum
    {
        //�������������� ������������ �������
        Dictionary<T, List<PlacedObjectTooltip>> placedObjectTypeTooltipDictionary = new();

        // �� ���� ������� ������������������ PlacedObject � �� ���������� ��������� ������� ��������� ����� �� ��� (��� ����� ������ �������).
        for (int indexPlacedObject = 0; indexPlacedObject < _namesPlacedObjectList.Count; indexPlacedObject++)
        {
            T placedObjectSubtype = default;
            PlacedObjectType placedObjectType = _namesPlacedObjectList[indexPlacedObject].placedObjectType;

            // ���� ������������� ��� T ���������� �� ���� ��� �������� [indexPlacedObject] �� ��������� � ���� �������
            if (placedObjectSubtype is BodyArmorType && placedObjectType != PlacedObjectType.BodyArmorType)          
                continue; //��������� � ���� �����
            if (placedObjectSubtype is HeadArmorType && placedObjectType != PlacedObjectType.HeadArmorType)
                continue; //��������� � ���� �����
            if (placedObjectSubtype is CombatDroneType && placedObjectType != PlacedObjectType.CombatDroneType)
                continue; //��������� � ���� �����
            if (placedObjectSubtype is GrappleType && placedObjectType != PlacedObjectType.GrappleType)
                continue; //��������� � ���� �����
            if (placedObjectSubtype is GrenadeType && placedObjectType != PlacedObjectType.GrenadeType)
                continue; //��������� � ���� �����
            if (placedObjectSubtype is HealItemType && placedObjectType != PlacedObjectType.HealItemType)
                continue; //��������� � ���� �����
            if (placedObjectSubtype is ShieldItemType && placedObjectType != PlacedObjectType.ShieldItemType)
                continue; //��������� � ���� �����
            if (placedObjectSubtype is ShootingWeaponType && placedObjectType != PlacedObjectType.ShootingWeaponType)
                continue; //��������� � ���� �����
            if (placedObjectSubtype is SpotterFireItemType && placedObjectType != PlacedObjectType.SpotterFireItemType)
                continue; //��������� � ���� �����
            if (placedObjectSubtype is SwordType && placedObjectType != PlacedObjectType.SwordType)
                continue; //��������� � ���� �����
            
            // ����� �������� �������� ��������
            placedObjectSubtype = SheetProcessor.ParseEnum<T>(_namesPlacedObjectList[indexPlacedObject].placedObjectSubtype);
           
            // �������������� ������ ������� ����� ���������
            List<PlacedObjectTooltip> placedObjectTooltipList = new List<PlacedObjectTooltip>();
            //��������� ������ ������ �� ������ ������, ��� ������� ���� ������������ ������� 
            for (int indexString = 0; indexString < _namesPlacedObjectList[indexPlacedObject].textList.Count; indexString++)
            {
                string name = _namesPlacedObjectList[indexPlacedObject].textList[indexString];
                string descriptions = _descriptionsPlacedObjectList[indexPlacedObject].textList[indexString];
                string details = _detailsPlacedObjectList[indexPlacedObject].textList[indexString];
                string sideEffects = _sideEffectsPlacedObjectList[indexPlacedObject].textList[indexString];

                PlacedObjectTooltip placedObjectTooltip = new PlacedObjectTooltip(name, descriptions, details, sideEffects);
                placedObjectTooltipList.Add(placedObjectTooltip);
            }
            //�������� ���� � ��������      
            placedObjectTypeTooltipDictionary[placedObjectSubtype] = placedObjectTooltipList;
        }
        return placedObjectTypeTooltipDictionary;
    }

    /// <summary>
    /// ����������� ������ � �������
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


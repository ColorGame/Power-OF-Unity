using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEngine.Networking;


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
    [System.Serializable]
    public struct TextListPlacedObjectType
    {
        public PlacedObjectType placedObjectType;
        public Enum placedObjectSubtype;
        public List<string> textList;

        public TextListPlacedObjectType(PlacedObjectType placedObjectType,Enum placedObjectSubtype, List<string> stringList) //�����������
        {
            this.placedObjectType = placedObjectType;
            this.placedObjectSubtype = placedObjectSubtype;
            this.textList = stringList;

            switch (placedObjectType)
            {
                case PlacedObjectType.BodyArmorType:
                    this.placedObjectSubtype = (BodyArmorType)placedObjectSubtype;
                    break;
            }
        }
    }

    /// <summary>
    /// ������ ������� � ������� ��� ����������� ���� (PlacedObjectType) ����������� ��������. 2� �������(������=1)
    /// </summary>
    private const int PLACED_OBJECT_TYPE_COLUMN_INDEX = 1;
    /// <summary>
    /// ������ ������� � ������� ��� ����������� ������� (PlacedObjectType) ����������� ��������. 3� �������(������=2)
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

    //������ � ��������� ������� ��������� ����� 
    [Tooltip("������ ���� ��������� �� ������ ������)")]
    [SerializeField] private List<TextListPlacedObjectType> _namesPlacedObjectList;    

    [Tooltip("������ ��������  ��������� �� ������ ������")]
    [SerializeField] private List<TextListPlacedObjectType> _descriptionsPlacedObjectList;   

    [Tooltip("������ ������������  ��������� �� ������ ������")]
    [SerializeField] private List<TextListPlacedObjectType> _detailsPlacedObjectList;

    [Tooltip("������ �������� ��������  ��������� �� ������ ������")]
    [SerializeField] private List<TextListPlacedObjectType> _sideEffectsPlacedObjectList;

    // ��������� �������. 1���� - ��� ��� ������������ ������� 1�������� -���������� �������.
    // 2����(����������� �������) - ��� ��� ������(�������� Pistol_1A_Base) 2�������� - ������ ����������� ��������� �� ������ ������
    private Dictionary<PlacedObjectType, Dictionary<Enum, List<PlacedObjectTooltip>>> _placedObjecTypeTooltipDict = null;

    public PlacedObjectTooltip GetTooltipPlacedObject(PlacedObjectTypeSO placedObjectTypeSO, Enum enumPlacedObject, LanguageIndex languageIndex = LanguageIndex.Russian)
    {
        BuildLookup();

        List<PlacedObjectTooltip> placedObjectTooltipList = new();

        switch (placedObjectTypeSO)
        {
            case BodyArmorTypeSO:
                placedObjectTooltipList = _placedObjecTypeTooltipDict[PlacedObjectType.BodyArmorType][enumPlacedObject];
                break;
            case HeadArmorTypeSO:
                placedObjectTooltipList = _placedObjecTypeTooltipDict[PlacedObjectType.HeadArmorType][enumPlacedObject];
                break;
            case CombatDroneTypeSO:
                placedObjectTooltipList = _placedObjecTypeTooltipDict[PlacedObjectType.CombatDroneType][enumPlacedObject];
                break;
            case GrappleTypeSO:
                placedObjectTooltipList = _placedObjecTypeTooltipDict[PlacedObjectType.GrappleType][enumPlacedObject];
                break;
            case GrenadeTypeSO:
                placedObjectTooltipList = _placedObjecTypeTooltipDict[PlacedObjectType.GrenadeType][enumPlacedObject];
                break;
            case HealItemTypeSO:
                placedObjectTooltipList = _placedObjecTypeTooltipDict[PlacedObjectType.HealItemType][enumPlacedObject];
                break;
            case ShieldItemTypeSO:
                placedObjectTooltipList = _placedObjecTypeTooltipDict[PlacedObjectType.ShieldItemType][enumPlacedObject];
                break;
            case ShootingWeaponTypeSO:
                placedObjectTooltipList = _placedObjecTypeTooltipDict[PlacedObjectType.ShootingWeaponType][enumPlacedObject];
                break;
            case SpotterFireItemTypeSO:
                placedObjectTooltipList = _placedObjecTypeTooltipDict[PlacedObjectType.SpotterFireItemType][enumPlacedObject];
                break;
            case SwordTypeSO:
                placedObjectTooltipList = _placedObjecTypeTooltipDict[PlacedObjectType.SwordType][enumPlacedObject];
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
        BuildLookup();
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
            
            // �� ���� ������� ������ (���������� ��������� ������������ �������)
            switch (textPlacedObjectType.placedObjectType)
            {
                case PlacedObjectType.BodyArmorType:
                    textPlacedObjectType.placedObjectSubtype = SheetProcessor.ParseEnum<BodyArmorType>(gridDateArray[rowIndex, PLACED_OBJECT_SUBTYPE_COLUMN_INDEX]);
                    break;
                case PlacedObjectType.HeadArmorType:
                    textPlacedObjectType.placedObjectSubtype = SheetProcessor.ParseEnum<HeadArmorType>(gridDateArray[rowIndex, PLACED_OBJECT_SUBTYPE_COLUMN_INDEX]);
                    break;
                case PlacedObjectType.CombatDroneType:
                     textPlacedObjectType.placedObjectSubtype = SheetProcessor.ParseEnum<CombatDroneType>(gridDateArray[rowIndex, PLACED_OBJECT_SUBTYPE_COLUMN_INDEX]);
                    break;
                case PlacedObjectType.GrappleType:
                     textPlacedObjectType.placedObjectSubtype = SheetProcessor.ParseEnum<GrappleType>(gridDateArray[rowIndex, PLACED_OBJECT_SUBTYPE_COLUMN_INDEX]);
                    break;
                case PlacedObjectType.GrenadeType:
                     textPlacedObjectType.placedObjectSubtype = SheetProcessor.ParseEnum<GrenadeType>(gridDateArray[rowIndex, PLACED_OBJECT_SUBTYPE_COLUMN_INDEX]);
                    break;
                case PlacedObjectType.HealItemType:
                     textPlacedObjectType.placedObjectSubtype = SheetProcessor.ParseEnum<HealItemType>(gridDateArray[rowIndex, PLACED_OBJECT_SUBTYPE_COLUMN_INDEX]);
                    break;
                case PlacedObjectType.ShieldItemType:
                     textPlacedObjectType.placedObjectSubtype = SheetProcessor.ParseEnum<ShieldItemType>(gridDateArray[rowIndex, PLACED_OBJECT_SUBTYPE_COLUMN_INDEX]);
                    break;
                case PlacedObjectType.ShootingWeaponType:
                     textPlacedObjectType.placedObjectSubtype = SheetProcessor.ParseEnum<ShootingWeaponType>(gridDateArray[rowIndex, PLACED_OBJECT_SUBTYPE_COLUMN_INDEX]);
                    break;
                case PlacedObjectType.SpotterFireItemType:
                     textPlacedObjectType.placedObjectSubtype = SheetProcessor.ParseEnum<SpotterFireItemType>(gridDateArray[rowIndex, PLACED_OBJECT_SUBTYPE_COLUMN_INDEX]);
                    break;
                case PlacedObjectType.SwordType:
                     textPlacedObjectType.placedObjectSubtype = SheetProcessor.ParseEnum<SwordType>(gridDateArray[rowIndex, PLACED_OBJECT_SUBTYPE_COLUMN_INDEX]);
                    break;
            }

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
    private void BuildLookup()
    {

        if (_placedObjecTypeTooltipDict != null) return; // ������� ���� Dictionary ��� ��������
        _placedObjecTypeTooltipDict = GetPlacedObjectTooltipDictionary();
    }
    
    /// <summary>
    /// ��������� �������, ����������� ��������� ��� ������������ �������, �� ��������� �������
    /// </summary>
    private Dictionary<PlacedObjectType, Dictionary<Enum, List<PlacedObjectTooltip>>> GetPlacedObjectTooltipDictionary()
    {
        //�������������� ������������ �������
        Dictionary<PlacedObjectType, Dictionary<Enum, List<PlacedObjectTooltip>>> placedObjectTypeTooltipDictionary = new();
        //�������������� ���������  �������
        Dictionary<Enum, List<PlacedObjectTooltip>> nestedDictionary = new();

        // �� ���� ������� ������������������ PlacedObject � �� ���������� ��������� ������� ��������� ����� �� ��� (��� ����� ������ �������).
        for (int indexPlacedObject = 0; indexPlacedObject < _namesPlacedObjectList.Count; indexPlacedObject++)
        {
            PlacedObjectType placedObjectType = _namesPlacedObjectList[indexPlacedObject].placedObjectType;
            Enum placedObjectSubtype = _namesPlacedObjectList[indexPlacedObject].placedObjectSubtype;
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
            //�������� ���� � �������� ������� ����������  ������� � ����� � ���������
            nestedDictionary[placedObjectSubtype] = placedObjectTooltipList;

            placedObjectTypeTooltipDictionary[placedObjectType] = nestedDictionary;
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



/*
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
    /// ����������� ����� �� ������ ������, ��� ������� ���� ������������ ������� 
    /// </summary>
    [System.Serializable]
    public struct TextListPlacedObjectType
    {
        public PlacedObjectType placedObjectType;
        public List<string> textList;

        public TextListPlacedObjectType(PlacedObjectType placedObjectType, List<string> textList) //�����������
        {
            this.placedObjectType = placedObjectType;
            this.textList = textList;
        }
    }



    /// <summary>
    /// ������ ������� � ������� ��� ����������� ����������� ������� - <see cref="PlacedObjectType"/>
    /// </summary>
    private const int PLACED_OBJECT_SUBTYPE_COLUMN_INDEX = 1;

    /// <summary>
    /// ��������� ������ ������, � ���� �������, � ������� �� ����� ��������� ��� ����. ����� ���������� �� 2� ������, 1-��� ����� �������
    /// </summary>
    private const int START_ROW_INDEX = 1;

    /// <summary>
    /// ��������� ������ �������, � ���� �������, � ������� �� ����� ��������� ��� ����. ����� ���������� � 3-�� ������ (������ =2)  ��� ��������� ������  
    /// </summary>
    private const int START_COLUMN_INDEX = 2;

    /// <summary>
    /// ������ ������ ��������� �������. 1-� ������ (������=0)
    /// </summary>
    private const int TABLE_HEADER_ROW_INDEX = 0;

    /// <summary>
    /// �������� ����� ���� �������
    /// </summary>
    private enum TablePage
    {
        NameBodyArmor,
        Descriptions,
        Details,
        SideEffects
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
    private readonly Dictionary<TablePage, int> _pageIDDictionary = new() { { TablePage.NameBodyArmor, 1901640271 }, { TablePage.Descriptions, 563410214 }, { TablePage.Details, 350745494 }, { TablePage.SideEffects, 2116600352 } };

    [SerializeField] private LanguageIndex _languageIndex;

    [Tooltip("������ ���� ��������� �� ������ ������)")]
    [SerializeField] private List<TextListPlacedObjectType> _namesPlacedObjectList;

    [Tooltip("������ ��������  ��������� �� ������ ������")]
    [SerializeField] private List<TextListPlacedObjectType> _descriptionsPlacedObjectList;

    [Tooltip("������ ������������  ��������� �� ������ ������")]
    [SerializeField] private List<TextListPlacedObjectType> _detailsPlacedObjectList;

    [Tooltip("������ �������� ��������  ��������� �� ������ ������")]
    [SerializeField] private List<TextListPlacedObjectType> _sideEffectsPlacedObjectList;

    /// <summary>
    /// ������� (���� - ��� ������������ �������; �������� - ������ ����������� ��������� �� ������ ������)
    /// </summary>
    private Dictionary<PlacedObjectType, List<PlacedObjectTooltip>> _placedObjectTooltipDictionary = null;

    private Dictionary<BodyArmorType, List<PlacedObjectTooltip>> _bodyArmorTypeTooltipDict = null;
    private Dictionary<CombatDroneType, List<PlacedObjectTooltip>> _swordTypeTooltipDict = null;
    private Dictionary<GrappleType, List<PlacedObjectTooltip>> _grappleTypeTooltipDict = null;
    private Dictionary<GrenadeType, List<PlacedObjectTooltip>> _grenadeTypeTooltipDict = null;
    private Dictionary<HeadArmorType, List<PlacedObjectTooltip>> _headArmorTypeTooltipDict = null;
    private Dictionary<HealItemType, List<PlacedObjectTooltip>> _healItemTypeTooltipDict = null;
    private Dictionary<ShieldItemType, List<PlacedObjectTooltip>> _shieldItemTypeTooltipDict = null;
    private Dictionary<ShootingWeaponType, List<PlacedObjectTooltip>> _shootingWeaponTypeTooltipDict = null;
    private Dictionary<SpotterFireItemType, List<PlacedObjectTooltip>> _spotterFireItemTypeTooltipDict = null;
    private Dictionary<SwordType, List<PlacedObjectTooltip>> _combatDroneTypeTooltipDict = null;



    public PlacedObjectTooltip GetTooltipPlacedObject(PlacedObjectTypeSO placedObjectTypeSO, Enum enumPlacedObject)
    {
        BuildLookup();

        List<PlacedObjectTooltip> placedObjectTooltipList;

        switch (placedObjectTypeSO)
        {
            default:
            case BodyArmorTypeSO:
                placedObjectTooltipList = _bodyArmorTypeTooltipDict[(BodyArmorType)enumPlacedObject]; // ������� ������ ����������� ��������� �� ������ ������ ��� ����������� PlacedObjectType
                break;

            case GrappleTypeSO:
                placedObjectTooltipList = _grappleTypeTooltipDict[(GrappleType)enumPlacedObject]; // ������� ������ ����������� ��������� �� ������ ������ ��� ����������� PlacedObjectType
                break;

        }
        return placedObjectTooltipList[(int)_languageIndex]; // �� ������� ����������� ����� ������ ����������� ���������
    }

    public PlacedObjectTooltip GetTooltipPlacedObject(PlacedObjectType placedObjectType)
    {
        BuildLookup();
        List<PlacedObjectTooltip> placedObjectTooltipList = _placedObjectTooltipDictionary[placedObjectType]; // ������� ������ ����������� ��������� �� ������ ������ ��� ����������� PlacedObjectType
        return placedObjectTooltipList[(int)_languageIndex]; // �� ������� ����������� ����� ������ ����������� ���������

    }

    public string GetNamePlacedObject(PlacedObjectType placedObjectType)
    {
        BuildLookup();
        List<PlacedObjectTooltip> placedObjectTooltipList = _placedObjectTooltipDictionary[placedObjectType]; // ������� ������ ����������� ��������� �� ������ ������ ��� ����������� PlacedObjectType              
        return placedObjectTooltipList[(int)_languageIndex].name.ToString(); // �� ������� ����������� ����� ������ ������
    }



    [ContextMenu("OpenGoogleSheet")]
    private void Open()
    {
        Application.OpenURL($"https://docs.google.com/spreadsheets/d/{_sheetId}/edit#gid={_pageIDDictionary[TablePage.NameBodyArmor]}");
    }

    [ContextMenu("DownloadSheets")]
    private void DownloadSheets() // ������������� ������
    {
        // � �������� �������� ���� �������
        EditorCoroutineUtility.StartCoroutineOwnerless(DownloadRawCvsTable(_sheetId, _pageIDDictionary));// ���� ����� ��������� EditorCoroutine ��� �������-���������.EditorCoroutine ����������� �� ��� ���, ���� �� ���������� ��� �� ����� ������� � ������� StopCoroutine(EditorCoroutine).
    }

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
            case TablePage.NameBodyArmor:
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
        yield return null;// yield return null - ��� �����, � ������� ���������� ������������������ � �������������� � ��������� �����.
    }

    private List<TextListPlacedObjectType> GetCompleteTable(string rawCSVText) //���������� �������  
    {
        // ������������� ���� ������� ����� ���������       
        List<TextListPlacedObjectType> textPlacedObjectList = new List<TextListPlacedObjectType>();

        string[,] gridDateArray = SheetProcessor.ProcessData(rawCSVText); // ������� ��������������� ����� � ��������� ������

        if (!LanguageIndexInTableCorrectOrder(gridDateArray)) return null; //������ null  ���� �� ���������              

        // ��������� ������       
        for (int rowIndex = START_ROW_INDEX; rowIndex < gridDateArray.GetLength(0); rowIndex++) //��������� ����� // GetLength(0)- ������ ����� ������ ����� ���������� �������  (���� string[4,5] - �� ������� 4 )
        {



            // ������� ��� ������������ ������� � ���� ������
            PlacedObjectType placedObjectType = SheetProcessor.ParseEnum<PlacedObjectType>(gridDateArray[rowIndex, PLACED_OBJECT_SUBTYPE_COLUMN_INDEX]);// ����������� ������ �� ������ � Enum ����<PlacedObjectType>

            //���� ������ ������,  � ���� ��� �� - ������� � ��������� ��������� � ������ � ���� ��� ������ ������� �� 
            if (textPlacedObjectList.Count == 0 || placedObjectType != textPlacedObjectList[textPlacedObjectList.Count - 1].placedObjectType)
            {
                // �������� ��������� TextListPlacedObjectType � �������������� ��� ����
                TextListPlacedObjectType textPlacedObjectType = new TextListPlacedObjectType(placedObjectType, new List<string>());

                textPlacedObjectList.Add(textPlacedObjectType); // ������� � ������ ��������� ���������

                // ��������� �������               
                for (int columnIndex = START_COLUMN_INDEX; columnIndex < gridDateArray.GetLength(1); columnIndex++) // �������� ������ � ������ ������ // GetLength(1)- ������ ����� ������ ����� ���������� �������  (���� string[4,5] - �� ������� 5 )
                {
                    string text = gridDateArray[rowIndex, columnIndex];

                    textPlacedObjectType.textList.Add(text);
                }
            }
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
        for (int columnIndex = START_COLUMN_INDEX; columnIndex < gridDateArray.GetLength(1); columnIndex++) // �������� ������ � ������ ������ // GetLength(1)- ������ ����� ������ ����� ���������� �������  (���� string[4,5] - �� ������� 5 )
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
    private void BuildLookup()
    {
        if (_placedObjectTooltipDictionary != null) return; // ������� ���� Dictionary ��� ��������
        _placedObjectTooltipDictionary = GetPlacedObjectTooltipDictionary();
    }

    /// <summary>
    /// �������� ������� ����������� ��������� ��� ������������ �������
    /// </summary>
    private Dictionary<PlacedObjectType, List<PlacedObjectTooltip>> GetPlacedObjectTooltipDictionary()
    {
        //�������������� ������������ �������
        Dictionary<PlacedObjectType, List<PlacedObjectTooltip>> placedObjectTypeTooltipDictionary = new();

        // �� ���� ������� ������������������ PlacedObjectType � ������ ��������� ������� ��������� ����� �� ��� (��� ����� ������ �������).
        for (int indexPlacedObject = 0; indexPlacedObject < _namesPlacedObjectList.Count; indexPlacedObject++)
        {
            PlacedObjectType placedObjectType = _namesPlacedObjectList[indexPlacedObject].placedObjectType;
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
            placedObjectTypeTooltipDictionary[placedObjectType] = placedObjectTooltipList;
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
*/
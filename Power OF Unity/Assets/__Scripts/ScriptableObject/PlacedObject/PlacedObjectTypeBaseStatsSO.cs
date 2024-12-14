using System;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEngine.Networking;


[CreateAssetMenu(fileName = "PlacedObjectTypeBaseStatsSO", menuName = "ScriptableObjects/PlacedObjectTypeBaseStats")]

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
    public struct TextPlacedObjectType
    {
        public PlacedObjectType placedObjectType;
        public List<string> stringList;

        public TextPlacedObjectType(PlacedObjectType placedObjectType, List<string> stringList) //�����������
        {
            this.placedObjectType = placedObjectType;
            this.stringList = stringList;
        }
    }

    /// <summary>
    /// ������ ������� � ������� ��� ����������� ����������� ������� - <see cref="PlacedObjectType"/>
    /// </summary>
    private const int PLACED_OBJECT_TYPE_COLUMN_INDEX = 1;

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
        Names,
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
    private readonly Dictionary<TablePage, int> _pageIDDictionary = new() { { TablePage.Names, 1901640271 }, { TablePage.Descriptions, 563410214 }, { TablePage.Details, 350745494 }, { TablePage.SideEffects, 2116600352 } };

    [SerializeField] private LanguageIndex _languageIndex;
        
    [Tooltip("������ ���� ��������� �� ������ ������)")]
    [SerializeField] private List<TextPlacedObjectType> _namesPlacedObjectList;

    [Tooltip("������ ��������  ��������� �� ������ ������")]
    [SerializeField] private List<TextPlacedObjectType> _descriptionsPlacedObjectList;

    [Tooltip("������ ������������  ��������� �� ������ ������")]
    [SerializeField] private List<TextPlacedObjectType> _detailsPlacedObjectList;

    [Tooltip("������ �������� ��������  ��������� �� ������ ������")]
    [SerializeField] private List<TextPlacedObjectType> _sideEffectsPlacedObjectList;

    /// <summary>
    /// ������� (���� - ��� ������������ �������; �������� - ������ ����������� ��������� �� ������ ������)
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
                List<PlacedObjectTooltip> placedObjectTooltipList = _grappleTypeTooltipDict[(GrappleType)enumPlacedObject]; // ������� ������ ����������� ��������� �� ������ ������ ��� ����������� PlacedObjectType
                return placedObjectTooltipList[(int)_languageIndex]; // �� ������� ����������� ����� ������ ����������� ���������
               

        }
       
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
        Application.OpenURL($"https://docs.google.com/spreadsheets/d/{_sheetId}/edit#gid={_pageIDDictionary[TablePage.Names]}");
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
        yield return null;// yield return null - ��� �����, � ������� ���������� ������������������ � �������������� � ��������� �����.
    }

    private List<TextPlacedObjectType> GetCompleteTable(string rawCSVText) //���������� �������  
    {
        // ������������� ���� ������� ����� ���������       
        List<TextPlacedObjectType> textPlacedObjectList = new List<TextPlacedObjectType>();

        string[,] gridDateArray = SheetProcessor.ProcessData(rawCSVText); // ������� ��������������� ����� � ��������� ������

        if (!LanguageIndexInTableCorrectOrder(gridDateArray)) return null; //������ null  ���� �� ���������              

        // ��������� ������       
        for (int rowIndex = START_ROW_INDEX; rowIndex < gridDateArray.GetLength(0); rowIndex++) //��������� ����� // GetLength(0)- ������ ����� ������ ����� ���������� �������  (���� string[4,5] - �� ������� 4 )
        {
            // ������� ��� ������������ ������� � ���� ������
            PlacedObjectType placedObjectType = SheetProcessor.ParseEnum<PlacedObjectType>(gridDateArray[rowIndex, PLACED_OBJECT_TYPE_COLUMN_INDEX]);// ����������� ������ �� ������ � Enum ����<PlacedObjectType>

            //���� ������ ������,  � ���� ��� �� - ������� � ��������� ��������� � ������ � ���� ��� ������ ������� �� 
            if (textPlacedObjectList.Count == 0 || placedObjectType != textPlacedObjectList[textPlacedObjectList.Count - 1].placedObjectType)
            {
                // �������� ��������� TextPlacedObjectType � �������������� ��� ����
                TextPlacedObjectType textPlacedObjectType = new TextPlacedObjectType(placedObjectType, new List<string>());

                textPlacedObjectList.Add(textPlacedObjectType); // ������� � ������ ��������� ���������

                // ��������� �������               
                for (int columnIndex = START_COLUMN_INDEX; columnIndex < gridDateArray.GetLength(1); columnIndex++) // �������� ������ � ������ ������ // GetLength(1)- ������ ����� ������ ����� ���������� �������  (���� string[4,5] - �� ������� 5 )
                {
                    string text = gridDateArray[rowIndex, columnIndex];

                    textPlacedObjectType.stringList.Add(text);
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
       if(_placedObjectTooltipDictionary!= null) return; // ������� ���� Dictionary ��� ��������
        _placedObjectTooltipDictionary = GetPlacedObjectTooltipDictionary();
    }

    /// <summary>
    /// �������� ������� ����������� ��������� ��� ������������ �������
    /// </summary>
    private Dictionary<PlacedObjectType, List<PlacedObjectTooltip>> GetPlacedObjectTooltipDictionary()
    {
        //�������������� ������������ �������
        Dictionary<PlacedObjectType, List<PlacedObjectTooltip>> placedObjectTooltipDictionary = new ();

        // �� ���� ������� ������������������ PlacedObjectType � ������ ��������� ������� ��������� ����� �� ��� (��� ����� ������ �������).
        for (int indexPlacedObject = 0; indexPlacedObject < _namesPlacedObjectList.Count; indexPlacedObject++)
        {
            PlacedObjectType placedObjectType = _namesPlacedObjectList[indexPlacedObject].placedObjectType;
            // �������������� ������ ������� ����� ���������
            List<PlacedObjectTooltip> placedObjectTooltipList = new List<PlacedObjectTooltip>();
            //��������� ������ ������ �� ������ ������, ��� ������� ���� ������������ ������� 
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
    /// ����������� ������ � �������
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

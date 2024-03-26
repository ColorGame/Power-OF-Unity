
using System;
using System.Collections.Generic;
using UnityEngine;
using static PlacedObjectTypeBaseStatsSO;

[CreateAssetMenu(fileName = "GameAssets", menuName = "ScriptableObjects/GameAssets")]
public class GameAssets : ScriptableObject //������� ������
{

    private static GameAssets _instance; // �.�. �� ������������ � ������ �� �� ���� ��������� ��� � �����

    // ����� ���-�� ������� ������ � ����� �������� ��� �������� ��� (get) � ��� ������������� ������� ��������� ������� ������� ��� ������ ������������
    public static GameAssets Instance
    {
        get //(�������� �������� get) ����� ������������ ������� � �� ����� ����������
        {
            if (_instance == null) // ���� ��������� ������� �� �������� ���
            {
                _instance = Resources.Load<GameAssets>(typeof(GameAssets).Name); //� ���� ���������� - (_instance) ��������� ������ ������������ ����, ���������� �� ������ path(����) � ����� Resources(��� ����� � ������ � ����� ScriptableObjects  � � ����� Prefab).                                                                     
            }
            return _instance; //������ ���� ���������� (���� ��� �� ������� �� ������ ������������)
        }
    }

    /// <summary>
    ///���������: ��� ���������� ����� � ��� ������
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

    public Transform grenadeProjectilePrefab; // ������ �������
    public Transform bulletProjectilePrefab; // ������ ����
    public Transform placedObjectTypeButtonPrefab; // ������ ������ ������
    public Transform levelGridSystemVisualSinglePrefab; // ������ ������������ ���� �����
    public Transform inventoryGridSystemVisualSinglePrefab; // ������ ������������ ���� ����� ���������
    [Header("������� FX-������� ������")]
    public Transform grenadeExplosionFXPrefab; // ������ �������� ������ ������� //�������� ��������� ������� � TRAIL ���������������(Destroy) ����� ������������
    public Transform grenadeSmokeFXPrefab; // ������ ���� �� ������� // ���������� ��� ����� ������ ������������� � ����
    public Transform comboPartnerFXPrefab; // ������  �������� ��������������
    public Transform electricityWhiteFXPrefab; // ������ ���������������� ������
    public Transform bulletHitFXPrefab; // ������ ������ �� ���������� ����
    public Transform healFXPrefab; // ������ �������� ���������
    public Transform spotterFireFXPrefab; // ������ �������� ����� ��� ����������

    [Header("��������� ��� ���������� ����� � ��� ������")]
    [SerializeField] private List<EnemyTypePrefabUnit> _enemyTypePrefabUnitList;

    /// <summary>
    /// ������� (���� - ��� ���������� �����; �������� - ��� ������)
    /// </summary>
    private Dictionary<EnemyUnitType, Transform> _enemyTypePrefabUnitDictionary = null;

    /// <summary>
    /// ��������� ������ ����� ������� ������� �����
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
        else{Debug.Log("������� -- Enemy Type Prefab Unit List -- ��� ���������!!!");}
    }

    public Transform GetEnemyPrefab(EnemyUnitType enemyType)
    {
        BuildLookup();
        return _enemyTypePrefabUnitDictionary[enemyType];
    }

    /// <summary>
    /// ����� �� ������
    /// </summary>    
    private void BuildLookup()
    {
        if (_enemyTypePrefabUnitDictionary != null) return; // ������� ���� Dictionary ��� ��������
        _enemyTypePrefabUnitDictionary = GetEnemyTypePrefabUnitDictionary();
    }

    private Dictionary<EnemyUnitType, Transform> GetEnemyTypePrefabUnitDictionary()
    {
        //�������������� ������������ �������
        Dictionary<EnemyUnitType, Transform> enemyTypePrefabUnitDictionary = new Dictionary<EnemyUnitType, Transform>();

        for (int index = 0; index < _enemyTypePrefabUnitList.Count; index++)
        {
            EnemyTypePrefabUnit enemyTypePrefabUnit = _enemyTypePrefabUnitList[index];
            //�������� �������
            enemyTypePrefabUnitDictionary[enemyTypePrefabUnit.enemyType] = enemyTypePrefabUnit.transformPrefab;
        }
        return enemyTypePrefabUnitDictionary;
    }



}
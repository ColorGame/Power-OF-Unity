using UnityEngine;

[CreateAssetMenu(fileName = "GameAssetsSO", menuName = "ScriptableObjects/Date/GameAssetsSO")]
public class GameAssetsSO : ScriptableObject //������� ������
{

    private static GameAssetsSO _instance; // �.�. �� ������������ � ������ �� �� ���� ��������� ��� � �����

    // ����� ���-�� ������� ������ � ����� �������� ��� �������� ��� (get) � ��� ������������� ������� ��������� ������� ������� ��� ������ ������������
    public static GameAssetsSO Instance
    {
        get //(�������� �������� get) ����� ������������ ������� � �� ����� ����������
        {
            if (_instance == null) // ���� ��������� ������� �� �������� ���
            {
                _instance = Resources.Load<GameAssetsSO>(typeof(GameAssetsSO).Name); //� ���� ���������� - (_instance) ��������� ������ ������������ ����, ���������� �� ������ path(����) � ����� Resources(��� ����� � ������ � ����� ScriptableObjects  � � ����� Prefab).                                                                     
            }
            return _instance; //������ ���� ���������� (���� ��� �� ������� �� ������ ������������)
        }
    }
     
    public Transform grenadeProjectilePrefab; // ������ �������
    public Transform bulletProjectilePrefab; // ������ ����
    public Transform LevelGridVisualSinglePrefab; // ������ ������������ ���� �����
    public EquipmentGridVisualSingle EquipmentGridInVisualSingleScreenSpacePrefab; // ������ ������������ ���� ����� ���������
    public EquipmentGridVisualSingle EquipmentGridInVisualSingleWorldSpacePrefab; // ������ ������������ ���� ����� ���������

    [Header("������ - ��� ������������� �������� � UI")]
    public ActionButtonUI actionButtonUI; // ������ ������ ������ ��������
    public PlacedObjectSelectButtonUI placedObjectSelectButton; // ������ ������ �������  ���� PlacedObject
    public PlacedObjectBuySellCountButtonUI placedObjectBuySellCountButton; //������ ��������� ���������� ������� � ������� ������� ���� PlacedObject
    public UnitEnemySelectAtLevelButtonUI unitEnemySelectAtLevelButton; // ������ - ������ ���������� ����� �� ������� ������.
    public UnitSelectAtLevelButtonUI unitFriendSelectAtLevelButton; // ������ - ������ �������������� ����� �� ������� ������.
    public UnitSelectAtEquipmentButtonUI unitSelectAtEquipmentButton; // ������ - ������  �����, ��� ��������� ����������
    public UnitSelectAtManagementButtonUI unitSelectAtManagementButton; // ������ - ������  �����, � ���� ����������� ������

    [Header("������� FX-������� ������")]
    public Transform grenadeExplosionFXPrefab; // ������ �������� ������ ������� //�������� ��������� ������� � TRAIL ���������������(Destroy) ����� ������������
    public Transform grenadeSmokeFXPrefab; // ������ ���� �� ������� // ���������� ��� ����� ������ ������������� � ����
    public Transform comboPartnerFXPrefab; // ������  �������� ��������������
    public Transform electricityWhiteFXPrefab; // ������ ���������������� ������
    public Transform bulletHitFXPrefab; // ������ ������ �� ���������� ����
    public Transform healFXPrefab; // ������ �������� ���������
    public Transform spotterFireFXPrefab; // ������ �������� ����� ��� ����������
    
      



}
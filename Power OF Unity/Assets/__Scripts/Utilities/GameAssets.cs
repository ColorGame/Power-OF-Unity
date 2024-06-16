using UnityEngine;

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
     
    public Transform grenadeProjectilePrefab; // ������ �������
    public Transform bulletProjectilePrefab; // ������ ����
    public Transform placedObjectTypeButtonPrefab; // ������ ������ ������
    public Transform levelGridSystemVisualSinglePrefab; // ������ ������������ ���� �����
    public Transform inventoryGridSystemVisualSinglePrefab; // ������ ������������ ���� ����� ���������
    public Transform actionButtonUIPrefab; // ������ ������ ������ ��������
    [Header("������� FX-������� ������")]
    public Transform grenadeExplosionFXPrefab; // ������ �������� ������ ������� //�������� ��������� ������� � TRAIL ���������������(Destroy) ����� ������������
    public Transform grenadeSmokeFXPrefab; // ������ ���� �� ������� // ���������� ��� ����� ������ ������������� � ����
    public Transform comboPartnerFXPrefab; // ������  �������� ��������������
    public Transform electricityWhiteFXPrefab; // ������ ���������������� ������
    public Transform bulletHitFXPrefab; // ������ ������ �� ���������� ����
    public Transform healFXPrefab; // ������ �������� ���������
    public Transform spotterFireFXPrefab; // ������ �������� ����� ��� ����������
    
      



}
using UnityEngine;
// ������� � ����� (����� ������ ��� �������������� ����������)
[CreateAssetMenu(fileName = "PlacedObjectGeneralListForAutoCompletionSO", menuName = "ScriptableObjects/Date/PlacedObjectGeneralListForAutoCompletionSO")]
public class PlacedObjectGeneralListForAutoCompletionSO : ScriptableObject
{
    private static PlacedObjectGeneralListForAutoCompletionSO _instance; // �.�. �� ������������ � ������ �� �� ���� ��������� ��� � �����

    // ����� ���-�� ������� ������ � ����� �������� ��� �������� ��� (get) � ��� ������������� ������� ��������� ������� ������� ��� ������ ������������
    public static PlacedObjectGeneralListForAutoCompletionSO Instance
    {
        get //(�������� �������� get) ����� ������������ ������� � �� ����� ����������
        {
            if (_instance == null) // ���� ��������� ������� �� �������� ���
            {
                _instance = Resources.Load<PlacedObjectGeneralListForAutoCompletionSO>(typeof(PlacedObjectGeneralListForAutoCompletionSO).Name); //� ���� ���������� - (_instance) ��������� ������ ������������ ����, ���������� �� ������ path(����) � ����� Resources(��� ����� � ������ � ����� ScriptableObjects  � � ����� Prefab).                                                                     
            }
            return _instance; //������ ���� ���������� (���� ��� �� ������� �� ������ ������������)
        }
    }


    [Header("BodyArmor")]
    public GameObject[] BodyArmor2DArray;

    [Header("HeadArmor")]
    public GameObject[] HeadArmor2DArray;


    [Header("Grapple")]
    public GameObject[] Grapple2DArray;
    [Space]
    public GameObject[] GrapplePrefab3DArray;

    [Header("Grenade")]
    public GameObject[] Grenade2DArray;
    [Space]
    public GameObject[] GrenadePrefab3DArray;

    [Header("HealItem")]
    public GameObject[] HealItem2DArray;
    [Space]
    public GameObject[] HealItemPrefab3DArray;

    [Header("ShieldItem")]
    public GameObject[] ShieldItem2DArray;
    [Space]
    public GameObject[] ShieldItemPrefab3DArray;

    [Header("CombatDrone")]
    public GameObject[] CombatDrone2DArray;
    [Space]
    public GameObject[] CombatDronePrefab3DArray;

    [Header("Shooting")]
    public GameObject[] Shooting2DArray;
    [Space]
    public GameObject[] ShootingPrefab3DArray;

    [Header("SpotterFireItem")]
    public GameObject[] SpotterFireItem2DArray;
    [Space]
    public GameObject[] SpotterFireItemPrefab3DArray;

    [Header("SpotterFireItem")]
    public GameObject[] Sword2DArray;
    [Space]
    public GameObject[] SwordPrefab3DArray;

    [Header("������ ���� SO ������ �����(Armor)")]
    public PlacedObjectTypeSO[] ArmorSOArray;

    [Header("������ ���� SO ������ ���������(Action)")]
    public PlacedObjectTypeSO[] ActionSOArray;
}


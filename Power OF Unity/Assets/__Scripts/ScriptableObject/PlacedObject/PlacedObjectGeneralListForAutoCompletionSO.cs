using UnityEngine;
// УДАЛИТЬ в билде (нужно только для автозаполнения инспектора)
[CreateAssetMenu(fileName = "PlacedObjectGeneralListForAutoCompletionSO", menuName = "ScriptableObjects/Date/PlacedObjectGeneralListForAutoCompletionSO")]
public class PlacedObjectGeneralListForAutoCompletionSO : ScriptableObject
{
    private static PlacedObjectGeneralListForAutoCompletionSO _instance; // т.к. мы обрабатываем в ручную то не надо создавать его в сцене

    // Когда кто-то получит доступ к этому свойству оно запустит код (get) и при необходимости создаст экземпляр ИГРОВЫХ АКТИВОВ или вернет существующую
    public static PlacedObjectGeneralListForAutoCompletionSO Instance
    {
        get //(расширим свойство get) Будем обрабатывать вручную а не через компилятор
        {
            if (_instance == null) // Если экземпляр нулевой то создадим его
            {
                _instance = Resources.Load<PlacedObjectGeneralListForAutoCompletionSO>(typeof(PlacedObjectGeneralListForAutoCompletionSO).Name); //В поле экземпляра - (_instance) установим ресурс запрошенного типа, хранящийся по адресу path(путь) в папке Resources(эту папку я создал в папке ScriptableObjects  и в папке Prefab).                                                                     
            }
            return _instance; //Вернем поле экземпляра (если оно не нулевое то вернем существующее)
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

    [Header("Список всех SO файлов БРОНИ(Armor)")]
    public PlacedObjectTypeSO[] ArmorSOArray;

    [Header("Список всех SO файлов ДЕЙСТВИЕМ(Action)")]
    public PlacedObjectTypeSO[] ActionSOArray;
}


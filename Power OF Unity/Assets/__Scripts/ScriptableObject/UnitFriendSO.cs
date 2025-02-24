using UnityEngine;

[CreateAssetMenu(fileName = "UnitFriend", menuName = "ScriptableObjects/UnitFriend")]
public class UnitFriendSO : UnitTypeSO
{
    [Header("���� ��� �������� �����")]

    [SerializeField] private Transform _unitAvatarPortfolioViewPrefab;
    [SerializeField] private UnitBigExoskeletonView _unitBigExoskeletonViewPrefab;
    [SerializeField] private UnitMilitaryAndCyberSoldierView _unitMilitaryAndCyberSoldierViewPrefab;
    [SerializeField] private UnitSpaceSoldierView _unitSpaceSoldierViewPrefab;

    [Header("��������� ����� ��� ����� ")]
    [SerializeField] private uint _priceHiring;

    public Transform GetUnitAvatarPortfolioViewPrefab() { return _unitAvatarPortfolioViewPrefab; }

    /// <summary>
    /// ������� ������ ����� � ����������� �� ���� ����� ������� �� ����������
    /// </summary>
    /// <remarks>���� �������� null �� �������� ��������� ������ ��� �����</remarks>
    public UnitWithChangeHeadArmorView GetUnitViewPrefab(BodyArmorTypeSO bodyArmorTypeSO)
    {
        if (bodyArmorTypeSO == null)
        {
            return _unitSpaceSoldierViewPrefab;
        }

        switch (bodyArmorTypeSO.GetBodyArmorType())
        {
            default:
                return _unitSpaceSoldierViewPrefab;

            case BodyArmorType.BodyArmor_1A_Military:
            case BodyArmorType.BodyArmor_1B_MilitaryMod:
            case BodyArmorType.BodyArmor_3A_Cyber:
            case BodyArmorType.BodyArmor_3B_CyberMod:
                return _unitMilitaryAndCyberSoldierViewPrefab;

            case BodyArmorType.BodyArmor_2A_Space:
            case BodyArmorType.BodyArmor_2B_SpaceMod:
                return _unitSpaceSoldierViewPrefab;

            case BodyArmorType.BodyArmor_4A_BigExoskeleton:
                return _unitBigExoskeletonViewPrefab;
        }
    }
    public uint GetPriceHiring() { return _priceHiring; }

    [ContextMenu("��������������")]
    protected override void AutoCompletion()
    {
        SearchNameAndCompletion(name);
        _priceHiring = 200;
        base.AutoCompletion();
    }


    protected void SearchNameAndCompletion(string nameFail)
    {
        _unitCorePrefab = PlacedObjectGeneralListForAutoCompletionSO.Instance.UnitCoreFriend;

        GameObject[] gameObjectArray = PlacedObjectGeneralListForAutoCompletionSO.Instance.UnitPrefabArray;

        foreach (GameObject gameObject in gameObjectArray)
        {
            if (gameObject.name.Contains(nameFail))
            {
                if (gameObject.name.Contains("Avatar"))
                    _unitAvatarPortfolioViewPrefab = gameObject.transform;

                if (gameObject.name.Contains("BigExoskeleton"))
                    if (gameObject.TryGetComponent(out UnitBigExoskeletonView component))
                        _unitBigExoskeletonViewPrefab = component;

                if (gameObject.name.Contains("Military"))
                    if (gameObject.TryGetComponent(out UnitMilitaryAndCyberSoldierView component))
                        _unitMilitaryAndCyberSoldierViewPrefab = component;

                if (gameObject.name.Contains("SpaceSoldier"))
                    if (gameObject.TryGetComponent(out UnitSpaceSoldierView component))
                        _unitSpaceSoldierViewPrefab = component;
            }            
        }

        if (_unitAvatarPortfolioViewPrefab == null)
            Debug.Log($"�� ������� ��������� AvatarPortfolio. ������� ��� {name}");
        if (_unitBigExoskeletonViewPrefab == null)
            Debug.Log($"�� ������� ��������� BigExoskeleton. ������� ��� {name}");
        if (_unitMilitaryAndCyberSoldierViewPrefab == null)
            Debug.Log($"�� ������� ��������� MilitaryAndCyberSoldier. ������� ��� {name}");
        if (_unitSpaceSoldierViewPrefab == null)
            Debug.Log($"�� ������� ��������� UnitSpaceSolder. ������� ��� {name}");
    }
}

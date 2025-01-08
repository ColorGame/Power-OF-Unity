using UnityEngine;

[CreateAssetMenu(fileName = "UnitFriend", menuName = "ScriptableObjects/UnitFriend")]
public class UnitFriendSO : UnitTypeSO
{
    [Header("���� ��� �������� �����")]

    [SerializeField] private Transform _unitAvatarPortfolioViewPrefab;
    [SerializeField] private UnitBigExoskeletonView _unitBigExoskeletonViewPrefab;
    [SerializeField] private UnitMilitaryAndCyberSoldier _unitMilitaryAndCyberSoldierViewPrefab;
    [SerializeField] private UnitSpaceSolderView _unitSpaceSolderViewPrefab;
   
    [Header("��������� ����� ��� ����� ")]
    [SerializeField] private uint _priceHiring;

    public Transform GetUnitAvatarPortfolioViewPrefab() { return _unitAvatarPortfolioViewPrefab; }

    /// <summary>
    /// ������� ������ ����� � ����������� �� ���� ����� ������� �� ����������
    /// </summary>
    /// <remarks>���� �������� null �� �������� ��������� ������ ��� �����</remarks>
    public UnitView GetUnitViewPrefab(BodyArmorTypeSO  bodyArmorTypeSO)
    {
        if(bodyArmorTypeSO == null)
        {
            return _unitSpaceSolderViewPrefab;
        }
        
        switch (bodyArmorTypeSO.GetBodyArmorType())
        {
            default:           
             return _unitSpaceSolderViewPrefab;

            case BodyArmorType.BodyArmorMilitary:
            case BodyArmorType.BodyArmorMilitaryMod:
            case BodyArmorType.BodyArmorCyber:
            case BodyArmorType.BodyArmorCyberMod:
                return _unitMilitaryAndCyberSoldierViewPrefab;

            case BodyArmorType.BodyArmorSpace:
            case BodyArmorType.BodyArmorSpaceMod:
                return _unitSpaceSolderViewPrefab;              

            case BodyArmorType.BodyArmorBigExoskeleton:
                return _unitBigExoskeletonViewPrefab;
        }
    }   
    public uint GetPriceHiring() {  return _priceHiring; }

}

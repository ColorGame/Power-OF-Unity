using UnityEngine;

[CreateAssetMenu(fileName = "UnitFriend", menuName = "ScriptableObjects/UnitFriend")]
public class UnitFriendSO : UnitTypeSO
{
    [Header("���� ��� �������� �����")]

    [SerializeField] private Transform _unitAvatarPortfolioVisualPrefab;
    [SerializeField] private UnitBigExoskeletonView _unitBigViewPrefab;
    [SerializeField] private UnitMilitaryAndCyberSoldier _unitSoldierViewPrefab;
    [SerializeField] private UnitSpaceSolderView _unitSpaceSolderViewPrefab;
   
    [Header("��������� ����� ��� ����� ")]
    [SerializeField] private uint _priceHiring;

    public Transform GetUnitAvatarPortfolioVisualPrefab() { return _unitAvatarPortfolioVisualPrefab; }

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
                return _unitSoldierViewPrefab;

            case BodyArmorType.BodyArmorSpace:
            case BodyArmorType.BodyArmorSpaceMod:
                return _unitSpaceSolderViewPrefab;              

            case BodyArmorType.BodyArmorBigExoskeleton:
                return _unitBigViewPrefab;
        }
    }   
    public uint GetPriceHiring() {  return _priceHiring; }

}

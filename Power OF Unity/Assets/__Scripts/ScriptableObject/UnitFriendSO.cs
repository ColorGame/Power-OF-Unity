using UnityEngine;

[CreateAssetMenu(fileName = "UnitFriend", menuName = "ScriptableObjects/UnitFriend")]
public class UnitFriendSO : UnitTypeSO
{
    [Header("Поля для префабов ЮНИТА")]

    [SerializeField] private Transform _unitAvatarPortfolioViewPrefab;
    [SerializeField] private UnitBigExoskeletonView _unitBigExoskeletonViewPrefab;
    [SerializeField] private UnitMilitaryAndCyberSoldier _unitMilitaryAndCyberSoldierViewPrefab;
    [SerializeField] private UnitSpaceSolderView _unitSpaceSolderViewPrefab;
   
    [Header("Стоимость юнита при найме ")]
    [SerializeField] private uint _priceHiring;

    public Transform GetUnitAvatarPortfolioViewPrefab() { return _unitAvatarPortfolioViewPrefab; }

    /// <summary>
    /// Вернуть визуал юнита в зависимости от типа БРОНИ которой он экипирован
    /// </summary>
    /// <remarks>Если передать null то вернется дефолтный визуал без брони</remarks>
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

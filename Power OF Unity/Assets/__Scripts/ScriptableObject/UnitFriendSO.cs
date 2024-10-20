using UnityEngine;

[CreateAssetMenu(fileName = "UnitFriend", menuName = "ScriptableObjects/UnitFriend")]
public class UnitFriendSO : UnitTypeSO
{
    [Header("Поля для префабов ЮНИТА")]

    [SerializeField] private Transform _unitAvatarPortfolioVisualPrefab;
    [SerializeField] private UnitBigExoskeletonView _unitBigViewPrefab;
    [SerializeField] private UnitSoldierView _unitSoldierViewPrefab;
    [SerializeField] private UnitSpaceSolderView _unitSpaceSolderViewPrefab;
   
    [Header("Стоимость юнита при найме ")]
    [SerializeField] private uint _priceHiring;

    public Transform GetUnitAvatarPortfolioVisualPrefab() { return _unitAvatarPortfolioVisualPrefab; }

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
        
        switch (bodyArmorTypeSO.GetPlacedObjectType())
        {
            default:           
             return _unitSpaceSolderViewPrefab;

            case PlacedObjectType.BodyArmorMilitary:
            case PlacedObjectType.BodyArmorMilitaryMod:
                return _unitSoldierViewPrefab;

            case PlacedObjectType.BodyArmorSpace:
            case PlacedObjectType.BodyArmorSpaceMod:
                return _unitSpaceSolderViewPrefab;              

            case PlacedObjectType.BodyArmorBigExoskeleton:
                return _unitBigViewPrefab;
        }
    }   
    public uint GetPriceHiring() {  return _priceHiring; }

}

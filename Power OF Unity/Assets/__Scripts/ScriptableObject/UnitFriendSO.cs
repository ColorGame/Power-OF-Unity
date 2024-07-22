using UnityEngine;

[CreateAssetMenu(fileName = "UnitFriend", menuName = "ScriptableObjects/UnitFriend")]
public class UnitFriendSO : UnitTypeSO
{
    [Header("Поля для префабов ЮНИТА")]

    [SerializeField] private Transform _unitAvatarPortfolioVisualPrefab;
    [SerializeField] private Transform _unitBaseVisualPrefab;
    [SerializeField] private Transform _unitEasyVisualPrefab;
    [SerializeField] private Transform _unitMediumVisualPrefab;
    [SerializeField] private Transform _unitHardVisualPrefab;

    [SerializeField] private UnitArmorType _basicAmorType; // Базовый тип брони юнита


    public Transform GetUnitAvatarPortfolioVisualPrefab() { return _unitAvatarPortfolioVisualPrefab; }
    public Transform GetUnitVisualPrefab(UnitArmorType unitArmorType)
    {
        switch (unitArmorType)
        {
            default:
            case UnitArmorType.Base:
                return _unitBaseVisualPrefab;

            case UnitArmorType.Easy:
                return _unitEasyVisualPrefab;

            case UnitArmorType.Medium:
                return _unitMediumVisualPrefab;

            case UnitArmorType.Hard:
                return _unitHardVisualPrefab;
        }
    }
    public UnitArmorType GetUnitArmorType() { return _basicAmorType; }

}

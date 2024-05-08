using UnityEngine;
/// <summary>
/// Создает дружественных унитов в ЯДРЕ  в CoreEntryPoint.
/// </summary>
/// <remarks>
/// Добавляет нанятых и удаляет уволенных юнитов
/// </remarks>
public class UnitFriendSpawnerOnCore : MonoBehaviour
{
   
    private UnitManager _unitManager;

    public void Initialize(UnitManager unitManager)
    {
        _unitManager = unitManager;
        
        /*foreach (UnitTypeSO unitTypeSO in _unitTypeBasicListSO.list)
        {
            Transform UnitPortfolioVisualPrefab = Instantiate(unitTypeSO.GetUnitPortfolioVisualPrefab(), transform);
            Unit unit = UnitPortfolioVisualPrefab.GetComponent<Unit>();
            _unitManager.AddMyUnitList(unit);
            UnitPortfolioVisualPrefab.gameObject.SetActive(false);
        }*/
    }



}

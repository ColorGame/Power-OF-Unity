using UnityEngine;
/// <summary>
/// ������� ������������� ������ � ����  � CoreEntryPoint.
/// </summary>
/// <remarks>
/// ��������� ������� � ������� ��������� ������
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

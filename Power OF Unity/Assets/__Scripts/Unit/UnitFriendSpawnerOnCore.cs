using UnityEngine;
/// <summary>
/// Создает дружественных унитов в ЯДРЕ  в CoreEntryPoint.
/// </summary>
/// <remarks>
/// Добавляет нанятых и удаляет уволенных юнитов
/// </remarks>
public class UnitFriendSpawnerOnCore : MonoBehaviour
{
    private UnitTypeBasicListSO _unitTypeBasicListSO;
    private UnitManager _unitManager;

    public void Initialize(UnitManager unitManager)
    {
        _unitManager = unitManager;
        _unitTypeBasicListSO = Resources.Load<UnitTypeBasicListSO>(typeof(UnitTypeBasicListSO).Name);    // Загружает ресурс запрошенного типа, хранящийся по адресу path(путь) в папке Resources(эту папку я создал в папке ScriptableObjects).
                                                                                                         // Что бы не ошибиться в имени пойдем другим путем. Создадим экземпляр UnitTypeBasicListSO и назавем также как и класс, потом для поиска SO будем извлекать имя класса которое совпадает с именем экземпляра

        foreach (UnitTypeSO unitTypeSO in _unitTypeBasicListSO.list)
        {
            Transform UnitPortfolioVisualPrefab = Instantiate(unitTypeSO.GetUnitPortfolioVisualPrefab(), transform);
            Unit unit = UnitPortfolioVisualPrefab.GetComponent<Unit>();
            _unitManager.AddMyUnitList(unit);
            UnitPortfolioVisualPrefab.gameObject.SetActive(false);
        }
    }



}

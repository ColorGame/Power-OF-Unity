using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/UnitType")] // Для создания экзкмпляров ScriptableObject. В всплывающем меню во вкладке create появиться вкладка ScriptableObjects внутри будут UnitType(Тип здания)
public class UnitTypeSO : ScriptableObject //Тип Юнита - это контейнер данных
{
    [SerializeField] private Transform _unitPortfolioVisualPrefab;
    [SerializeField] private Transform _unitEasyVisualPrefab;
    [SerializeField] private Transform _unitMediumVisualPrefab;
    [SerializeField] private Transform _unitHardPrefab;

    public string nameString;   // Имя Юнита
    public int health; // Здоровье
    public int moveDistance; // Дистанция движения (измеряется в узлах сетки, включает узел с самими Юнитом)
    public int shootDistance; // Дистанция выстрела

    public Transform GetUnitPortfolioVisualPrefab()
    {
        return _unitPortfolioVisualPrefab;
    }
}
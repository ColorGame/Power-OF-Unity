
using UnityEngine;

/// <summary>
/// Размещенный объект c ДЕЙСТВИЕМ (ACTION)
/// </summary>
/// <remarks>
/// abstract - НЕЛЬЗЯ создать экземпляр данного класса.
/// На практике вы, скорее всего, будете использовать подкласс, такой как "SwordTypeSO"  "ShootingWeaponTypeSO" или "GrenadeTypeSO" "HealItemTypeSO".
/// </remarks>

public abstract class PlacedObjectTypeWithActionSO : PlacedObjectTypeSO
{
    [Header("Префаб размещаемого объекта 3D(для создания в МИРЕ)")]
    [SerializeField] private Transform _prefab3D;
    [Header("Контролер переопределения АНИМАЦИИ")]
    [SerializeField] private AnimatorOverrideController _animatorOverrideController = null;
    
   
    /// <summary>
    /// Получить базовое действие для данного PlacedObjectTypeWithActionSO
    /// </summary>
    /// <remarks>
    /// В аргумент передаюм юнита у которого хотим получить BaseAction
    /// </remarks>
    public abstract BaseAction GetAction(Unit unit);   
    public AnimatorOverrideController GetAnimatorOverrideController() { return _animatorOverrideController; }
    public Transform GetPrefab3D() { return _prefab3D; }


}

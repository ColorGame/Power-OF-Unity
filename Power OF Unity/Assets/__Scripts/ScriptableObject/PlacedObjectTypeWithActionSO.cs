/// <summary>
/// Размещенный объект c ДЕЙСТВИЕМ (ACTION)
/// </summary>
/// <remarks>
/// abstract - НЕЛЬЗЯ создать экземпляр данного класса.
/// На практике вы, скорее всего, будете использовать подкласс, такой как "SwordTypeSO"  "ShootingWeaponTypeSO" или "GrenadeTypeSO" "HealItemTypeSO".
/// </remarks>

public abstract class PlacedObjectTypeWithActionSO : PlacedObjectTypeSO
{
    /// <summary>
    /// Получить базовое действие для данного PlacedObjectTypeWithActionSO
    /// </summary>
    /// <remarks>
    /// В аргумент передаюм юнита у которого хотим получить BaseAction
    /// </remarks>
    public abstract BaseAction GetAction(Unit unit);
}


using System;
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
    [SerializeField] protected GameObject _prefab3D;
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
    public GameObject GetPrefab3D() { return _prefab3D; }



    protected void Search3DPrefab(string nameFail, GameObject[] prefab3DArray)
    {
        int prefabDeleteLastCharName = 3; // Количество символов для удаления в имени префаба (чтобы убрать _3D )
      

        foreach (GameObject go in prefab3DArray)
        {
            string prefabName = go.name.Remove(go.name.Length - prefabDeleteLastCharName); // Получим имя префаба без последних 2 символов
            if (nameFail.Equals(prefabName, StringComparison.OrdinalIgnoreCase)) // Сравним имя SO с полученым без учета регистра
            {
                _prefab3D = go;
            }
        }
       

        if (_prefab3D == null)
        {
            Debug.Log($"Не удалось заполнить (_prefab3D). Проверь имя {name}");
        }
        
    }
}

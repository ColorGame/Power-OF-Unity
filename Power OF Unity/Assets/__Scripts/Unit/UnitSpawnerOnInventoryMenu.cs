using UnityEngine;

public class UnitSpawnerOnInventoryMenu : MonoBehaviour
{
    private UnitInventorySystem _unitInventorySystem;

    public void Init(UnitInventorySystem unitInventorySystem)
    {
        _unitInventorySystem = unitInventorySystem;

        Setup();
    }

    private void Setup()
    {
        //CreateSelectUnit(_unitInventorySystem.GetSelectedUnit());

        _unitInventorySystem.OnSelectedUnitChanged += UnitInventorySystem_OnSelectedUnitChanged;
    }

    private void UnitInventorySystem_OnSelectedUnitChanged(object sender, Unit selectedUnit)
    {
        CreateSelectUnit(selectedUnit);
    }

    private void CreateSelectUnit(Unit selectedUnit)
    {
        foreach (Transform attachTransform in transform) // Очистим точку спавна
        {
            Destroy(attachTransform.gameObject);
        }

        Transform unitVisualPrefab = selectedUnit.GetUnitTypeSO<UnitFriendSO>().GetUnitVisualPrefab(selectedUnit.GetUnitArmorType());
        Instantiate(unitVisualPrefab, transform);
    }
}

using UnityEngine;

public class UnitSetupMenuEntryPoint : MonoBehaviour, IEntryPoint
{
    private UnitSelectedForEquip _unitSelectedForEquip;
    private PlacedObjectTypeButton _placedObjectTypeButton;
    private PickUpDrop _pickUpDrop;
    private InventoryGrid _inventoryGrid;
    private InventoryGridVisual _inventoryGridVisual;

    public void Process(DIContainer container)
    {
        GetComponent();
        Register();
        Init(container);
    }

    private void GetComponent()
    {
        _pickUpDrop = GetComponentInChildren<PickUpDrop>(true);
        _placedObjectTypeButton = GetComponentInChildren<PlacedObjectTypeButton>(true);
        _inventoryGrid = GetComponentInChildren<InventoryGrid>(true);
        _inventoryGridVisual = GetComponentInChildren<InventoryGridVisual>(true);
    }

    private void Register()
    {
        _unitSelectedForEquip = new UnitSelectedForEquip();
    }

    private void Init(DIContainer container)
    {
        _unitSelectedForEquip.Init(_pickUpDrop);
        _pickUpDrop.Init(container.Resolve<GameInput>(), container.Resolve<TooltipUI>(), _inventoryGrid);
        _placedObjectTypeButton.Init(container.Resolve<TooltipUI>(), _pickUpDrop);
        _inventoryGrid.Init(_pickUpDrop, container.Resolve<TooltipUI>());
        _inventoryGridVisual.Init(_pickUpDrop, _inventoryGrid);
    }
}

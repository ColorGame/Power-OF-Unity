using UnityEngine;

public class UnitSetupMenuEntryPoint : MonoBehaviour, IEntryPoint
{
    private UnitInventorySystem _unitInventorySystem;
    private PlacedObjectTypeButton _placedObjectTypeButton;
    private PickUpDropPlacedObject _pickUpDropPlacedObject;
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
        _pickUpDropPlacedObject = GetComponentInChildren<PickUpDropPlacedObject>(true);
        _placedObjectTypeButton = GetComponentInChildren<PlacedObjectTypeButton>(true);
        _inventoryGrid = GetComponentInChildren<InventoryGrid>(true);
        _inventoryGridVisual = GetComponentInChildren<InventoryGridVisual>(true);
    }

    private void Register()
    {
        _unitInventorySystem = new UnitInventorySystem();
    }

    private void Init(DIContainer container)
    {
        _unitInventorySystem.Init(_pickUpDropPlacedObject, container.Resolve<UnitManager>(), _inventoryGrid);
        _pickUpDropPlacedObject.Init(container.Resolve<GameInput>(), container.Resolve<TooltipUI>(), _inventoryGrid);
        _placedObjectTypeButton.Init(container.Resolve<TooltipUI>(), _pickUpDropPlacedObject);
        _inventoryGrid.Init(_pickUpDropPlacedObject, container.Resolve<TooltipUI>());
        _inventoryGridVisual.Init(_pickUpDropPlacedObject, _inventoryGrid);
    }
}

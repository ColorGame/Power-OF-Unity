using UnityEngine;

public class UnitSetupMenuEntryPoint : MonoBehaviour, IEntryPoint
{
    private UnitInventorySystem _unitInventorySystem;
    private PlacedObjectSelectButtonsSystemUI _placedObjectSelectButtonsSystemUI;
    private PickUpDropPlacedObject _pickUpDropPlacedObject;
    private InventoryGrid _inventoryGrid;
    private InventoryGridVisual _inventoryGridVisual;
    private UnitSelectAtInventoryButtonsSystemUI _unitSelectAtInventoryButtonsSystemUI;
    private UnitPortfolioUI _unitPortfolioUI;
    private UnitSpawnerOnInventoryMenu _unitSpawnerOnInventoryMenu;

    public void Process(DIContainer container)
    {
        GetComponent();
        Register();
        Init(container);
    }

    private void GetComponent()
    {
        _pickUpDropPlacedObject = GetComponentInChildren<PickUpDropPlacedObject>(true);
        _placedObjectSelectButtonsSystemUI = GetComponentInChildren<PlacedObjectSelectButtonsSystemUI>(true);
        _unitSelectAtInventoryButtonsSystemUI = GetComponentInChildren<UnitSelectAtInventoryButtonsSystemUI>(true);
        _inventoryGrid = GetComponentInChildren<InventoryGrid>(true);
        _inventoryGridVisual = GetComponentInChildren<InventoryGridVisual>(true);
        _unitPortfolioUI = GetComponentInChildren<UnitPortfolioUI>(true);
        _unitSpawnerOnInventoryMenu = GetComponentInChildren<UnitSpawnerOnInventoryMenu>(true);
    }

    private void Register()
    {
        _unitInventorySystem = new UnitInventorySystem();
    }

    private void Init(DIContainer container)
    {
        _inventoryGrid.Init(container.Resolve<TooltipUI>());
        _pickUpDropPlacedObject.Init(container.Resolve<GameInput>(), container.Resolve<TooltipUI>(), _inventoryGrid);
        _unitPortfolioUI.Init(_unitInventorySystem); // должна иниц. перед _unitInventorySystem 
        _unitSpawnerOnInventoryMenu.Init(_unitInventorySystem);// должна иниц. перед _unitInventorySystem 
        _unitInventorySystem.Init(_pickUpDropPlacedObject, container.Resolve<UnitManager>(), _inventoryGrid);
        _placedObjectSelectButtonsSystemUI.Init(container.Resolve<TooltipUI>(), _pickUpDropPlacedObject);
        _unitSelectAtInventoryButtonsSystemUI.Init(container.Resolve<UnitManager>(), _unitInventorySystem);
        _inventoryGridVisual.Init(_pickUpDropPlacedObject, _inventoryGrid, _unitInventorySystem);
    }
}

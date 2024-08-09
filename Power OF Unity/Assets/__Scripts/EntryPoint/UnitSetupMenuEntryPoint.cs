using UnityEngine;

public class UnitSetupMenuEntryPoint : MonoBehaviour, IEntryPoint
{
    private UnitInventorySystem _unitInventorySystem;
    private PickUpDropPlacedObject _pickUpDropPlacedObject;
    private InventoryGrid _inventoryGrid;
    private InventoryGridVisual _inventoryGridVisual;

    private UnitPortfolioUI _unitPortfolioUI;
    private UnitSelectAtInventoryButtonsSystemUI _unitSelectAtInventoryButtonsSystemUI;
    private PlacedObjectSelectButtonsSystemUI _placedObjectSelectButtonsSystemUI;
    private UpperMenuBarOnUnitSetupUI _upperMenuBarOnUnitSetupUI;
    private GameMenuUI _gameMenuUI;
    private QuitGameSubMenuUI _quitGameSubMenuUI;
    private SaveGameSubMenuUI _saveGameSubMenuUI;

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
        _inventoryGrid = GetComponentInChildren<InventoryGrid>(true);
        _inventoryGridVisual = GetComponentInChildren<InventoryGridVisual>(true);

        _unitPortfolioUI = GetComponentInChildren<UnitPortfolioUI>(true);
        _unitSelectAtInventoryButtonsSystemUI = GetComponentInChildren<UnitSelectAtInventoryButtonsSystemUI>(true);
        _placedObjectSelectButtonsSystemUI = GetComponentInChildren<PlacedObjectSelectButtonsSystemUI>(true);
        _upperMenuBarOnUnitSetupUI = GetComponentInChildren<UpperMenuBarOnUnitSetupUI>(true);
        _gameMenuUI = GetComponentInChildren<GameMenuUI>(true);
        _quitGameSubMenuUI = GetComponentInChildren<QuitGameSubMenuUI>(true);
        _saveGameSubMenuUI = GetComponentInChildren<SaveGameSubMenuUI>(true);

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
        _upperMenuBarOnUnitSetupUI.Init(container.Resolve<GameInput>(), _gameMenuUI);
        _gameMenuUI.Init(container.Resolve<GameInput>(), container.Resolve<OptionsSubMenuUI>(), _quitGameSubMenuUI,_saveGameSubMenuUI, container.Resolve<LoadGameSubMenuUI>());
        _quitGameSubMenuUI.Init(container.Resolve<GameInput>(), container.Resolve<ScenesService>());
        _saveGameSubMenuUI.Init(container.Resolve<GameInput>());
        _inventoryGridVisual.Init(_pickUpDropPlacedObject, _inventoryGrid, _unitInventorySystem);
    }
}

using UnityEngine;

public class UnitSetupEntryPoint : MonoBehaviour, IEntryPoint
{
    private UnitInventorySystem _unitInventorySystem;
    private PickUpDropPlacedObject _pickUpDropPlacedObject;
    private InventoryGrid _inventoryGrid;
    private InventoryGridVisual _inventoryGridVisual;

    private UnitPortfolioUI _unitPortfolioUI;
    private UnitSelectAtInventoryButtonsSystemUI _unitSelectAtInventoryButtonsSystemUI;
    private PlacedObjectWithActionSelectButtonsSystemUI _placedObjectSelectButtonsSystemUI;
    private UpperMenuBarOnUnitSetupUI _upperMenuBarOnUnitSetupUI;
    private GameMenuUI _gameMenuUI;
    private QuitGameSubMenuUI _quitGameSubMenuUI;
    private SaveGameSubMenuUI _saveGameSubMenuUI;
    private UnitManagerTabUI _unitManagerTabUI;

    private SpawnerOnUnitSetupScen _unitSpawnerOnInventoryMenu;

    public void Process(DIContainer container)
    {
        GetComponent();
        Register();
        Init(container);
    }

    private void GetComponent()
    {        

        _unitPortfolioUI = GetComponentInChildren<UnitPortfolioUI>(true);
        _unitSelectAtInventoryButtonsSystemUI = GetComponentInChildren<UnitSelectAtInventoryButtonsSystemUI>(true);
        _placedObjectSelectButtonsSystemUI = GetComponentInChildren<PlacedObjectWithActionSelectButtonsSystemUI>(true);
        _upperMenuBarOnUnitSetupUI = GetComponentInChildren<UpperMenuBarOnUnitSetupUI>(true);
        _unitManagerTabUI = GetComponentInChildren<UnitManagerTabUI>(true);
        _gameMenuUI = GetComponentInChildren<GameMenuUI>(true);
        _quitGameSubMenuUI = GetComponentInChildren<QuitGameSubMenuUI>(true);
        _saveGameSubMenuUI = GetComponentInChildren<SaveGameSubMenuUI>(true);

        _unitSpawnerOnInventoryMenu = GetComponentInChildren<SpawnerOnUnitSetupScen>(true);

        _pickUpDropPlacedObject = GetComponentInChildren<PickUpDropPlacedObject>(true);
        _inventoryGrid = GetComponentInChildren<InventoryGrid>(true);
        _inventoryGridVisual = GetComponentInChildren<InventoryGridVisual>(true);
    }

    private void Register()
    {
        _unitInventorySystem = new UnitInventorySystem();
    }

    private void Init(DIContainer container)
    {

        _unitPortfolioUI.Init(container.Resolve<UnitManager>()); 
        _unitSelectAtInventoryButtonsSystemUI.Init(container.Resolve<UnitManager>());
        _placedObjectSelectButtonsSystemUI.Init(container.Resolve<TooltipUI>(), _pickUpDropPlacedObject);
        _unitManagerTabUI.Init(container.Resolve<UnitManager>(), container.Resolve<TooltipUI>());
        _gameMenuUI.Init(container.Resolve<GameInput>(), container.Resolve<OptionsSubMenuUI>(), _quitGameSubMenuUI,_saveGameSubMenuUI, container.Resolve<LoadGameSubMenuUI>());
        _quitGameSubMenuUI.Init(container.Resolve<GameInput>(), container.Resolve<ScenesService>());
        _saveGameSubMenuUI.Init(container.Resolve<GameInput>());

        _unitSpawnerOnInventoryMenu.Init(container.Resolve<UnitManager>());

        _inventoryGrid.Init(container.Resolve<TooltipUI>()); //1
        _inventoryGridVisual.Init(_pickUpDropPlacedObject, _inventoryGrid, _unitInventorySystem);//2
        _pickUpDropPlacedObject.Init(container.Resolve<GameInput>(), container.Resolve<TooltipUI>(), _inventoryGrid, container.Resolve<UnitManager>()); //3
        _unitInventorySystem.Init(_pickUpDropPlacedObject, container.Resolve<UnitManager>(), _inventoryGrid, _inventoryGridVisual); //4
        
        _upperMenuBarOnUnitSetupUI.Init(container.Resolve<GameInput>(), _gameMenuUI, _unitPortfolioUI, _unitSelectAtInventoryButtonsSystemUI, _pickUpDropPlacedObject, _unitInventorySystem, _placedObjectSelectButtonsSystemUI, _unitManagerTabUI);
    }

    private void OnDestroy()
    {
        _unitInventorySystem.OnDestroy();
        _unitInventorySystem = null;
    }
}

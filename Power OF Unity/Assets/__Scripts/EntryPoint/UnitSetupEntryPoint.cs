using UnityEngine;

public class UnitSetupEntryPoint : MonoBehaviour, IEntryPoint
{
    private UnitEquipmentSystem _unitEquipmentSystem;
    private PickUpDropPlacedObject _pickUpDropPlacedObject;
    private EquipmentGrid _equipmentGrid;
    private ItemGridVisual _itemGridVisual;

    private UnitPortfolioUI _unitPortfolioUI;
    private UnitSelectAtEquipmentButtonsSystemUI _unitSelectAtEquipmentButtonsSystemUI;
    private ItemSelectButtonsSystemUI _itemSelectButtonsSystemUI;
    private ArmorSelectButtonsSystemUI _armorSelectButtonsSystemUI;
    private UpperMenuBarOnUnitSetupUI _upperMenuBarOnUnitSetupUI;
    private GameMenuUI _gameMenuUI;
    private QuitGameSubMenuUI _quitGameSubMenuUI;
    private SaveGameSubMenuUI _saveGameSubMenuUI;
    private UnitManagerTabUI _unitManagerTabUI;

    private SpawnerOnUnitSetupScen _unitSpawnerOnEquipmentMenu;

    public void Process(DIContainer container)
    {
        GetComponent();
        Register();
        Init(container);
    }

    private void GetComponent()
    {        

        _unitPortfolioUI = GetComponentInChildren<UnitPortfolioUI>(true);
        _unitSelectAtEquipmentButtonsSystemUI = GetComponentInChildren<UnitSelectAtEquipmentButtonsSystemUI>(true);
        _itemSelectButtonsSystemUI = GetComponentInChildren<ItemSelectButtonsSystemUI>(true);
        _armorSelectButtonsSystemUI = GetComponentInChildren<ArmorSelectButtonsSystemUI>(true);
        _upperMenuBarOnUnitSetupUI = GetComponentInChildren<UpperMenuBarOnUnitSetupUI>(true);
        _unitManagerTabUI = GetComponentInChildren<UnitManagerTabUI>(true);
        _gameMenuUI = GetComponentInChildren<GameMenuUI>(true);
        _quitGameSubMenuUI = GetComponentInChildren<QuitGameSubMenuUI>(true);
        _saveGameSubMenuUI = GetComponentInChildren<SaveGameSubMenuUI>(true);

        _unitSpawnerOnEquipmentMenu = GetComponentInChildren<SpawnerOnUnitSetupScen>(true);

        _pickUpDropPlacedObject = GetComponentInChildren<PickUpDropPlacedObject>(true);
        _equipmentGrid = GetComponentInChildren<EquipmentGrid>(true);
        _itemGridVisual = GetComponentInChildren<ItemGridVisual>(true);
    }

    private void Register()
    {
        _unitEquipmentSystem = new UnitEquipmentSystem();
    }

    private void Init(DIContainer container)
    {

        _unitPortfolioUI.Init(container.Resolve<UnitManager>()); 
        _unitSelectAtEquipmentButtonsSystemUI.Init(container.Resolve<UnitManager>());
        _itemSelectButtonsSystemUI.Init(container.Resolve<TooltipUI>(), _pickUpDropPlacedObject, container.Resolve<WarehouseManager>());
        _unitManagerTabUI.Init(container.Resolve<UnitManager>(), container.Resolve<TooltipUI>());
        _gameMenuUI.Init(container.Resolve<GameInput>(), container.Resolve<OptionsSubMenuUI>(), _quitGameSubMenuUI,_saveGameSubMenuUI, container.Resolve<LoadGameSubMenuUI>());
        _quitGameSubMenuUI.Init(container.Resolve<GameInput>(), container.Resolve<ScenesService>());
        _saveGameSubMenuUI.Init(container.Resolve<GameInput>());

        _unitSpawnerOnEquipmentMenu.Init(container.Resolve<UnitManager>());

        _equipmentGrid.Init(container.Resolve<TooltipUI>()); //1

        _itemGridVisual.Init(_pickUpDropPlacedObject, _equipmentGrid, _unitEquipmentSystem);//2

        _pickUpDropPlacedObject.Init(
            container.Resolve<GameInput>(), 
            container.Resolve<TooltipUI>(), 
            _equipmentGrid, 
            container.Resolve<UnitManager>(),
            container.Resolve<WarehouseManager>()); //3

        _unitEquipmentSystem.Init(_pickUpDropPlacedObject, container.Resolve<UnitManager>(), _equipmentGrid, _itemGridVisual); //4
        
        _upperMenuBarOnUnitSetupUI.Init(
            container.Resolve<GameInput>(), 
            _gameMenuUI, 
            _unitPortfolioUI, 
            _unitSelectAtEquipmentButtonsSystemUI, 
            _pickUpDropPlacedObject, 
            _unitEquipmentSystem, 
            _itemSelectButtonsSystemUI, 
            _armorSelectButtonsSystemUI, 
            _unitManagerTabUI);
    }

    private void OnDestroy()
    {
        _unitEquipmentSystem.OnDestroy();
        _unitEquipmentSystem = null;
    }
}

using UnityEngine;

public class UnitSetupEntryPoint : MonoBehaviour, IEntryPoint
{
    private UnitEquipmentSystem _unitEquipmentSystem;
    private PickUpDropPlacedObject _pickUpDropPlacedObject;
    private EquipmentGrid _equipmentGrid;
    private ItemGridVisual _itemGridVisual;
    private ArmorGridVisual _armorGridVisual;

    private UnitPortfolioUI _unitPortfolioUI;
    private UnitSelectAtEquipmentButtonsSystemUI _unitSelectAtEquipmentButtonsSystemUI;
    private ItemSelectButtonsSystemUI _itemSelectButtonsSystemUI;
    private ArmorSelectButtonsSystemUI _armorSelectButtonsSystemUI;
    private MarketUI _marketUI;
    private UpperMenuBarOnUnitSetupUI _upperMenuBarOnUnitSetupUI;
    private UnitSelectForManagementButtonsSystemUI _unitSelectForManagementButtonsSystemUI;

    private SpawnerOnUnitSetupScen _unitSpawnerOnEquipmentMenu;

    public void Inject(DIContainer container)
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
        _marketUI = GetComponentInChildren<MarketUI>(true);
        _upperMenuBarOnUnitSetupUI = GetComponentInChildren<UpperMenuBarOnUnitSetupUI>(true);
        _unitSelectForManagementButtonsSystemUI = GetComponentInChildren<UnitSelectForManagementButtonsSystemUI>(true);

        _unitSpawnerOnEquipmentMenu = GetComponentInChildren<SpawnerOnUnitSetupScen>(true);

        _pickUpDropPlacedObject = GetComponentInChildren<PickUpDropPlacedObject>(true);
        _equipmentGrid = GetComponentInChildren<EquipmentGrid>(true);
        _itemGridVisual = GetComponentInChildren<ItemGridVisual>(true);
        _armorGridVisual = GetComponentInChildren<ArmorGridVisual>(true);
    }

    private void Register()
    {
        _unitEquipmentSystem = new UnitEquipmentSystem();
    }

    private void Init(DIContainer container)
    {

        _unitPortfolioUI.Init(container.Resolve<UnitManager>());
        _unitSelectAtEquipmentButtonsSystemUI.Init(container.Resolve<UnitManager>(), container.Resolve<TooltipUI>());
        _itemSelectButtonsSystemUI.Init(container.Resolve<TooltipUI>(), _pickUpDropPlacedObject, container.Resolve<WarehouseManager>());
        _armorSelectButtonsSystemUI.Init(container.Resolve<TooltipUI>(), _pickUpDropPlacedObject, container.Resolve<WarehouseManager>());
        _marketUI.Init(container.Resolve<TooltipUI>(), container.Resolve<WarehouseManager>(), container.Resolve<HashAnimationName>());
        _unitSelectForManagementButtonsSystemUI.Init(container.Resolve<UnitManager>(), container.Resolve<TooltipUI>());

        _unitSpawnerOnEquipmentMenu.Init(container.Resolve<UnitManager>());

        _equipmentGrid.Init(container.Resolve<TooltipUI>()); //1

        _itemGridVisual.Init(_pickUpDropPlacedObject, _equipmentGrid, _unitEquipmentSystem);//2
        _armorGridVisual.Init(_pickUpDropPlacedObject, _equipmentGrid, _unitEquipmentSystem);

        _pickUpDropPlacedObject.Init(
            container.Resolve<GameInput>(),
            container.Resolve<TooltipUI>(),
            _equipmentGrid,
            container.Resolve<UnitManager>(),
            container.Resolve<WarehouseManager>()); //3

        _unitEquipmentSystem.Init(_pickUpDropPlacedObject, container.Resolve<UnitManager>(), _equipmentGrid, _itemGridVisual, _armorGridVisual, _itemSelectButtonsSystemUI); //4

        _upperMenuBarOnUnitSetupUI.Init(
            container.Resolve<GameInput>(),
            container.Resolve<WarehouseManager>(),
            container.Resolve<GameMenuUIProvider>(),
            _unitPortfolioUI,
            _unitSelectAtEquipmentButtonsSystemUI,
            _pickUpDropPlacedObject,
            _unitEquipmentSystem,
            _itemSelectButtonsSystemUI,
            _armorSelectButtonsSystemUI,
            _marketUI,
            _unitSelectForManagementButtonsSystemUI);
    }

    private void OnDestroy()
    {
        _unitEquipmentSystem.OnDestroy();
        _unitEquipmentSystem = null;
    }
}

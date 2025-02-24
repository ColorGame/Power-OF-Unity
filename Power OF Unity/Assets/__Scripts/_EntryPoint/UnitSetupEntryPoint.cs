using UnityEngine;

public class UnitSetupEntryPoint : MonoBehaviour, IEntryPoint, IStartScene
{
    [SerializeField] private UnitPortfolioUI _unitPortfolioUI;
    [SerializeField] private UnitSelectAtEquipmentButtonsSystemUI _unitSelectAtEquipmentButtonsSystemUI;
    [SerializeField] private ItemSelectButtonsSystemUI _itemSelectButtonsSystemUI;
    [SerializeField] private ArmorSelectButtonsSystemUI _armorSelectButtonsSystemUI;
    [SerializeField] private MarketUI _marketUI;
    [SerializeField] private UpperMenuBarOnUnitSetupUI _upperMenuBarOnUnitSetupUI;
    [SerializeField] private UnitSelectForManagementButtonsSystemUI _unitSelectForManagementButtonsSystemUI;

    [SerializeField] private SpawnerOnUnitSetupScen _unitSpawnerOnEquipmentMenu;

    [SerializeField] private PickUpDropPlacedObject _pickUpDropPlacedObject;
    [SerializeField] private EquipmentGrid _equipmentGrid;
    [SerializeField] private ItemGridVisual _itemGridVisual;
    [SerializeField] private ArmorGridVisual _armorGridVisual;

    private UnitEquipmentSystem _unitEquipmentSystem;


    public void Inject(DIContainer container)
    {       
        Register();
        Init(container);
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
            _unitEquipmentSystem,
            container.Resolve<UnitManager>()); //3

        _unitEquipmentSystem.Init(
            _pickUpDropPlacedObject,
            container.Resolve<UnitManager>(),
            _equipmentGrid,
            container.Resolve<WarehouseManager>(),
            _itemGridVisual,
            _armorGridVisual,
            _itemSelectButtonsSystemUI); //4

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

    public void StartScene()
    {
        _itemSelectButtonsSystemUI.StartAnimation();
    }
}

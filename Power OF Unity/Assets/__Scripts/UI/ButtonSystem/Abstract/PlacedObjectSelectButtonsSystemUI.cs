/// <summary>
/// Система кнопок - выбора ОБЪЕКТА типа PlacedObject
/// </summary>
public abstract class PlacedObjectSelectButtonsSystemUI : ObjectSelectButtonsSystemUI
{
    protected PickUpDropPlacedObject _pickUpDrop;
    protected WarehouseManager _warehouseManager;

    public void Init(TooltipUI tooltipUI, PickUpDropPlacedObject pickUpDrop, WarehouseManager warehouseManager)
    {
        _tooltipUI = tooltipUI;
        _pickUpDrop = pickUpDrop;
        _warehouseManager = warehouseManager;
        Setup();
    }
}

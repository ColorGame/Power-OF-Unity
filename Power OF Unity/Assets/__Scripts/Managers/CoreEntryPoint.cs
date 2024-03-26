
using UnityEngine;

using UnityEngine.InputSystem.UI;

/// <summary>
/// Ядро - Системная точка входа.
/// Один основной объект, который никогда не уничтожается, с подсистемами в качестве дочерних, или внутри если они не наследуют MonoBehaviour
/// </summary>
public class CoreEntryPoint : PersistentSingleton<CoreEntryPoint> // Пока этот объект создает Bootstrapper, НО в дальнейшем мы бкдем его создавать на первой сцене загрузки
{
    public GameInput gameInput { get; private set; }
    public VirtualMouseCustom virtualMouseCustom { get; private set; }
    public SoundManager soundManager { get; private set; }
    public MusicManager musicManager { get; private set; }
    public JsonSavingSystem jsonSavingSystem { get; private set; }
    public TooltipUI tooltipUI { get; private set; }
    public OptionsMenuUI optionsMenuUI { get; private set; }
    public PickUpDrop pickUpDrop { get; private set; }
    public InventoryGridVisual inventoryGridVisual { get; private set; }
    public PlacedObjectTypeButton placedObjectTypeButton { get; private set; }
    public InventoryGrid inventoryGrid { get; private set; }
    public UnitManager unitManager { get; private set; }


    protected override void Awake()
    {
        base.Awake();

        gameInput = new GameInput(); // Создадим экземпляр       
        jsonSavingSystem = new JsonSavingSystem();
        unitManager = new UnitManager();



        virtualMouseCustom = GetComponentInChildren<VirtualMouseCustom>(true);
        soundManager = GetComponentInChildren<SoundManager>(true);
        musicManager = GetComponentInChildren<MusicManager>(true);
        optionsMenuUI = GetComponentInChildren<OptionsMenuUI>(true);
        tooltipUI = GetComponentInChildren<TooltipUI>(true);
        pickUpDrop = GetComponentInChildren<PickUpDrop>(true);
        placedObjectTypeButton = GetComponentInChildren<PlacedObjectTypeButton>(true);
        inventoryGrid = GetComponentInChildren<InventoryGrid>(true);
        inventoryGridVisual = GetComponentInChildren<InventoryGridVisual>(true);       


        if(gameInput!=null)
            gameInput.Initialize(); // Инициализируем поля и сделаем подписки
        else Debug.Log("Нет GameInput");

        if (virtualMouseCustom != null)
            virtualMouseCustom.Initialize(gameInput);
        else Debug.Log("Нет VirtualMouseCustom");

        if (optionsMenuUI != null)
            optionsMenuUI.Initialize(gameInput, soundManager, musicManager);
        else Debug.Log("Нет OptionsMenuUI");

        if (tooltipUI != null)
            tooltipUI.Initialize(virtualMouseCustom, gameInput);
        else Debug.Log("Нет TooltipUI");

        if (pickUpDrop != null)
            pickUpDrop.Initialize(gameInput, tooltipUI,inventoryGrid);
        else Debug.Log("Нет PickUpDrop");

        if (placedObjectTypeButton)
            placedObjectTypeButton.Initialize(tooltipUI, pickUpDrop);
        else Debug.Log("Нет PlacedObjectTypeButton");

        if (inventoryGrid != null)
            inventoryGrid.Initialize(pickUpDrop,tooltipUI);
        else Debug.Log("Нет InventoryGrid");

        if (inventoryGridVisual != null)
            inventoryGridVisual.Initialize(pickUpDrop, inventoryGrid);
        else Debug.Log("Нет InventoryGridVisual");

        if (unitManager != null)
            unitManager.Initialize(tooltipUI);
        else Debug.Log("Нет UnitManager");

        Debug.Log("CoreEntryPoint  Awake_CANCALED");
    }

       
}


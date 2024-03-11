
using UnityEngine;

using UnityEngine.InputSystem.UI;

/// <summary>
/// ядро - —истемна€ точка входа.
/// ќдин основной объект, который никогда не уничтожаетс€, с подсистемами в качестве дочерних, или внутри если они не наследуют MonoBehaviour
/// </summary>
public class CoreEntryPoint : PersistentSingleton<CoreEntryPoint>
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


    protected override void Awake()
    {
        base.Awake();

        gameInput = new GameInput(); // —оздадим экземпл€р
        gameInput.Initialize(); // »нициализируем пол€ и сделаем подписки
        jsonSavingSystem = new JsonSavingSystem();


        virtualMouseCustom = GetComponentInChildren<VirtualMouseCustom>(true);
        soundManager = GetComponentInChildren<SoundManager>(true);
        musicManager = GetComponentInChildren<MusicManager>(true);
        optionsMenuUI = GetComponentInChildren<OptionsMenuUI>(true);
        tooltipUI = GetComponentInChildren<TooltipUI>(true);
        pickUpDrop = GetComponentInChildren<PickUpDrop>(true);
        placedObjectTypeButton = GetComponentInChildren<PlacedObjectTypeButton>(true);
        inventoryGrid = GetComponentInChildren<InventoryGrid>(true);
        inventoryGridVisual = GetComponentInChildren<InventoryGridVisual>(true);



        if (virtualMouseCustom != null)
            virtualMouseCustom.Initialize(gameInput);

        if (optionsMenuUI != null)
            optionsMenuUI.Initialize(gameInput, soundManager, musicManager);

        if(pickUpDrop != null)
            pickUpDrop.Initialize(gameInput, tooltipUI,inventoryGrid);

        if (placedObjectTypeButton)
            placedObjectTypeButton.Initialize(tooltipUI, pickUpDrop);

        if (inventoryGrid != null)
            inventoryGrid.Initialize(pickUpDrop,tooltipUI);

        if (inventoryGridVisual != null)
            inventoryGridVisual.Initialize(pickUpDrop, inventoryGrid);


        Debug.Log("CoreEntryPoint  Awake_CANCALED");
    }

       
}


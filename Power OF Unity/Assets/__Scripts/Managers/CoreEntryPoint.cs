
using UnityEngine;

using UnityEngine.InputSystem.UI;

/// <summary>
/// ���� - ��������� ����� �����.
/// ���� �������� ������, ������� ������� �� ������������, � ������������ � �������� ��������, ��� ������ ���� ��� �� ��������� MonoBehaviour
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
        

        gameInput = new GameInput(); // �������� ���������
        gameInput.Initialize(); // �������������� ���� � ������� ��������

        virtualMouseCustom = GetComponentInChildren<VirtualMouseCustom>(true);
        if (virtualMouseCustom != null)
            virtualMouseCustom.Initialize(gameInput);

        soundManager = GetComponentInChildren<SoundManager>(true);      

        musicManager = GetComponentInChildren<MusicManager>(true);         

        optionsMenuUI = GetComponentInChildren<OptionsMenuUI>(true);
        if (optionsMenuUI != null)
            optionsMenuUI.Initialize(gameInput, soundManager, musicManager);

        tooltipUI = GetComponentInChildren<TooltipUI>(true);

        pickUpDrop = GetComponentInChildren<PickUpDrop>(true);
        if(pickUpDrop != null)
            pickUpDrop.Initialize(gameInput);

        jsonSavingSystem = new JsonSavingSystem();

        Debug.Log("CoreEntryPoint  Awake_CANCALED");
    }

       
}


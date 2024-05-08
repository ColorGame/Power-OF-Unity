
using UnityEngine;

using UnityEngine.InputSystem.UI;

/// <summary>
/// ���� - ��������� ����� �����.
/// ���� �������� ������, ������� ������� �� ������������, � ������������ � �������� ��������, ��� ������ ���� ��� �� ��������� MonoBehaviour
/// </summary>
public class CoreEntryPoint : PersistentSingleton<CoreEntryPoint> // ���� ���� ������ ������� Bootstrapper, �� � ���������� �� ����� ��� ��������� �� ������ ����� ��������. 
{
    public GameInput gameInput { get; private set; }
    public VirtualMouseCustom virtualMouseCustom { get; private set; }
    public SoundManager soundManager { get; private set; }
    public MusicManager musicManager { get; private set; }
    public JsonSavingSystem jsonSavingSystem { get; private set; }
    public TooltipUI tooltipUI { get; private set; }
    public OptionsMenuUI optionsMenuUI { get; private set; }

    // ������� ���� � ���������� ����� ��������� �������� ����� ������ ������ ���� UnitMenuEntryPoint. ���� ������� �������� ��� ������� ����� ���� ��� �������� ����������
    public UnitManager unitManager { get; private set; }
    public UnitSelectedForEquip unitSelectedForEquip { get; private set; }
    public UnitFriendSpawnerOnCore unitFriendSpawnerOnCore { get; private set; }
    public PlacedObjectTypeButton placedObjectTypeButton { get; private set; }
    public PickUpDrop pickUpDrop { get; private set; }
    public InventoryGrid inventoryGrid { get; private set; }
    public InventoryGridVisual inventoryGridVisual { get; private set; }


    protected override void Awake()
    {
        base.Awake();

        gameInput = new GameInput(); // �������� ���������       
        jsonSavingSystem = new JsonSavingSystem();
        unitManager = new UnitManager();
        unitSelectedForEquip = new UnitSelectedForEquip();



        virtualMouseCustom = GetComponentInChildren<VirtualMouseCustom>(true);
        soundManager = GetComponentInChildren<SoundManager>(true);
        musicManager = GetComponentInChildren<MusicManager>(true);
        optionsMenuUI = GetComponentInChildren<OptionsMenuUI>(true);
        tooltipUI = GetComponentInChildren<TooltipUI>(true);
        unitFriendSpawnerOnCore = GetComponentInChildren<UnitFriendSpawnerOnCore>(true);
        pickUpDrop = GetComponentInChildren<PickUpDrop>(true);
        placedObjectTypeButton = GetComponentInChildren<PlacedObjectTypeButton>(true);
        inventoryGrid = GetComponentInChildren<InventoryGrid>(true);
        inventoryGridVisual = GetComponentInChildren<InventoryGridVisual>(true);

        if (gameInput != null)
            gameInput.Initialize(); // �������������� ���� � ������� ��������
        else Debug.Log("��� GameInput");

        if (virtualMouseCustom != null)
            virtualMouseCustom.Initialize(gameInput);
        else Debug.Log("��� VirtualMouseCustom");

        if (optionsMenuUI != null)
            optionsMenuUI.Initialize(gameInput, soundManager, musicManager);
        else Debug.Log("��� OptionsMenuUI");

        if (tooltipUI != null)
            tooltipUI.Initialize(virtualMouseCustom, gameInput);
        else Debug.Log("��� TooltipUI");

        if (unitManager != null)
            unitManager.Initialize(tooltipUI);
        else Debug.Log("��� UnitManager");

        if (unitSelectedForEquip != null)
            unitSelectedForEquip.Initialize(pickUpDrop);
        else Debug.Log("��� UnitSelectedForEquip");

        if (unitFriendSpawnerOnCore != null)
            unitFriendSpawnerOnCore.Initialize(unitManager);
        else Debug.Log("��� UnitFriendSpawnerOnCore");

        if (pickUpDrop != null)
            pickUpDrop.Initialize(gameInput, tooltipUI, inventoryGrid);
        else Debug.Log("��� PickUpDrop");

        if (placedObjectTypeButton)
            placedObjectTypeButton.Initialize(tooltipUI, pickUpDrop);
        else Debug.Log("��� PlacedObjectTypeButton");

        if (inventoryGrid != null)
            inventoryGrid.Initialize(pickUpDrop, tooltipUI);
        else Debug.Log("��� InventoryGrid");

        if (inventoryGridVisual != null)
            inventoryGridVisual.Initialize(pickUpDrop, inventoryGrid);
        else Debug.Log("��� InventoryGridVisual");

        
       

       

        


        //Unit unit = unitManager.GetMyUnitList()[0];
       // unit.gameObject.SetActive(true);

//unitSelectedForEquip.SetSelectedUnit(unit.GetComponent<UnitInventory>());

        Debug.Log("CoreEntryPoint  Awake_CANCALED");
    }


}


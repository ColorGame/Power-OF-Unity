
using UnityEngine;

public class GameSceneEntryPoint : MonoBehaviour
{
    public CameraFollow cameraFollow { get; private set; }
    public MouseOnGameGrid mouseOnGameGrid { get; private set; }
    public UnitActionSystem unitActionSystem { get; private set; }
    public UnitSpawnerOnLevel unitSpawner { get; private set; }
    public SelectUnitButtonUI selectUnitButtonUI { get; private set; }
    public OptionsMenagerUI optionsMenagerUI { get; private set; }
    public EnemyAI enemyAI { get; private set; }
    public TurnSystem turnSystem { get; private set; }
    public LevelGrid levelGrid { get; private set; }

    private CoreEntryPoint _coreEntryPoint;


    private void Awake()
    {
        cameraFollow = GetComponentInChildren<CameraFollow>(true);
        mouseOnGameGrid = GetComponentInChildren<MouseOnGameGrid>(true);
        unitActionSystem = GetComponentInChildren<UnitActionSystem>(true);
        unitSpawner = GetComponentInChildren<UnitSpawnerOnLevel>(true);
        selectUnitButtonUI = GetComponentInChildren<SelectUnitButtonUI>(true);
        optionsMenagerUI = GetComponentInChildren<OptionsMenagerUI>(true);
        enemyAI = GetComponentInChildren<EnemyAI>(true);
        turnSystem = GetComponentInChildren<TurnSystem>(true);
        levelGrid = GetComponentInChildren<LevelGrid>(true);

        _coreEntryPoint = CoreEntryPoint.Instance;

        if (cameraFollow != null)
            cameraFollow.Initialize(_coreEntryPoint.gameInput, _coreEntryPoint.optionsMenuUI);
        else Debug.Log("Нет CameraFollow");

        if (mouseOnGameGrid != null)
            mouseOnGameGrid.Initialize(_coreEntryPoint.gameInput);
        else Debug.Log("Нет MouseOnGameGrid");

        if (unitActionSystem != null)
            unitActionSystem.Initialize(_coreEntryPoint.gameInput, _coreEntryPoint.unitManager);
        else Debug.Log("Нет UnitActionSystem");
        
        if (unitSpawner != null)
            unitSpawner.Initialize(_coreEntryPoint.unitManager, turnSystem, levelGrid);
        else Debug.Log("Нет UnitSpawnerOnLevel");

        if (selectUnitButtonUI != null)
            selectUnitButtonUI.Initialize(_coreEntryPoint.unitManager, turnSystem, unitActionSystem);
        else Debug.Log("Нет SelectUnitButtonUI");

        if (optionsMenagerUI != null)
            optionsMenagerUI.Initialize(_coreEntryPoint.unitManager);
        else Debug.Log("Нет OptionsMenagerUI");        
       
        if(enemyAI!=null)
            enemyAI.Initialize(_coreEntryPoint.unitManager, turnSystem);
        else Debug.Log("Нет EnemyAI");



        Debug.Log("GameSceneEntryPoint  Awake_CANCALED");
    }

    private void Start()
    {


    }
}

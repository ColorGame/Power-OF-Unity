using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class GameplayEntryPoint : MonoBehaviour, IEntryPoint
{

    private CameraFollow _cameraFollow;
    private MouseOnGameGrid _mouseOnGameGrid;
    private UnitSpawnerOnLevel _unitSpawnerOnLevel;
    private UnitActionSystem _unitActionSystem;
    private UnitSelectAtLevelButtonsSystemUI _unitSelectAtLevelButtonsSystemUI;
    private OptionsMenagerUI _optionsMenagerUI;
    private ActionButtonSystemUI _actionButtonSystemUI;
    private EnemyAI _enemyAI;
    private LevelGrid _levelGrid;
    private TurnSystem _turnSystem;
    private TurnSystemUI _turnSystemUI;
    private LevelGridVisual _levelGridVisual;
    private GameEndUI _gameEndUI;


    public  void Inject(DIContainer container)
    {
        GetComponent();
        Register();
        Init(container);
        InitEntity();
    }

    private void GetComponent()
    {
        _cameraFollow = GetComponentInChildren<CameraFollow>(true);
        _mouseOnGameGrid = GetComponentInChildren<MouseOnGameGrid>(true);
        _unitSpawnerOnLevel = GetComponentInChildren<UnitSpawnerOnLevel>(true);
        _unitActionSystem = GetComponentInChildren<UnitActionSystem>(true);
        _unitSelectAtLevelButtonsSystemUI = GetComponentInChildren<UnitSelectAtLevelButtonsSystemUI>(true);
        _optionsMenagerUI = GetComponentInChildren<OptionsMenagerUI>(true);
        _actionButtonSystemUI = GetComponentInChildren<ActionButtonSystemUI>(true);
        _enemyAI = GetComponentInChildren<EnemyAI>(true);
        _levelGrid = GetComponentInChildren<LevelGrid>(true);
        _levelGridVisual = GetComponentInChildren<LevelGridVisual>(true);
        _gameEndUI = GetComponentInChildren<GameEndUI>(true);
        _turnSystemUI = GetComponentInChildren<TurnSystemUI>(true);
    }

    private void Register()
    {
        _turnSystem = new TurnSystem();
    }

    private void Init(DIContainer container)
    {
        _cameraFollow.Init(container.Resolve<GameInput>(), container.Resolve<OptionsSubMenuUIProvider>());
      //  FloorVisibility.Init(_levelGrid, _cameraFollow); // инициализировать до спавна юнитов
        _mouseOnGameGrid.Init(container.Resolve<GameInput>(), _levelGrid);
        _unitSpawnerOnLevel.Init(container.Resolve<UnitManager>(), _turnSystem, container.Resolve<SoundManager>(), _levelGrid, _unitActionSystem, _cameraFollow, container.Resolve<HashAnimationName>());
        _unitActionSystem.Init(container.Resolve<GameInput>(), container.Resolve<UnitManager>(), _turnSystem, _levelGrid, _mouseOnGameGrid);
        _unitSelectAtLevelButtonsSystemUI.Init(container.Resolve<UnitManager>(), _turnSystem, _unitActionSystem, _cameraFollow);
        _optionsMenagerUI.Init(container.Resolve<UnitManager>(), _unitActionSystem);
        _actionButtonSystemUI.Init(_unitActionSystem, _turnSystem, container.Resolve<TooltipUI>());
        _enemyAI.Init(container.Resolve<UnitManager>(), _turnSystem, _unitActionSystem);
        _levelGridVisual.Init(_unitActionSystem, _levelGrid, _mouseOnGameGrid);
        _gameEndUI.Init(container.Resolve<ScenesService>());
        _turnSystemUI.Init(_turnSystem);
        
        DoorInteract.Init(container.Resolve<SoundManager>(), _levelGrid, container.Resolve<HashAnimationName>());
        BarrelInteract.Init(container.Resolve<SoundManager>(), _levelGrid);
        DestructibleCrate.Init(container.Resolve<SoundManager>(), _levelGrid);
        SphereInteract.Init(container.Resolve<SoundManager>(), _levelGrid);
        PathfindingLinkMonoBehaviour.Init(_levelGrid);
    }

    private void InitEntity()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        //Инициализирую PathfindingGridDate
        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithPresent<PathfindingGridDate>().Build(entityManager);// способ  Создадия конструктора для обращения к сущности у которых есть PathfindingGridDate
        Entity entityPathfinding = entityQuery.GetSingletonEntity(); //Получим сущность

        PathfindingGridDate pathfindingGridDate = entityManager.GetComponentData<PathfindingGridDate>(entityPathfinding);
        pathfindingGridDate.anchorGrid = transform.position;
        entityManager.SetComponentData(entityPathfinding, pathfindingGridDate);
        entityManager.SetComponentEnabled<PathfindingGridDate>(entityPathfinding, true);


        /*
        entityQuery = new EntityQueryBuilder(Allocator.Temp).WithPresent<PathfindingParams>().Build(entityManager);// способ  Создадия конструктора для обращения к сущности 
        PathfindingParams pathfindingParams = entityManager.GetComponentData<PathfindingParams>(entityPathfinding);
        pathfindingParams.startPosition = new Unity.Mathematics.int3(0, 0, 0);
        pathfindingParams.endPosition = new Unity.Mathematics.int3(12, 0, 7);
        entityManager.SetComponentData(entityPathfinding, pathfindingParams);
        entityManager.SetComponentEnabled<PathfindingParams>(entityPathfinding, true);*/
    }

}

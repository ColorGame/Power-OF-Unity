using Unity.Cinemachine;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using static PathNodeSystem;

public class GameplayEntryPoint : MonoBehaviour, IEntryPoint
{

    [SerializeField] private CameraFollow _cameraFollow;
    [SerializeField] private MouseOnGameGrid _mouseOnGameGrid;
    [SerializeField] private UnitSpawnerOnLevel _unitSpawnerOnLevel;
    [SerializeField] private UnitActionSystem _unitActionSystem;
    [SerializeField] private UnitSelectAtLevelButtonsSystemUI _unitSelectAtLevelButtonsSystemUI;
    [SerializeField] private OptionsMenagerUI _optionsMenagerUI;
    [SerializeField] private ActionButtonSystemUI _actionButtonSystemUI;
    [SerializeField] private EnemyAI _enemyAI;
    [SerializeField] private LevelGrid _levelGrid;
    [SerializeField] private LevelGridVisual _levelGridVisual;
    [SerializeField] private GameEndUI _gameEndUI;
    [SerializeField] private TurnSystemUI _turnSystemUI;

    private TurnSystem _turnSystem;
    private PathfindingProviderSystem _pathfindingProviderSystem;


    public void Inject(DIContainer container)
    {
        Register();
        GetEntity();       
        Init(container);
    }

    private void Register()
    {
        _turnSystem = new TurnSystem();
    }

    private void GetEntity()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        //Инициализирую PathfindingGridDate
        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithPresent<PathfindingGridDate>().Build(entityManager);// способ  Создадия конструктора для обращения к сущности у которых есть PathfindingGridDate
        Entity pathfindingEntity = entityQuery.GetSingletonEntity(); //Получим сущность

        //Получим компонент и настроим его      
        PathfindingGridDate defaultPathNodeArray = entityManager.GetComponentData<PathfindingGridDate>(pathfindingEntity);
        defaultPathNodeArray.anchorGrid = transform.position;
        entityManager.SetComponentData(pathfindingEntity, defaultPathNodeArray);
        entityManager.SetComponentEnabled<PathfindingGridDate>(pathfindingEntity, true);

        //Получим систему
        _pathfindingProviderSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PathfindingProviderSystem>();//это НЕ работает с ISystem        
    }   

    private void Init(DIContainer container)
    {
        _cameraFollow.Init(
            container.Resolve<GameInput>(), 
            container.Resolve<OptionsSubMenuUIProvider>());
        
        FloorVisibility.Init(
            _levelGrid, 
            _cameraFollow); // инициализировать до спавна юнитов
        
        _mouseOnGameGrid.Init(
            container.Resolve<GameInput>(),
            _levelGrid);

        _pathfindingProviderSystem.Init(
           _unitActionSystem,
           _mouseOnGameGrid,
           _levelGrid);

        _unitSpawnerOnLevel.Init(
           container.Resolve<UnitManager>(),
           _turnSystem, container.Resolve<SoundManager>(),
           _levelGrid,
           _unitActionSystem,
           _cameraFollow,
           container.Resolve<HashAnimationName>(),
           _pathfindingProviderSystem);

        _unitActionSystem.Init(
           container.Resolve<GameInput>(),
           container.Resolve<UnitManager>(),
           _turnSystem,
           _levelGrid,
           _mouseOnGameGrid);       

        _levelGridVisual.Init(
            _unitActionSystem,
            _levelGrid,
            _mouseOnGameGrid,
            _pathfindingProviderSystem);

        _unitSelectAtLevelButtonsSystemUI.Init(
            container.Resolve<UnitManager>(),
            _turnSystem,
            _unitActionSystem,
            _cameraFollow);
        
        _optionsMenagerUI.Init(
            container.Resolve<UnitManager>(),
            _unitActionSystem);
        
        _actionButtonSystemUI.Init(
            _unitActionSystem,
            _turnSystem,
            container.Resolve<TooltipUI>());
        
        _enemyAI.Init(
            container.Resolve<UnitManager>(),
            _turnSystem,
            _unitActionSystem);               
        
        _gameEndUI.Init(
            container.Resolve<ScenesService>());
        
        _turnSystemUI.Init(
            _turnSystem);

        DoorInteract.Init(
            container.Resolve<SoundManager>(),
            _levelGrid,
            container.Resolve<HashAnimationName>());
        
        BarrelInteract.Init(
            container.Resolve<SoundManager>(),
            _levelGrid);
        
        DestructibleCrate.Init(
            container.Resolve<SoundManager>(), 
            _levelGrid);
        
        SphereInteract.Init(
            container.Resolve<SoundManager>(),
            _levelGrid);
        
        PathfindingLinkMonoBehaviour.Init(
            _levelGrid);
    }

   

}

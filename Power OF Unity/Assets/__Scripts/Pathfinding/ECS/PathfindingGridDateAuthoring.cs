using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Данные сетки поиска пути.<br/>
/// Полностью копирует основную сетку
/// </summary>

public class PathfindingGridDateAuthoring : MonoBehaviour
{
    public GridParamsSO _gridParamsSO;
     public SceneName _sceneName;
    public class Baker : Baker<PathfindingGridDateAuthoring>
    {
        public override void Bake(PathfindingGridDateAuthoring authoring)
        {
            LevelGridParameters levelGridParameters = authoring._gridParamsSO.GetLevelGridParameters(authoring._sceneName);

            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new PathfindingGridDate
            {
                width = levelGridParameters.width,
                height = levelGridParameters.height,
                cellSize = levelGridParameters.cellSize,
                floorAmount = levelGridParameters.floorAmount,
                floorHeight = levelGridParameters.floorHeight,                
            });
            SetComponentEnabled<PathfindingGridDate>(entity, false); // По умолчанию выключу
        }
    }
}




/// <summary>
/// Данные сетки поиска пути.<br/>
/// Полностью копирует основную сетку.<br/>
/// Якорь трансформа сетки будет определяться при инициализации в GameplayEntryPoint
/// </summary>
public struct PathfindingGridDate : IComponentData, IEnableableComponent
{
    public int width;     // Ширина
    public int height;    // Высота
    public float cellSize;  // Размер ячейки
    public int floorAmount; // Количество этажей
    public float floorHeight;// Высота этажа

    public float3 anchorGrid; //Якорь трансформа сетки будет определяться при инициализации в GameplayEntryPoint
}
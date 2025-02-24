using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// ������ ����� ������ ����.<br/>
/// ��������� �������� �������� �����
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
            SetComponentEnabled<PathfindingGridDate>(entity, false); // �� ��������� �������
        }
    }
}




/// <summary>
/// ������ ����� ������ ����.<br/>
/// ��������� �������� �������� �����.<br/>
/// ����� ���������� ����� ����� ������������ ��� ������������� � GameplayEntryPoint
/// </summary>
public struct PathfindingGridDate : IComponentData, IEnableableComponent
{
    public int width;     // ������
    public int height;    // ������
    public float cellSize;  // ������ ������
    public int floorAmount; // ���������� ������
    public float floorHeight;// ������ �����

    public float3 anchorGrid; //����� ���������� ����� ����� ������������ ��� ������������� � GameplayEntryPoint
}
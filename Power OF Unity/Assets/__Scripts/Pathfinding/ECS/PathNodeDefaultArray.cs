using Unity.Collections;
using Unity.Entities;
/// <summary>
/// Массив взех узлов пути по умолчанию.<br/> Добавить этот компонет к сущности PathNodeCreatorSystem
/// </summary>
public struct PathNodeDefaultArray : IComponentData
{   
    public NativeArray<PathNode> pathNodeArray;
}

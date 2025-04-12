using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Точки пути (Список мировых позиций)
/// </summary>
public struct PointsPath
{
    public NativeList<float3> worldPositionList;
}
using Pathfinding;
using UnityEngine;


public class PathfindingLinkMonoBehaviour : MonoBehaviour // ������ ��� ������ ���� // ������������ � ����� � ������� PathfindingLink
{

    public Vector3 linkPositionA;
    public Vector3 linkPositionB;

    private static LevelGrid _levelGrid;

    public PathfindingLink GetPathfindingLink() //�������� ������ ��� ������ ����
    {
        return new PathfindingLink
        {// ������� �������� ������� �� ������� �����������
            gridPositionA = _levelGrid.GetGridPosition(linkPositionA),
            gridPositionB = _levelGrid.GetGridPosition(linkPositionB)
        };
    }

    public static void Init(LevelGrid levelGrid)
    {
        _levelGrid = levelGrid;
    }

    private void Start()
    {
        // ������� ���� � ����� �������� � �������� ��
        GraphNode graphNodeA = AstarPath.active.GetNearest(linkPositionA).node;
        GraphNode graphNodeB = AstarPath.active.GetNearest(linkPositionB).node;
        uint cost = (uint)Mathf.RoundToInt(((Int3)(linkPositionA - linkPositionB)).costMagnitude);

        graphNodeA.AddConnection(graphNodeB, cost);
        graphNodeB.AddConnection(graphNodeA, cost);
    }

}

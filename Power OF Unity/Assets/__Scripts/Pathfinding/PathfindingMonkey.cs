//#define HEX_GRID_SYSTEM //������������ �������� ������� //  � C# ��������� ��� �������� �������������, ����������� ������� �� ������������� ��������� ���� ��������� ������������. 
//��� ��������� ���������� ������� ������������� ������ ��������� ����� �� ����������� � ��������� ��� � ��� �������� �����, ��� ��� ����������. 

using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

// �������� ������� ���������� ������� PathfindingMonkey, ������� � Project Settings/ Script Execution Order � �������� ���������� PathfindingMonkey ���� Default Time, ����� PathfindingMonkey ���������� ������ �� ���� ��� ��������� �������� ����� ���� 

public class PathfindingMonkey : MonoBehaviour // ����� ���� // ������ ������� ����� ������������ ������ ��� ������ ���� (����� �� ����� LevelGrid)
{

    public static PathfindingMonkey Instance { get; private set; }   //(������� SINGLETON) ��� �������� ������� ����� ���� ������� (SET-���������) ������ ���� �������, �� ����� ���� �������� GET ����� ������ �������
                                                                     // _instance - ���������, � ��� ����� ���� ��������� PathfindingMonkey ����� ���� ��� static. Instance ����� ��� ���� ����� ������ ������, ����� ����, ����� ����������� �� Event.



    private const int MOVE_STRAIGHT_COST = 10; // ��������� �������� ����� (��� �������� ����� 10 � �� 1 ��� �� �� ������������ float)
    private const int MOVE_DIAGONAL_COST = 14; // ��������� �������� �� ��������� ( ������������� �� ������� �������� ������ ���������� �� ����� ��������� ������� �������������� ������������. � ����� ��� �������� ����� 14 � �� 1,4 ��� �� �� ������������ float)

    //���������� ��� ������ ���������� ������ GetNeighbourList() ����� ��������
    // ���������� �������� ������� ��� ������ �������� �����
    private GridPositionXZ UP = new(0, 1,0);
    private GridPositionXZ DOWN = new(0, -1, 0);
    private GridPositionXZ RIGHT = new(1, 0, 0);
    private GridPositionXZ LEFT = new(-1, 0, 0);

    [SerializeField] private Transform _pathfindingGridDebugObject; // ������ ������� ����� //������������ ��� ������ ��������� � ����� ��������� ������ CreateDebugObject
    [SerializeField] private LayerMask _obstaclesCoverLayerMask; // ����� ���� ����������� (�������� � ����������) ���� ������� Obstacles � Cover// ����� �� ���� ������ � ���� ���������� ����� ����� -Obstacles ����� ������ � �� �������� Cover
    [SerializeField] private LayerMask _floorLayerMask; // ����� ���� ���� (�������� � ����������) ���� ������� MousePlane -��� ��� � ����
    [SerializeField] private Transform _pathfindingLinkContainer; // ��������� ������ ��� ������ ���� // � ���������� �������� �� ����� PathfindingLinkContainer
    
    private LevelGridParameters _gridParameters;
    private int _width;     // ������
    private int _height;    // ������
    private float _cellSize;// ������ ������
    private Vector3 _globalOffset = Vector3.zero; // �������� ����� � ������� �����������
    private int _floorAmount; // ���������� ������
    private List<GridSystemXZ<PathNodeMonkey>> _gridSystemList; //������ ������� �������� ������ � ����� PathNodeMonkey
    private List<PathfindingLink> _pathfindingLinkList; //������ ������ ��� ������ ���� (�.�. ������ �� ����� ������� ����� ���� ��������� - �������� ��� ������ � �������� ����� ���������� � 2 �����������)
    private List<GridPositionXZ> _gridPositionInAirList; // ������ ������� � �������

    private LevelGrid _levelGrid;

    private void Awake()
    {
        // ���� �� �������� � ���������� �� �������� �� �����
        if (Instance != null) // ������� �������� ��� ���� ������ ���������� � ��������� ����������
        {
            Debug.LogError("There's more than one PathfindingGridDate!(��� ������, ��� ���� PathfindingGridDate!) " + transform + " - " + Instance);
            Destroy(gameObject); // ��������� ���� ��������
            return; // �.�. � ��� ��� ���� ��������� PathfindingMonkey ��������� ����������, ��� �� �� ��������� ������ ����
        }
        Instance = this;
    }

    public void Init(LevelGrid levelGrid)
    {
        _levelGrid = levelGrid;
    }

    public void Setup(LevelGridParameters gridParameters, int floorAmount) // �������� ���� ������ �����
    {
        _width = gridParameters.width;
        _height = gridParameters.height;
        _cellSize = gridParameters.cellSize;
        _floorAmount = floorAmount;

        _gridSystemList = new List<GridSystemXZ<PathNodeMonkey>>();
        _gridPositionInAirList = new List<GridPositionXZ>();

        for (int floor = 0; floor < floorAmount; floor++)
        {
            GridSystemXZ<PathNodeMonkey> gridSystem = new GridSystemXZ<PathNodeMonkey>(_gridParameters, // �������� ����� 10 �� 10 � �������� 2 ������� �� ����� _floor c �������� ������� ����� � � ������ ������ �������� ������ ���� PathNodeMonkey
                    (GridSystemXZ<PathNodeMonkey> g, GridPositionXZ gridPosition) => new PathNodeMonkey(gridPosition), _levelGrid.transform.position, floor);   //� 5 ��������� ��������� ������� ������� �������� ����� ������ => new PathNodeMonkey(_gridPositioAnchor) � ��������� �� ��������. (������ ��������� ����� ������� � ��������� �����)

           // gridSystem.CreateDebugObject(_pathfindingGridDebugObject); // �������� ��� ������ � ������ ������// ���������� ������ ����� ������� �.�. ��������� ���������

            _gridSystemList.Add(gridSystem);
        }

        

        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                for (int flooor = 0; flooor < floorAmount; flooor++)
                {
                    GridPositionXZ gridPosition = new GridPositionXZ(x, z, flooor); // ������� �����
                    Vector3 worldPosition = _levelGrid.GetWorldPosition(gridPosition); // ������� ������� ����������
                    float raycastOffsetDistance = 1f; // ��������� �������� ����

                    GetNode(x, z, flooor).SetIsWalkable(false); //��� ������ ������� ��� ���� �������������

                    //��������� ���. // ��� ������� ���������� ��� �� ��� ��������� �������� ����� �������� ������ ����,  ������� ������� ��� �����, � ����� �������� ����, ����� ������ �������� � ��� ���� ������ ��� �������� ����, ��� ����� ����������������� � ��������� ������ ����
                    if (Physics.Raycast(
                         worldPosition + Vector3.up * raycastOffsetDistance,
                         Vector3.down,
                         raycastOffsetDistance * 2,
                         _floorLayerMask)) // ���� ��� ����� � ��� �� ������� ��� ���������� (Raycast -������ bool ����������)
                    {
                        GetNode(x, z, flooor).SetIsWalkable(true);
                    }
                    else // ���� ��� ������� ��� ���� �� ������� �� � ��������� ������
                    {
                        _gridPositionInAirList.Add(gridPosition);
                    }

                    //��������� ���. ��� �� ����� �������� ������ ���������(������ �����), ������� ������� ��� ����, � ����� �������� �����, ����� ������ �������� � ��� ���� ������ ��� �������� ����, ��� ����� ����������������� � ��������� ������ ����
                    // ����� ������� ��������� � UNITY � �� ������� ������� ����. Project Settings/Physics/Queries Hit Backfaces - ��������� �������, � ����� ����� �������� �� ����� ���������
                    if (Physics.Raycast(
                         worldPosition + Vector3.down * raycastOffsetDistance,
                         Vector3.up,
                         raycastOffsetDistance * 2,
                         _obstaclesCoverLayerMask)) // ���� ��� ����� � ����������� ��� � ������� �� ��������� ������ �� ���������� (Raycast -������ bool ����������)
                    {
                        GetNode(x, z, flooor).SetIsWalkable(false);
                    }
                }
            }
        }

        _pathfindingLinkList = new List<PathfindingLink>();

      //  if (_pathfindingLinkContainer.childCount !=0) // ���� ��������� �������� �������� ������� ��
       // {
            foreach (Transform pathfindingLinkTransform in _pathfindingLinkContainer) // ��������� �������� �������� ���������� ������ ��� ������ ����
            {
                if (pathfindingLinkTransform.TryGetComponent(out PathfindingLinkMonoBehaviour pathfindingLinkMonoBehaviour)) // � ��������� ������� ��������� ������� PathfindingLinkMonoBehaviour
                {
                    _pathfindingLinkList.Add(pathfindingLinkMonoBehaviour.GetPathfindingLink()); // // ������� �������� ������� � ������� � ������ ������ ��� ������ ����
                }
            }
       // }

    }

    public List<GridPositionXZ> FindPath(GridPositionXZ startGridPosition, GridPositionXZ endGridPosition, out int pathLength) // ����� ���� // ���� �� ����������� �������� ����� "out", �� ������� ������ ���������� �������� ��� ���� ���������� pathLength-����� ����, �� ���� ��� ������ ������� ��������.
    {
        List<PathNodeMonkey> openList = new List<PathNodeMonkey>();     // �������� ������ "�������� ������" - ��� ���� ������� ��������� ����� 
        List<PathNodeMonkey> closedList = new List<PathNodeMonkey>();   // �������� ������ "�������� ������" - ��� ����, � ������� ��� ��� ���������� �����

        PathNodeMonkey startNode = GetGridSystem(startGridPosition.floor).GetGridObject(startGridPosition); // ������� ��������� ����, ������ GridObjectUnitXZ ���� PathNodeMonkey � ���������� ��� startGridPosition
        PathNodeMonkey endNode = GetGridSystem(endGridPosition.floor).GetGridObject(endGridPosition); // ������� �������� ����, ������ GridObjectUnitXZ ���� PathNodeMonkey � ���������� ��� endGridPosition

        openList.Add(startNode); //������� � ������ ��������� ����

        for (int x = 0; x < _width; x++) // � ����� ������� ��� ���� �������� ������� � ������� ���������
        {
            for (int z = 0; z < _height; z++)
            {
                for (int flooor = 0; flooor < _floorAmount; flooor++)
                {
                    GridPositionXZ gridPosition = new GridPositionXZ(x, z, flooor); // ������� �����

                    PathNodeMonkey  pathNode = GetGridSystem(flooor).GetGridObject(gridPosition); // ������� ������ ����� ���� PathNodeMonkey

                    pathNode.SetGCost(int.MaxValue); // ��������� �������� G ������������ ������
                    pathNode.SetHCost(0);            // ��������� �������� H =0
                    pathNode.CalculateFCost();       // ��������� �������� F
                    pathNode.ResetCameFromPathNode(); // ������� ������ �� ���������� ���� ���� 
                }
            }
        }

        startNode.SetGCost(0); // G -��� ��������� �������� �� ����������� ���� � ���������� ������� ���������. �� ��� �� ������ �� ������������ ������� G=0
        startNode.SetHCost(CalculateHeuristicDistance(startGridPosition, endGridPosition)); // ��������� ����������� H ���������
        startNode.CalculateFCost();

        while (openList.Count > 0) // ���� � �������� ������ ���� �������� �� ��� �������� ��� ���� ���� ��� ������. ���� ����� �������� ���� �� ��������� ��� ������
        {
            PathNodeMonkey currentNode = GetLowestFCostPathNode(openList); // ������� ���� ���� � ���������� ���������� F �� openList � ������� ��� ������� �����

            if (currentNode == endNode) // ��������� ����� �� ��� ������� ���� ��������� ����
            {
                // �������� ��������� ����
                pathLength = endNode.GetGCost(); // ������ ��������� ����� ����
                return CalculatePath(endNode); // ������ ���������� ����
            }

            openList.Remove(currentNode); // ������ ������� ���� �� ��������� ������
            closedList.Add(currentNode);  // � ������� � �������� ������ // ��� �������� ��� �� ������ �� ����� ����

            foreach (PathNodeMonkey neighbourNode in GetNeighbourList(currentNode)) // ��������� ���� �������� ����
            {
                if (closedList.Contains(neighbourNode)) // ��������� ��� �� �������� ���� ����� � "�������� ������"
                {
                    //�� ��� ������ �� ����� ����
                    continue;
                }

                //  ��������� ���� �� ��������� ��� ������
                if (!neighbourNode.GetIsWalkable()) // ��������, �������� ���� - �������� ��� ������ // ���� �� �������� �� ������� � "�������� ������" � �������� � ���������� ����
                {
                    closedList.Add(neighbourNode);
                    continue;
                }
#if HEX_GRID_SYSTEM // ���� ������������ �������� �������

                int tentativeGCost =currentNode.GetGCost() + MOVE_STRAIGHT_COST;// ��������������� ��������� G = ������� G + ��������� �������� �����

#else//� ��������� ������ ������������� 
                int tentativeGCost = currentNode.GetGCost() + CalculateHeuristicDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition()); // ��������������� ��������� G = ������� G + ��������� ����������� �� �������� � ��������� ����
#endif

                if (tentativeGCost < neighbourNode.GetGCost()) // ���� ��������������� ��������� G ������ ��������� G ��������� ����(�� ��������� ��� ����� �������� MaxValue ) (�� ����� ����� ���� ��� �� ������� � ���� �������� ����)
                {
                    neighbourNode.SetCameFromPathNode(currentNode); // ���������� - �� �������� ���� ������ - � �������� ���� ����
                    neighbourNode.SetGCost(tentativeGCost); // ��������� �� �������� ���� ����������� �������� G
                    neighbourNode.SetHCost(CalculateHeuristicDistance(neighbourNode.GetGridPosition(), endGridPosition)); // ��������� �������� H �� �������� �� ���������
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode)) // ���� �������� ������ �� �������� ����� ��������� ���� �� ������� ���
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        // ���� �� ������
        pathLength = 0;
        return null;
    }

    public int CalculateHeuristicDistance(GridPositionXZ gridPositionA, GridPositionXZ gridPositionB) // ��������� �������������(���������������) ���������� ����� ������ 2�� ��������� ��������� ��� ����� �������� "H"
    {
#if HEX_GRID_SYSTEM // ���� ������������ �������� �������

        return Mathf.RoundToInt(MOVE_STRAIGHT_COST *Vector3.Distance(GetGridSystem(gridPositionA.floor).GetWorldPosition(gridPositionA), GetGridSystem(gridPositionB.floor).GetWorldPosition(gridPositionB)));


#else//� ��������� ������ ������������� 
        GridPositionXZ gridPositionDistance = gridPositionA - gridPositionB; //����� ���� ������������� (��������� ���������� ��������� ����� ������ � ������ ������ - �� ������� ���� �������� ������������ ������ �����)

        // ��� �������� ������ �� ������ (����� ������������ ��� ������ ����� ������)
        //int totalDistance = Mathf.Abs(gridPositionDistance.x) + Mathf.Abs(gridPositionDistance.y); // ������� ����� ��������� �� ������ (�� ��������� �������� �� ��������� ������ �� ������. �������� �� ����� (0,0) � ����� (3,2) �������� ��� ���� � ����� � ��� ���� ����� ����� 5)

        int xDistsnce = Mathf.Abs(gridPositionDistance.x); //����������� � (0,0) �� (1,1) ����� �������� 1 ����������, ������ ���� ����������� �� ������ // ���� � (0,0) �� (2,1) �� ��������� ����� ��� ����� ����. ������� ��� ������� ���������� ���������� ���� ����� ����������� ����� �� ���������� (x,y)
        int zDistsnce = Mathf.Abs(gridPositionDistance.z);
        int remaining = Mathf.Abs(xDistsnce - zDistsnce); // ���������� ��������� ����� ������������ �� ������, ����� �� ������
        int CalculateDistance = MOVE_DIAGONAL_COST * Mathf.Min(xDistsnce, zDistsnce) + MOVE_STRAIGHT_COST * remaining; // ������ ����������� ���������� � ������� GridPositionXZ ��� ����� �������� �� ��������� � �������� �� ������
        return CalculateDistance;
#endif
    }

    private PathNodeMonkey GetLowestFCostPathNode(List<PathNodeMonkey> pathNodeList) // �������� ���� ���� � ���������� ���������� F  � �������� ��������� ������ ����� ����// ��� ����������� ������ ����� �� ������ �� ���� ������
                                                                         // ���� ��������� ����� ����� ���������� ���������� F �� �������� ������ ���������� � ������
                                                                         // � ����� ������ �� ���������� ������ ������ ������� (������� �� ������������ ������ � ������ GetNeighbourList())
    {
        PathNodeMonkey lowestFCostPathNode = pathNodeList[0]; // ������� ������ ������� � ������
        for (int i = 0; i < pathNodeList.Count; i++) // ��������� � ����� ������ ��������
        {
            if (pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost()) // ���� �������� F ������ �������� ������ �������� �� ������� ��� lowestFCostPathNode
            {
                lowestFCostPathNode = pathNodeList[i];
            }
        }
        return lowestFCostPathNode;
    }

    private GridSystemXZ<PathNodeMonkey> GetGridSystem(int floor) // �������� �������� ������� ��� ������� �����
    {
        return _gridSystemList[floor];
    }

    private PathNodeMonkey GetNode(int x, int z, int floor) // �������� ���� � ������������ �� GridPositionXZ(x,y)
    {
        return GetGridSystem(floor).GetGridObject(new GridPositionXZ(x, z, floor));
    }

    private List<PathNodeMonkey> GetNeighbourList(PathNodeMonkey currentNode) // �������� ������ ������� ��� currentNode (��� ������������ 6 �������� , ��� ����������  8 �������
    {
        List<PathNodeMonkey> neighbourList = new List<PathNodeMonkey>(); // �������������� ����� ������ �������

        GridPositionXZ gridPosition = currentNode.GetGridPosition();

        // ������� �������� ����� �� ���� �� ������� ����� ����� ������� ������ ������ �� ������� ������

        if (gridPosition.x - 1 >= 0)
        {
            //Left
            neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0, gridPosition.floor)); // ������� ���� �����

#if HEX_GRID_SYSTEM // ���� ������������ �������� �������
// ��� ����������� ��� ������
#else//� ��������� ������ ������������� 
            if (gridPosition.z - 1 >= 0)
            {
                //Left Down
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1, gridPosition.floor)); // ������� ���� ����� � ����
            }

            if (gridPosition.z + 1 < _height)
            {
                //Left Up
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1, gridPosition.floor)); // ������� ���� ����� � �����
            }
#endif
        }

        if (gridPosition.x + 1 < _width)
        {
            //Right
            neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0, gridPosition.floor)); // ������� ���� ������

#if HEX_GRID_SYSTEM // ���� ������������ �������� �������
// ��� ����������� ��� ������
#else//� ��������� ������ ������������� 
            if (gridPosition.z - 1 >= 0)
            {
                //Right Down
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1, gridPosition.floor)); // ������� ���� ������ � ����
            }

            if (gridPosition.z + 1 < _height)
            {
                //Right Up
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1, gridPosition.floor)); // ������� ���� ������ � �����
            }
#endif
        }

        // ��� ������������� ���� �������� - �� ����� ����� � ����� � �����. ������ - �� ������ � ����� � ������ �����. (������� � ������ ������� ��������� ����� �������� oddRow)
        if (gridPosition.z - 1 >= 0)
        {
            //Down
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1, gridPosition.floor)); // ������� ���� �����
        }
        if (gridPosition.z + 1 < _height)
        {
            //Up
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1, gridPosition.floor)); // ������� ���� ������
        }


#if HEX_GRID_SYSTEM // ���� ������������ �������� �������

        bool oddRow = gridPosition.z % 2 == 1; // oddRow - �������� ���.���� ������ �� �� ��������� � �������� ����

        if (oddRow) // ���� ��������
        {
            if (gridPosition.x + 1 < _width)
            {
                if (gridPosition.z - 1 >= 0)
                {
                    neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1, gridPosition.floor));// ������� ���� ������ �����
                }
                if (gridPosition.z + 1 < _height)
                {
                    neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1, gridPosition.floor)); // ������� ���� ������ � �����
                }
            }
        }
        else // ���� ������
        {
            if (gridPosition.x - 1 >= 0)
            {
                if (gridPosition.z - 1 >= 0)
                {
                    neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1, gridPosition.floor)); // ������� ���� ����� �����
                }
                if (gridPosition.z + 1 < _height)
                {
                    neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1, gridPosition.floor)); // ������� ���� ����� � �����
                }
            }
        }
#endif

        List<PathNodeMonkey> totalNeighbourList = new List<PathNodeMonkey>(); // ����� ������ �������
        totalNeighbourList.AddRange(neighbourList); //AddRange- ��������� �������� ��������� ��������� � ����� ������

        List<GridPositionXZ> pathfindingLinkGridPositionList = GetPathfindingLinkConnectedGridPositionList(gridPosition); // �������� ������ �������� ������� ������� ������� � ���� �������� ��������

        foreach (GridPositionXZ pathfindingLinkGridPosition in pathfindingLinkGridPositionList) // ��������� ������ � ����������� GridPositionXZ(�������� �������) � PathNodeMonkey(���� ����)
        {
            totalNeighbourList.Add(
                GetNode( // ������� ���� � ������������
                    pathfindingLinkGridPosition.x,
                    pathfindingLinkGridPosition.z,
                    pathfindingLinkGridPosition.floor
                )
            );
        }


        return totalNeighbourList;
    }

    private List<GridPositionXZ> GetPathfindingLinkConnectedGridPositionList(GridPositionXZ gridPosition) // �������� ������ �������� ������� ������� ������� � ���� �������� ��������
    {
        List<GridPositionXZ> gridPositionList = new List<GridPositionXZ>(); // �������������� ������

        foreach (PathfindingLink pathfindingLink in _pathfindingLinkList) // ��������� ����� ������ ��� ������ ����
        {
            // ���� ���� �������� ������� ���������  � ����� �� ������� ������ �� ������� � ������ ������ ������� ������
            if (pathfindingLink.gridPositionA == gridPosition)
            {
                gridPositionList.Add(pathfindingLink.gridPositionB);
            }
            if (pathfindingLink.gridPositionB == gridPosition)
            {
                gridPositionList.Add(pathfindingLink.gridPositionA);
            }
        }

        return gridPositionList;
    }

   /* //2 ������ ���������� ������ GetNeighbourList() ����� ��������
    private List<PathNodeMonkey> GetNeighbourList(PathNodeMonkey currentNode) // �������� ������ ������� ��� currentNode
    {
        List<PathNodeMonkey> neighbourList = new List<PathNodeMonkey>(); // �������������� ����� ������ �������

        GridPositionXZ _gridPositioAnchor = currentNode.GetGridPosition(); // ������� �������� ������� ������ ������

        GridPositionXZ[] neigboursPositions = //�������� ������ �������� ������� �������� �����
        {
         _gridPositioAnchor + UP,
         _gridPositioAnchor + UP + RIGHT,
         _gridPositioAnchor + RIGHT,
         _gridPositioAnchor + RIGHT + DOWN,
         _gridPositioAnchor + DOWN,
         _gridPositioAnchor + DOWN + LEFT,
         _gridPositioAnchor + LEFT,
         _gridPositioAnchor + LEFT + UP
         };

        foreach (GridPositionXZ p in neigboursPositions) // � ����� �������� �� ������������ ���� �������� �������
        {
            if (_itemGridSystemXYList.IsValidGridPosition(p))
            {
                neighbourList.Add(GetNode(p.x, p.y));
            }
        }

        return neighbourList;
    }*/

    private List<GridPositionXZ> CalculatePath(PathNodeMonkey endNode) //���������� ���� (����� ����������� ������ � �������� �����������)
    {
        List<PathNodeMonkey> pathNodeList = new List<PathNodeMonkey>(); // �������������� ������ ����� ����
        pathNodeList.Add(endNode);
        PathNodeMonkey currentNod = endNode;

        while (currentNod.GetCameFromPathNode() != null) //� ����� ������� ������������ ���� � ������. ������������ ���� - ��� �� � �������� �� ���� ������ (� ���� ����� 1 ������ �� �������� � �������� ������). �������� ���� ����� ��������� � � ���� GetCameFromPathNode() = null - ���� ���������
        {
            pathNodeList.Add(currentNod.GetCameFromPathNode());//������� � ������ ��� ������������ ����
            currentNod = currentNod.GetCameFromPathNode(); // ������������ ���� ����������� �������
        }

        pathNodeList.Reverse(); // �.�. �� ������ � ����� ���� ����������� ��� ������ ��� �� �������� �������� �� ������ � ������
        // ��������� ��� ������ "PathNodeMonkey-�����" � ������ "GridPositionXZ - ������� �����"
        List<GridPositionXZ> gridPositionList = new List<GridPositionXZ>();

        foreach (PathNodeMonkey pathNode in pathNodeList)
        {
            gridPositionList.Add(pathNode.GetGridPosition());
        }

        return gridPositionList;
    }

    public void SetIsWalkableGridPosition(GridPositionXZ gridPosition, bool isWalkable) // ���������� ��� ����� ��� ������ (� ����������� �� isWalkable)  ������ �� ���������� � �������� �������� �������
    {
        GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    }

    public bool IsWalkableGridPosition(GridPositionXZ gridPosition) // ����� ������ �� ���������� � �������� �������� �������
    {
        return GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).GetIsWalkable();
    }

    public bool HasPath(GridPositionXZ startGridPosition, GridPositionXZ endGridPosition) // ����� ���� ?
    {
        return FindPath(startGridPosition, endGridPosition, out int pathLength) != null; // ���� ���� ���� �� startGridPosition � endGridPosition �� �������� true ���� ���� ��� �������� false (out int pathLength �������� ��� �� ��������������� ���������)
    }

    public int GetPathLength(GridPositionXZ startGridPosition, GridPositionXZ endGridPosition) // �������� ����� ���� (�������� F -endGridPosition)  
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }

    public List<GridPositionXZ> GetGridPositionInAirList() //�������� ������ ������� � �������
    {
        return _gridPositionInAirList;
    }

}

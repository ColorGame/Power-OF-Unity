using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;


// �������� ������� ���������� ������� LevelGrid, ������� � Project Settings/ Script Execution Order � �������� ���������� LevelGrid ���� Default Time, ����� LevelGrid ���������� ������ �� ���� ��� ��������� �������� ����� ���� ( � StartScene() �� ��������� ����� PathfindingMonkey - ��������� ������ ����)

public class LevelGrid : MonoBehaviour // �������� ������ ������� ��������� ������ ������� ������ . �������� ������ ��������� ��� �������� ������������� ����� � �������� ������� �����
{
    public event EventHandler<GridPositionXZ> OnAddUnitAtGridPosition; 
    public event EventHandler<GridPositionXZ> OnRemoveUnitAtGridPosition; 


    [SerializeField] private Transform _gridDebugObjectPrefab; // ������ ������� ����� //������������ ��� ������ ��������� � ����� ��������� ������ CreateDebugObject
    [SerializeField] private GridParamsSO _levelGridPAramsSO;
    [SerializeField] private SceneName _sceneName;

    private LevelGridParameters _gridParameters;
    private List<GridSystemXZ<GridObjectUnitXZ>> _gridSystemList; //������ �������� ������ .� �������� ������� ��� GridObjectUnitXZ

    private void Awake()
    {
        _gridParameters = _levelGridPAramsSO.GetLevelGridParameters(_sceneName);
        _gridSystemList = new List<GridSystemXZ<GridObjectUnitXZ>>(); // �������������� ������

        for (int floor = 0; floor < _gridParameters.floorAmount; floor++) // ��������� ����� � �� ������ �������� �������� �������
        {
            GridSystemXZ<GridObjectUnitXZ> gridSystem = new GridSystemXZ<GridObjectUnitXZ>(_gridParameters, // �������� ����� � � ������ ������ �������� ������ ���� GridObjectUnitXZ
                 (GridSystemXZ<GridObjectUnitXZ> gridSystem, GridPositionXZ gridPosition) => new GridObjectUnitXZ(gridSystem, gridPosition),transform.position, floor); //� 5 ��������� ��������� ������� ������� �������� ����� ������ => new GridObjectUnitXZ(gridSystem, _gridPositioAnchor) � ��������� �� ��������. (������ ��������� ����� ������� � ��������� �����)

         //  gridSystem.CreateDebugObject(_gridDebugObjectPrefab); // �������� ��� ������ � ������ ������ // �������������� �.�. PathfindingGridDebugObject ����� ��������� ��������������� ������ _gridDebugObjectPrefab

            _gridSystemList.Add(gridSystem); // ������� � ������ ��������� �����
        }
    }  

    private GridSystemXZ<GridObjectUnitXZ> GetGridSystem(int floor) // �������� �������� ������� ��� ������� �����
    {
        return _gridSystemList[floor];
    }    

    public List<Unit> GetUnitListAtGridPosition(GridPositionXZ gridPosition) // �������� ������ ������ � �������� ������� �����
    {
        GridObjectUnitXZ gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition); // ������� GridObjectUnitXZ ������� ��������� � _gridPositioAnchor
        return gridObject.GetUnitList();// ������� �����
    }

    public void AddUnitAtGridPosition(GridPositionXZ gridPosition, Unit unit) // �������� ������������� ����� � �������� ������� �����
    {
        GridObjectUnitXZ gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition); // ������� GridObjectUnitXZ ������� ��������� � _gridPositioAnchor
        gridObject.AddUnit(unit); // �������� ����� 
        OnAddUnitAtGridPosition?.Invoke(this, gridPosition);
    }

    public void RemoveUnitAtGridPosition(GridPositionXZ gridPosition, Unit unit) // �������� ����� �� �������� ������� �����
    {
        GridObjectUnitXZ gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition); // ������� GridObjectUnitXZ ������� ��������� � _gridPositioAnchor
        gridObject.RemoveUnit(unit); // ������ �����
        OnRemoveUnitAtGridPosition?.Invoke(this, gridPosition);
    }

    public void UnitMovedGridPosition(Unit unit, GridPositionXZ fromGridPosition, GridPositionXZ toGridPosition) // ���� ��������� � �������� ������� �� ������� fromGridPosition � ������� toGridPosition
    {
        RemoveUnitAtGridPosition(fromGridPosition, unit); // ������ ����� �� ������� ������� �����
        AddUnitAtGridPosition(toGridPosition, unit);  // ������� ����� � ��������� ������� �����
    }

    public int GetFloor(Vector3 worldPosition) // �������� ����
    {
        return Mathf.RoundToInt(worldPosition.y / _gridParameters.floorHeight); // ������� ������� �� � �� ������ ����� � �������� �� ������ ��� ����� ������� ����
    }

    // ��� �� �� ���������� ��������� ���������� LevelGrid (� �� ������ ��������� ����_gridSystem) �� ������������ ������ � GridPositionXZ ������� �������� ������� ��� ������� � GridPositionXZ
    public GridPositionXZ GetGridPosition(Vector3 worldPosition) // ������� �������� ������� ��� ������� ���������
    {
        int floor = GetFloor(worldPosition); // ������ ����
        return GetGridSystem(floor).GetGridPosition(worldPosition); // ��� ����� ����� ������ �������� �������
    }

   /* /// <summary>
    /// �������� ���� ����� - ���� �����, ��� ���������� ��������� ������ �����, � ����� ��������������� ������������������, �� 
    /// ����� �������� ������ ���� �� ���������� �������, ��� ������� �������.
    /// </summary>
    public LevelGridNode GetGridNode(GridPositionXZ gridPosition) // �������� ���� ����� (A* PathfindingGridDate Project4.2.18) ��� ����� �������� ������� ()
    {
        int width = AstarPath.active.data.layerGridGraph.width;
        int depth = AstarPath.active.data.layerGridGraph.depth;
        *//*int layerCount = AstarPath.active.data.layerGridGraph.LayerCount;
        float nodeSize = AstarPath.active.data.layerGridGraph.nodeSize;     *//*

        LevelGridNode gridNode = (LevelGridNode)AstarPath.active.data.layerGridGraph.nodes[gridPosition.x + gridPosition.z * width + gridPosition.floor * width * depth];

        *//*// �������� ���������� GraphNode � GridPositionXZ  
        Debug.Log("x " + _gridPositioAnchor.x + "y " + _gridPositioAnchor.y +"Floor " + _gridPositioAnchor._floor + 
            " node" + " x " + gridNode.XCoordinateInGrid + "y " + gridNode.ZCoordinateInGrid +"Layer " + gridNode.LayerCoordinateInGrid );*//*
        return gridNode;
    }*/

  /*  /// <summary>
    /// True, ���� ���� �������� ��� �������.
    /// </summary>
    public bool WalkableNode(GridPositionXZ gridPosition)
    {
       return GetGridNode(gridPosition).Walkable;
    }
*/
    public Vector3 GetWorldPosition(GridPositionXZ gridPosition) => GetGridSystem(gridPosition.floor).GetWorldPosition(gridPosition); // �������� �������

    public bool IsValidGridPosition(GridPositionXZ gridPosition) // �������� �� ���������� �������� ��������
    {
        if (gridPosition.floor < 0 || gridPosition.floor >= _gridParameters.floorAmount) // ������� �� ������� ����� ������
        {
            return false;
        }
        else
        {
            return GetGridSystem(gridPosition.floor).IsValidGridPosition(gridPosition); // �������� ������� ��� ��������� ������� � IsValidGridPosition �� _itemGridSystemXYList
        }

    }
    public int GetWidth() => GetGridSystem(0).GetWidth(); // ��� ����� ����� ���������� ����� ����� �� ����� 0 ����
    public int GetHeight() => GetGridSystem(0).GetHeight();
    public float GetCellSize() => GetGridSystem(0).GetCellSize();
    public int GetFloorAmount() => _gridParameters.floorAmount;
    public float GetFloorHeght()=> _gridParameters.floorHeight;
    public Vector3 GetAnchorGrid() =>transform.position;

    public bool HasAnyUnitOnGridPosition(GridPositionXZ gridPosition) // ���� �� ����� ������ ���� �� ���� �������� �������
    {
        GridObjectUnitXZ gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition); // ������� GridObjectUnitXZ ������� ��������� � _gridPositioAnchor
        return gridObject.HasAnyUnit();
    }
    public Unit GetUnitAtGridPosition(GridPositionXZ gridPosition) // �������� ����� � ���� �������� �������
    {
        GridObjectUnitXZ gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition); // ������� GridObjectUnitXZ ������� ��������� � _gridPositioAnchor
        return gridObject.GetUnit();
    }


    // IInteractable ��������� �������������� - ��������� � ������ InteractAction ����������������� � ����� �������� (�����, �����, ������...) - ������� ��������� ���� ���������
    public IInteractable GetInteractableAtGridPosition(GridPositionXZ gridPosition) // �������� ��������� �������������� � ���� �������� �������
    {
        GridObjectUnitXZ gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition); // ������� GridObjectUnitXZ ������� ��������� � _gridPositioAnchor
        return gridObject.GetInteractable();
    }
    public void SetInteractableAtGridPosition(GridPositionXZ gridPosition, IInteractable interactable) // ���������� ���������� ��������� �������������� � ���� �������� �������
    {
        GridObjectUnitXZ gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition); // ������� GridObjectUnitXZ ������� ��������� � _gridPositioAnchor
        gridObject.SetInteractable(interactable);
    }
    public void ClearInteractableAtGridPosition(GridPositionXZ gridPosition) // �������� ��������� �������������� � ���� �������� �������
    {
        GridObjectUnitXZ gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition); // ������� GridObjectUnitXZ ������� ��������� � _gridPositioAnchor
        gridObject.ClearInteractable(); // �������� ��������� �������������� � ���� �������� �������
    }

}

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static PlacedObjectTypeSO;

public class PlacedObject : MonoBehaviour // ����������� ������ (������� � ��������� ������ �� �����)
{
    public static PlacedObject CreateInGrid(GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY, Vector2Int gridPosition, PlacedObjectTypeSO.Dir dir, PlacedObjectTypeSO placedObjectTypeSO, Transform parent) // (static ���������� ��� ����� ����������� ������ � �� ������ ������ ����������)
    {        
        Vector3 offset = placedObjectTypeSO.GetOffsetVisualFromParent(); // �������� �������� ����� � ���������� ��������� ������� � ������ worldPosition
        Vector3 worldPosition = InventoryGrid.Instance.GetWorldPositionLowerLeft�ornerCell(gridPosition, gridSystemXY);
        Transform placedObjectTransform = Instantiate(placedObjectTypeSO.prefab, worldPosition, Quaternion.Euler(parent.rotation.eulerAngles.x, 0, placedObjectTypeSO.GetRotationAngle(dir)), parent); //parent.rotation.eulerAngles.x- ��� �� ��� �������� ��� ��������
        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        placedObject._placedObjectTypeSO = placedObjectTypeSO;
        placedObject._dir = dir;
        placedObject._offsetVisualFromParent = offset;       
        placedObject.Setup();        

        return placedObject;
    }

    public static PlacedObject CreateInWorld(Vector3 worldPosition, PlacedObjectTypeSO.Dir dir, PlacedObjectTypeSO placedObjectTypeSO, Transform parent)
    {
        Vector3 offset = placedObjectTypeSO.GetOffsetVisualFromParent(); // �������� �������� ����� ������� ������ � ������ worldPosition
        Transform placedObjectTransform = Instantiate(placedObjectTypeSO.prefab, worldPosition - offset, Quaternion.Euler(parent.rotation.eulerAngles.x, 0, placedObjectTypeSO.GetRotationAngle(dir)), parent); //parent.rotation.eulerAngles.x- ��� �� ��� �������� ��� ��������

        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        placedObject._placedObjectTypeSO = placedObjectTypeSO;
        placedObject._dir = dir;
        placedObject._offsetVisualFromParent = offset;      
        placedObject.Setup();

        return placedObject;
    }

    public static PlacedObject CreateCanvas(Transform parent, Vector2 anchoredPosition, Vector2Int gridPosition, PlacedObjectTypeSO.Dir dir, PlacedObjectTypeSO placedObjectTypeSO)
    {
        Transform placedObjectTransform = Instantiate(placedObjectTypeSO.prefab, parent);
        placedObjectTransform.rotation = Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0);
        placedObjectTransform.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;

        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        placedObject._placedObjectTypeSO = placedObjectTypeSO;       
        placedObject._dir = dir;
        placedObject.Setup();

        return placedObject;
    }


    private PlacedObjectTypeSO _placedObjectTypeSO;
    private GridSystemTiltedXY<GridObjectInventoryXY> _gridSystemXY; // ����� � ������� ������������ ��� ������
    private Vector2Int _gridPositioAnchor; // �������� ������� �����
    private PlacedObjectTypeSO.Dir _dir;
    private Vector3 _targetRotation;
    private Vector3 _targetPosition;
    private Vector3 _startPosition;
    private Vector3 _scaleOriginal;
    private bool _grabbed; // �������   
    private bool _moveStartPosition = false; // ����������� � ��������� �������    
    private Transform _visual;
    private Vector3 _offsetVisualFromParent;


    private void Start()
    {
        Setup(); // ���� ������ �� ������� ����� �� ����� ��������� ���
    }

    private void LateUpdate()
    {
        if (_grabbed) // ���� ������ ����� ��
        {
            float moveSpeed = 20f;
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * moveSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_targetRotation), Time.deltaTime * 15f);
            //transform.rotation = Quaternion.Lerp(transform.rotation, PickUpDropSystem.Instance.GetPlacedObjectRotation(), Time.deltaTime * 15f);// ������ �������� ������
        }

        if (_moveStartPosition) // ���� ���� ����������� � ��������� ������� � � ����� ��������� ������
        {
            float moveSpeed = 20f;
            transform.position = Vector3.Lerp(transform.position, _startPosition, Time.deltaTime * moveSpeed);

            float stoppingDistance = 0.1f; // ��������� ��������� //����� ���������//
            if (Vector3.Distance(transform.position, _startPosition) < stoppingDistance)  // ���� ��������� �� ������� ������� ������ ��� ��������� ��������� // �� �������� ����        
            {
                _moveStartPosition = false; //���������� �������� �� �������� _startPosition               
                Destroy(gameObject);
            }
        }
    }

    protected virtual void Setup()
    {
        _visual = transform.GetChild(0); //������� ���������� ������   
        _visual.localPosition = _offsetVisualFromParent;    // ��������� � �������� ���������� ����� ��� ���������� ������ (���� ������ ���������� ������� � ������� ���������)           
        _startPosition = transform.position;  // �������� ��������� �������
        _scaleOriginal = transform.localScale;
    }

    public Vector3 GetOffsetVisualFromParent()
    {
        return _offsetVisualFromParent;
    }

    public void Grab() // ���������
    {
        _grabbed = true;
        //�������� ������������ �������    
        float _scaleMultiplier = 1.1f; // ��������� �������� ��� ������� ���������� ��� �������
        transform.localScale = _scaleOriginal * _scaleMultiplier;
    }

    public void Drop() // �������
    {
        _grabbed = false;
        transform.localScale = _scaleOriginal;
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
    }

    public void SetTargetRotation(Vector3 targetRotation)
    {
        _targetRotation = targetRotation;
    }  

    public Vector2Int GetGridPositionAnchor()
    {
        return _gridPositioAnchor;
    }

    public void SetGridPositionAnchor(Vector2Int gridPosition)
    {
        _gridPositioAnchor = gridPosition;
    }

    public GridSystemTiltedXY<GridObjectInventoryXY> GetGridSystemXY() //������� ����� �� ������� �������� ��� �������
    {
        return _gridSystemXY;
    }

    public void SetGridSystemXY(GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY) //��������� ����� �� ������� �������� ��� �������
    {
        _gridSystemXY = gridSystemXY;
    }

    public List<Vector2Int> GetOccupiesGridPositionList() // �������� ������ ���������� ������� � �����. 
    {
        return _placedObjectTypeSO.GetGridPositionList(_gridPositioAnchor, _dir); // (� �������� ��������� ��� ���������� �������� ������� � ����������)
    }


    public List<Vector2Int> GetTryOccupiesGridPositionList(Vector2Int gridPosition) //�������� ������ ������� � �����, ������� �������� ������. (� �������� ��������� �������� ������� ��� ����� ����������)
    {
        return _placedObjectTypeSO.GetGridPositionList(gridPosition, _dir);
    }

    public PlacedObjectTypeSO.Dir GetDir() // �������� �����������
    {
        return _dir;
    }

    public override string ToString()
    {
        return _placedObjectTypeSO.nameString;
    }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO()
    {
        return _placedObjectTypeSO;
    }

    public Vector3 GetStartPosition()
    {
        return _startPosition;
    }

    public void SetStartPosition(Vector3 startPosition)
    {
        _startPosition = startPosition;
    }

    public void SetMoveStartPosition(bool moveStartPosition) // ���������� ���� ������������ � ��������� �������
    {
        _moveStartPosition = moveStartPosition;
    }

    public virtual void DestroySelf() // ���������� ����
    {
        Destroy(gameObject);
    }
}

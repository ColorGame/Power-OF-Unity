using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// ��������� ������������ �������(������� ��������� �� �����, � ����).
/// </summary>
/// <remarks>
/// ���������� � ���������������� ������� 
/// </remarks>
public class PlacedObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// �������� ��������� PlacedObject � �������� ���������� � ����� ���������.
    /// </summary>
    /// <remarks>����� ��������� PlacedObject, ���� �� �������� ����������</remarks>
    public static PlacedObject CreateAddTryPlacedInGrid(PlacedObjectParameters placedObjectParameters, InventoryGrid inventoryGrid, PickUpDropPlacedObject pickUpDropPlacedObject) // (static ���������� ��� ����� ����������� ������ � �� ������ ������ ����������)
    {
        PlacedObjectTypeSO placedObjectTypeSO = placedObjectParameters.placedObjectTypeSO;
        Vector2Int gridPositionAnchor = placedObjectParameters.gridPositioAnchor;
        GridSystemXY<GridObjectInventoryXY> gridSystemXY = inventoryGrid.GetGridSystemXY(placedObjectParameters.slot);
        Transform canvasContainer = inventoryGrid.GetCanvas().transform;
        Vector3 worldPosition = gridSystemXY.GetWorldPositionLowerLeft�ornerCell(gridPositionAnchor);
        Transform placedObjectTransform = Instantiate(placedObjectTypeSO.GetPrefab2D(), worldPosition, Quaternion.Euler(canvasContainer.rotation.eulerAngles.x, 0, 0), canvasContainer); //canvasContainer.rotation.eulerAngles.x- ��� �� ��� �������� ��� ��������
        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        if (inventoryGrid.TryAddPlacedObjectAtGridPosition(gridPositionAnchor, placedObject, gridSystemXY))
        {
            Destroy(placedObject.gameObject);           
        }
        placedObject._placedObjectTypeSO = placedObjectTypeSO;
        placedObject._offsetVisualFromParent = placedObjectTypeSO.GetOffsetVisualFromParent();
        placedObject._pickUpDropPlacedObject = pickUpDropPlacedObject;
        placedObject.Setup();

        return placedObject;
    }

    /// <summary>
    /// �������� ��������� PlacedObject � ������� ������������
    /// </summary>   
    /// <remarks>
    /// ��������� ���������� � ����� ���������� "worldPosition".
    /// </remarks>
    public static PlacedObject CreateInWorld(Vector3 worldPosition, PlacedObjectTypeSO placedObjectTypeSO, Transform parent, PickUpDropPlacedObject pickUpDropPlacedObject)
    {
        Vector3 offset = placedObjectTypeSO.GetOffsetVisualFromParent(); // �������� �������� ����� ������� ������ � ������ worldPosition
        Transform placedObjectTransform = Instantiate(placedObjectTypeSO.GetPrefab2D(), worldPosition - offset, Quaternion.Euler(parent.rotation.eulerAngles.x, 0, 0), parent); //canvasContainer.rotation.eulerAngles.x- ��� �� ��� �������� ��� ��������
        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        placedObject._placedObjectTypeSO = placedObjectTypeSO;
        placedObject._offsetVisualFromParent = offset;
        placedObject._pickUpDropPlacedObject = pickUpDropPlacedObject;
        placedObject.Setup();

        return placedObject;
    }

    private PlacedObjectTypeSO _placedObjectTypeSO;
    private GridSystemXY<GridObjectInventoryXY> _gridSystemXY; // ����� � ������� ������������ ��� ������
    private PickUpDropPlacedObject _pickUpDropPlacedObject;
    private Vector2Int _gridPositioAnchor; // �������� ������� �����  
    private Vector3 _targetRotation;
    private Vector3 _targetPosition;
    private Vector3 _startPosition;
    private Vector3 _scaleOriginal;
    private bool _grabbed; // �������   
    private bool _moveStartPosition = false; // ����������� � ��������� �������        
    private Vector3 _offsetVisualFromParent;
    private List<InventorySlot> _canPlacedOnSlotList;// ����� ��������� ��� ����� ���������� ��� ������   

    protected virtual void Setup()
    {
        _canPlacedOnSlotList = _placedObjectTypeSO.GetCanPlacedOnSlotList();
        _scaleOriginal = transform.localScale; // �������� ������������ �������
        _startPosition = transform.position;  // �������� ��������� �������       
    }



    private void LateUpdate()
    {
        if (_grabbed) // ���� ������ ����� ��
        {
            float moveSpeed = 20f;
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * moveSpeed);
            if (_targetRotation != Vector3.zero)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_targetRotation), Time.deltaTime * 15f);
            }
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

    public void SetTargetPosition(Vector3 targetPosition) { _targetPosition = targetPosition; }
    public void SetTargetRotation(Vector3 targetRotation) { _targetRotation = targetRotation; }
    public Vector3 GetOffsetVisualFromParent() { return _offsetVisualFromParent; }
    public Vector2Int GetGridPositionAnchor() { return _gridPositioAnchor; }
    public void SetGridPositionAnchor(Vector2Int gridPosition) { _gridPositioAnchor = gridPosition; }

    /// <summary>
    /// ������� ����� �� ������� �������� ��� �������
    /// </summary>
    public GridSystemXY<GridObjectInventoryXY> GetGridSystemXY() { return _gridSystemXY; }

    /// <summary>
    /// ��������� ����� �� ������� �������� ��� �������
    /// </summary>
    public void SetGridSystemXY(GridSystemXY<GridObjectInventoryXY> gridSystemXY) { _gridSystemXY = gridSystemXY; }

    /// <summary>
    /// �������� ������ ���������� ������� � �����. (� �������� ��������� ��� ���������� �������� ������� � ����������)
    /// </summary>
    public List<Vector2Int> GetOccupiesGridPositionList() { return _placedObjectTypeSO.GetGridPositionList(_gridPositioAnchor); }

    /// <summary>
    /// �������� ������ ������� � �����, ������� �������� ������. (� �������� ��������� �������� ������� ��� ����� ����������)
    /// </summary>
    public List<Vector2Int> GetTryOccupiesGridPositionList(Vector2Int gridPosition) { return _placedObjectTypeSO.GetGridPositionList(gridPosition); }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO() { return _placedObjectTypeSO; }
    public Vector3 GetStartPosition() { return _startPosition; }
    public void SetStartPosition(Vector3 startPosition) { _startPosition = startPosition; }
    /// <summary>
    /// ���������� ����, ��� ���� ������������ � ��������� �������
    /// </summary>    
    public void SetFlagMoveStartPosition(bool moveStartPosition) { _moveStartPosition = moveStartPosition; }
        
    /// <summary>
    /// �������� ���� ��� ����� ���������� ��� ������
    /// </summary>
    public List<InventorySlot> GetCanPlacedOnGridList() { return _canPlacedOnSlotList; }

    public PlacedObjectParameters GetPlacedObjectParameters()
    {
        return new PlacedObjectParameters(_gridSystemXY.GetGridSlot(), _gridPositioAnchor, this);
    }

    public void OnPointerEnter(PointerEventData eventData) /// ���� ��� ��� ���� �������� �� ��������� ���� ������ � PickUpDropPlacedObject
    {
        _pickUpDropPlacedObject.SetPlacedObjectMouseEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _pickUpDropPlacedObject.SetPlacedObjectMouseEnter(null);
    }

}

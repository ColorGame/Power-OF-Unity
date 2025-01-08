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
    /// �������� ��������� PlacedObject � �����.
    /// </summary>
    public static PlacedObject CreateInGrid(PlacedObjectGridParameters placedObjectParameters, PickUpDropPlacedObject pickUpDropPlacedObject, GridSystemXY<GridObjectEquipmentXY> gridSystemXY) // (static ���������� ��� ����� ����������� ������ � �� ������ ������ ����������)
    {
        PlacedObjectTypeSO placedObjectTypeSO = placedObjectParameters.placedObjectTypeSO;
        Vector2Int gridPositionAnchor = placedObjectParameters.gridPositioAnchor;     
        Canvas canvas = pickUpDropPlacedObject.GetCanvas();

        Vector3 worldPosition = new Vector3();
        switch (placedObjectParameters.slot)
        {          
            case EquipmentSlot.BagSlot:
                 worldPosition = gridSystemXY.GetWorldPositionLowerLeft�ornerCell(gridPositionAnchor);
                break;    
                
            case EquipmentSlot.MainWeaponSlot:
            case EquipmentSlot.OtherWeaponsSlot:
            case EquipmentSlot.HeadArmorSlot:
            case EquipmentSlot.BodyArmorSlot:
                
                Vector3 offset = placedObjectTypeSO.GetOffsetVisual�enterFromAnchor()* canvas.scaleFactor;
                worldPosition = gridSystemXY.GetWorldPositionGridCenter()- offset;
                break;
        }

        PlacedObject placedObject = CreateInWorld(worldPosition, placedObjectTypeSO, canvas.transform, pickUpDropPlacedObject);
        placedObject._gridSystemXY = gridSystemXY;
        placedObject._gridPositioAnchor = gridPositionAnchor;

        return placedObject;
    }

    /// <summary>
    /// �������� ��������� PlacedObject � ������� ������������
    /// </summary>
    /// <remarks> �����(������ ����� ����) ���������� ������� ����� = worldPosition</remarks>
    public static PlacedObject CreateInWorld(Vector3 worldPosition, PlacedObjectTypeSO placedObjectTypeSO, Transform parent, PickUpDropPlacedObject pickUpDropPlacedObject)
    {       
        Transform placedObjectTransform = Instantiate(placedObjectTypeSO.GetPrefab2D(), worldPosition , Quaternion.Euler(parent.rotation.eulerAngles.x, 0, 0), parent); //canvasContainer.rotation.eulerAngles.x- ��� �� ��� �������� ��� ��������
        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        placedObject._placedObjectTypeSO = placedObjectTypeSO;
        placedObject._offsetOffsetCenterFromAnchor = placedObjectTypeSO.GetOffsetVisual�enterFromAnchor();
        placedObject._pickUpDropPlacedObject = pickUpDropPlacedObject;
        placedObject.Setup();

        return placedObject;
    }


    private PlacedObjectTypeSO _placedObjectTypeSO;
    private GridSystemXY<GridObjectEquipmentXY> _gridSystemXY; // ����� � ������� ����������� ��� ������
    private PickUpDropPlacedObject _pickUpDropPlacedObject;
    private Vector2Int _gridPositioAnchor; // �������� ������� �����  
    private Vector3 _targetRotation;
    private Vector3 _targetPosition;
    private Vector3 _dropPositionWhenDeleted; // ������� ����������� ��� �������� �������
    private Vector3 _scaleOriginal;
    private bool _grabbed; // �������   
    private bool _moveStartPosition = false; // ����������� � ��������� �������        
    private Vector3 _offsetOffsetCenterFromAnchor;
    private HashSet<EquipmentSlot> _canPlacedOnSlotList;// ����� ���������� ��� ����� ���������� ��� ������   

    protected virtual void Setup()
    {
        _canPlacedOnSlotList = _placedObjectTypeSO.GetCanPlacedOnSlotList();
        _scaleOriginal = transform.localScale; // �������� ������������ �������       
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
            transform.position = Vector3.Lerp(transform.position, _dropPositionWhenDeleted, Time.deltaTime * moveSpeed);

            float stoppingDistance = 0.5f; // ��������� ��������� //����� ���������//
            if (Vector3.Distance(transform.position, _dropPositionWhenDeleted) < stoppingDistance)  // ���� ��������� �� ������� ������� ������ ��� ��������� ��������� // �� �������� ����        
            {
                _moveStartPosition = false; //���������� �������� �� �������� _dropPositionWhenDeleted               
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
    /// <summary>
    /// �������� ������ ������������ �����
    /// </summary>
    public Vector3 GetOffsetCenterFromAnchor() { return _offsetOffsetCenterFromAnchor; }
    public Vector2Int GetGridPositionAnchor() { return _gridPositioAnchor; }
    public void SetGridPositionAnchor(Vector2Int gridPosition) { _gridPositioAnchor = gridPosition; }

    /// <summary>
    /// ������� ����� �� ������� �������� ��� �������
    /// </summary>
    public GridSystemXY<GridObjectEquipmentXY> GetGridSystemXY() { return _gridSystemXY; }

    /// <summary>
    /// ��������� ����� �� ������� �������� ��� �������
    /// </summary>
    public void SetGridSystemXY(GridSystemXY<GridObjectEquipmentXY> gridSystemXY) { _gridSystemXY = gridSystemXY; }

    /// <summary>
    /// �������� ������ ���������� ������� � �����. (� �������� ��������� ��� ���������� �������� ������� � ����������)
    /// </summary>
    public List<Vector2Int> GetOccupiesGridPositionList() { return _placedObjectTypeSO.GetGridPositionList(_gridPositioAnchor); }

    /// <summary>
    /// �������� ������ ������� � �����, ������� �������� ������. (� �������� ��������� �������� ������� ��� ����� ����������)
    /// </summary>
    public List<Vector2Int> GetTryOccupiesGridPositionList(Vector2Int gridPosition) { return _placedObjectTypeSO.GetGridPositionList(gridPosition); }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO() { return _placedObjectTypeSO; }
   /// <summary>
   /// ���������� �������, ���� ����� ������������ ��������, ��� ��������.
   /// </summary>
    public void SetDropPositionWhenDeleted(Vector3 position) { _dropPositionWhenDeleted = position; }
    /// <summary>
    /// ���������� ����, ��� ���� ������������ � ��������� �������
    /// </summary>    
    public void SetFlagMoveStartPosition(bool moveStartPosition) { _moveStartPosition = moveStartPosition; }
        
    /// <summary>
    /// �������� ���� ��� ����� ���������� ��� ������
    /// </summary>
    public HashSet<EquipmentSlot> GetCanPlacedOnSlotList() { return _canPlacedOnSlotList; }

    public PlacedObjectGridParameters GetPlacedObjectGridParameters()
    {
        return new PlacedObjectGridParameters(_gridSystemXY.GetGridSlot(), _gridPositioAnchor, _placedObjectTypeSO,this);
    }


    // ��� ������� � ������ ScrenSpace

    public void OnPointerEnter(PointerEventData eventData)// ���� ��� ��� ���� �������� �� ��������� ���� ������ � PickUpDropPlacedObject
    {
        _pickUpDropPlacedObject.SetPlacedObjectMouseEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _pickUpDropPlacedObject.SetPlacedObjectMouseEnter(null);
    }

}

using System;
using UnityEngine;
/// <summary>
/// ��������� ������� ���� �� ����� 
/// </summary>
public class MouseOnGameGrid : MonoBehaviour // ����� ���������� �� ��������� ������� ���� � �����
{
    public event EventHandler<OnMouseGridPositionChangedEventArgs> OnMouseGridPositionChanged; // ������� ������� ���� �� ����� ����������
    public class OnMouseGridPositionChangedEventArgs : EventArgs // �������� ����� �������, ����� � ��������� ������� ��������
    {
        public GridPositionXZ lastMouseGridPosition; // ������� �������� ������� ����
        public GridPositionXZ newMouseGridPosition;  // ����� �������� ������� ����
    }

    [SerializeField] private LayerMask _mousePlaneLayerMask; // ����� ���� ��������� ���� (�������� � ����������)

    private GameInput _gameInput;
    private LevelGrid _levelGrid;
    private GridPositionXZ _mouseGridPosition;  // �������� ������� ����


    public void Init(GameInput gameInput, LevelGrid levelGrid)
    {
        _gameInput = gameInput;
        _levelGrid = levelGrid;
    }

    private void Start()
    {
        _mouseGridPosition = _levelGrid.GetGridPosition(GetPositionOnlyHitVisible());  // ��������� ��� ������ �������� ������� ���� // ������ �������� � Awake() �.�. � ���������� ����� ��������� ������� ������ (��� ��������� ������ GameInput ��� MouseOnGameGrid ����������)
    }

    // ��� �����, �������� ��� ������� �� �������� ����.
    /*private void Update()
    {
        transform.position = MouseOnGameGrid.GetTransformPosition(); // ��� ����� �������� �� ������ �����
    }*/

    private void Update()
    {
        GridPositionXZ newMouseGridPosition = _levelGrid.GetGridPosition(GetPositionOnlyHitVisible()); // ������� ����� �������� ������� ����
        if (_levelGrid.IsValidGridPosition(newMouseGridPosition) && _mouseGridPosition != newMouseGridPosition) // ���� ��� ���������� �������� ������� � ��� �� ����� ���������� �� ...
        {
            OnMouseGridPositionChanged?.Invoke(this, new OnMouseGridPositionChangedEventArgs //�������� - ������� ������� ���� �� ����� ���������� � ��������� ���������� � ����� �������� �������
            {
                lastMouseGridPosition = _mouseGridPosition,
                newMouseGridPosition = newMouseGridPosition,

            }); // �������� ������� � ���������

            _mouseGridPosition = newMouseGridPosition; // ��������� ���������� ������� �� �����
        }
    }

    public Vector3 GetPosition() // �������� ������� (static ���������� ��� ����� ����������� ������ � �� ������ ������ ����������) // ��� ����������� ����
    {
        Ray ray = Camera.main.ScreenPointToRay(_gameInput.GetMouseScreenPosition()); // ��� �� ������ � ����� �� ������ ��� ���������� ������ ����
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, _mousePlaneLayerMask); // Instance._coverLayerMask - ����� ������ ��� �������� ����� ����� 1<<6  �.�. mousePlane ��� 6 �������
        return raycastHit.point; // ���� ��� ������� � �������� �� Physics.Raycast ����� true, � raycastHit.point ������ "����� ����� � ������� ������������, ��� ��� ����� � ���������", � ���� false �� ����� ������� ����������� ������ ������ ��������(� ����� ������ ������ ������� ������).
    }

    public Vector3 GetPositionOnlyHitVisible() // �������� ������� ��� ���������, ������ ��� ������� �������� (static ���������� ��� ����� ����������� ������ � �� ������ ������ ����������) // � ��������� ��������� ������� ��� ���������� ��������� ����� ��� ����������� �� ������� �� �������� �������� ��������, � ����� �� �� ����� �������� �� ������� ��� ����� �.�. ������ ���� ������ ���������
    {
        Ray ray = Camera.main.ScreenPointToRay(_gameInput.GetMouseScreenPosition()); // ��� �� ������ � ����� �� ������ ��� ���������� ������ ����
        RaycastHit[] raycastHitArray = Physics.RaycastAll(ray, float.MaxValue, _mousePlaneLayerMask); // �������� ������ ���� ��������� ����
        System.Array.Sort(raycastHitArray, (RaycastHit raycastHitA, RaycastHit raycastHitB) => // ����������� �������� � ����� ���������� ������� �� ��������� �� ����� �������� ���� (�.�. ��� ����������� ��������)
        {
            return Mathf.RoundToInt(raycastHitA.distance - raycastHitB.distance); // ����� ���������� // ��������� IComparer ������������� �����, ������� ���������� ��� �������.
        });

        foreach (RaycastHit raycastHit in raycastHitArray) // ��������� ��� ���������� ������
        {
            if (raycastHit.transform.TryGetComponent(out Renderer renderer)) //��������� �������� �� ������� � ������� ����� ��� ��������� Renderer
            {
                if (renderer.enabled) // � ���� �� �����
                {
                    return raycastHit.point;// ������ "����� ����� � ������� ������������, ��� ��� ����� � ���������"
                }
                // ���� �� �� ����� ������ ���������� ��������� � ����
            }
        }

        // ���� �� �� ��� �� ������� ������ ����
        return Vector3.zero;
    }
}

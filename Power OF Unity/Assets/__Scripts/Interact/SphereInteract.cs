using System;
using UnityEngine;

public class SphereInteract : MonoBehaviour, IInteractable // �������������� � ������ // �������� ����� ����������� 
{


    public event EventHandler OnInteractSphereActivated; //�������� ������� ����� - ����� �������������� �������������

    [SerializeField] private Material _greenMaterial;
    [SerializeField] private Material _redMaterial;
    [SerializeField] private MeshRenderer _meshRenderer;

    [SerializeField] private bool _isGreen; //�� ������� (��� ������������ ��������� ����)

    private static SoundManager _soundManager;
    private static LevelGrid _levelGrid;

    private GridPositionXZ _gridPosition; // �������� ������� ����    
    private Action _onInteractionComplete; // ������� �������������� ���������// �������� ������� � ������������ ���� - using System;
                                           //�������� ��� ������� ��� ������������ ���������� (� ��� ����� ��������� ������ ������� �� ���������).
                                           //Action- ���������� �������. ���� ��� �������. ������� Func<>. 
                                           //�������� ��������. ����� ���������� ������� � ������� �� �������� ��������, ����� �����, � ������������ ����� ����, ����������� ����������� �������.
                                           //�������� ��������. ����� �������� �������� ������� �� ������� ������
    private bool _isActive;
    private float _timer; // ������ ������� �� ����� ��������� ���������� ����������������� � �����

    public static void Init(SoundManager soundManager, LevelGrid levelGrid)
    {
        _soundManager = soundManager;
        _levelGrid = levelGrid;
    }

    private void Start()
    {
        if (gameObject.activeSelf) // ���� ��� � �������� ��������� �� ...
        {
            UpdateInteractableAtGridPosition(); // �������� �������������� � �������� ��������
        }

        if (_isGreen) // ���������� ������ � ������������ � �������� ����������
        {
            SetColorGreen();
        }
        else
        {
            SetColorRed();
        };
    }

    public void UpdateInteractableAtGridPosition() // �������� �������������� � �������� ��������
    {
        _gridPosition = _levelGrid.GetGridPosition(transform.position); // ��������� �������� ������� ����
        _levelGrid.SetInteractableAtGridPosition(_gridPosition, this); // � � ���������� �������� ������� ��������� ��� ��� � ����������� Interactable(��������������)
    }



    private void Update()
    {
        if (!_isActive) // ���� ���������� �������������� � ����� �� ��� ������� � ����������� ��� ����
        {
            return;// ������� � ���������� ��� ����
        }

        _timer -= Time.deltaTime; // �������� ������

        if (_timer <= 0f)
        {
            _isActive = false; // ��������� �������� ���������
            _onInteractionComplete(); // ������� ����������� ������� ������� ��� �������� ������� Interact(). � ����� ������ ��� ActionComplete() �� ������� ��������� � ������ UI
        }
    }

    private void SetColorGreen()
    {
        _isGreen = true;
        _meshRenderer.material = _greenMaterial;
    }

    private void SetColorRed()
    {
        _isGreen = false;
        _meshRenderer.material = _redMaterial;
    }

    public void Interact(Action onInteractionComplete) // ��������������. � �������� ������� ������� �������������� ��������� 
    {
        _onInteractionComplete = onInteractionComplete; // �������� ���������� �������
        _isActive = true; // ����������� ��� � �������� ���������
        _timer = 1.5f; // ������ ����� ��������� ���������  //����� ���������//

        if (_isGreen) // ��� �������������� ������� ����. ���� ��� �������� �� ���������� �� ��������
        {
            SetColorRed();
        }
        else
        {
            SetColorGreen();
            _soundManager.PlayOneShot(SoundName.Interact);
            OnInteractSphereActivated?.Invoke(this, EventArgs.Empty); // �������� ������� - ����� �������������� �������������
        }

    }
}

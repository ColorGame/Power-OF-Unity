
using System;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteract : MonoBehaviour, IInteractable //�����-�������������� �������� ����� ����������� 
{
   
    public static event EventHandler OnAnyDoorIsLocked; //������� ������� ����� -����� ����� �������(������ ������� �������)
                                                        // static - ���������� ��� event ����� ������������ ��� ����� ������, � �� ��� ���������� �����.

    public event EventHandler OnDoorOpened; //������� ������� ����� - ����� �������




    [SerializeField] private bool _isOpen; //������� (�����)
    [SerializeField] private bool _isInteractable = true; //����� ����������������� (�� ��������� true)

    private static SoundManager _soundManager;
    private static LevelGrid _levelGrid;
    private static HashAnimationName _hashAnimationName;

    private bool _isActive;
    private float _timer; // ������ ������� �� ����� ��������� ���������� ����������������� � ������
    private Transform[] _transformChildrenDoorArray;  //������ �������� �������� ����� (��� ���� �����[0] �����[1] b ������[2] �����)
    private Animator _animator; //�������� �� �����
   // private SingleNodeBlocker _singleNodeBlocker; // ����������� ���� �� ����� �����

    private Action _onInteractionComplete; // ������� �������������� ���������// �������� ������� � ������������ ���� - using System;
                                           //�������� ��� ������� ��� ������������ ���������� (� ��� ����� ��������� ������ ������� �� ���������).
                                           //Action- ���������� �������. ���� ��� �������. ������� Func<>. 
                                           //�������� ��������. ����� ���������� ������� � ������� �� �������� ��������, ����� �����, � ������������ ����� ����, ����������� ����������� �������.
                                           //�������� ��������. ����� �������� �������� ������� �� ������� ������

    private List<GridPositionXZ> _doorGridPositionList = new List<GridPositionXZ>();

    public static void Init(SoundManager soundManager, LevelGrid levelGrid, HashAnimationName hashAnimationName)
    {
        _soundManager = soundManager;
        _levelGrid = levelGrid;
        _hashAnimationName = hashAnimationName;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _transformChildrenDoorArray = GetComponentsInChildren<Transform>();
      //  _singleNodeBlocker = GetComponent<SingleNodeBlocker>();
    }


    private void Start()
    {
        foreach (GridPositionXZ gridPosition in GetDoorGridPositionList()) // ��������� ������ �������� ������� ������� �������� �����
        {
            _levelGrid.SetInteractableAtGridPosition(gridPosition, this); // � ���������� �������� ������� ��������� ��� �������� ������ ����� � ����������� Interactable(��������������)            
        }
        UpdateStateDoor(_isOpen);


    }

    private void Update()
    {
        if (!_isActive) // ���� ���������� �������������� � ������ �� ��� ������� � ����������� ��� ����
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

    public void UpdateStateDoor(bool isOpen) //������� ��������� ����� � ����������� �� ���������� ������� ����������
    {
        if (isOpen) // ���������� ������������� �� ��������� ��������� ����� 
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }



    public void Interact(Action onInteractionComplete) // ��������������. � �������� ������� ������� �������������� ���������
    {
        _onInteractionComplete = onInteractionComplete; // �������� ���������� �������
        _isActive = true; // ����������� ����� � �������� ���������
        _timer = 1.5f; // ������ ����� ��������� ���������  //����� ���������//

        if (_isInteractable) // ���� � ������ ����� ����������������� ��
        {
            if (_isOpen) // ��� ��������������, ���� ����� ������� ����� �� ��������� � ��������
            {
                CloseDoor();
            }
            else
            {
                OpenDoor();
            }
        }
        else
        {
            // ����� ����������� ���� ���������� ���������� ��� ��������� �������
            _soundManager.PlayOneShot(SoundName.DoorClosed);
            OnAnyDoorIsLocked?.Invoke(this, EventArgs.Empty); // �������� ������� ����� ����� ������� (��� ���������� �������)
        }
    }

    private void OpenDoor() // ������� �����
    {
        _isOpen = true;
        _animator.CrossFade(_hashAnimationName.DoorOpen, 0.2f);
        //_animation.SetBool("IsOpen", _isOpen); // �������� ������� ���������� "GetIsOpen". ��������� �� �������� _isOpen        

        foreach (GridPositionXZ gridPosition in _doorGridPositionList) // ��������� ������ �������� ������� ������� �������� �����
        {
            // PathfindingMonkey.Instance.SetIsWalkableGridPosition(_gridPositioAnchor, true); // ��������� ��� ����� ������ �� ���� �������� �������
          //  GraphNode graphNode = _levelGrid.GetGridNode(gridPosition); // ������� ����������� ����
          //  BlockManager.Instance.InternalUnblock(graphNode, _singleNodeBlocker); // ������������ ����
        }
        _soundManager.PlayOneShot(SoundName.DoorOpen);

        // �������� �������
        OnDoorOpened?.Invoke(this, EventArgs.Empty);       
    }

    private void CloseDoor() // ������� �����
    {
       /* var selectorList = new List<SingleNodeBlocker>() { _singleNodeBlocker };   // ������ ����������� ������� ����� ������������. ������� ���� ���� �����

        // �������� ����� ������ ����� ��� ����
        foreach (GridPositionXZ gridPosition in _doorGridPositionList) // ��������� ������ �������� ������� ������� �������� �����
        {
            GraphNode graphNode = _levelGrid.GetGridNode(gridPosition); // ������� ����������� ����
            if (BlockManager.Instance.NodeContainsAnyExcept(graphNode, selectorList)) // ���� ��� ���� �������������� ������. ���� ������ (SingleNodeBlocker), ���������� ��� �� ����, ��� � �����
            {
                _animator.CrossFade(_hashAnimationName.DoorBlocked, 0.2f); // ����� �������������
                return; // ������� � ���������� ��� ����
            }
        }

        foreach (GridPositionXZ gridPosition in _doorGridPositionList) // ��������� ������ �������� ������� ������� �������� �����
        {
            //  PathfindingMonkey.Instance.SetIsWalkableGridPosition(_gridPositioAnchor, false); // ��������� ��� ������ ������ �� ���� �������� �������
            GraphNode graphNode = _levelGrid.GetGridNode(gridPosition); // ������� ����������� ����
            BlockManager.Instance.InternalBlock(graphNode, _singleNodeBlocker); // ����������� ����
        }*/

        _isOpen = false;
        _animator.CrossFade(_hashAnimationName.DoorClose, 0.2f);
        // _animation.SetBool("IsOpen", _isOpen); // �������� ������� ���������� "GetIsOpen". ��������� �� �������� _isOpen
        _soundManager.PlayOneShot(SoundName.DoorOpen);
    }

    private List<GridPositionXZ> GetDoorGridPositionList() //�������� ������ �������� ������� �����  
    {
        //������� ����� � ��, ����� ������� ����� �������� https://community.gamedev.tv/t/larger-doors-and-doors-that-can-be-shot-through/220723*/

        float offsetFromEdgeGrid = 0.01f; // �������� �� ���� ����� (������) //����� ���������//

        GridPositionXZ childreGridPositionLeft = _levelGrid.GetGridPosition(_transformChildrenDoorArray[1].position + _transformChildrenDoorArray[1].right * offsetFromEdgeGrid); // ��������� �������� ������� ��������� ������� ����� ����� �������� ����� (��������� �� ������� ������ ��� �� �� ������� �� �������� �����)
        GridPositionXZ childreGridPositionRight = _levelGrid.GetGridPosition(_transformChildrenDoorArray[2].position - _transformChildrenDoorArray[2].right * offsetFromEdgeGrid); // ��������� �������� ������� ��������� ������� ����� ������ �������� ����� (��������� �� ������� ������ ��� �� �� ������� �� �������� ������)

        //_doorGridPositionList = PathfindingMonkey.Instance.FindPath(childreGridPositionRight, childreGridPositionLeft, out int pathLength); // ����������� �� ������ ����

        // ������� ������ ���� ����� ������� ����� ������������ ��� ����, �� �� �������� ��� ������ ������������� �� ���������
        // ��������������� �����
        if (childreGridPositionLeft.x == childreGridPositionRight.x)
        {
            if (childreGridPositionLeft.z <= childreGridPositionRight.z) //��� ������� ��� ����� ������� ���� ������
            {
                for (int z = childreGridPositionLeft.z; z <= childreGridPositionRight.z; z++)  // ��������� �������� ������� �� ��� Z (�������� ������ �� ����� ������� �� ������).
                {
                    GridPositionXZ testGridPosition = new GridPositionXZ(childreGridPositionLeft.x, z, childreGridPositionLeft.floor); // ����������� �������� ������� �� ��� Z. �������� ������� �� X ����� ����� ����� �������

                    _doorGridPositionList.Add(testGridPosition); // ������� � ������ ����������� ������    
                }
            }

            if (childreGridPositionRight.z <= childreGridPositionLeft.z) //��� ������� ��� ������ ������� ���� �����
            {
                for (int z = childreGridPositionRight.z; z <= childreGridPositionLeft.z; z++)  // ��������� �������� ������� �� ��� Z (�������� ������ �� ������ ������� �� �����).
                {
                    GridPositionXZ testGridPosition = new GridPositionXZ(childreGridPositionLeft.x, z, childreGridPositionLeft.floor); // ����������� �������� ������� �� ��� Z. �������� ������� �� X ����� ����� ����� �������

                    _doorGridPositionList.Add(testGridPosition); // ������� � ������ ����������� ������    
                }
            }
        }

        // �������������� �����
        if (childreGridPositionLeft.z == childreGridPositionRight.z)
        {
            if (childreGridPositionLeft.x <= childreGridPositionRight.x) //��� ������� ��� ����� ������� ����� �� ������
            {
                for (int x = childreGridPositionLeft.x; x <= childreGridPositionRight.x; x++)  // ��������� �������� ������� �� ��� � (�������� ������ �� ����� ������� �� ������).
                {
                    GridPositionXZ testGridPosition = new GridPositionXZ(x, childreGridPositionLeft.z, childreGridPositionLeft.floor); // ����������� �������� ������� �� ��� �. �������� ������� �� Z ����� ����� ����� �������

                    _doorGridPositionList.Add(testGridPosition); // ������� � ������ ����������� ������    
                }
            }

            if (childreGridPositionLeft.x >= childreGridPositionRight.x) //��� ������� ��� ����� ������� ������ �� ������
            {
                for (int x = childreGridPositionRight.x; x <= childreGridPositionLeft.x; x++)  // ��������� �������� ������� �� ��� � (�������� ������ �� ������ ������� �� �����).
                {
                    GridPositionXZ testGridPosition = new GridPositionXZ(x, childreGridPositionLeft.z, childreGridPositionLeft.floor); // ����������� �������� ������� �� ��� �. �������� ������� �� Z ����� ����� ����� �������

                    _doorGridPositionList.Add(testGridPosition); // ������� � ������ ����������� ������    
                }
            }
        }

        return _doorGridPositionList;
    }

    public void SetIsInteractable(bool isInteractable) // ���������� ����� �����������������
    {
        _isInteractable = isInteractable;
    }
}


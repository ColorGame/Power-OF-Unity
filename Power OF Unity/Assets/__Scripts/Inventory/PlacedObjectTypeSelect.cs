using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacedObjectTypeSelect : MonoBehaviour // ��������� ��� ������������ ������� // ������� ������ ��� ������ ���������
{
    [SerializeField] private Transform _weaponSelectContainer; // ��������� ��� ������ ������ 
    [SerializeField] private Transform _itemSelectContainer; // ��������� ��� ������ ��������       
    [SerializeField] private Transform _moduleSelectContainer; // ��������� ��� ������ ������
    [SerializeField] private Button _weaponButtonPanel;  // ������ ��� ��������� ������ ������
    [SerializeField] private Button _itemButtonPanel;  // ������ ��� ��������� ������ ��������
    [SerializeField] private Button _moduleButtonPanel;  // ������ ��� ��������� ������ ������
    [SerializeField] private List<PlacedObjectTypeSO> _ignorePlacedObjectList; // ������ �������� ������� ���� ������������ ��� �������� ������ ���������


    private Transform[] _typeSelectContainerArray; // ������ ����������� ��� ������ ������ ��������� � �������
    private Dictionary<PlacedObjectTypeSO, Transform> _buttonTransformDictionary; // ������� (��� ������������ ������� - ����, Transform- -��������)
    private PlacedObjectTypeListSO _placedObjectTypeListSO; // ������ ����� ������������ �������    
    private ScrollRect _scrollRect; //��������� ��������� ������

    private void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _placedObjectTypeListSO = Resources.Load<PlacedObjectTypeListSO>(typeof(PlacedObjectTypeListSO).Name);    // ��������� ������ ������������ ����, ���������� �� ������ path(����) � ����� Resources(��� ����� � ������ � ����� ScriptableObjects).
                                                                                                                // ��� �� �� ��������� � ����� ������ ������ �����. �������� ��������� BuildingTypeListSO (������ ����� ����) � ������� ����� ��� � �����, ����� ��� ������ SO ����� ��������� ��� ������ ������� ��������� � ������ ����������
        _buttonTransformDictionary = new Dictionary<PlacedObjectTypeSO, Transform>(); // �������������� ����� �������
        _typeSelectContainerArray = new Transform[] { _weaponSelectContainer, _itemSelectContainer, _moduleSelectContainer };
    }

    private void Start()
    {
        CreatePlacedObjectTypeButton(); // ������� ������ ����� ����������� ��������
        

        _weaponButtonPanel.onClick.AddListener(() => //������� ������� ��� ������� �� ���� ������// AddListener() � �������� ������ �������� �������- ������ �� �������. ������� ����� ��������� �������� ����� ������ () => {...} 
        {
            ShowContainer(_weaponSelectContainer);
        }); // _weaponButtonPanel.onClick.AddListener(delegate { ShowContainer(_weaponSelectContainer); }); // ��� ������� ����������

        _itemButtonPanel.onClick.AddListener(() => //������� ������� ��� ������� �� ���� ������// AddListener() � �������� ������ �������� �������- ������ �� �������. ������� ����� ��������� �������� ����� ������ () => {...} 
        {
            ShowContainer(_itemSelectContainer);
        });

        _moduleButtonPanel.onClick.AddListener(() => //������� ������� ��� ������� �� ���� ������// AddListener() � �������� ������ �������� �������- ������ �� �������. ������� ����� ��������� �������� ����� ������ () => {...} 
        {
            ShowContainer(_moduleSelectContainer);
        });
    }

    private void ShowContainer(Transform typeSelectContainer) // �������� ��������� (� �������� �������� ������ ��������� ������)
    {
        foreach (Transform buttonContainer in _typeSelectContainerArray) // ��������� ������ �����������
        {
            if (buttonContainer == typeSelectContainer) // ���� ��� ���������� ��� ���������
            {
                buttonContainer.gameObject.SetActive(true); // ������� ���
            }
            else // � ��������� ������
            {
                buttonContainer.gameObject.SetActive(false); // ��������
            }
        }
        _scrollRect.content = (RectTransform)typeSelectContainer; // ��������� ���� ��������� ��� ������� ��� ���������
    }


    private void CreatePlacedObjectTypeButton() // ������� ������ ����� ����������� ��������
    {
        foreach (Transform typeSelectContainer in _typeSelectContainerArray)
        {
            foreach (Transform selectPlacedButton in typeSelectContainer)
            {
                Destroy(selectPlacedButton.gameObject); // ������ ������� ������ ������������� � Transform
            }
        }


        foreach (PlacedObjectTypeSO placedObjectTypeSO in _placedObjectTypeListSO.list) // ��������� ������ ����� ����������� ��������
        {
            if (_ignorePlacedObjectList.Contains(placedObjectTypeSO)) continue; // ��������� ������� ��� ������� �� ���� ��������� ������
                       
            Transform buttonTransform = null;
            switch (placedObjectTypeSO) // � ����������� �� ���� �������� � ������ ���������
            {
                case WeaponTypeSO weaponTypeSO:
                    buttonTransform = Instantiate(GameAssets.Instance.placedObjectTypeButtonPrefab, _weaponSelectContainer); // �������� ������ � ������� �������� � ���������
                    break;

                case ItemTypeSO itemTypeSO:
                    buttonTransform = Instantiate(GameAssets.Instance.placedObjectTypeButtonPrefab, _itemSelectContainer); // �������� ������ � ������� �������� � ���������
                    break;

                case ModuleTypeSO moduleTypeSO:
                    buttonTransform = Instantiate(GameAssets.Instance.placedObjectTypeButtonPrefab, _moduleSelectContainer); // �������� ������ � ������� �������� � ���������
                    break;
            }

            Transform visualButton = Instantiate(placedObjectTypeSO.visual, buttonTransform); // �������� ������ ������ � ����������� �� ���� ������������ ������� � ������� �������� � ������ 
            Transform[] childrenArray = visualButton.GetComponentsInChildren<Transform>(); // ������ ��� �������� ������� ������� � ������� ����, ��� �� ��� �� ����������� �� ���������� �����
            foreach (Transform child in childrenArray)
            {
                child.gameObject.layer = 13;
            }

            buttonTransform.GetComponent<Button>().onClick.AddListener(() => //������� ������� ��� ������� �� ���� ������// AddListener() � �������� ������ �������� �������- ������ �� �������. ������� ����� ��������� �������� ����� ������ () => {...} 
            {
                PickUpDropSystem.Instance.CreatePlacedObject(buttonTransform.position, placedObjectTypeSO); // �������� ������ ������ � ������� ������                
            });

            MouseEnterExitEventsUI mouseEnterExitEventsUI = buttonTransform.GetComponent<MouseEnterExitEventsUI>(); // ������ �� ������ ��������� - ������� ����� � ������ ����� 

            mouseEnterExitEventsUI.OnMouseEnter += (object sender, EventArgs e) => // ���������� �� �������
            {
                TooltipUI.Instance.Show(placedObjectTypeSO.nameString + "\n" + "��������������"); // ��� ��������� �� ������ ������� ��������� � ��������� �����
            };
            mouseEnterExitEventsUI.OnMouseExit += (object sender, EventArgs e) =>
            {
                TooltipUI.Instance.Hide(); // ��� ��������� ���� ������ ���������
            };

            _buttonTransformDictionary[placedObjectTypeSO] = buttonTransform; // �������� ������� ����� �������� (������� ���� ������� ���� ��������� ������ ���������� �� �������)
        }
    }
}

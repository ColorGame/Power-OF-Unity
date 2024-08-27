using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������ ��������. ������������ ������ ������� �� ������.
/// </summary>
/// <remarks>
/// ����������� � ������
/// </remarks>
public class ActionButtonUI : MonoBehaviour
{   
    [SerializeField] private GameObject _selectedButtonVisualUI; // ����� �������� � ����. GameObject ��� �� ������ ��� �������� ����� ������ // � ���������� ���� �������� �����
    [SerializeField] private Transform _placedObjectVisual;         
   
    private Button _button;
    private UnitActionSystem _unitActionSystem;
    private PlacedObjectTypeWithActionSO _placedObjectTypeWithActionSO;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void SetMoveAction(MoveAction moveAction, UnitActionSystem unitActionSystem) 
    {     
        _unitActionSystem = unitActionSystem;        
        _button.onClick.AddListener(() =>
        {
            _unitActionSystem.SetSelectedAction(moveAction); //���������� ��������� ��������
        });
    }

    public void SetBaseActionAndplacedObjectTypeWithActionSO(BaseAction baseAction,PlacedObjectTypeWithActionSO placedObjectTypeWithActionSO, UnitActionSystem unitActionSystem)
    {
        _placedObjectTypeWithActionSO = placedObjectTypeWithActionSO;
        _unitActionSystem = unitActionSystem;
       
       /* _placedObjectImage.sprite = PlacedObjectTypeWithActionSO.GetVisual2D();
        _placedObjectImage.rectTransform.localScale = Vector3.one * PlacedObjectTypeWithActionSO.GetScaleImageButton();*/
        
        //������� ������� ��� ������� �� ���� ������
        _button.onClick.AddListener(() =>
        {
            baseAction.SetPlacedObjectTypeWithActionSO(placedObjectTypeWithActionSO);
            _unitActionSystem.SetSelectedAction(baseAction); //���������� ��������� ��������
        });

        InteractableEnable();
    }



    public void UpdateSelectedVisual() // (���������� �������) ��������� � ���������� ������������ ������.(���������� �������� ��� ������ ������ �������� ��������)
    {
        BaseAction selectedBaseAction = _unitActionSystem.GetSelectedAction(); // ������� �������� ��������
                                                                               //_selectedButtonVisualUI.SetActive(_placedObject == selectedBaseAction.GetPlacedObjectTypeSO());   // �������� ����� ���� ��������� �������� ��������� � ��������� ������� �� ��������� �� ���� ������ // ���� �� ��������� �� ������� false � ����� �����������

        if (_placedObjectTypeWithActionSO == selectedBaseAction.GetPlacedObjectTypeWithActionSO())
            _button.Select();
        // ����� �������� ���� ������ ��� ��������� ������
        /*if (selectedBaseAction == _baseAction) // ���� ������ �������
        {
            _button.image.color = _color; // ������� ��� ����
        }
        else
        {
            _button.image.color = new Color(_color.r, _color.g, _color.b, 0.5f); // ������� �� ��������������
        }*/
    }


    //������ ������ ������ ����� ����� ���������
    public void InteractableEnable() // �������� ��������������
    {
        _button.interactable = true;
       
        UpdateSelectedVisual(); // ������� ����������� ����� ������ � ����������� �� ��������������� ��������
    }

    public void InteractableDesabled() // ��������� �������������� // ������ ����������� �� �������� � ������ ����(������������� � ���������� color  Desabled)
    {
        _button.interactable = false;
        _selectedButtonVisualUI.SetActive(false); //�������� �����

        /*Color textColor = _textColor; // �������� � ��������� ���������� ���� ������
        textColor.a = 0.1f; // ������� �������� ����� ������
        _textMeshPro.color = textColor; // ������� ������� ���� ����� (���� ����������)*/

    }

    public void HandleStateButton(bool isBusy) // ���������� ��������� ������
    {
        if (isBusy) // ���� �����
        {
            InteractableDesabled(); // ��������� ��������������
        }
        else
        {
            InteractableEnable(); // �������� ��������������
        }
    }
}

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
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _selectedButtonVisualUI; // ����� �������� � ����. GameObject ��� �� ������ ��� �������� ����� ������ // � ���������� ���� �������� �����
    [SerializeField] private Transform _placedObjectVisual;         
   
    private UnitActionSystem _unitActionSystem;
    private PlacedObjectTypeSO _placedObjectTypeSO;

    public void SetMoveAction(MoveAction moveAction, UnitActionSystem unitActionSystem) 
    {     
        _unitActionSystem = unitActionSystem;        
        _button.onClick.AddListener(() =>
        {
            _unitActionSystem.SetSelectedAction(moveAction); //���������� ��������� ��������
        });
    }

    public void SetBaseActionAndPlacedObjectTypeSO(BaseAction baseAction,PlacedObjectTypeSO placedObjectTypeSO, UnitActionSystem unitActionSystem)
    {
        _placedObjectTypeSO = placedObjectTypeSO;
        _unitActionSystem = unitActionSystem;
       
       /* _placedObjectImage.sprite = placedObjectTypeSO.GetVisual2D();
        _placedObjectImage.rectTransform.localScale = Vector3.one * placedObjectTypeSO.GetScaleImageButton();*/
        
        //������� ������� ��� ������� �� ���� ������
        _button.onClick.AddListener(() =>
        {
            baseAction.SetPlacedObjectTypeSO(placedObjectTypeSO);
            _unitActionSystem.SetSelectedAction(baseAction); //���������� ��������� ��������
        });

        InteractableEnable();
    }



    public void UpdateSelectedVisual() // (���������� �������) ��������� � ���������� ������������ ������.(���������� �������� ��� ������ ������ �������� ��������)
    {
        BaseAction selectedBaseAction = _unitActionSystem.GetSelectedAction(); // ������� �������� ��������
                                                                               //_selectedButtonVisualUI.SetActive(_placedObject == selectedBaseAction.GetPlacedObjectTypeSO());   // �������� ����� ���� ��������� �������� ��������� � ��������� ������� �� ��������� �� ���� ������ // ���� �� ��������� �� ������� false � ����� �����������

        if (_placedObjectTypeSO == selectedBaseAction.GetPlacedObjectTypeSO())
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

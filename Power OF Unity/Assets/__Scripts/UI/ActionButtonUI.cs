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
    [SerializeField] private TextMeshProUGUI _textMeshPro; // TextMeshProUGUI ��� ����������������� ����������
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _selectedButtonVisualUI; // ����� �������� � ����. GameObject ��� �� ������ ��� �������� ����� ������ // � ���������� ���� �������� �����

    [SerializeField] private Sprite _spriteShoot; // � ������� ������ �������� ��������������� ������ ������
    [SerializeField] private Sprite _spriteGrenade;
    [SerializeField] private Sprite _spriteMove;
    [SerializeField] private Sprite _spriteSpin;
    [SerializeField] private Sprite _spriteSword;
    [SerializeField] private Sprite _spriteInteract;
    [SerializeField] private Sprite _spriteHeal;
    [SerializeField] private Sprite _spriteBinoculars;
    [SerializeField] private Sprite _spriteHook;


    private BaseAction _baseAction;   
    private Color _textColor;
    private UnitActionSystem _unitActionSystem;
   
    public void SetBaseAction(BaseAction baseAction, UnitActionSystem unitActionSystem) // ��������� ������� �������� �� ������ (� �������� �������� ���� baseAction)
    {
        _baseAction = baseAction; // �������� ���������� ��� ������� ��������
        _textMeshPro.text = baseAction.GetActionName().ToUpper(); // � �������� ������ ������� ���������� ��� ��������� ��������  //ToUpper()- � ������� ��������               
        _textColor = _textMeshPro.color; // �������� ���� ������

        _unitActionSystem = unitActionSystem;

        //������� ������� ��� ������� �� ���� ������
        _button.onClick.AddListener(() =>
        {
            _unitActionSystem.SetSelectedAction(baseAction); //���������� ��������� ��������
        });

        switch (_baseAction) // � ����������� �� ���������� �������� �������� ��������� ������ ������
        {
            case ShootAction shootAction:
                _button.image.sprite = _spriteShoot;
                break;
            case GrenadeAction grenadeAction:
                _button.image.sprite = _spriteGrenade;
                break;
            case MoveAction moveAction:
                _button.image.sprite = _spriteMove;
                break;
            case SpinAction spinAction:
                _button.image.sprite = _spriteSpin;
                break;
            case HealAction healAction:
                _button.image.sprite = _spriteHeal;
                break;
            case SwordAction swordAction:
                _button.image.sprite = _spriteSword;
                break;
            case InteractAction interactAction:
                _button.image.sprite = _spriteInteract;
                break;
            case SpotterFireAction spotterFireAction:
                _button.image.sprite = _spriteBinoculars;
                break;
            case GrappleAction comboAction:
                _button.image.sprite = _spriteHook;
                break;

        }
    }    

    public void UpdateSelectedVisual() // (���������� �������) ��������� � ���������� ������������ ������.(���������� �������� ��� ������ ������ �������� ��������)
    {
        BaseAction selectedBaseAction = _unitActionSystem.GetSelectedAction(); // ������� �������� ��������
        _selectedButtonVisualUI.SetActive(selectedBaseAction == _baseAction);   // �������� ����� ���� ��������� �������� ��������� � ��������� ������� �� ��������� �� ���� ������
                                                                                // ���� �� ��������� �� ������� false � ����� �����������

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
    private void InteractableEnable() // �������� ��������������
    {
        _button.interactable = true;
        _textMeshPro.color = _textColor;
        UpdateSelectedVisual(); // ������� ����������� ����� ������ � ����������� �� ��������������� ��������
    }

    private void InteractableDesabled() // ��������� �������������� // ������ ����������� �� �������� � ������ ����(������������� � ���������� color  Desabled)
    {
        _button.interactable = false;

        Color textColor = _textColor; // �������� � ��������� ���������� ���� ������
        textColor.a = 0.1f; // ������� �������� ����� ������
        _textMeshPro.color = textColor; // ������� ������� ���� ����� (���� ����������)

        _selectedButtonVisualUI.SetActive(false); //�������� �����
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

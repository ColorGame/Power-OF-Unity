using UnityEngine;
using UnityEngine.UI;

public class EquipmentGridVisualSingleSetColor : MonoBehaviour
{
    private Image _image; // ����� ������ �������� ��� ������������ ����� ����������
    private bool _isBusy; //������     

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void Hide() // ������
    {
        _image.enabled = false;
    }

    public void Show(Color color) //�������� � ���������� ���������� ����
    {
        _image.enabled = true;
        _image.color = color;
    }

    public void SetIsBusyAndColor(bool isBusy, Color color)
    {
        _isBusy = isBusy;
        if (_isBusy) // ���� ������ ������ ��
        {
            Hide(); // ������ �����
        }
        else
        {
            Show(color); // ��������
        }
    }

    public virtual bool GetIsBusy()
    {
        return _isBusy;
    }
}

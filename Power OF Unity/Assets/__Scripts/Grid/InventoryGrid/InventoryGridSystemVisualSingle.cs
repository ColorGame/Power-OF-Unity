using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryGridSystemVisualSingle : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRendererQuad; // ����� ������ �������� ��� ������������ ����� ���������

    private bool _isBusy; //������ 

    public void Hide() // ������
    {
        _meshRendererQuad.enabled = false;
    }

    public void Show(Material material) //�������� � ���������� ���������� ��������
    {
        _meshRendererQuad.enabled = true;
        _meshRendererQuad.material = material;
    }

    public void SetIsBusyAndMaterial(bool isBusy, Material material)
    {
        _isBusy = isBusy;
        if (_isBusy) // ���� ������ ������ ��
        {
            Hide(); // ������ �����
        }
        else
        {
            Show(material); // ��������
        }
    }

    public bool GetIsBusy()
    {
        return _isBusy;
    }

   
   
}

using UnityEngine;
/// <summary>
/// ������ ����� ���������, ��������� � ������ ������� �����, � ������ ���� � ����������� �� ���������� ����������
/// </summary>
public class InventoryGridVisualSingleInWorldSpace : InventoryGridVisualSingle
{
    [SerializeField] private MeshRenderer _meshRendererQuad; // ����� ������ �������� ��� ������������ ����� ���������

   

    protected override void Hide() // ������
    {
        _meshRendererQuad.enabled = false;
    }

    public override void Show(Material material) //�������� � ���������� ���������� ��������
    {
        _meshRendererQuad.enabled = true;
        _meshRendererQuad.material = material;       
    }  
}

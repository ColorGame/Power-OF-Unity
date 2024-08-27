using UnityEngine;
/// <summary>
/// ������ ����� ����������, ��������� � ������ ������� �����, � ������ ���� � ����������� �� ���������� ����������
/// </summary>
public class EquipmentGridVisualSingleInWorldSpace : EquipmentGridVisualSingle
{
    [SerializeField] private MeshRenderer _meshRendererQuad; // ����� ������ �������� ��� ������������ ����� ����������

    public override void Init(float cellSize)
    {      
        transform.localScale = Vector2.one * cellSize;
    }

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

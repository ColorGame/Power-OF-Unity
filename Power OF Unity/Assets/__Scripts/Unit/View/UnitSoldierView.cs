using UnityEngine;
/// <summary>
/// ������ ����� "������". 
/// </summary>
/// <remarks>�������� ����� - 1.BodyArmorMilitary 2.BodyArmorMilitaryMod </remarks>
public class UnitSoldierView : UnitView
{
    [Header("���������� � ������� ����� �����������\n��������� SkinnedMeshRenderer")]    
    [SerializeField] private Transform _viewBodyArmorMilitary;
    [SerializeField] private Transform _viewBodyArmorMilitaryMod;
    [Header("���������� � ������� ����� �����������\n��������� MeshRenderer")]
    [SerializeField] private Transform _beard;
    [SerializeField] private Transform _hair;
    [SerializeField] private Transform _headArmorMilitary;
    [SerializeField] private Transform _headArmorJunker;

    private SkinnedMeshRenderer _viewBodyArmorMilitaryRender;       // ������� ����������
    private SkinnedMeshRenderer _viewBodyArmorMilitaryModRender;    // ���������� ������� ���������� 

    private MeshRenderer[] _headArmorMilitaryMeshRendererArray;     // ���������� ������� ����
    private MeshRenderer[] _headArmorJunkerMeshRendererArray;       // ��������� ������� ����
    private MeshRenderer[] _hairMeshRendererArray;                  // ������
    private MeshRenderer[] _beardMeshRendererArray;                 // ������




    private void Awake()
    {
        InitHashData();
    }
    private void InitHashData()
    {
        _viewBodyArmorMilitaryRender = _viewBodyArmorMilitary.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewBodyArmorMilitaryModRender = _viewBodyArmorMilitaryMod.GetComponentInChildren<SkinnedMeshRenderer>();

        _headArmorMilitaryMeshRendererArray = _headArmorMilitary.GetComponentsInChildren<MeshRenderer>();
        _headArmorJunkerMeshRendererArray = _headArmorJunker.GetComponentsInChildren<MeshRenderer>();
        _hairMeshRendererArray = _hair.GetComponentsInChildren<MeshRenderer>();
        _beardMeshRendererArray = _beard.GetComponentsInChildren<MeshRenderer>();
    }

    protected override void SetBodyArmor(BodyArmorTypeSO bodyArmorTypeSO)
    {
        // ���� ��� ������� �� ����� ������� � ��������� null (null ����� ������� ������ UnitSpaceSolderView �.�. �������� ��������� ��������� �����)

        switch (bodyArmorTypeSO.GetPlacedObjectType())
        {
            case PlacedObjectType.BodyArmorMilitary:
                //��������
                _viewBodyArmorMilitaryRender.enabled = true;
                //������
                _viewBodyArmorMilitaryModRender.enabled = false;
                break;

            case PlacedObjectType.BodyArmorMilitaryMod:
                //��������
                _viewBodyArmorMilitaryModRender.enabled = true;
                //������
                _viewBodyArmorMilitaryRender.enabled = false;
                break;
        }
    }

    public override void SetHeadArmor(HeadArmorTypeSO headArmorTypeSO)
    {
        if (headArmorTypeSO == null)
        {
            // ��������
            Show(_hairMeshRendererArray);
            Show(_beardMeshRendererArray);
            // ������             
            Hide(_headArmorMilitaryMeshRendererArray);
            Hide(_headArmorJunkerMeshRendererArray);           
            return; // ������� � ���������� ��� ����
        }

        switch (headArmorTypeSO.GetPlacedObjectType())
        {
            case PlacedObjectType.HeadArmorMilitary:
                // ��������               
                Show(_headArmorMilitaryMeshRendererArray);
                Show(_beardMeshRendererArray);
                // ������                 
                Hide(_hairMeshRendererArray);
                Hide(_headArmorJunkerMeshRendererArray);                
                break;

            case PlacedObjectType.HeadArmorJunker:
                // ��������
                Show(_headArmorJunkerMeshRendererArray);
                Show(_beardMeshRendererArray);
                // ������                
                Hide(_hairMeshRendererArray);
                Hide(_headArmorMilitaryMeshRendererArray);                
                break;
            
        }
    }

}

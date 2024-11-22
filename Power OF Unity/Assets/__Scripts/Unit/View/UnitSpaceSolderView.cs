using UnityEngine;
/// <summary>
/// ������ ����� "����������� ������". 
/// </summary>
/// <remarks>�������� ����� - 1.���������(��� �����) 2.BodyArmorSpace 3.BodyArmorSpaceMod   </remarks>
public class UnitSpaceSolderView : UnitView
{
    [Header("���������� � ������� ����� �����������\n��������� SkinnedMeshRenderer")]
    [SerializeField] private Transform _viewDefault;
    [SerializeField] private Transform _viewBodyArmorSpace;
    [SerializeField] private Transform _viewBodyArmorSpaceMod;
    [SerializeField] private Transform _viewHeadArmorSpaceNoFace;
    [SerializeField] private Transform _viewHead;
    [Header("���������� � ������� ����� �����������\n��������� MeshRenderer")]
    [SerializeField] private Transform _hair;
    [SerializeField] private Transform _headArmorMilitary;
    [SerializeField] private Transform _headArmorJunker;
    [SerializeField] private Transform _beard;


    private SkinnedMeshRenderer _viewDefaultRenderer;               // ��� �����
    private SkinnedMeshRenderer _viewBodyArmorSpaceRenderer;        // ����������� ����������
    private SkinnedMeshRenderer _viewBodyArmorSpaceModRenderer;     // ����������� ��� ������������ ���������� (����������)

    private SkinnedMeshRenderer _viewHeadArmorSpaceNoFaceRenderer;  // ����������� ���� ����������� ��� ����
    private SkinnedMeshRenderer _viewHeadRenderer;                  // ���� ������, ����� ��� ����� ����� ��������� ���
    private MeshRenderer[] _headArmorMilitaryMeshRendererArray;     // ���������� ������� ����
    private MeshRenderer[] _headArmorJunkerMeshRendererArray;       // ��������� ������� ����
    private MeshRenderer[] _hairMeshRendererArray;                  // ������
    private MeshRenderer[] _beardMeshRendererArray;                 // ������

   
    /// <summary>
    /// ����� ������?
    /// </summary>
    /// <remarks>���� true �� ���� �������� ���� ������</remarks>
    private bool _isArmorOn = false;

    private void Awake()
    {
        InitHashData();       
    }

    private void InitHashData()
    {
        _viewDefaultRenderer = _viewDefault.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewBodyArmorSpaceRenderer = _viewBodyArmorSpace.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewBodyArmorSpaceModRenderer = _viewBodyArmorSpaceMod.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewHeadArmorSpaceNoFaceRenderer = _viewHeadArmorSpaceNoFace.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewHeadRenderer = _viewHead.GetComponentInChildren<SkinnedMeshRenderer>();

        _headArmorMilitaryMeshRendererArray = _headArmorMilitary.GetComponentsInChildren<MeshRenderer>();
        _headArmorJunkerMeshRendererArray = _headArmorJunker.GetComponentsInChildren<MeshRenderer>();
        _hairMeshRendererArray = _hair.GetComponentsInChildren<MeshRenderer>();
        _beardMeshRendererArray = _beard.GetComponentsInChildren<MeshRenderer>();
    }

    protected override void SetBodyArmor(BodyArmorTypeSO bodyArmorTypeSO)
    {
        if (bodyArmorTypeSO == null)
        {
            // ��������
            _viewDefaultRenderer.enabled = true;
            // ������ 
            _viewBodyArmorSpaceRenderer.enabled = false;
            _viewBodyArmorSpaceModRenderer.enabled = false;
            // ��������� 
            _isArmorOn = false;
            return; // ������� � ���������� ��� ����
        }

        switch (bodyArmorTypeSO.GetPlacedObjectType())
        {
            case PlacedObjectType.BodyArmorSpace:
                // ��������
                _viewBodyArmorSpaceRenderer.enabled = true;
                // ������ 
                _viewDefaultRenderer.enabled = false;
                _viewBodyArmorSpaceModRenderer.enabled = false;
                // ��������� 
                _isArmorOn = true;
                break;

            case PlacedObjectType.BodyArmorSpaceMod:
                // ��������
                _viewBodyArmorSpaceRenderer.enabled = true;
                _viewBodyArmorSpaceModRenderer.enabled = true;
                // ������ 
                _viewDefaultRenderer.enabled = false;
                // ��������� 
                _isArmorOn = true;
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
            _viewHeadArmorSpaceNoFaceRenderer.enabled = false;
            Hide(_headArmorMilitaryMeshRendererArray);
            Hide(_headArmorJunkerMeshRendererArray);
            // ��������� 
            _viewHeadRenderer.enabled = _isArmorOn; // ������� ��� ������ ���� ������ � ����������� �� ��������� �����(BodyArmor)
            return; // ������� � ���������� ��� ����
        }

        switch (headArmorTypeSO.GetPlacedObjectType())
        {
            case PlacedObjectType.HeadArmorMilitary:
                // ��������               
                Show(_headArmorMilitaryMeshRendererArray);
                Show(_beardMeshRendererArray);
                // ������ 
                _viewHeadArmorSpaceNoFaceRenderer.enabled = false;
                Hide(_hairMeshRendererArray);
                Hide(_headArmorJunkerMeshRendererArray);
                // ��������� 
                _viewHeadRenderer.enabled = _isArmorOn; // ������� ��� ������ ���� ������ � ����������� �� ��������� �����(BodyArmor)
                break;

            case PlacedObjectType.HeadArmorJunker:
                // ��������
                Show(_headArmorJunkerMeshRendererArray);
                Show(_beardMeshRendererArray);
                // ������ 
                _viewHeadArmorSpaceNoFaceRenderer.enabled = false;
                Hide(_hairMeshRendererArray);
                Hide(_headArmorMilitaryMeshRendererArray);
                // ��������� 
                _viewHeadRenderer.enabled = _isArmorOn; // ������� ��� ������ ���� ������ � ����������� �� ��������� �����(BodyArmor)
                break;

            case PlacedObjectType.HeadArmorSpaceNoFace:
                // ��������
                _viewHeadArmorSpaceNoFaceRenderer.enabled = true;
                // ������ 
                _viewHeadRenderer.enabled = false;
                Hide(_hairMeshRendererArray);
                Hide(_beardMeshRendererArray);
                Hide(_headArmorMilitaryMeshRendererArray);
                Hide(_headArmorJunkerMeshRendererArray);
                break;
        }
    }
  
}

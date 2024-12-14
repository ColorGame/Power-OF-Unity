using System.Collections.Generic;
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


    private SkinnedMeshRenderer _viewDefaultSkinMesh;               // ��� �����
    private SkinnedMeshRenderer _viewBodyArmorSpaceSkinMesh;        // ����������� ����������
    private SkinnedMeshRenderer _viewBodyArmorSpaceModSkinMesh;     // ����������� ��� ������������ ���������� (����������)

    private SkinnedMeshRenderer[] _viewBodySkinMeshArray;

    private SkinnedMeshRenderer _viewHeadArmorSpaceNoFaceSkinMesh;  // ����������� ���� ����������� ��� ����
    private SkinnedMeshRenderer _viewHeadSkinMesh;                  // ���� ������, ����� ��� ����� ����� ��������� ���


    /// <summary>
    /// ����������� ����� ������?
    /// </summary>
    /// <remarks>���� true �� ���� �������� ���� ������</remarks>
    private bool _isBodyArmorSpaceOn = false;

    private void Awake()
    {
        _viewDefaultSkinMesh = _viewDefault.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewBodyArmorSpaceSkinMesh = _viewBodyArmorSpace.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewBodyArmorSpaceModSkinMesh = _viewBodyArmorSpaceMod.GetComponentInChildren<SkinnedMeshRenderer>();

        _viewBodySkinMeshArray = new SkinnedMeshRenderer[]
        {
            _viewDefaultSkinMesh,
            _viewBodyArmorSpaceSkinMesh,
            _viewBodyArmorSpaceModSkinMesh,
        };

        _viewHeadArmorSpaceNoFaceSkinMesh = _viewHeadArmorSpaceNoFace.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewHeadSkinMesh = _viewHead.GetComponentInChildren<SkinnedMeshRenderer>();

        InitMeshRender();
    }


    protected override void SetBodyArmor(BodyArmorTypeSO bodyArmorTypeSO)
    {
        if (bodyArmorTypeSO == null)
        {
            SetSkinMeshInEnumerable(new HashSet<SkinnedMeshRenderer> { _viewDefaultSkinMesh }, _viewBodySkinMeshArray);           
            // ��������� 
            _isBodyArmorSpaceOn = false;
            return; 
        }

        // ��������� 
        _isBodyArmorSpaceOn = true;       

        switch (bodyArmorTypeSO.GetBodyArmorType())
        {
            case BodyArmorType.BodyArmorSpace:
                SetSkinMeshInEnumerable(new HashSet<SkinnedMeshRenderer> { _viewBodyArmorSpaceSkinMesh }, _viewBodySkinMeshArray);               
                break;

            case BodyArmorType.BodyArmorSpaceMod:
                SetSkinMeshInEnumerable(
                    new HashSet<SkinnedMeshRenderer> { _viewBodyArmorSpaceSkinMesh, _viewBodyArmorSpaceModSkinMesh }, 
                    _viewBodySkinMeshArray);               
                break;
        }
    }

    public override void SetHeadArmor(HeadArmorTypeSO headArmorTypeSO)
    {
        _viewHeadSkinMesh.enabled = _isBodyArmorSpaceOn; // ������� ��� ������ ���� ������ � ����������� �� ��������� �����(BodyArmor)
        _viewHeadArmorSpaceNoFaceSkinMesh.enabled = false; // �������� ����������� ���� ����������� ��� ���� (� ������� ������ � �����  case HeadArmorType.HeadArmorSpaceNoFace:)

        if (headArmorTypeSO == null)
        {            
            SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
            {
                _hairMeshArray,
                _beardMeshArray,
            });               
            return; // ������� � ���������� ��� ����
        }

        switch (headArmorTypeSO.GetHeadArmorType())
        {
            case HeadArmorType.HeadArmorMilitary:               
                SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
                {
                    _headArmorMilitaryMeshArray,
                    _beardMeshArray,
                });                            
                break;

            case HeadArmorType.HeadArmorJunker:               
                SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
                {
                    _headArmorJunkerMeshArray,
                    _beardMeshArray,
                });                       
                break;

            case HeadArmorType.HeadArmorSpaceNoFace:
                // ��������
                _viewHeadArmorSpaceNoFaceSkinMesh.enabled = true;
                // ������ 
                _viewHeadSkinMesh.enabled = false; // ���� �� ����� �.�. _viewHeadArmorSpaceNoFaceSkinMesh ���� ��� � ������� � ����
                HideMeshInEnumerable(_headViewList);
                break;

            case HeadArmorType.HeadArmorCyberNoFace:
                SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
                {
                    _headArmorCyberNoFaceMeshArray
                });                
                break;

            case HeadArmorType.HeadArmorCyberZenica:
                SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
                {
                    _headArmorCyberZenicaMeshArray,
                    _beardMeshArray,
                });               
                break;

            case HeadArmorType.HeadArmorCyberXO:
                SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
                {
                    _headArmorCyberXOMeshArray,
                    _beardMeshArray,
                });
                break;
        }
    }

}

using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ������ ����� "������". 
/// </summary>
/// <remarks>�������� ����� - 1.BodyArmorMilitary 2.BodyArmorMilitaryMod 3.BodyArmorCyber</remarks>
public class UnitMilitaryAndCyberSoldier : UnitView
{
    [Header("���������� � ������� ����� �����������\n��������� SkinnedMeshRenderer")]
    [SerializeField] private Transform _viewBodyArmorMilitary;
    [SerializeField] private Transform _viewBodyArmorMilitaryMod;
    [SerializeField] private Transform _viewBodyArmorCyber;
    [Header("���������� ��� ��������� �������������� ���������")]
    [SerializeField] private Transform _attachSpine01;     //��������� �������������� �������� ��� �����
    [SerializeField] private Transform _attachSpine02;     //��������� ������
    [SerializeField] private Transform _attachSpine03;     //��������� �����
    [SerializeField] private Transform _attachNeck;        //��������� �����


    private SkinnedMeshRenderer _viewBodyArmorMilitarySkinMesh;       // ������� ����������
    private SkinnedMeshRenderer _viewBodyArmorMilitaryModSkinMesh;    // ���������� ������� ���������� 
    private SkinnedMeshRenderer _viewBodyArmorCyberSkinMesh;          // ����� ����������

    private SkinnedMeshRenderer[] _viewBodySkinMeshArray;

    private MeshRenderer[] _attachSpine01MeshArray;
    private MeshRenderer[] _attachSpine02MeshArray;
    private MeshRenderer[] _attachSpine03MeshArray;
    private MeshRenderer[] _attachNeckMeshArray;

    private MeshRenderer[][] _attachViewMeshArray;


    private void Awake()
    {
        _viewBodyArmorMilitarySkinMesh = _viewBodyArmorMilitary.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewBodyArmorMilitaryModSkinMesh = _viewBodyArmorMilitaryMod.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewBodyArmorCyberSkinMesh = _viewBodyArmorCyber.GetComponentInChildren<SkinnedMeshRenderer>();

        _viewBodySkinMeshArray = new SkinnedMeshRenderer[] 
        {
            _viewBodyArmorMilitarySkinMesh,
            _viewBodyArmorMilitaryModSkinMesh,
            _viewBodyArmorCyberSkinMesh,
        };

        _attachSpine01MeshArray = _attachSpine01.GetComponentsInChildren<MeshRenderer>();
        _attachSpine02MeshArray = _attachSpine02.GetComponentsInChildren<MeshRenderer>();
        _attachSpine03MeshArray = _attachSpine03.GetComponentsInChildren<MeshRenderer>();
        _attachNeckMeshArray = _attachNeck.GetComponentsInChildren<MeshRenderer>();

        _attachViewMeshArray = new MeshRenderer[][]
        {
            _attachSpine01MeshArray,
            _attachSpine02MeshArray,
            _attachSpine03MeshArray,
            _attachNeckMeshArray
        };

        InitMeshRender();
    }

    protected override void SetBodyArmor(BodyArmorTypeSO bodyArmorTypeSO)
    {
        // ���� ��� ������� �� ����� ������� � ��������� null (null ����� ������� ������ UnitSpaceSolderView �.�. �������� ��������� ��������� �����)

        switch (bodyArmorTypeSO.GetBodyArmorType())
        {
            case BodyArmorType.BodyArmorMilitary:              
                ShowMeshInEnumerable(_attachViewMeshArray);
                SetSkinMeshInEnumerable(new HashSet<SkinnedMeshRenderer> { _viewBodyArmorMilitarySkinMesh }, _viewBodySkinMeshArray);                
                break;

            case BodyArmorType.BodyArmorMilitaryMod:                
                ShowMeshInEnumerable(_attachViewMeshArray);
                SetSkinMeshInEnumerable(new HashSet<SkinnedMeshRenderer> { _viewBodyArmorMilitaryModSkinMesh }, _viewBodySkinMeshArray);                
                break;

            case BodyArmorType.BodyArmorCyber:               
                HideMeshInEnumerable( _attachViewMeshArray);
                SetSkinMeshInEnumerable(new HashSet<SkinnedMeshRenderer> { _viewBodyArmorCyberSkinMesh }, _viewBodySkinMeshArray);                
                break;
        }
    }

    public override void SetHeadArmor(HeadArmorTypeSO headArmorTypeSO)
    {
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

            case HeadArmorType.HeadArmorCyberNoFace:
                SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
                {
                    _headArmorCyberNoFaceMeshArray,
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

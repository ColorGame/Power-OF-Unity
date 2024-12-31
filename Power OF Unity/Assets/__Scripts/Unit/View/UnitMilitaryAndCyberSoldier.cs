using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ������ ����� "������". 
/// </summary>
/// <remarks>�������� ����� - 1.BodyArmorMilitary 2.BodyArmorMilitaryMod 3.BodyArmorCyber 4.BodyArmorCyberMod</remarks>
public class UnitMilitaryAndCyberSoldier : UnitView
{
    // ����� �� ����������� ������ ��� ������� �����, ������� � ����� ������� ���������� � ������� ��������� ������ �����
    [Header("��������� ������� ����� ��� ����")]
    [SerializeField] private Transform _viewBodyArmorMilitary;       
    [SerializeField] private Transform _viewBodyArmorMilitaryMod;    
    [SerializeField] private Transform _viewBodyArmorCyber;          
    [SerializeField] private Transform _viewBodyArmorCyberMod;       
    [Header("������ ���. ��������� ��� Military")]
    [SerializeField] private MeshRenderer[] _attachMeshMilitaryArray;
    [Header("������ ���. ��������� ��� CuberMod")]
    [SerializeField] private MeshRenderer[] _attachMeshCuberModArray;

    private SkinnedMeshRenderer _viewBodyArmorMilitarySkinMesh;       // ������� ����������
    private SkinnedMeshRenderer _viewBodyArmorMilitaryModSkinMesh;    // ������� ���������� ����������
    private SkinnedMeshRenderer _viewBodyArmorCyberSkinMesh;          // ����� ����������
    private SkinnedMeshRenderer _viewBodyArmorCyberModSkinMesh;       // ����� ���������� ����������


    private MeshRenderer[][] _attachFullMeshArray;   
    private SkinnedMeshRenderer[] _viewFullBodySkinMeshArray;

    private void Awake()
    {
        _viewBodyArmorMilitarySkinMesh = _viewBodyArmorMilitary.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewBodyArmorMilitaryModSkinMesh = _viewBodyArmorMilitaryMod.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewBodyArmorCyberSkinMesh = _viewBodyArmorCyber.GetComponentInChildren<SkinnedMeshRenderer>();      
        _viewBodyArmorCyberModSkinMesh = _viewBodyArmorCyberMod.GetComponentInChildren<SkinnedMeshRenderer>();   

        _viewFullBodySkinMeshArray = new SkinnedMeshRenderer[]
        {
            _viewBodyArmorMilitarySkinMesh,
            _viewBodyArmorMilitaryModSkinMesh,
            _viewBodyArmorCyberSkinMesh,
            _viewBodyArmorCyberModSkinMesh,
        };

        _attachFullMeshArray = new MeshRenderer[][]
        {
            _attachMeshMilitaryArray,
            _attachMeshCuberModArray,
        };

        InitMeshRender();
    }

    protected override void SetBodyArmor(BodyArmorTypeSO bodyArmorTypeSO)
    {
        // ���� ��� ������� �� ����� ������� � ��������� null (null ����� ������� ������ UnitSpaceSolderView �.�. �������� ��������� ��������� �����)

        switch (bodyArmorTypeSO.GetBodyArmorType())
        {
            case BodyArmorType.BodyArmorMilitary:
                SetMeshArrayInEnumerable(new HashSet<MeshRenderer[]> { _attachMeshMilitaryArray }, _attachFullMeshArray);
                SetSkinMeshInEnumerable(new HashSet<SkinnedMeshRenderer> { _viewBodyArmorMilitarySkinMesh }, _viewFullBodySkinMeshArray);
                break;

            case BodyArmorType.BodyArmorMilitaryMod:
                SetMeshArrayInEnumerable(new HashSet<MeshRenderer[]> { _attachMeshMilitaryArray }, _attachFullMeshArray);
                SetSkinMeshInEnumerable(new HashSet<SkinnedMeshRenderer> { _viewBodyArmorMilitaryModSkinMesh }, _viewFullBodySkinMeshArray);
                break;

            case BodyArmorType.BodyArmorCyber:
                HideMeshArrayInEnumerable(_attachFullMeshArray);
                SetSkinMeshInEnumerable(new HashSet<SkinnedMeshRenderer> { _viewBodyArmorCyberSkinMesh }, _viewFullBodySkinMeshArray);
                break;

            case BodyArmorType.BodyArmorCyberMod:
                SetMeshArrayInEnumerable(new HashSet<MeshRenderer[]> { _attachMeshCuberModArray }, _attachFullMeshArray);
                SetSkinMeshInEnumerable(new HashSet<SkinnedMeshRenderer> { _viewBodyArmorCyberModSkinMesh }, _viewFullBodySkinMeshArray);
                break;
        }
    }


}

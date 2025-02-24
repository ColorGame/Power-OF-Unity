using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ������ ����� � ������������ ������ ������. ����������� �����
/// </summary>
public abstract class UnitWithChangeHeadArmorView : UnitView
{   
    [Header("����� ��� ������")]
    [SerializeField] protected MeshRenderer[] _headArmorMilitaryMeshArray;       // ���������� ������� ����
    [SerializeField] protected MeshRenderer[] _headArmorJunkerMeshArray;         // ��������� ������� ����
    [SerializeField] protected MeshRenderer[] _headArmorSpaceNoFaceMeshArray;    // ����������� ���� ����������� ��� ����
    [SerializeField] protected MeshRenderer[] _headArmorCyberXOMeshArray;        // ����� ���� XO
    [SerializeField] protected MeshRenderer[] _headArmorCyberZenicaMeshArray;    // ����� ���� ������
    [SerializeField] protected MeshRenderer[] _headArmorCyberNoFaceMeshArray;    // ����� ���� ����������� ��� ����
    [SerializeField] protected MeshRenderer[] _headArmorCyberNoFaceModMeshArray; // ����� ���� ���������� ����������� ��� ����
    // ����� �� ����������� ������ � ������ ��� ������� �����, ������� � ����� ������� ���������� � ������� ��������� ������ �����
    [Header("���������� ��� ����� � ������")]
    [SerializeField] protected Transform _hair;
    [SerializeField] protected Transform _beard;

    /// <summary>
    /// ������ ������ ������
    /// </summary>
    protected List<MeshRenderer[]> _headViewList;

    protected MeshRenderer[] _hairMeshArray;                  // ������
    protected MeshRenderer[] _beardMeshArray;                 // ������

    //��������� ������� ����� ������� ����� ���������� (���� ���� �������� ������ ���� MeshRenderer[] �� ������ ������� �� ������������������)
    protected MeshRenderer[][] _withoutHeadArmorMeshArrayEnumerable;
    protected MeshRenderer[][] _militaryHeadArmorMeshArrayEnumerable;
    protected MeshRenderer[][] _junkerHeadArmorMeshArrayEnumerable;
    protected MeshRenderer[][] _cyberXOHeadArmorMeshArrayEnumerable;
    protected MeshRenderer[][] _spaceNoFaceHeadArmorMeshArrayEnumerable;
    protected MeshRenderer[][] _cyberZenicaHeadArmorMeshArrayEnumerable;
    protected MeshRenderer[][] _cyberNoFaceHeadArmorMeshArrayEnumerable;
    protected MeshRenderer[][] _cyberNoFaceModHeadArmorMeshArrayEnumerable;


    protected void InitMeshRender()
    {
        _hairMeshArray = _hair.GetComponentsInChildren<MeshRenderer>();
        _beardMeshArray = _beard.GetComponentsInChildren<MeshRenderer>();

        _headViewList = new List<MeshRenderer[]>
        {
            _headArmorMilitaryMeshArray,
            _headArmorJunkerMeshArray,
            _headArmorSpaceNoFaceMeshArray,
            _headArmorCyberXOMeshArray,
            _headArmorCyberZenicaMeshArray,
            _headArmorCyberNoFaceMeshArray,
            _headArmorCyberNoFaceModMeshArray,
            _hairMeshArray,
            _beardMeshArray,
        };

        //������� ������������ �������
        _withoutHeadArmorMeshArrayEnumerable = new MeshRenderer[][]
        {
            _hairMeshArray,
            _beardMeshArray
        };
        _militaryHeadArmorMeshArrayEnumerable = new MeshRenderer[][]
        {
            _headArmorMilitaryMeshArray,
            _beardMeshArray
        };
        _junkerHeadArmorMeshArrayEnumerable = new MeshRenderer[][]
        {
             _headArmorJunkerMeshArray,
             _beardMeshArray
        };
        _cyberXOHeadArmorMeshArrayEnumerable = new MeshRenderer[][]
        {
            _headArmorCyberXOMeshArray,
            _beardMeshArray
        };
        _cyberZenicaHeadArmorMeshArrayEnumerable = new MeshRenderer[][]
        {
            _headArmorCyberZenicaMeshArray,
            _beardMeshArray
        };

    }

    /// <summary>
    /// ��������� ����� ������
    /// </summary>
    public override void SetHeadArmor(HeadArmorTypeSO headArmorTypeSO)
    {
        if (headArmorTypeSO == null)
        {
            SetMeshArrayInHeadViewList(_withoutHeadArmorMeshArrayEnumerable);
            return; // ������� � ���������� ��� ����
        }

        switch (headArmorTypeSO.GetHeadArmorType())
        {
            case HeadArmorType.HeadArmor_1A_Military:
                SetMeshArrayInHeadViewList(_militaryHeadArmorMeshArrayEnumerable);
                break;
            case HeadArmorType.HeadArmor_1B_Junker:
                SetMeshArrayInHeadViewList(_junkerHeadArmorMeshArrayEnumerable);
                break;
            case HeadArmorType.HeadArmor_2A_CyberXO:
                SetMeshArrayInHeadViewList(_cyberXOHeadArmorMeshArrayEnumerable);
                break;
            case HeadArmorType.HeadArmor_2B_SpaceNoFace:
                SetMeshArrayInHeadViewList(_headArmorSpaceNoFaceMeshArray);
                break;
            case HeadArmorType.HeadArmor_3A_1_CyberZenica:
                SetMeshArrayInHeadViewList(_cyberZenicaHeadArmorMeshArrayEnumerable);
                break;
            case HeadArmorType.HeadArmor_3A_2_CyberNoFace:
                SetMeshArrayInHeadViewList(_headArmorCyberNoFaceMeshArray);
                break;
            case HeadArmorType.HeadArmor_3B_CyberNoFaceMod:
                SetMeshArrayInHeadViewList(_headArmorCyberNoFaceModMeshArray);
                break;
        }
    }   

    /// <summary>
    /// ��������� "_headViewList" � ������ ���, ����� ����� ����������� "showMeshArrayEnumerable"
    /// </summary>
    protected void SetMeshArrayInHeadViewList(IEnumerable<MeshRenderer[]> showMeshArrayEnumerable)
    {
        SetMeshArrayInEnumerable(showMeshArrayEnumerable, _headViewList);
    }
    /// <summary>
    /// ��������� "_headViewList" � ������ ���, ����� ����� ����������� "showMeshArray"
    /// </summary>
    protected void SetMeshArrayInHeadViewList(MeshRenderer[] showMeshArray)
    {
        SetMeshArrayInEnumerable(showMeshArray, _headViewList);
    }   
}

using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ������ ����� "������� ����������". 
/// </summary>
/// <remarks>�������� ����� - 1.BodyArmorBigExoskeleton </remarks>
public class UnitBigExoskeletonView : UnitView
{
    [Header("���. ��������� ��� BigExoskeleton\n� ������� ����� �������������������� MeshRenderer")]
    [SerializeField] private MeshRenderer[] _headArmorBigCoverClearMeshArray;
    [SerializeField] private MeshRenderer[] _headArmorBigCoverMeshArray;   
    [SerializeField] private MeshRenderer[] _headMeshArray;


    private void Awake()
    {       
        InitMeshRender();

        _headViewList.AddRange(new List<MeshRenderer[]>
        {
            _headArmorBigCoverClearMeshArray,
            _headArmorBigCoverMeshArray,           
            _headMeshArray,
        });
    }



    protected override void SetBodyArmor(BodyArmorTypeSO bodyArmorTypeSO)
    {
        // ������ ����� (� ����� ������� ������ ���� ��� �����)       
    }

    public override void SetHeadArmor(HeadArmorTypeSO headArmorTypeSO)
    {
        if (headArmorTypeSO == null)
        {
            SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
            {
                _hairMeshArray,
                _beardMeshArray,
                _headMeshArray,
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
                    _headMeshArray,
                });
                break;

            case HeadArmorType.HeadArmorJunker:
                SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
                {
                    _headArmorJunkerMeshArray,
                    _beardMeshArray,
                    _headMeshArray,
                });
                break;

            case HeadArmorType.HeadArmorSpaceNoFace:
                SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
                {
                    _headArmorSpaceNoFaceMeshArray,
                    _headMeshArray,
                });
                break;            

            case HeadArmorType.HeadArmorCyberZenica:
                SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
                {
                    _headArmorCyberZenicaMeshArray,
                    _beardMeshArray,
                    _headMeshArray,
                });
                break;

            case HeadArmorType.HeadArmorCyberXO:
                SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
                {
                    _headArmorCyberXOMeshArray,
                    _beardMeshArray,
                    _headMeshArray,
                });
                break;

            case HeadArmorType.HeadArmorCyberNoFace:
                SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
                {
                    _headArmorCyberNoFaceMeshArray,
                    _headMeshArray,
                });
                break;

            case HeadArmorType.HeadArmorCyberModNoFace:
                SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
                {
                    _headArmorCyberModNoFaceMeshArray,
                    _headMeshArray,
                });
                break;

            case HeadArmorType.HeadArmorBigCoverClear:
                SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
                {
                    _headArmorBigCoverClearMeshArray,
                    _hairMeshArray,
                    _beardMeshArray,
                    _headMeshArray,
                });
                break;

            case HeadArmorType.HeadArmorBigCover:
                SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
                {
                    _headArmorBigCoverMeshArray,
                });
                break;
        }
    }



}

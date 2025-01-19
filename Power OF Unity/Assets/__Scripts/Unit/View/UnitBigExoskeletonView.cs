using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Визуал юнита "Большой экзоскилет". 
/// </summary>
/// <remarks>сотояние брони - 1.BodyArmor_4A_BigExoskeleton </remarks>
public class UnitBigExoskeletonView : UnitView
{
    [Header("Доп. контйнеры для BigExoskeleton\nв которых будем переключатьвидимость MeshRenderer")]
    [SerializeField] private MeshRenderer[] _headArmorBigCoverClearMeshArray;
    [SerializeField] private MeshRenderer[] _headArmorBigCoverMeshArray;
    [SerializeField] private MeshRenderer[] _headMeshArray;

 
    private MeshRenderer[][] _bigCoverClearHeadArmor;

    private void Awake()
    {
        InitMeshRender();

        _headViewList.AddRange(new List<MeshRenderer[]>
        {
            _headArmorBigCoverClearMeshArray,
            _headArmorBigCoverMeshArray,
            _headMeshArray,
        });
        //Запоним хэшированные массивы
        _withoutHeadArmorMeshArrayEnumerable = new MeshRenderer[][]
        {
            _hairMeshArray,
            _beardMeshArray,
            _headMeshArray
        };
        _militaryHeadArmorMeshArrayEnumerable = new MeshRenderer[][]
        {
            _headArmorMilitaryMeshArray,
            _beardMeshArray,
            _headMeshArray,
        };
        _junkerHeadArmorMeshArrayEnumerable = new MeshRenderer[][]
        {
             _headArmorJunkerMeshArray,
             _beardMeshArray,
             _headMeshArray
        };
        _cyberXOHeadArmorMeshArrayEnumerable = new MeshRenderer[][]
        {
            _headArmorCyberXOMeshArray,
            _beardMeshArray,
            _headMeshArray
        };
        _spaceNoFaceHeadArmorMeshArrayEnumerable = new MeshRenderer[][]
        {
            _headArmorSpaceNoFaceMeshArray,
            _headMeshArray
        };
        _cyberZenicaHeadArmorMeshArrayEnumerable = new MeshRenderer[][]
        {
            _headArmorCyberZenicaMeshArray,
            _beardMeshArray,
            _headMeshArray
        };
        _cyberNoFaceHeadArmorMeshArrayEnumerable = new MeshRenderer[][]
        {
             _headArmorCyberNoFaceMeshArray,
             _headMeshArray
        };
        _cyberNoFaceModHeadArmorMeshArrayEnumerable = new MeshRenderer[][]
        {
             _headArmorCyberNoFaceModMeshArray,
             _headMeshArray
        };
        _bigCoverClearHeadArmor = new MeshRenderer[][]
        {
             _headArmorBigCoverClearMeshArray,
             _hairMeshArray,
             _beardMeshArray,
             _headMeshArray
        };
    }



    protected override void SetBodyArmor(BodyArmorTypeSO bodyArmorTypeSO)
    {
        // Пустой метод (у этого визуала только один вид брони)       
    }

    public override void SetHeadArmor(HeadArmorTypeSO headArmorTypeSO)
    {
        if (headArmorTypeSO == null)
        {
            SetMeshArrayInHeadViewList(_withoutHeadArmorMeshArrayEnumerable);
            return; // выходим и игнорируем код ниже
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
                SetMeshArrayInHeadViewList(_spaceNoFaceHeadArmorMeshArrayEnumerable);
                break;
            case HeadArmorType.HeadArmor_3A_1_CyberZenica:
                SetMeshArrayInHeadViewList(_cyberZenicaHeadArmorMeshArrayEnumerable);
                break;
            case HeadArmorType.HeadArmor_3A_2_CyberNoFace:
                SetMeshArrayInHeadViewList(_cyberNoFaceHeadArmorMeshArrayEnumerable);
                break;
            case HeadArmorType.HeadArmor_3B_CyberNoFaceMod:
                SetMeshArrayInHeadViewList(_cyberNoFaceModHeadArmorMeshArrayEnumerable);
                break;
            case HeadArmorType.HeadArmor_4A_BigCoverClear:
                SetMeshArrayInHeadViewList(_bigCoverClearHeadArmor);
                break;
            case HeadArmorType.HeadArmor_4B_BigCover:
                SetMeshArrayInHeadViewList(_headArmorBigCoverMeshArray);
                break;
        }
    }
}

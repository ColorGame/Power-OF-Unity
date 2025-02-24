using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Визуал ЮНИТА с изменяющейся броней головы. Абстрактный класс
/// </summary>
public abstract class UnitWithChangeHeadArmorView : UnitView
{   
    [Header("Броня для головы")]
    [SerializeField] protected MeshRenderer[] _headArmorMilitaryMeshArray;       // Стандартый военный шлем
    [SerializeField] protected MeshRenderer[] _headArmorJunkerMeshArray;         // Улучшеный военный шлем
    [SerializeField] protected MeshRenderer[] _headArmorSpaceNoFaceMeshArray;    // Космический шлем закрывающий все лицо
    [SerializeField] protected MeshRenderer[] _headArmorCyberXOMeshArray;        // Кибер шлем XO
    [SerializeField] protected MeshRenderer[] _headArmorCyberZenicaMeshArray;    // Кибер шлем Зеница
    [SerializeField] protected MeshRenderer[] _headArmorCyberNoFaceMeshArray;    // Кибер шлем закрывающий все лицо
    [SerializeField] protected MeshRenderer[] _headArmorCyberNoFaceModMeshArray; // Кибер шлем улучшенный закрывающий все лицо
    // Чтобы не настраивать волосы и бороду для каждого юнита, закинем в общем префабе контейнеры в которых храняться нужные вьюхи
    [Header("Контейнеры для волос и бороды")]
    [SerializeField] protected Transform _hair;
    [SerializeField] protected Transform _beard;

    /// <summary>
    /// Список Вьюшек головы
    /// </summary>
    protected List<MeshRenderer[]> _headViewList;

    protected MeshRenderer[] _hairMeshArray;                  // Волосы
    protected MeshRenderer[] _beardMeshArray;                 // Борода

    //Двумерный массивы мешей которые будем показывать (если надо показать только один MeshRenderer[] то массив оставим не инициализированным)
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

        //Запоним хэшированные массивы
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
    /// Настройка брони головы
    /// </summary>
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
    /// Переберет "_headViewList" и скроет все, кроме кроме полученного "showMeshArrayEnumerable"
    /// </summary>
    protected void SetMeshArrayInHeadViewList(IEnumerable<MeshRenderer[]> showMeshArrayEnumerable)
    {
        SetMeshArrayInEnumerable(showMeshArrayEnumerable, _headViewList);
    }
    /// <summary>
    /// Переберет "_headViewList" и скроет все, кроме кроме полученного "showMeshArray"
    /// </summary>
    protected void SetMeshArrayInHeadViewList(MeshRenderer[] showMeshArray)
    {
        SetMeshArrayInEnumerable(showMeshArray, _headViewList);
    }   
}

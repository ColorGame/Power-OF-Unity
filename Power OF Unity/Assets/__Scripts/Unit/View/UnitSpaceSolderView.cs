using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Визуал юнита "Космический солдат". 
/// </summary>
/// <remarks>сотояние брони - 1.Дефолтное(без брони) 2.BodyArmorSpace 3.BodyArmorSpaceMod   </remarks>
public class UnitSpaceSolderView : UnitView
{
    [Header("Контейнеры в которых будем переключать\nвидимость SkinnedMeshRenderer")]
    [SerializeField] private Transform _viewDefault;
    [SerializeField] private Transform _viewBodyArmorSpace;
    [SerializeField] private Transform _viewBodyArmorSpaceMod;
    [SerializeField] private Transform _viewHeadArmorSpaceNoFace;
    [SerializeField] private Transform _viewHead;


    private SkinnedMeshRenderer _viewDefaultSkinMesh;               // Без брони
    private SkinnedMeshRenderer _viewBodyArmorSpaceSkinMesh;        // Космический бронежилет
    private SkinnedMeshRenderer _viewBodyArmorSpaceModSkinMesh;     // Модификации для космического бронежилет (дополнение)

    private SkinnedMeshRenderer[] _viewBodySkinMeshArray;

    private SkinnedMeshRenderer _viewHeadArmorSpaceNoFaceSkinMesh;  // Космический шлем закрывающий все лицо
    private SkinnedMeshRenderer _viewHeadSkinMesh;                  // Бюст головы, когда нет брони будем отключать его


    /// <summary>
    /// Космическая броня надета?
    /// </summary>
    /// <remarks>Если true то надо включить бюст головы</remarks>
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
            // настроить 
            _isBodyArmorSpaceOn = false;
            return; 
        }

        // настроить 
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
        _viewHeadSkinMesh.enabled = _isBodyArmorSpaceOn; // Покажем или скроем бюст головы в зависимости от состояния брони(BodyArmor)
        _viewHeadArmorSpaceNoFaceSkinMesh.enabled = false; // Отключим Космический шлем закрывающий все лицо (и включим только в блоке  case HeadArmorType.HeadArmorSpaceNoFace:)

        if (headArmorTypeSO == null)
        {            
            SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
            {
                _hairMeshArray,
                _beardMeshArray,
            });               
            return; // выходим и игнорируем код ниже
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
                // показать
                _viewHeadArmorSpaceNoFaceSkinMesh.enabled = true;
                // скрыть 
                _viewHeadSkinMesh.enabled = false; // Бюст не нужен т.к. _viewHeadArmorSpaceNoFaceSkinMesh идет уже с головой и шеей
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

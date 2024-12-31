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
    [SerializeField] private Transform _viewHead;
    [SerializeField] private MeshRenderer _neckMask; // Черная вставка закрывающая шею


    private SkinnedMeshRenderer _viewDefaultSkinMesh;               // Без брони
    private SkinnedMeshRenderer _viewBodyArmorSpaceSkinMesh;        // Космический бронежилет
    private SkinnedMeshRenderer _viewBodyArmorSpaceModSkinMesh;     // Модификации для космического бронежилет (дополнение)
    private SkinnedMeshRenderer _viewHeadSkinMesh;                  // Бюст головы, (космическая броня идет без головы, поэтому вклю)

    private SkinnedMeshRenderer[] _viewFullBodySkinMeshArray;


    private void Awake()
    {
        _viewDefaultSkinMesh = _viewDefault.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewBodyArmorSpaceSkinMesh = _viewBodyArmorSpace.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewBodyArmorSpaceModSkinMesh = _viewBodyArmorSpaceMod.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewHeadSkinMesh = _viewHead.GetComponentInChildren<SkinnedMeshRenderer>();

        _viewFullBodySkinMeshArray = new SkinnedMeshRenderer[]
        {
            _viewDefaultSkinMesh,
            _viewBodyArmorSpaceSkinMesh,
            _viewBodyArmorSpaceModSkinMesh,
            _viewHeadSkinMesh,
        };

        InitMeshRender();
    }


    protected override void SetBodyArmor(BodyArmorTypeSO bodyArmorTypeSO)
    {
        if (bodyArmorTypeSO == null)
        {
            SetSkinMeshInEnumerable(new HashSet<SkinnedMeshRenderer> { _viewDefaultSkinMesh },
                _viewFullBodySkinMeshArray);
            _neckMask.enabled = false;
            return;
        }

        switch (bodyArmorTypeSO.GetBodyArmorType())
        {
            case BodyArmorType.BodyArmorSpace:
                SetSkinMeshInEnumerable(new HashSet<SkinnedMeshRenderer> { _viewBodyArmorSpaceSkinMesh, _viewHeadSkinMesh },
                    _viewFullBodySkinMeshArray);
                break;

            case BodyArmorType.BodyArmorSpaceMod:
                SetSkinMeshInEnumerable(
                    new HashSet<SkinnedMeshRenderer> { _viewBodyArmorSpaceSkinMesh, _viewBodyArmorSpaceModSkinMesh, _viewHeadSkinMesh },
                    _viewFullBodySkinMeshArray);
                break;
        }
        _neckMask.enabled = true;
    }

}

using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Визуал юнита "Большой экзоскилет". 
/// </summary>
/// <remarks>сотояние брони - 1.BodyArmorBigExoskeleton </remarks>
public class UnitBigExoskeletonView : UnitView
{
    [Header("Контейнеры в которых будем переключать\nвидимость MeshRenderer")]
    [SerializeField] private Transform _headArmorBigCoverClear;
    [SerializeField] private Transform _headArmorBigCover;
    [SerializeField] private Transform _headArmorSpaceNoFace;
    [SerializeField] private Transform _headArmorMilitary;
    [SerializeField] private Transform _headArmorJunker;
    [SerializeField] private Transform _headWithFace;
    [SerializeField] private Transform _hair;
    [SerializeField] private Transform _beard;

    private MeshRenderer[] _headArmorBigCoverClearMeshRendererArray;
    private MeshRenderer[] _headArmorBigCoverMeshRendererArray;
    private MeshRenderer[] _headArmorSpaceNoFaceMeshRendererArray;
    private MeshRenderer[] _headArmorMilitaryMeshRendererArray;
    private MeshRenderer[] _headArmorJunkerMeshRendererArray;
    private MeshRenderer[] _headWithFaceMeshRendererArray;
    private MeshRenderer[] _hairMeshRendererArray;
    private MeshRenderer[] _beardMeshRendererArray;

    private IEnumerable<MeshRenderer[]> _enumerableHeadArmorView;
    private void Awake()
    {
        InitHashData();
    }

    private void InitHashData()
    {
        _headArmorBigCoverClearMeshRendererArray = _headArmorBigCoverClear.GetComponentsInChildren<MeshRenderer>();
        _headArmorBigCoverMeshRendererArray = _headArmorBigCover.GetComponentsInChildren<MeshRenderer>();
        _headArmorSpaceNoFaceMeshRendererArray = _headArmorSpaceNoFace.GetComponentsInChildren<MeshRenderer>();
        _headArmorMilitaryMeshRendererArray = _headArmorMilitary.GetComponentsInChildren<MeshRenderer>();
        _headArmorJunkerMeshRendererArray = _headArmorJunker.GetComponentsInChildren<MeshRenderer>();
        _headWithFaceMeshRendererArray = _headWithFace.GetComponentsInChildren<MeshRenderer>();
        _hairMeshRendererArray = _hair.GetComponentsInChildren<MeshRenderer>();
        _beardMeshRendererArray = _beard.GetComponentsInChildren<MeshRenderer>();

        _enumerableHeadArmorView = new MeshRenderer[][] 
        {
            _headArmorBigCoverClearMeshRendererArray,
            _headArmorBigCoverMeshRendererArray,
            _headArmorSpaceNoFaceMeshRendererArray,
            _headArmorMilitaryMeshRendererArray,
            _headArmorJunkerMeshRendererArray,
            _headWithFaceMeshRendererArray,
            _hairMeshRendererArray,
            _beardMeshRendererArray,
        };
    }

    public override void SetBodyArmor(BodyArmorTypeSO bodyArmorTypeSO)
    {
        // Пустой метод       
    }

    public override void SetHeadArmor(HeadArmorTypeSO headArmorTypeSO)
    {
        if (headArmorTypeSO == null)
        {
            ShowArray(new HashSet<MeshRenderer[]>
            {
                _hairMeshRendererArray,
                _beardMeshRendererArray,
                _headWithFaceMeshRendererArray,
            });           
            return; // выходим и игнорируем код ниже
        }

        switch (headArmorTypeSO.GetPlacedObjectType())
        {
            case PlacedObjectType.HeadArmorMilitary:
                ShowArray(new HashSet<MeshRenderer[]>
                {
                    _headArmorMilitaryMeshRendererArray,
                    _beardMeshRendererArray,
                    _headWithFaceMeshRendererArray,
                });                
                break;

            case PlacedObjectType.HeadArmorJunker:
                ShowArray(new HashSet<MeshRenderer[]>
                {
                    _headArmorJunkerMeshRendererArray,
                    _beardMeshRendererArray,
                    _headWithFaceMeshRendererArray,
                });                
                break;

            case PlacedObjectType.HeadArmorSpaceNoFace:
                ShowArray(new HashSet<MeshRenderer[]>
                {
                    _headArmorSpaceNoFaceMeshRendererArray,
                });
                break;
            case PlacedObjectType.HeadArmorBigCoverClear:
                ShowArray(new HashSet<MeshRenderer[]>
                {
                    _headArmorBigCoverClearMeshRendererArray,
                    _hairMeshRendererArray,
                    _beardMeshRendererArray,
                    _headWithFaceMeshRendererArray,
                });
                break;
            case PlacedObjectType.HeadArmorBigCover:
                ShowArray(new HashSet<MeshRenderer[]>
                {
                    _headArmorBigCoverMeshRendererArray,                    
                });
                break;
        }
    }

    public override void SetMainWeapon(PlacedObjectTypeWithActionSO placedObjectTypeWithActionSO)
    {

    }
    
    private void ShowArray(HashSet<MeshRenderer[]> showHashList)
    {
        foreach (MeshRenderer[] viewHead in _enumerableHeadArmorView)
        {            
            bool contains = showHashList.Contains(viewHead);
            foreach (MeshRenderer view in viewHead)
            {
                view.enabled = contains;
            }
        }
    }
}

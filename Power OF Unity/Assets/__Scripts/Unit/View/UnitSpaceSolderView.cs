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
    [Header("Контейнеры в которых будем переключать\nвидимость MeshRenderer")]
    [SerializeField] private Transform _hair;
    [SerializeField] private Transform _headArmorMilitary;
    [SerializeField] private Transform _headArmorJunker;
    [SerializeField] private Transform _beard;


    private SkinnedMeshRenderer _viewDefaultRenderer;               // Без брони
    private SkinnedMeshRenderer _viewBodyArmorSpaceRenderer;        // Космический бронежилет
    private SkinnedMeshRenderer _viewBodyArmorSpaceModRenderer;     // Модификации для космического бронежилет (дополнение)

    private SkinnedMeshRenderer _viewHeadArmorSpaceNoFaceRenderer;  // Космический шлем закрывающий все лицо
    private SkinnedMeshRenderer _viewHeadRenderer;                  // Бюст головы, когда нет брони будем отключать его
    private MeshRenderer[] _headArmorMilitaryMeshRendererArray;     // Стандартый военный шлем
    private MeshRenderer[] _headArmorJunkerMeshRendererArray;       // Улучшеный военный шлем
    private MeshRenderer[] _hairMeshRendererArray;                  // Волосы
    private MeshRenderer[] _beardMeshRendererArray;                 // Борода

   
    /// <summary>
    /// Броня надета?
    /// </summary>
    /// <remarks>Если true то надо включить бюст головы</remarks>
    private bool _isArmorOn = false;

    private void Awake()
    {
        InitHashData();       
    }

    private void InitHashData()
    {
        _viewDefaultRenderer = _viewDefault.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewBodyArmorSpaceRenderer = _viewBodyArmorSpace.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewBodyArmorSpaceModRenderer = _viewBodyArmorSpaceMod.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewHeadArmorSpaceNoFaceRenderer = _viewHeadArmorSpaceNoFace.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewHeadRenderer = _viewHead.GetComponentInChildren<SkinnedMeshRenderer>();

        _headArmorMilitaryMeshRendererArray = _headArmorMilitary.GetComponentsInChildren<MeshRenderer>();
        _headArmorJunkerMeshRendererArray = _headArmorJunker.GetComponentsInChildren<MeshRenderer>();
        _hairMeshRendererArray = _hair.GetComponentsInChildren<MeshRenderer>();
        _beardMeshRendererArray = _beard.GetComponentsInChildren<MeshRenderer>();
    }

    protected override void SetBodyArmor(BodyArmorTypeSO bodyArmorTypeSO)
    {
        if (bodyArmorTypeSO == null)
        {
            // показать
            _viewDefaultRenderer.enabled = true;
            // скрыть 
            _viewBodyArmorSpaceRenderer.enabled = false;
            _viewBodyArmorSpaceModRenderer.enabled = false;
            // настроить 
            _isArmorOn = false;
            return; // выходим и игнорируем код ниже
        }

        switch (bodyArmorTypeSO.GetPlacedObjectType())
        {
            case PlacedObjectType.BodyArmorSpace:
                // показать
                _viewBodyArmorSpaceRenderer.enabled = true;
                // скрыть 
                _viewDefaultRenderer.enabled = false;
                _viewBodyArmorSpaceModRenderer.enabled = false;
                // настроить 
                _isArmorOn = true;
                break;

            case PlacedObjectType.BodyArmorSpaceMod:
                // показать
                _viewBodyArmorSpaceRenderer.enabled = true;
                _viewBodyArmorSpaceModRenderer.enabled = true;
                // скрыть 
                _viewDefaultRenderer.enabled = false;
                // настроить 
                _isArmorOn = true;
                break;
        }
    }

    public override void SetHeadArmor(HeadArmorTypeSO headArmorTypeSO)
    {
        if (headArmorTypeSO == null)
        {
            // показать
            Show(_hairMeshRendererArray);
            Show(_beardMeshRendererArray);
            // скрыть 
            _viewHeadArmorSpaceNoFaceRenderer.enabled = false;
            Hide(_headArmorMilitaryMeshRendererArray);
            Hide(_headArmorJunkerMeshRendererArray);
            // настроить 
            _viewHeadRenderer.enabled = _isArmorOn; // Покажем или скроем бюст головы в зависимости от состояния брони(BodyArmor)
            return; // выходим и игнорируем код ниже
        }

        switch (headArmorTypeSO.GetPlacedObjectType())
        {
            case PlacedObjectType.HeadArmorMilitary:
                // показать               
                Show(_headArmorMilitaryMeshRendererArray);
                Show(_beardMeshRendererArray);
                // скрыть 
                _viewHeadArmorSpaceNoFaceRenderer.enabled = false;
                Hide(_hairMeshRendererArray);
                Hide(_headArmorJunkerMeshRendererArray);
                // настроить 
                _viewHeadRenderer.enabled = _isArmorOn; // Покажем или скроем бюст головы в зависимости от состояния брони(BodyArmor)
                break;

            case PlacedObjectType.HeadArmorJunker:
                // показать
                Show(_headArmorJunkerMeshRendererArray);
                Show(_beardMeshRendererArray);
                // скрыть 
                _viewHeadArmorSpaceNoFaceRenderer.enabled = false;
                Hide(_hairMeshRendererArray);
                Hide(_headArmorMilitaryMeshRendererArray);
                // настроить 
                _viewHeadRenderer.enabled = _isArmorOn; // Покажем или скроем бюст головы в зависимости от состояния брони(BodyArmor)
                break;

            case PlacedObjectType.HeadArmorSpaceNoFace:
                // показать
                _viewHeadArmorSpaceNoFaceRenderer.enabled = true;
                // скрыть 
                _viewHeadRenderer.enabled = false;
                Hide(_hairMeshRendererArray);
                Hide(_beardMeshRendererArray);
                Hide(_headArmorMilitaryMeshRendererArray);
                Hide(_headArmorJunkerMeshRendererArray);
                break;
        }
    }
  
}

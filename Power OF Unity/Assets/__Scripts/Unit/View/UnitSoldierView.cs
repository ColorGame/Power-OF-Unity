using UnityEngine;
/// <summary>
/// Визуал юнита "Солдат". 
/// </summary>
/// <remarks>сотояние брони - 1.BodyArmorMilitary 2.BodyArmorMilitaryMod </remarks>
public class UnitSoldierView : UnitView
{
    [Header("Контейнеры в которых будем переключать\nвидимость SkinnedMeshRenderer")]    
    [SerializeField] private Transform _viewBodyArmorMilitary;
    [SerializeField] private Transform _viewBodyArmorMilitaryMod;
    [Header("Контейнеры в которых будем переключать\nвидимость MeshRenderer")]
    [SerializeField] private Transform _beard;
    [SerializeField] private Transform _hair;
    [SerializeField] private Transform _headArmorMilitary;
    [SerializeField] private Transform _headArmorJunker;

    private SkinnedMeshRenderer _viewBodyArmorMilitaryRender;       // Военный бронежилет
    private SkinnedMeshRenderer _viewBodyArmorMilitaryModRender;    // Улучшенный военный бронежилет 

    private MeshRenderer[] _headArmorMilitaryMeshRendererArray;     // Стандартый военный шлем
    private MeshRenderer[] _headArmorJunkerMeshRendererArray;       // Улучшеный военный шлем
    private MeshRenderer[] _hairMeshRendererArray;                  // Волосы
    private MeshRenderer[] _beardMeshRendererArray;                 // Борода




    private void Awake()
    {
        InitHashData();
    }
    private void InitHashData()
    {
        _viewBodyArmorMilitaryRender = _viewBodyArmorMilitary.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewBodyArmorMilitaryModRender = _viewBodyArmorMilitaryMod.GetComponentInChildren<SkinnedMeshRenderer>();

        _headArmorMilitaryMeshRendererArray = _headArmorMilitary.GetComponentsInChildren<MeshRenderer>();
        _headArmorJunkerMeshRendererArray = _headArmorJunker.GetComponentsInChildren<MeshRenderer>();
        _hairMeshRendererArray = _hair.GetComponentsInChildren<MeshRenderer>();
        _beardMeshRendererArray = _beard.GetComponentsInChildren<MeshRenderer>();
    }

    protected override void SetBodyArmor(BodyArmorTypeSO bodyArmorTypeSO)
    {
        // Этот тип визуала не может принять в аргументе null (null может принять только UnitSpaceSolderView т.к. содержит дефолтное состояние брони)

        switch (bodyArmorTypeSO.GetPlacedObjectType())
        {
            case PlacedObjectType.BodyArmorMilitary:
                //показать
                _viewBodyArmorMilitaryRender.enabled = true;
                //скрыть
                _viewBodyArmorMilitaryModRender.enabled = false;
                break;

            case PlacedObjectType.BodyArmorMilitaryMod:
                //показать
                _viewBodyArmorMilitaryModRender.enabled = true;
                //скрыть
                _viewBodyArmorMilitaryRender.enabled = false;
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
            Hide(_headArmorMilitaryMeshRendererArray);
            Hide(_headArmorJunkerMeshRendererArray);           
            return; // выходим и игнорируем код ниже
        }

        switch (headArmorTypeSO.GetPlacedObjectType())
        {
            case PlacedObjectType.HeadArmorMilitary:
                // показать               
                Show(_headArmorMilitaryMeshRendererArray);
                Show(_beardMeshRendererArray);
                // скрыть                 
                Hide(_hairMeshRendererArray);
                Hide(_headArmorJunkerMeshRendererArray);                
                break;

            case PlacedObjectType.HeadArmorJunker:
                // показать
                Show(_headArmorJunkerMeshRendererArray);
                Show(_beardMeshRendererArray);
                // скрыть                
                Hide(_hairMeshRendererArray);
                Hide(_headArmorMilitaryMeshRendererArray);                
                break;
            
        }
    }

}

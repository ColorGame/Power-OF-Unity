using UnityEngine;
/// <summary>
/// Визуал сетки экипировки, создается в каждой позиции сетки, и меняет цвет в зависимости от переданных параметров
/// </summary>
public class EquipmentGridVisualSingleInWorldSpace : EquipmentGridVisualSingle
{
    [SerializeField] private MeshRenderer _meshRendererQuad; // Будем менять материал для визуализиции сетки экипировки

    public override void Init(float cellSize)
    {      
        transform.localScale = Vector2.one * cellSize;
    }

    protected override void Hide() // Скрыть
    {
        _meshRendererQuad.enabled = false;
    }

    public override void Show(Material material) //Показать и Установить переданный материал
    {
        _meshRendererQuad.enabled = true;
        _meshRendererQuad.material = material;       
    }

    
}

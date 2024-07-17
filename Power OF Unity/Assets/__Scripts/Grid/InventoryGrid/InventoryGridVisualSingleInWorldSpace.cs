using UnityEngine;
/// <summary>
/// Визуал сетки инвенторя, создается в каждой позиции сетки, и меняет цвет в зависимости от переданных параметров
/// </summary>
public class InventoryGridVisualSingleInWorldSpace : InventoryGridVisualSingle
{
    [SerializeField] private MeshRenderer _meshRendererQuad; // Будем менять материал для визуализиции сетки инвенторя

   

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

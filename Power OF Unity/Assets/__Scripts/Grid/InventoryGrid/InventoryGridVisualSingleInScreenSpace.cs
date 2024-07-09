using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Визуал сетки инвенторя, создается в каждой позиции сетки, и меняет цвет в зависимости от переданных параметров
/// </summary>
public class InventoryGridVisualSingleInScreenSpace : InventoryGridVisualSingle
{
    [SerializeField] private Image _image; // Будем менять материал для визуализиции сетки инвенторя

    private void Start()
    {
        RectTransform rectTransform = (RectTransform)transform;
        rectTransform.sizeDelta = Vector2.one * InventoryGrid.GetCellSize();
    }   

    public override  void Hide() // Скрыть
    {
        _image.enabled = false;
    }

    public override void Show(Material material) //Показать и Установить переданный материал
    {
        _image.enabled = true;
        _image.material = material;
    }
}

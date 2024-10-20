using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Визуал сетки экипировки, создается в каждой позиции сетки, и меняет цвет в зависимости от переданных параметров
/// </summary>
public class EquipmentGridVisualSingleInScreenSpace : EquipmentGridVisualSingle
{
    [SerializeField] private Image _image; // Будем менять материал для визуализиции сетки экипировки


    public override void Init(float cellSize)
    {
       /* RectTransform rectTransform = (RectTransform)transform;
        rectTransform.sizeDelta = Vector2.one * cellSize;*/
    }   

    protected override  void Hide() // Скрыть
    {
        _image.enabled = false;
    }

    public override void Show(Material material) //Показать и Установить переданный материал
    {
        _image.enabled = true;
        _image.material = material;
    }
}

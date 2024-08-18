using UnityEngine;
/// <summary>
/// Абстрактный класс. Визуал сетки инвенторя, создается в каждой ячейки и меняет цвет в зависимости от переданных параметров
/// </summary>
public abstract class InventoryGridVisualSingle : MonoBehaviour
{
    private bool _isBusy; //Занято  

    public abstract void Init(float cellSize);

    protected abstract void Hide(); // Скрыть

    public abstract void Show(Material material); //Показать и Установить переданный материал
   

    public virtual void SetIsBusyAndMaterial(bool isBusy, Material material)
    {
        _isBusy = isBusy;
        if (_isBusy) // Если ячейка занята то
        {
            Hide(); // Скроем сетку
        }
        else
        {
            Show(material); // Показать
        }
    }

    public virtual bool GetIsBusy()
    {
        return _isBusy;
    }
}

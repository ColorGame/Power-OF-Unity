using UnityEngine;
using UnityEngine.UI;

public class EquipmentGridVisualSingleSetColor : MonoBehaviour
{
    private Image _image; // Будем менять материал для визуализиции сетки экипировки
    private bool _isBusy; //Занято     

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void Hide() // Скрыть
    {
        _image.enabled = false;
    }

    public void Show(Color color) //Показать и Установить переданный цвет
    {
        _image.enabled = true;
        _image.color = color;
    }

    public void SetIsBusyAndColor(bool isBusy, Color color)
    {
        _isBusy = isBusy;
        if (_isBusy) // Если ячейка занята то
        {
            Hide(); // Скроем сетку
        }
        else
        {
            Show(color); // Показать
        }
    }

    public virtual bool GetIsBusy()
    {
        return _isBusy;
    }
}

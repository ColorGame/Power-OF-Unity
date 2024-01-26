using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryGridSystemVisualSingle : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRendererQuad; // Будем менять материал для визуализиции сетки инвенторя

    private bool _isBusy; //Занято 

    public void Hide() // Скрыть
    {
        _meshRendererQuad.enabled = false;
    }

    public void Show(Material material) //Показать и Установить переданный материал
    {
        _meshRendererQuad.enabled = true;
        _meshRendererQuad.material = material;
    }

    public void SetIsBusyAndMaterial(bool isBusy, Material material)
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

    public bool GetIsBusy()
    {
        return _isBusy;
    }

   
   
}

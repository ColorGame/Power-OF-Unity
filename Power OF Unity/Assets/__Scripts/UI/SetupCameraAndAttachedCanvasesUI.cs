using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
/// <summary>
/// Настройка камеры и прикрепленных ХОЛСТОВ(Canvas)
/// </summary>
/// <remarks>
/// Должен висеть на КАМЕРЕ
/// </remarks>
public class SetupCameraAndAttachedCanvasesUI : MonoBehaviour
{
    private Canvas[] _canvasAttachedArray;
    private Camera _cameraInventoryUI;
    private void Awake()
    {
        _canvasAttachedArray = GetComponentsInChildren<Canvas>();
        _cameraInventoryUI = GetComponent<Camera>();
    }

    private void Start()
    {
        // При смене сцены надо вновь добавлять в стек текущей камеры
        Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(_cameraInventoryUI); //Добавим в стек основной камеры нашу камеру инвенторя


        _cameraInventoryUI.cullingMask = LayerMask.GetMask("UI","Inventory","InventoryHidden"); // Настроим слои маски для отображения

        foreach (Canvas canvas in _canvasAttachedArray)
        {
            canvas.worldCamera = _cameraInventoryUI; // Установим для холста камеру которая будет его рендерить
        }      
    }
}

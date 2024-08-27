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
    private Camera _cameraEquipmentUI;
    private void Awake()
    {
        _canvasAttachedArray = GetComponentsInChildren<Canvas>();
        _cameraEquipmentUI = GetComponent<Camera>();
    }

    private void Start()
    {
        // При смене сцены надо вновь добавлять в стек текущей камеры
        Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(_cameraEquipmentUI); //Добавим в стек основной камеры нашу камеру экипировки


        _cameraEquipmentUI.cullingMask = LayerMask.GetMask("UI","Equipment","EquipmentHidden"); // Настроим слои маски для отображения

        foreach (Canvas canvas in _canvasAttachedArray)
        {
            canvas.worldCamera = _cameraEquipmentUI; // Установим для холста камеру которая будет его рендерить
        }      
    }
}

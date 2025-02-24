using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Видимость этажа // Должна висеть на всех объектах которые хотим скрыть
/// </summary>
/// <remarks>
/// Если изменить материал который поддерживает альфа канал то можно изменять прозрачность объектов
/// </remarks>
public class FloorVisibility : MonoBehaviour
{
    [Header("Динамическое изминение этажа. \nДля ЮНИТА поставить галочку")]
    [SerializeField] private bool _dynamicFloorPosition = false; // Динамическая позиция этажа (для объектов которые могут перемещаться и менять этаж нахождения)
    [Header("Renderer который надо игнорировать \nпри включении и отключении визуализации объектов.")]
    [SerializeField] private List<Renderer> _ignoreRendererList; // Список Renderer который надо игнорировать при включении и отключении визуализации объектов // Это относиться к зеленому кругу на юните у которого своя логика отключения и включения

    private static LevelGrid _levelGrid;
    private static CameraFollow _cameraFollow;

    private Renderer[] _rendererArray; // Массив Renderer дочерних объектов
    private Canvas _canvas;
    private int _floor; // Этаж    
    private bool _cameraZoomActionStarted = false; // Началось действие увеличения камеры
    private float _cameraHeight;

    private static bool _isInit = false;

    private void Awake()
    {
        _rendererArray = GetComponents<Renderer>();
    }

    public static void Init(LevelGrid levelGrid, CameraFollow cameraFollow)
    {
        _levelGrid = levelGrid;
        _cameraFollow = cameraFollow;

        _isInit = true;
    }

    private void Start()
    {
      //  SetupOnStart();
    }

    private void SetupOnStart()
    {
        _floor = _levelGrid.GetFloor(transform.position); // Получим этаж для нашей позиции(объекта на котором висит скрипт) 

        if (_floor == 0 && !_dynamicFloorPosition) // Если этаж на котором находяться объекты к которым прикриплен скрипт нулевой  и  этажность динамически НЕ изменяется (это касается юнитов) то...
        {
            Destroy(this); // Уничтожим этот скрипт что бы он просто так не занимал Update
        }
        _cameraFollow.OnCameraZoomStarted += CameraFollow_OnCameraZoomStarted;
        _cameraFollow.OnCameraZoomCompleted += CameraFollow_OnCameraZoomCompleted;
    }

    private void OnDestroy()
    {
        _cameraFollow.OnCameraZoomStarted -= CameraFollow_OnCameraZoomStarted;
        _cameraFollow.OnCameraZoomCompleted -= CameraFollow_OnCameraZoomCompleted;
    }
    private void CameraFollow_OnCameraZoomStarted(object sender, float e) { _cameraZoomActionStarted = true; _cameraHeight = e; }
    private void CameraFollow_OnCameraZoomCompleted(object sender, System.EventArgs e) { _cameraZoomActionStarted = false; }

    private void Update()
    {
        if (!_isInit) return;
        if (!_cameraZoomActionStarted) return; // Если камера не начала Zoom то выходим из апдейта (отключение объектов происходит при изминенеии ZOOM)

        float floorHeightOffset = 3f; // смещение высоты этажа // Для удобства отображения камеры
        bool showObject = _cameraHeight > _levelGrid.GetFloorHeght() * _floor + floorHeightOffset; // Показываемый объект при условии ( если Высота камеры больше Высоты этажа * на номер этажа + смещение)

        if (showObject || _floor == 0) // Если можно показать объект или этаж нулевой (что бы если высота камера окажеться меньше cameraHeight, униты на нулевом этаже не отключались)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show() // Показать
    {
        foreach (Renderer renderer in _rendererArray) // Переберем массив
        {
            if (_ignoreRendererList.Contains(renderer)) continue; // Если объект в списке исключения то пропустим его
            renderer.enabled = true;
        }
        if (_canvas != null)
        {
            _canvas.enabled = true;
        }
    }

    private void Hide() // Скрыть
    {
        foreach (Renderer renderer in _rendererArray)
        {
            if (_ignoreRendererList.Contains(renderer)) continue; // Если объект в списке исключения то пропустим его
            renderer.enabled = false;
        }
        if (_canvas != null)
        {
            _canvas.enabled = false;
        }
    }

    private void MoveAction_OnChangedFloorsStarted(object sender, MoveAction.OnChangeFloorsStartedEventArgs e)
    {
        _floor = e.targetGridPosition.floor; // Изменим этаж у нашего Юнита
    }

}

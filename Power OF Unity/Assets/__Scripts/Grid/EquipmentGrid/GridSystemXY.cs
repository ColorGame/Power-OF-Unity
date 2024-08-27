using System;
using UnityEngine;


/// <summary>
/// Сеточная система // Стандартный класс C# // Будем использовать конструктор для создания нашей сетки поэтому он не наследует MonoBehaviour/
/// </summary>
/// <remarks>
/// <TGridObject> - Generic, для того чтобы GridSystemXY могла работать не только с GridSystemXY но и с др. передоваемыми ей типами Объектов Сетки
/// </remarks>
public class GridSystemXY<TGridObject> 
{   

    protected int _width;     // Ширина
    protected int _height;    // Высота
    protected float _canvasScaleFactor;// Масштаб канваса на котором строиться сетка (ВЛИЯЕТ НА РАЗМЕР ЯЧЕЙКИ)
    protected float _cellSizeWithScaleFactor;// Размер ячейки с учетом масштаба канваса
    protected Transform _anchorGridTransform; // Якорь сетки
    protected TGridObject[,] _gridObjectArray; // Двумерный массив объектов сетки
    protected Vector3 _offsetСenterCell;// Сделаем смещение что бы центр ячейки не совподал  с (0.0) transform.position родителя 
    protected EquipmentSlot _slot;

    public GridSystemXY(float canvasScaleFactor, EquipmentGridParameters gridParameters, Func<GridSystemXY<TGridObject>, Vector2Int, TGridObject> createGridObject)  // Конструктор // Func - это встроенный ДЕЛЕГАТ (третий параметр в аргументе это тип<TGridObject> который возвращает наш делегат и назавем его createGridObject)
    {
        _width = gridParameters.width;
        _height = gridParameters.height;
        _canvasScaleFactor= canvasScaleFactor;
        _cellSizeWithScaleFactor = gridParameters.cellSize* canvasScaleFactor;
        _anchorGridTransform = gridParameters.anchorGridTransform;
        _slot = gridParameters.slot;
        _offsetСenterCell = new Vector3(0.5f, 0.5f, 0) * _cellSizeWithScaleFactor; // Расчитаем смещение для нашей сетки , чтобы начало сетки (0,0) было в центре нулевой ячейки

        _gridObjectArray = new TGridObject[_width, _height]; // создаем массив сетки определенного размером width на height
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Vector2Int gridPosition = new Vector2Int(x, y);
                _gridObjectArray[x, y] = createGridObject(this, gridPosition); // Вызовим делегат createGridObject и в аргумент передадим нашу GridSystemXY и позиции сетки. Сохраняем его в каждой ячейким сетки в двумерном массив где x,y это будут индексы массива.

                //Debug.DrawLine(GetWorldPositionCenterСornerCell(gridPosition), GetWorldPositionCenterСornerCell(gridPosition) + Vector3.right* .2f, Color.white, 1000); // для теста нарисуем маленькие линии в центре каждой ячейки сетки
            }
        }       
    }

    public EquipmentSlot GetGridSlot() { return _slot; } // получит слот сетки

    public virtual Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        Vector3 localPosition = worldPosition - _anchorGridTransform.position - _offsetСenterCell; // переведите обратно в 0 (удалим смещение якоря относительно начала координат и смещение центра нулевой ячекйки) 
        return new Vector2Int
            (
            Mathf.RoundToInt(localPosition.x / _cellSizeWithScaleFactor),  // Применяем Mathf.RoundToInt для преоброзования float в int
            Mathf.RoundToInt(localPosition.y / _cellSizeWithScaleFactor)
            );
    }
    /// <summary>
    /// Получим мировые координаты центра Сетки
    /// </summary>
    public virtual Vector3 GetWorldPositionGridCenter() 
    {        
        // К мировым координатам якоря добавим добавим смещение к центру сеточной системы
        float x = _anchorGridTransform.position.x+ _cellSizeWithScaleFactor * _width / 2; // Размер ячейки умножим на количество ячеек, которое занимает наш объект по Х и делим пополам
        float y = _anchorGridTransform.position.y + _cellSizeWithScaleFactor * _height / 2;

        return new Vector3(x, y, 0);
    }
    /// <summary>
    /// Получим мировые координаты центра ячейки
    /// </summary>
    public virtual Vector3 GetWorldPositionCenterСornerCell(Vector2Int gridPosition) 
    {
        return new Vector3(gridPosition.x, gridPosition.y, 0) * _cellSizeWithScaleFactor + _anchorGridTransform.position + _offsetСenterCell;   // Получим координаты нашей ячеки с учетом ее масштаба, добавим смещение самой сетки _anchorGridTransform и смещения нулевой ячейки
                                                                                                                    // мы хотим что бы центр ячейки был смещен относительного нашего _anchorGridTransform  и левый угол сетки совподал с ***Grid.transform.position                                                                                                              
    }
    /// <summary>
    /// Получим мировые координаты нижнего левого угола ячейки
    /// </summary>
    public virtual Vector3 GetWorldPositionLowerLeftСornerCell(Vector2Int gridPosition) 
    {
        return new Vector3(gridPosition.x, gridPosition.y, 0) * _cellSizeWithScaleFactor + _anchorGridTransform.position;   // Получим координаты нашей ячеки с учетом ее масштаба, добавим смещение самой сетки _anchorGridTransform                                                                                        
    }


    public void CreateDebugObject(Transform debugPrefab) // Создать объект отладки ( public что бы вызвать из класса Testing и создать отладку сетки)   // Тип Transform и GameObject взаимозаменяемы т.к. у любого GameObject есть Transform и у каждого Transform есть прикрипленный GameObject
                                                         // В основном для работы нам нужен Transform игрового объекта. Если в аргументе указать тип GameObject, тогда в методе, если бы мы хотели после создани GameObject изменить его масштаб, нам придется делать дополнительный шаг "debugGameObject.Transform.LocalScale..."
                                                         // Поэтому для краткости кода в аргументе указываем тип Transform.
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Vector2Int gridPosition = new Vector2Int(x, y); // Позиция сетке

                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPositionCenterСornerCell(gridPosition), Quaternion.identity);  // Созданим экземпляр отладочного префаба(debugPrefab) в каждой ячейки сетки // Т.к. нет расширения MonoBehaviour мы не можем напрямую использовать Instantiate только через GameObject.Instantiate
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>(); // У созданного объкта возьмем компонент GridDebugObject
                gridDebugObject.SetGridObject(GetGridObject(gridPosition)); // Вызываем медот SetGridObject() и передаем туда объекты сетки находящийся в позиции _gridPositioAnchor // GetGridObject(_gridPositioAnchor) as GridObjectUnitXZ - временно определим <TGridObject> как GridObjectUnitXZ
            }
        }
    }

    public TGridObject GetGridObject(Vector2Int gridPosition) // Вернет объекты которые находятся в данной позиции сетки .Сделаем публичной т.к. будем вдальнейшем вызывать из вне.
    {
        return _gridObjectArray[gridPosition.x, gridPosition.y]; // x,y это индексы массива по которым можем вернуть данные массива
    }

    public bool IsValidGridPosition(Vector2Int gridPosition) // Является ли Допустимой Сеточной Позицией  // Проверяем что переданные нам значения больше 0 и меньше ширины и высоты нашей сетки
    {
        return gridPosition.x >= 0 &&
                gridPosition.y >= 0 &&
                gridPosition.x < _width &&
                gridPosition.y < _height;
    }

    public Vector3 GetRotationAnchorGrid()
    {
        return _anchorGridTransform.eulerAngles;
    }

    public Transform GetAnchorGrid()
    {
        return _anchorGridTransform;
    }

    public int GetWidth()
    {
        return _width;
    }

    public int GetHeight()
    {
        return _height;
    }
    /// <summary>
    /// Размер ячейки с учетом масштаба CANVAS
    /// </summary>
    public float GetCellSizeWithScaleFactor()
    {
        return _cellSizeWithScaleFactor;
    }

}

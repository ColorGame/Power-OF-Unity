using Unity.Entities;
using Unity.Mathematics;
/// <summary>
/// Элемент буффера - сеточная позиция пути
/// </summary>
[InternalBufferCapacity(20)]//Количество элементов в буфере
public struct PathPositionBufferElement: IBufferElementData
{
    public int3 gridPosition;

    //публичный статический неявный оператор который принимает сеточную позицию и
    //возвращает новый элемент буфера, установив значение свойства = значению которые мы передаем
    public static implicit operator PathPositionBufferElement(int3 gridPosition)
    {
        return new PathPositionBufferElement { gridPosition = gridPosition };
    }
}

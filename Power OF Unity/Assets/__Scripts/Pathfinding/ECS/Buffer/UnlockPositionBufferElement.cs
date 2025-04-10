using Unity.Entities;
using Unity.Mathematics;
/// <summary>
/// Элемент буффера - Позиция для разблокировки узла пути
/// </summary>
[InternalBufferCapacity(10)]//Количество элементов в буфере
public struct UnlockPositionBufferElement: IBufferElementData, IEnableableComponent
{
    public int3 gridPosition;

    //публичный статический неявный оператор который принимает сеточную позицию и
    //возвращает новый элемент буфера, установив значение свойства = значению которые мы передаем
    public static implicit operator UnlockPositionBufferElement(int3 gridPosition)
    {
        return new UnlockPositionBufferElement { gridPosition = gridPosition };
    }
}

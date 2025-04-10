using Unity.Entities;
using Unity.Mathematics;
/// <summary>
/// Элемент буффера - Позиция для блокировки узла пути
/// </summary>
[InternalBufferCapacity(10)]//Количество элементов в буфере
public struct LockPositionBufferElement : IBufferElementData, IEnableableComponent
{
    public float3 worldPosition;

    //публичный статический неявный оператор который принимает сеточную позицию и
    //возвращает новый элемент буфера, установив значение свойства = значению которые мы передаем
    public static implicit operator LockPositionBufferElement(float3 worldPosition)
    {
        return new LockPositionBufferElement { worldPosition = worldPosition };
    }
}
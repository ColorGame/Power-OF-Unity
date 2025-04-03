using Unity.Entities;
using Unity.Mathematics;


/// <summary>
/// Элемент буффера - мировая позиция пути
/// </summary>
[InternalBufferCapacity(20)]//Количество элементов в буфере
public struct PathWorldPositionBufferElement : IBufferElementData
{
    public float3 worldPosition;

    //публичный статический неявный оператор который принимает сеточную позицию и
    //возвращает новый элемент буфера, установив значение свойства = значению которые мы передаем
    public static implicit operator PathWorldPositionBufferElement(float3 worldPosition)
    {
        return new PathWorldPositionBufferElement { worldPosition = worldPosition };
    }
}
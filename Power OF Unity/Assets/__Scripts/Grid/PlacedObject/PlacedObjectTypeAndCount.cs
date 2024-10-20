using System;

/// <summary>
/// Размещаемый объект - ТИП и КОЛИЧЕСТВО. 
/// </summary>
[Serializable] // Чтобы созданная структура могла отображаться в инспекторе
public struct PlacedObjectTypeAndCount 
{ 
    public PlacedObjectTypeSO placedObjectTypeSO;
    public uint count;

    /// <summary>
    /// Размещаемый объект - ТИП и КОЛИЧЕСТВО. 
    /// </summary>
    public PlacedObjectTypeAndCount(PlacedObjectTypeSO placedObjectTypeSO, uint count)
    {
        this.placedObjectTypeSO = placedObjectTypeSO;
        this.count = count;
    }
}

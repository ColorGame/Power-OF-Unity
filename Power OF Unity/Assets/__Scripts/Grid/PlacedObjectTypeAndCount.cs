using System;

/// <summary>
/// Размещаемый объект - ТИП и КОЛИЧЕСТВО. 
/// </summary>
[Serializable] // Чтобы созданная структура могла отображаться в инспекторе
public struct PlacedObjectTypeAndCount 
{ 
    public PlacedObjectTypeSO placedObjectTypeSO;
    public int count;

    /// <summary>
    /// Размещаемый объект - ТИП и КОЛИЧЕСТВО. 
    /// </summary>
    public PlacedObjectTypeAndCount(PlacedObjectTypeSO placedObjectTypeSO, int count)
    {
        this.placedObjectTypeSO = placedObjectTypeSO;
        this.count = count;
    }
}

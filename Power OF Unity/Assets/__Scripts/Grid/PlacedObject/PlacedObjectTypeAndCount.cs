using System;

/// <summary>
/// ����������� ������ - ��� � ����������. 
/// </summary>
[Serializable] // ����� ��������� ��������� ����� ������������ � ����������
public struct PlacedObjectTypeAndCount 
{ 
    public PlacedObjectTypeSO placedObjectTypeSO;
    public uint count;

    /// <summary>
    /// ����������� ������ - ��� � ����������. 
    /// </summary>
    public PlacedObjectTypeAndCount(PlacedObjectTypeSO placedObjectTypeSO, uint count)
    {
        this.placedObjectTypeSO = placedObjectTypeSO;
        this.count = count;
    }
}

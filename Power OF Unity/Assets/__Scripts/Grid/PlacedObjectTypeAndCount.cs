using System;

/// <summary>
/// ����������� ������ - ��� � ����������. 
/// </summary>
[Serializable] // ����� ��������� ��������� ����� ������������ � ����������
public struct PlacedObjectTypeAndCount 
{ 
    public PlacedObjectTypeSO placedObjectTypeSO;
    public int count;

    /// <summary>
    /// ����������� ������ - ��� � ����������. 
    /// </summary>
    public PlacedObjectTypeAndCount(PlacedObjectTypeSO placedObjectTypeSO, int count)
    {
        this.placedObjectTypeSO = placedObjectTypeSO;
        this.count = count;
    }
}

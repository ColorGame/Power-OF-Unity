/// <summary>
/// ����������� ��������� � ����������� �������.
/// </summary>
public struct PlacedObjectTooltip
{
    public string name;         //���
    public string description;  //��������
    public string details;      //�����������
    public string sideEffects;  //�������� �������

    public PlacedObjectTooltip(string name, string description, string details, string sideEffects)
    {
        this.name = name;
        this.description = description;
        this.details = details;
        this.sideEffects = sideEffects;

    }
}
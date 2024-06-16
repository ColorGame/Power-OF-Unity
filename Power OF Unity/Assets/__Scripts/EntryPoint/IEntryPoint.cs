
/// <summary>
/// ��������� ����� �����, ��� ��������� ������������ 
/// ���� ����������� �������� ��������� � ���������� ��� - "EntryPoint"
/// </summary>
public interface IEntryPoint
{
    /// <summary>
    /// �������� ������� ����������� ����� Awake() � OnEnable()
    /// </summary>   
    void Process(DIContainer container);
}


/// <summary>
/// ��������� ����� �����, ��� ��������� ������������ 
/// ���� ����������� �������� ��������� � ���������� ��� - "EntryPoint"
/// </summary>
public interface IEntryPoint
{
    /// <summary>
    /// ��������� �����������. �������� ������� ����������� ����� Awake() � OnEnable()
    /// </summary>   
    void Inject(DIContainer container);  
}

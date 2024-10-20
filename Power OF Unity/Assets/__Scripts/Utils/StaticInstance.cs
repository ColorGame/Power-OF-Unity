using UnityEngine;

/// <summary>
/// ����������� ��������� ����� �� ��������, �� ������ ����������� ����� �����
/// ����������� �� �������������� ������� ���������. ��� ������ ��� ������ ���������
/// � ��������� ��� �� ������������� ������ ��� �������
/// </summary>
public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    protected virtual void Awake() => Instance = this as T;

    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
}

/// <summary>
/// ��� ����������� ����������� ��������� � ������� ��������. ��� ��������� ��� ��������� �����
/// ������, ������� �������� ��������� ����������
/// </summary>
public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("��� ������, ��� ���� Singleton" + transform + " - " + Instance);
            Destroy(gameObject);
        }
        base.Awake(); //�������� ������������ � ������� ������
    }
}

/// <summary>
/// �������, � ��� ���� ���������� ������ singleton. ��� ���������� ��� �������� scene.
/// �������� �������� ��� ��������� �������, ������� ��������� ���������� ������ � ����������� ���������. 
/// ��� ���������������, ��� ������ ��������������� ����� ����������� ������ � �.�
/// </summary>
public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}


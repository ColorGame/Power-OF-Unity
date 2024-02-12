using UnityEngine;

public static class Bootstrapper // ���������
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] // ����������� ���� ������� ��� ��������� ��������� ������ ��� ������� ����� ���������� � �������� ������ �����.
                                                                               // ����������� ��������� ��������� ��� RuntimeInitializeLoadType, ����� ��������������, ����� ����� ���������� � ������������������ �������.
                                                                               // BeforeSceneLoad. ����� ����������� ������� �����, �� ������� Awake() ��� �� ���� �������. ����� ��� ������� ��������� �����������.
   
    
    public static void Execute()
    {
        Object bootstrapEntryPoint = Object.Instantiate(Resources.Load("BootstrapEntryPoint"));// ������� ������ BootstrapEntryPoint
        Object.DontDestroyOnLoad(bootstrapEntryPoint); // � �� ���������� ��� �������� ������ �����
    }

    /*public static void Execute()
    {
        Object.DontDestroyOnLoad(Addressables.InstatiateAsync("BootstrapEntryPoint").WaitForCompletion()); // ������� ������ BootstrapEntryPoint � �� ���������� ��� �������� ������ �����
    }*/
}
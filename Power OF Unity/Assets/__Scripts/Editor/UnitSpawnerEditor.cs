using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitSpawner))] // �������� ����������������� ��������� ��� ���������� ���� �������.

public class UnitSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        UnitSpawner unitSpawner = (UnitSpawner)target;
        if (GUILayout.Button("��������� ������� �������������")) // �������� ������
        {
            unitSpawner.AutomaticCompleteTable();
        }
    }
}
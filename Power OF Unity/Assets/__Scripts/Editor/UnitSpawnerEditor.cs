using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitSpawnerOnLevel))] // �������� ����������������� ��������� ��� ���������� ���� �������.

public class UnitSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        UnitSpawnerOnLevel unitSpawner = (UnitSpawnerOnLevel)target;
        if (GUILayout.Button("��������� ������� �������������")) // �������� ������
        {
            unitSpawner.AutomaticCompleteTable();
        }
    }
}
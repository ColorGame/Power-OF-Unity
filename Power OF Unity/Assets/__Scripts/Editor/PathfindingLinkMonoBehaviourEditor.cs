using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathfindingLinkMonoBehaviour))] // �������� ����������������� ��������� ��� ���������� ���� �������.
                                                     // ����� �� �������� ������ � Unity, �� ��������� �� ����������� �� MonoBehaviour �, �������������, �������� �����������, ������� �� ������ ������������ � GameObject.
                                                     // ����� �� ���������� ��������� � GameObject, ��������� ���������� ��������� �� ���������, ������� �� ������ ������������ ��� ��������� � �������������� ������ ������������� ����������
                                                     // ���������������� �������� - ��� ��������� ������, ������� �������� ���� ����� �� ��������� ������ ���������� ���� ���������� ���������� ���������.
public class PathfindingLinkMonoBehaviourEditor : Editor
{


    private void OnSceneGUI() // ������� ����������� ������ ��� ����� �������� ����� ��������� ��������� ������������ ������� � ������ ��������� �����.
    {
        PathfindingLinkMonoBehaviour pathfindingLinkMonoBehaviour = (PathfindingLinkMonoBehaviour)target; // target as PathfindingLinkMonoBehaviour -������ ������ ��������� //target- ����������� ������.

        EditorGUI.BeginChangeCheck(); // ��������� ����� ���� ���� ��� �������� ��������� ������������ ���������� ������������.
        Vector3 newLinkPositionA = Handles.PositionHandle(pathfindingLinkMonoBehaviour.linkPositionA, Quaternion.identity); //(������ ���������) ���������������� �������� ���������� 3D GUI � ��������� � ������ �����. PositionHandle-�������� ���������� �������.
        Vector3 newLinkPositionB = Handles.PositionHandle(pathfindingLinkMonoBehaviour.linkPositionB, Quaternion.identity);
        if (EditorGUI.EndChangeCheck()) // bool ���������� true, ���� ��������� ������������ ���������� ���������� � ������� ������ EditorGUI.BeginChangeCheck, � ��������� ������ false.
        {
            Undo.RecordObject(pathfindingLinkMonoBehaviour, "�������� ��������� ������"); // ���������� ��� ���������, ��������� � ������ ����� ������� RecordObject. ��� �� ����� ������� ctr + y ����� ���� ����������
            pathfindingLinkMonoBehaviour.linkPositionA = newLinkPositionA;
            pathfindingLinkMonoBehaviour.linkPositionB = newLinkPositionB;
        }
    }
}


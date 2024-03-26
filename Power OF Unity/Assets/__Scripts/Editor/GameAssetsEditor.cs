using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameAssets))] // �������� ����������������� ��������� ��� ���������� ���� �������.


public class GameAssetsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GameAssets gameAssets = (GameAssets)target;
        if(GUILayout.Button("��������� ������ ����� ������� EnemyTypePrefabUnit")) // �������� ������
        {
            gameAssets.CompleteFirstPartTableEnemyPrefab();
        }
    }
}

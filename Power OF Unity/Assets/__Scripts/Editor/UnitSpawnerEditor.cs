using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitSpawnerOnLevel))] // создание пользовательского редактора для созданного вами скрипта.

public class UnitSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        UnitSpawnerOnLevel unitSpawner = (UnitSpawnerOnLevel)target;
        if (GUILayout.Button("Заполнить таблицы Автоматически")) // Отрисуем кнопку
        {
            unitSpawner.AutomaticCompleteTable();
        }
    }
}
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitSpawner))] // создание пользовательского редактора для созданного вами скрипта.

public class UnitSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        UnitSpawner unitSpawner = (UnitSpawner)target;
        if (GUILayout.Button("Заполнить таблицы Автоматически")) // Отрисуем кнопку
        {
            unitSpawner.AutomaticCompleteTable();
        }
    }
}
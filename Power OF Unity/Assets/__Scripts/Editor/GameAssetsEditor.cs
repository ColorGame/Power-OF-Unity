using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameAssets))] // создание пользовательского редактора для созданного вами скрипта.


public class GameAssetsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GameAssets gameAssets = (GameAssets)target;
        if(GUILayout.Button("Заполнить первую часть таблицы EnemyTypePrefabUnit")) // Отрисуем кнопку
        {
            gameAssets.CompleteFirstPartTableEnemyPrefab();
        }
    }
}

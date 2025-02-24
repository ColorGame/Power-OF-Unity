using UnityEngine;

/// <summary>
/// ��������� ����� �� �������
/// </summary>
[CreateAssetMenu(fileName = "GridParamsSO", menuName = "ScriptableObjects/Date/GridParamsSO")]
public class GridParamsSO : ScriptableObject
{
    [Header("��������� ����� ��� ������� ������")]
    [SerializeField] private LevelGridParameters[] _levelGridParametersArray;

    public LevelGridParameters GetLevelGridParameters(SceneName sceneName)
    {
        switch (sceneName)
        {
            default:
            case SceneName.Level_1:
                return _levelGridParametersArray[0];
            case SceneName.Level_2:
                return _levelGridParametersArray[1];
        }

    }
}

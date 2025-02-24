using UnityEngine;

/// <summary>
/// Параметры сеток на уровнях
/// </summary>
[CreateAssetMenu(fileName = "GridParamsSO", menuName = "ScriptableObjects/Date/GridParamsSO")]
public class GridParamsSO : ScriptableObject
{
    [Header("Параметры сеток для каждого уровня")]
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

using UnityEngine;
/// <summary>
/// ядро визуала юнита
/// </summary>
public class UnitCoreView : MonoBehaviour, ISetupForSpawn
{
    private Unit _unit;

    public void SetupForSpawn(Unit unit)
    {
        _unit = unit;
    }

    public Unit GetUnit()
    {
        return _unit;
    }
}

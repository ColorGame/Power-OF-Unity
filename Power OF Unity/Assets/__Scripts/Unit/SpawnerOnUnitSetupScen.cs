using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class SpawnerOnUnitSetupScen : MonoBehaviour
{
    private UnitManager _unitManager;
    private Unit _selectedUnit;

    private void Awake()
    {
        ClearSpawner();
    }

    public void Init(UnitManager unitManager)
    {
        _unitManager = unitManager;

        Setup();
    }

    private void Setup()
    {      
        _unitManager.OnSelectedUnitChanged += UnitManager_OnSelectedUnitChanged;

        _selectedUnit = _unitManager.GetSelectedUnit();
        UpdateSpawn();
    }

    private void UnitManager_OnSelectedUnitChanged(object sender, Unit newSelectedUnit)
    {
        _selectedUnit = newSelectedUnit;
        UpdateSpawn();
    }

    private void UpdateSpawn()
    {
        ClearSpawner();

        if (_selectedUnit != null)
        {
            _selectedUnit.GetUnitEquipsViewFarm().CreateOnlyView(transform);
            _selectedUnit.SetUnitState(Unit.UnitState.Idle);
        }
    }

    private void ClearSpawner()
    {
        foreach (Transform attachTransform in transform) // Очистим точку спавна
        {
            Destroy(attachTransform.gameObject);
        }
    }
    private void OnDestroy()
    {
        _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;
    }
}

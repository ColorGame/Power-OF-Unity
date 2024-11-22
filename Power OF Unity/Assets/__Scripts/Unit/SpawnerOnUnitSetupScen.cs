using UnityEngine;

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
            _selectedUnit.GetUnitEquipsViewFarm().InstantiateOnlyUnitView(transform);            
        }
    }

    private void ClearSpawner()
    {
        foreach (Transform attachTransform in transform) // ������� ����� ������
        {
            Destroy(attachTransform.gameObject);
        }
    }
    private void OnDestroy()
    {
        _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;
    }
}

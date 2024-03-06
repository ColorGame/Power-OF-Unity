
using UnityEngine;

public class GameSceneEntryPoint : MonoBehaviour
{
    public CameraFollow _cameraFollow { get; private set; }
    public MouseOnGameGrid _mouseOnGameGrid { get; private set; }
    public UnitActionSystem _unitActionSystem { get; private set; }   

    private CoreEntryPoint  _bootstrapEntryPoint;


    private void Awake()
    {
        _cameraFollow = GetComponentInChildren<CameraFollow>(true);
        _mouseOnGameGrid = GetComponentInChildren<MouseOnGameGrid>(true);
        _unitActionSystem = GetComponentInChildren<UnitActionSystem>(true);

        _bootstrapEntryPoint = CoreEntryPoint.Instance;

        _cameraFollow.Initialize(_bootstrapEntryPoint.gameInput, _bootstrapEntryPoint.optionsMenuUI);
        _mouseOnGameGrid.Initialize(_bootstrapEntryPoint.gameInput);
        _unitActionSystem.Initialize(_bootstrapEntryPoint.gameInput);
        Debug.Log("GameSceneEntryPoint  Awake_CANCALED");
    }

    private void Start()
    {
       

    }
}

using UnityEngine;
using static UnitActionSystem;

public class IsLockedUI : MonoBehaviour // При взаимодействии с дверью, если ее нельзя открыть, появить надпись "Заперта"
{
    private UnitActionSystem _unitActionSystem;
    private void Awake()
    {
        _unitActionSystem = FindObjectOfType<UnitActionSystem>();
    }

    private void Start()
    {
        DoorInteract.OnAnyDoorIsLocked += DoorInteract_OnAnyDoorIsLocked; //Подпишемся на событие Любая Дверь Заперта(нельзя открыть вручную)

        _unitActionSystem.OnBusyChanged += UnitActionSystem_OnBusyChanged; // Подписываюсь на Event Занятость Изменена  и выполним UnitActionSystem_OnBusyChanged, эта фунуция получит от события булевый аргумент


        Hide(); // Скроем при старте
    }

    private void DoorInteract_OnAnyDoorIsLocked(object sender, System.EventArgs e)
    {
        Show(); 
    }
    
    private void UnitActionSystem_OnBusyChanged(object sender, OnUnitSystemEventArgs e)
    {
        if (!e.isBusy) // Когда игрок освобождается то скроем надпись
        {
            Hide();
        }
       
    }

    private void Show() // Показать
    {
        gameObject.SetActive(true);
    }

    private void Hide() // Скрыть
    {
        gameObject.SetActive(false);
    }
}

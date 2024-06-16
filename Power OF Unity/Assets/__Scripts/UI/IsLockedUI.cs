using UnityEngine;
using static UnitActionSystem;

public class IsLockedUI : MonoBehaviour // ��� �������������� � ������, ���� �� ������ �������, ������� ������� "�������"
{
    private UnitActionSystem _unitActionSystem;
    private void Awake()
    {
        _unitActionSystem = FindObjectOfType<UnitActionSystem>();
    }

    private void Start()
    {
        DoorInteract.OnAnyDoorIsLocked += DoorInteract_OnAnyDoorIsLocked; //���������� �� ������� ����� ����� �������(������ ������� �������)

        _unitActionSystem.OnBusyChanged += UnitActionSystem_OnBusyChanged; // ������������ �� Event ��������� ��������  � �������� UnitActionSystem_OnBusyChanged, ��� ������� ������� �� ������� ������� ��������


        Hide(); // ������ ��� ������
    }

    private void DoorInteract_OnAnyDoorIsLocked(object sender, System.EventArgs e)
    {
        Show(); 
    }
    
    private void UnitActionSystem_OnBusyChanged(object sender, OnUnitSystemEventArgs e)
    {
        if (!e.isBusy) // ����� ����� ������������� �� ������ �������
        {
            Hide();
        }
       
    }

    private void Show() // ��������
    {
        gameObject.SetActive(true);
    }

    private void Hide() // ������
    {
        gameObject.SetActive(false);
    }
}

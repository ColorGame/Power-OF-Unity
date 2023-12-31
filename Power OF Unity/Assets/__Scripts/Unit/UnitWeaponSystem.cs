using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitWeaponSystem : MonoBehaviour // ������� ���������� �����
{
    public static UnitWeaponSystem Instance { get; private set; }   //(������� SINGLETON) ��� �������� ������� ����� ���� ������� (SET-���������) ������ ���� �������, �� ����� ���� �������� GET ����� ������ �������
                                                                   // instance - ���������, � ��� ����� ���� ��������� UnitWeaponSystem ����� ���� ��� static. Instance ����� ��� ���� ����� ������ ������, ����� ����, ����� ����������� �� Event.

    private void Awake()
    {
        // ���� �� �������� � ���������� �� �������� �� �����
        if (Instance != null) // ������� �������� ��� ���� ������ ���������� � ��������� ����������
        {
            Debug.LogError("There's more than one UnitWeaponSystem!(��� ������, ��� ���� UnitWeaponSystem!) " + transform + " - " + Instance);
            Destroy(gameObject); // ��������� ���� ��������
            return; // �.�. � ��� ��� ���� ��������� UnitWeaponSystem ��������� ����������, ��� �� �� ��������� ������ ����
        }
        Instance = this;
    }

    private void Start()
    {
      
    }
    
}

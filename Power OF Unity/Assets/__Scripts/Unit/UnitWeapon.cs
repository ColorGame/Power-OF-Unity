using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitWeapon : MonoBehaviour // Оружие юнита
{
    private Unit _unit;

    private void Awake()
    {
        _unit = GetComponent<Unit>();
    }


}

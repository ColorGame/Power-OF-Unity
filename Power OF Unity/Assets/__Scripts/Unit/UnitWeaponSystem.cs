using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitWeaponSystem : MonoBehaviour // —истема вооружени€ юнита
{
    public static UnitWeaponSystem Instance { get; private set; }   //(ѕј““≈–Ќ SINGLETON) Ёто свойство которое может быть заданно (SET-присвоено) только этим классом, но может быть прочитан GET любым другим классом
                                                                   // instance - экземпл€р, ” нас будет один экземпл€р UnitWeaponSystem можно сдел его static. Instance нужен дл€ того чтобы другие методы, через него, могли подписатьс€ на Event.

    private void Awake()
    {
        // ≈сли ты акуратем в инспекторе то проверка не нужна
        if (Instance != null) // —делаем проверку что этот объект существует в еденичном екземпл€ре
        {
            Debug.LogError("There's more than one UnitWeaponSystem!(“ам больше, чем один UnitWeaponSystem!) " + transform + " - " + Instance);
            Destroy(gameObject); // ”ничтожим этот дубликат
            return; // т.к. у нас уже есть экземпл€р UnitWeaponSystem прекратим выполнение, что бы не выполнить строку ниже
        }
        Instance = this;
    }

    private void Start()
    {
      
    }
    
}

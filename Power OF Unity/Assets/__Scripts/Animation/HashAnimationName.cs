
using UnityEngine;

public class HashAnimationName
{
    public HashAnimationName() { }

    public int DoorOpen { get => Animator.StringToHash("DoorOpen"); }
    public int DoorClose { get => Animator.StringToHash("DoorClose"); }
    public int DoorBlocked { get => Animator.StringToHash("DoorBlocked"); }

    public int OptionsSubMenuOpen { get => Animator.StringToHash("OptionsSubMenuOpen"); }
    public int OptionsSubMenuClose { get => Animator.StringToHash("OptionsSubMenuClose"); }

    public int GameMenuOpen { get => Animator.StringToHash("GameMenuOpen"); }
    public int GameMenuClose { get => Animator.StringToHash("GameMenuClose"); }

    public int QuitGameSubMenuOpen { get => Animator.StringToHash("QuitGameSubMenuOpen"); }
    public int QuitGameSubMenuClose { get => Animator.StringToHash("QuitGameSubMenuClose"); }

    public int SaveGameSubMenuOpen { get => Animator.StringToHash("SaveGameSubMenuOpen"); }
    public int SaveGameSubMenuClose { get => Animator.StringToHash("SaveGameSubMenuClose"); }

    public int LoadGameSubMenuOpen { get => Animator.StringToHash("LoadGameSubMenuOpen"); }
    public int LoadGameSubMenuClose { get => Animator.StringToHash("LoadGameSubMenuClose"); }

    public int PlacedObjectBuyTabOpen { get => Animator.StringToHash("PlacedObjectBuyTabOpen"); }
    public int PlacedObjectSellTabOpen { get => Animator.StringToHash("PlacedObjectSellTabOpen"); }

    public int PlacedObjectBuyTabFirstOpen { get => Animator.StringToHash("PlacedObjectBuyTabFirstOpen"); }
    public int PlacedObjectSellTabFirstOpen { get => Animator.StringToHash("PlacedObjectSellTabFirstOpen"); }

    public int MaiMenuOpen { get => Animator.StringToHash("MaiMenuOpen"); }
    
    
    // Анимации юнита     
    public int Idle_Unarmed { get => Animator.StringToHash("Idle_Unarmed"); }
    public int Pistol_Idle_GunDown_TwoHanded { get => Animator.StringToHash("Pistol_Idle_GunDown_TwoHanded"); }
    public int Rifle_Idle_GunDown_TwoHanded { get => Animator.StringToHash("Rifle_Idle_GunDown_TwoHanded"); }
    public int Shield_Idle { get => Animator.StringToHash("Shield_Idle"); }
    public int Shield_Pistol_Idle { get => Animator.StringToHash("Shield_Pistol_Idle"); }
    public int Shield_Sword_Idle { get => Animator.StringToHash("Shield_Sword_Idle"); }
    public int Sword_Idle_OneHanded { get => Animator.StringToHash("Sword_Idle_OneHanded"); }
    public int Sword_Idle_TwoHanded { get => Animator.StringToHash("Sword_Idle_TwoHanded"); }
}

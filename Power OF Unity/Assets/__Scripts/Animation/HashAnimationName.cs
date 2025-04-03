
using UnityEngine;

public class HashAnimationName
{
    public HashAnimationName() { }

    public const int LAYER_WAlK = 1;

    public int DoorOpen { get => Animator.StringToHash("Base Layer.DoorOpen"); }
    public int DoorClose { get => Animator.StringToHash("Base Layer.DoorClose"); }
    public int DoorBlocked { get => Animator.StringToHash("Base Layer.DoorBlocked"); }

    public int OptionsSubMenuOpen { get => Animator.StringToHash("Base Layer.OptionsSubMenuOpen"); }
    public int OptionsSubMenuClose { get => Animator.StringToHash("Base Layer.OptionsSubMenuClose"); }

    public int GameMenuOpen { get => Animator.StringToHash("Base Layer.GameMenuOpen"); }
    public int GameMenuClose { get => Animator.StringToHash("Base Layer.GameMenuClose"); }

    public int QuitGameSubMenuOpen { get => Animator.StringToHash("Base Layer.QuitGameSubMenuOpen"); }
    public int QuitGameSubMenuClose { get => Animator.StringToHash("Base Layer.QuitGameSubMenuClose"); }

    public int SaveGameSubMenuOpen { get => Animator.StringToHash("Base Layer.SaveGameSubMenuOpen"); }
    public int SaveGameSubMenuClose { get => Animator.StringToHash("Base Layer.SaveGameSubMenuClose"); }

    public int LoadGameSubMenuOpen { get => Animator.StringToHash("Base Layer.LoadGameSubMenuOpen"); }
    public int LoadGameSubMenuClose { get => Animator.StringToHash("Base Layer.LoadGameSubMenuClose"); }

    public int PlacedObjectBuyTabOpen { get => Animator.StringToHash("Base Layer.PlacedObjectBuyTabOpen"); }
    public int PlacedObjectSellTabOpen { get => Animator.StringToHash("Base Layer.PlacedObjectSellTabOpen"); }

    public int PlacedObjectBuyTabFirstOpen { get => Animator.StringToHash("Base Layer.PlacedObjectBuyTabFirstOpen"); }
    public int PlacedObjectSellTabFirstOpen { get => Animator.StringToHash("Base Layer.PlacedObjectSellTabFirstOpen"); }

    public int MaiMenuOpen { get => Animator.StringToHash("Base Layer.MaiMenuOpen"); }
    
    
    // Анимации юнита     
    public int Unarmed { get => Animator.StringToHash("Base Layer.Unarmed_StateMachine.Idle_Unarmed1"); }
    /// <summary>
    /// Пистолет - это одноручное оружие.<br/>
    /// Но при АНИМАЦИИ юнит держит его ДВУМЯ РУКАМИ
    /// </summary>
    public int Pistol_Idle { get => Animator.StringToHash("Base Layer.Pistol_Idle"); } 
    public int Rifle_Idle { get => Animator.StringToHash("Base Layer.Rifle_Idle"); }
    public int Shield_Idle_OtherHandUnarmed { get => Animator.StringToHash("Base Layer.Shield_Idle_OtherHandUnarmed"); }
    public int Shield_Idle_OtherPistol { get => Animator.StringToHash("Base Layer.Shield_Idle_OtherPistol"); }
    /// <summary>
    /// Анимация ШИТА и ОДНОРУКОГО МЕЧА
    /// </summary>
    public int Shield_Idle_OtherSword { get => Animator.StringToHash("Base Layer.Shield_Idle_OtherSword"); }
    public int Sword_Idle_OneHanded { get => Animator.StringToHash("Base Layer.Sword_Idle_OneHanded"); }
    public int Sword_Idle_TwoHanded { get => Animator.StringToHash("Base Layer.Sword_Idle_TwoHanded"); }

    public int Walk { get => Animator.StringToHash("Walk.Walk"); }
   
}

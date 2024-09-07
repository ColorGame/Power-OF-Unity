
using UnityEngine;

public class HashAnimationName 
{
    public HashAnimationName() { }

    public int DoorOpen = Animator.StringToHash("DoorOpen");
    public int DoorClose = Animator.StringToHash("DoorClose");
    public int DoorBlocked = Animator.StringToHash("DoorBlocked");

    public int OptionsSubMenuOpen = Animator.StringToHash("OptionsSubMenuOpen");
    public int OptionsSubMenuClose = Animator.StringToHash("OptionsSubMenuClose");    
    
    public int GameMenuOpen = Animator.StringToHash("GameMenuOpen");
    public int GameMenuClose = Animator.StringToHash("GameMenuClose");    

    public int QuitGameSubMenuOpen = Animator.StringToHash("QuitGameSubMenuOpen");
    public int QuitGameSubMenuClose = Animator.StringToHash("QuitGameSubMenuClose");

    public int SaveGameSubMenuOpen = Animator.StringToHash("SaveGameSubMenuOpen");
    public int SaveGameSubMenuClose = Animator.StringToHash("SaveGameSubMenuClose");

    public int LoadGameSubMenuOpen = Animator.StringToHash("LoadGameSubMenuOpen");
    public int LoadGameSubMenuClose = Animator.StringToHash("LoadGameSubMenuClose");

    public int PlacedObjectSellTabOpen = Animator.StringToHash("PlacedObjectSellTabOpen");
    public int PlacedObjectSellTabClose = Animator.StringToHash("PlacedObjectSellTabClose");

    public int PlacedObjectBuyTabOpen = Animator.StringToHash("PlacedObjectBuyTabOpen");
    public int PlacedObjectBuyTabClose = Animator.StringToHash("PlacedObjectBuyTabClose");
}

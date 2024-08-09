
using UnityEngine;

public class HashAnimationName 
{
    public int DoorOpen = Animator.StringToHash("DoorOpen");
    public int DoorClose = Animator.StringToHash("DoorClose");
    public int DoorBlocked = Animator.StringToHash("DoorBlocked");

    public int OptionsSubMenuClose = Animator.StringToHash("OptionsSubMenuClose");    
    public int OptionsSubMenuOpen = Animator.StringToHash("OptionsSubMenuOpen");
    
    public int GameMenuClose = Animator.StringToHash("GameMenuClose");    
    public int GameMenuOpen = Animator.StringToHash("GameMenuOpen");

    public int QuitGameSubMenuClose = Animator.StringToHash("QuitGameSubMenuClose");
    public int QuitGameSubMenuOpen = Animator.StringToHash("QuitGameSubMenuOpen");

    public int SaveGameSubMenuClose = Animator.StringToHash("SaveGameSubMenuClose");
    public int SaveGameSubMenuOpen = Animator.StringToHash("SaveGameSubMenuOpen");

    public int LoadGameSubMenuClose = Animator.StringToHash("LoadGameSubMenuClose");
    public int LoadGameSubMenuOpen = Animator.StringToHash("LoadGameSubMenuOpen");
}

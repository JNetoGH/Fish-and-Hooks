using UnityEngine;


/// <summary>
/// The idea is to enable the game to be decoupled in a input level,
/// so the game can be ported easier to other devices. 
/// </summary>
public interface IHookStrategy
{
    public bool CanRun { get; set; }
    public Transform Hook { get; set; }
    public Transform BottomPivot { get; set; }
    public Transform TopPivot { get; set; }
   
    public void ResetHook();
}

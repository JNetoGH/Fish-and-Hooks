using Sirenix.OdinInspector;
using UnityEngine;


[CreateAssetMenu(fileName = "Fishing Set", menuName = "Fishing Set", order = 0)]
public class FishingSet : ScriptableObject
{
    
    [Title("Escape Bar")]
    [InfoBox("Rate the escape bar fills up per second, 0 is empty and 1 is full.")]
    public float escapeBarIncrement = 0.3f;
   
    [Title("Hook")]
    [InfoBox("Rate per second the hook decrements from the escape bar.")]
    public float hookEscapeDecrement = 0.1f;
    
    [Title("Fish")]
    [InfoBox("Multiplies a rand number from 0 to 1, the bigger, the longer it takes to get a new destination.")]
    public float fishTimerMultiplier = 1;
    [InfoBox("The value used in the fish's SmoothDump motion, the closer to 0, the quicker.")]
    public float fishSmoothMotion = 0.5f;
    
}
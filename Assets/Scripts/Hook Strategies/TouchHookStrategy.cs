using Sirenix.OdinInspector;
using UnityEngine;


public class TouchHookStrategy : MonoBehaviour, IHookStrategy
{
    
    public bool CanRun { get; set; }
    public Transform Hook { get; set; }
    public Transform BottomPivot { get; set; }
    public Transform TopPivot { get; set; }
    
    
    // The force added to the hook upwards.
    private const float HookUpForce = 0.003f;
    // The force added to the hook downwards each frame.
    private const float HookGravity = 0.002f;
    // How fast the hook can ascend.
    private const float HookMaxYVelocity = 0.02f;
    // How fast the hook can descend.
    private const float HookMinYVelocity = -0.04f;

    
    // Current speed which the hook area is moving in the Y-axis.
    [ReadOnly, SerializeField] private float _hookYVelocity;
    // A value from 1 to 0 used to interpolate in a lerp the hook between the top and bottom pivots.     
    [ReadOnly, SerializeField] private float _hookPosition;
    
    
    private void FixedUpdate()
    {
        if (!CanRun) 
            return;
        UpdateHook();
    }

    public void UpdateHook()
    {
        // FORCES ON THE HOOK TOUCH SCREEN
        // Applies upwards force to the hook if the required input is received.
        // Then, Applies Gravity to the hook velocity.
        if (Input.touchCount > 0)
        {     
            // The negative velocity used for fall can accumulate and take too long to be beaten.
            // So, if there is any accumulated negative velocity, it must be eliminated once an input is detected.
            if (_hookYVelocity < 0)
                _hookYVelocity = 0;
            
            _hookYVelocity += HookUpForce;
            
            // Removes accumulated velocity when the hook area has reached the top pivot, otherwise it gets stuck.
            if (Mathf.Approximately(_hookPosition, 1))
                _hookYVelocity = 0;
        }

        // IN CASE OF NO INPUTS
        // Removes accumulated velocity when the hook area has reached the bottom pivot, otherwise it gets stuck.
        else if (Mathf.Approximately(_hookPosition, 0))
        {
            _hookYVelocity = 0;
        }
        
        // applies gravity.
        _hookYVelocity -= HookGravity;
        
        // Keeps the velocity in the Min/Max range.
        _hookYVelocity = Mathf.Clamp(_hookYVelocity, HookMinYVelocity, HookMaxYVelocity);
        
        // Applies the hook's Y velocity to its internal position (0 to 1 value)
        // Makes sure to keep it in range by clamping it.
        // Then, lerps the hook's real position towards that previous (0 to 1) internal position, using the pivots.
        _hookPosition += _hookYVelocity;
        _hookPosition = Mathf.Clamp01(_hookPosition); 
        Hook.position = Vector3.Lerp(BottomPivot.position, TopPivot.position, _hookPosition);
    }
    
    public void ResetHook()
    {
        _hookPosition = 0;
        _hookYVelocity = 0;
    }
    
}
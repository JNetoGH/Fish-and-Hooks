using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class FishingBars : MonoBehaviour
{
    
    [Title("References")]
    [SerializeField] private Transform _topPivot;
    [SerializeField] private Transform _bottomPivot;
    [SerializeField] private Transform _fishIndicator;
    [SerializeField] private Transform _hook;
    [SerializeField] private Transform _hookUpperLimit;
    [SerializeField] private Transform _hookLowerLimit;
    [SerializeField] private Image _hookImage;
    
    
    [SerializeField] private float _hookPower = 5f;  
    [ReadOnly, SerializeField] private float _hookProgress;
    [SerializeField] private float _hookProgressDegradationPower = 0.01f;
   
    
    [Title("Hook Gameplay")]
    // The force added to the hook upwards.
    [SerializeField] private float _hookUpForce = 0.01f;
    // The force added to the hook downwards each frame.
    [SerializeField] private float _hookGravity = 0.005f;
    
    
    [Title("Hook Debugging")]
    // Current speed which the hook area is moving in the Y-axis.
    [ReadOnly, SerializeField] private float _hookYVelocity; 
    // A value from 1 to 0 used to interpolate in a lerp the hook between the top and bottom pivots.
    [ReadOnly, SerializeField, Range(0,1)] private float _hookPosition;
 
    
    [Title("Fish Gameplay")]
    // Multiplies a rand number from 0 to 1, the bigger, the longer it takes to get a new destination.
    [SerializeField] private float _fishTimerMultiplier = 1f; 
    // The value used in the fish's SmoothDump motion, the closer to 0, the quicker.
    [SerializeField] private float _fishSmoothMotion = 0.5f;
    
    
    [Title("Fish Debugging")]
    // Internal Velocity used by the smooth Damp.
    [ReadOnly, SerializeField] private float _fishYVelocity;
    // Timer responsible to set a new destination.
    [ReadOnly, SerializeField] private float _fishNewDestTimer;
    // Destination is a random value from 0 to 1, 0 being the bottom pivot, and 1, the top pivot.
    [ReadOnly, SerializeField, Range(0,1)] private float _fishDestination;
    // A value from 1 to 0 used to interpolate in a lerp the fish between the top and bottom pivots.
    [ReadOnly, SerializeField, Range(0,1)] private float _fishPosition;
    
    
    void Update()
    {
        UpdateFishTimer();
        UpdateFishPosition();
        UpdateHook();
        
        if (_fishIndicator.position.y >= _hookLowerLimit.position.y &&
            _fishIndicator.position.y <= _hookUpperLimit.position.y)
        {
            Debug.Log("The fish is in hook's Range!!!!");
            _hookImage.color = Color.green;
        }
        else
        {
            _hookImage.color = Color.yellow;
        }
    }
    
    private void UpdateFishTimer()
    {
        _fishNewDestTimer -= Time.deltaTime;
        
        if (_fishNewDestTimer <= 0)
        {
            // Gets a new random value for the fish's from 0 to 1,
            // and multiplies it by the timer multiplayer.
            _fishNewDestTimer = Random.value * _fishTimerMultiplier;
            
            // Assigns a new destination being a random number from 0 to 1.
            // Representing the distance between the top and bottom position.
            _fishDestination = Random.value;
        }
    }
    
    private void UpdateFishPosition()
    {
        // Moves the fish's position (0 to 1) value towards the destination (0 to 1 value).
        // Then, lerps the fish's real position towards that previous (0 to 1) internal position, using the pivots.
        _fishPosition = Mathf.SmoothDamp(_fishPosition, _fishDestination, ref _fishYVelocity, _fishSmoothMotion);
        _fishIndicator.position = Vector3.Lerp(_bottomPivot.position, _topPivot.position, _fishPosition);
    }
    
    private void UpdateHook()                                              
    {
        // FORCES ON THE HOOK
        // Applies upwards force to the hook if the required input is received.
        // Then, Applies Gravity to the hook velocity.
        if (Input.GetMouseButton(0))
        {     
            // The negative velocity used for fall can accumulate and take too long to be beaten.
            // So, if there is any accumulated negative velocity, it must be eliminated once an input is detected.
            if (_hookYVelocity < 0)
                _hookYVelocity = 0;
            
            _hookYVelocity += _hookUpForce * Time.deltaTime;
            
            // Removes accumulated velocity when the hook area has reached the top pivot, otherwise it gets stuck.
            if (Mathf.Approximately(_hookPosition, 1))
                _hookYVelocity = 0;
            
            Debug.Log("Hook Input");
        }
        else if (Mathf.Approximately(_hookPosition, 0))
        {
            // Removes accumulated velocity when the hook area has reached the bottom pivot, otherwise it gets stuck.
            _hookYVelocity = 0;
        }
        _hookYVelocity -= _hookGravity * Time.deltaTime;
        
        // Applies the hook's Y velocity to its internal position (0 to 1 value)
        // Makes sure to keep it in range by clamping it.
        // Then, lerps the hook's real position towards that previous (0 to 1) internal position, using the pivots.
        _hookPosition += _hookYVelocity;
        _hookPosition = Mathf.Clamp01(_hookPosition);
        _hook.position = Vector3.Lerp(_bottomPivot.position, _topPivot.position, _hookPosition);
    }
    
}

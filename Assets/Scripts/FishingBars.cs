using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FishingBars : MonoBehaviour
{
    
    /// <summary>
    /// Returns true when the fish is above the lower limit and under the upper limit.
    /// </summary>
    public bool IsFishInHooksRange { get; private set; }
    
    /// <summary>
    /// Use this property to block user inputs and the escape bar update.
    /// </summary>
    public bool CanRun { get; set; }
    
    /// <summary>
    /// Returns true when the escape bar's fill Y local scale reaches 1.
    /// </summary>
    public bool HasFishEscaped { get; private set; }
    
    
    [Title("References")]
    [SerializeField] private Transform _topPivot;
    [SerializeField] private Transform _bottomPivot;
    [SerializeField] private Transform _fishIndicator;
    [SerializeField] private Transform _hook;
    [SerializeField] private Transform _hookUpperLimit;
    [SerializeField] private Transform _hookLowerLimit;
    [SerializeField] private Image _hookImage;
    [SerializeField] private Transform _escapeBarFill;              
    
    
    [Title("Escape Bar Gameplay")]  
    // How per second the escape bar fills up, this bar uses it y scale to fill, 0 is empty and 1 is full.
    [SerializeField] public float _escapeBarIncrement = 0.3f;
    
    
    [Title("Hook Gameplay")]
    // The force added to the hook upwards.
    [SerializeField] private float _hookUpForce = 0.01f;
    // The force added to the hook downwards each frame.
    [SerializeField] private float _hookGravity = 0.005f;
    // How fast the hook can ascend.
    [SerializeField] private float _hookMaxYVelocity = 0.02f;
    // How fast the hook can descend.
    [SerializeField] private float _hookMinYVelocity = -0.04f;
    // How much per second the hook decrements from the escape bar
    [SerializeField] public float _hookEscapeDecrement = 0.1f;  
    
    
    [Title("Hook Debugging")]
    // Current speed which the hook area is moving in the Y-axis.
    [ReadOnly, SerializeField] private float _hookYVelocity; 
    // A value from 1 to 0 used to interpolate in a lerp the hook between the top and bottom pivots.
    [ReadOnly, SerializeField, Range(0,1)] private float _hookPosition;
 
    
    [Title("Fish Gameplay")]
    // Multiplies a rand number from 0 to 1, the bigger, the longer it takes to get a new destination.
    [SerializeField] public float _fishTimerMultiplier = 1f; 
    // The value used in the fish's SmoothDump motion, the closer to 0, the quicker.
    [SerializeField] public float _fishSmoothMotion = 0.5f;
    
    
    [Title("Fish Debugging")]
    // Internal Velocity used by the smooth Damp.
    [ReadOnly, SerializeField] private float _fishYVelocity;
    // Timer responsible to set a new destination.
    [ReadOnly, SerializeField] private float _fishNewDestTimer;
    // Destination is a random value from 0 to 1, 0 being the bottom pivot, and 1, the top pivot.
    [ReadOnly, SerializeField, Range(0,1)] private float _fishDestination;
    // A value from 1 to 0 used to interpolate in a lerp the fish between the top and bottom pivots.
    [ReadOnly, SerializeField, Range(0,1)] private float _fishPosition;

    private void Awake()
    {
        CanRun = false;
    }

    private void Start()
    {
        // Removing the guide images on the hook children while the game is running.s
        _hookUpperLimit.GetComponent<Image>().enabled = false;
        _hookLowerLimit.GetComponent<Image>().enabled = false;
        _topPivot.GetComponent<Image>().enabled = false;
        _bottomPivot.GetComponent<Image>().enabled = false;
    }

    void Update()
    {
        if (!CanRun)
            return;
        
        UpdateFishTimer();
        UpdateFishPosition();
        IsFishInHooksRange = (_fishIndicator.position.y >= _hookLowerLimit.position.y &&
                         _fishIndicator.position.y <= _hookUpperLimit.position.y);
        _hookImage.color = IsFishInHooksRange ? Color.green : Color.yellow;
        UpdateEscapeBar();
        if (_escapeBarFill.localScale.y >= 1)
            HasFishEscaped = true;
    }

    private void FixedUpdate()
    {
        if (!CanRun)
            return;
        
        UpdateHook();
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
        if (Input.touchCount > 0)
        {     
            // The negative velocity used for fall can accumulate and take too long to be beaten.
            // So, if there is any accumulated negative velocity, it must be eliminated once an input is detected.
            if (_hookYVelocity < 0)
                _hookYVelocity = 0;
            
            _hookYVelocity += _hookUpForce;
            
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
        _hookYVelocity -= _hookGravity;
        _hookYVelocity = Mathf.Clamp(_hookYVelocity, _hookMinYVelocity, _hookMaxYVelocity);
        
        // Applies the hook's Y velocity to its internal position (0 to 1 value)
        // Makes sure to keep it in range by clamping it.
        // Then, lerps the hook's real position towards that previous (0 to 1) internal position, using the pivots.
        _hookPosition += _hookYVelocity;
        _hookPosition = Mathf.Clamp01(_hookPosition);
        _hook.position = Vector3.Lerp(_bottomPivot.position, _topPivot.position, _hookPosition);
    }
    
    private void UpdateEscapeBar()
    {
        // Updates the escape bar's fill by using the decrement from the hook, and increment from the escape bar.
        // This bar uses it y scale to fill, 0 is empty and 1 is full (whole height).
        // Therefore a Clamp is required to keep it in grange
        Vector3 currentFillScale = _escapeBarFill.localScale;
        if (IsFishInHooksRange) 
            currentFillScale.y -= _hookEscapeDecrement * Time.deltaTime;
        else 
            currentFillScale.y += _escapeBarIncrement * Time.deltaTime;
        currentFillScale.y = Mathf.Clamp01(currentFillScale.y);
        _escapeBarFill.localScale = currentFillScale;
    }
    
    /// <summary>
    /// Resets the Fish and Hook internal positions back to 0, and the escape bar's scale.
    /// </summary>
    [Button]
    public void ResetTheBars()
    {
        _fishPosition = 0;
        _hookPosition = 0;
        
        Vector3 newScale = _escapeBarFill.localScale;
        newScale.y = 0;
        _escapeBarFill.localScale = newScale;
        
        HasFishEscaped = false;
    }
    
}

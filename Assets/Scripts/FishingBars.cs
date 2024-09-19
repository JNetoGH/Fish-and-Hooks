using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class FishingBars : MonoBehaviour
{
    
    /// <summary>                                                            
    /// Use this property to block user inputs and the escape bar update.    
    /// </summary>
    [field: SerializeField, ReadOnly]                                        
    public bool CanRun { get; set; }                                          
    
    /// <summary>
    /// Returns true when the fish is above the lower limit and under the upper limit.
    /// </summary>
    [field: SerializeField, ReadOnly]  
    public bool IsFishInHooksRange { get; private set; }
    
    /// <summary>
    /// Returns true when the escape bar's fill Y local scale reaches 1.
    /// </summary>
    [field: SerializeField, ReadOnly]  
    public bool HasFishEscaped { get; private set; }

    /// <summary>
    /// The current strategy set to handle the hook.
    /// </summary>
    /// <remarks>
    /// If none is assigned a default one will be.
    /// </remarks>
    public IHookStrategy HookStrategy
    {
        get => _hookStrategy;
        set
        {
            _hookStrategy = value;
            
            if (value == null) 
                return;

            // Removing other possible Hook strategies
            foreach (IHookStrategy strategy in GetComponents<IHookStrategy>())
                if (strategy != value)
                    Destroy(strategy as Component);
            
            // Setting the dependencies of this strategy.
            value.Hook = _hook;                                    
            value.BottomPivot = _bottomPivot;                      
            value.TopPivot = _topPivot;
        }
    }


    [Title("Escape Bar")]  
    // How per second the escape bar fills up, this bar uses it y scale to fill, 0 is empty and 1 is full.
    [SerializeField] public float _escapeBarIncrement = 0.3f;


    [Title("Hook")] 
    // How much per second the hook decrements from the escape bar
    [SerializeField] public float _hookEscapeDecrement = 0.1f; 
    // The color for when the fish is out of the hook.
    [SerializeField] private Color _fishOutColor = Color.yellow; 
    // The color for when the fish is in the hook.
    [SerializeField] private Color _fishInColor = Color.green; 
    // Backing field of its property.
    [ShowInInspector, ReadOnly] private IHookStrategy _hookStrategy;    
 
    
    [Title("Fish")]
    // Multiplies a rand number from 0 to 1, the bigger, the longer it takes to get a new destination.
    [SerializeField] public float _fishTimerMultiplier = 1f; 
    // The value used in the fish's SmoothDump motion, the closer to 0, the quicker.
    [SerializeField] public float _fishSmoothMotion = 0.5f;
    // Internal Velocity used by the smooth Damp.
    [ReadOnly, SerializeField] private float _fishYVelocity;
    // Timer responsible to set a new destination.
    [ReadOnly, SerializeField] private float _fishNewDestTimer;
    // Destination is a random value from 0 to 1, 0 being the bottom pivot, and 1, the top pivot.
    [ReadOnly, SerializeField, Range(0,1)] private float _fishDestination;
    // A value from 1 to 0 used to interpolate in a lerp the fish between the top and bottom pivots.
    [ReadOnly, SerializeField, Range(0,1)] private float _fishPosition;

    
    [Title("References")]                                    
    [SerializeField] private Transform _topPivot;             
    [SerializeField] private Transform _bottomPivot;          
    [SerializeField] private Transform _fishIndicator;        
    [SerializeField] private Transform _hook;                 
    [SerializeField] private Transform _hookUpperLimit;       
    [SerializeField] private Transform _hookLowerLimit;       
    [SerializeField] private Image _hookImage;                
    [SerializeField] private Transform _escapeBarFill;


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
        if (HookStrategy == null)                                             
            HookStrategy = gameObject.AddComponent<TouchHookStrategy>();      
        HookStrategy.CanRun = CanRun;
        
        if (!CanRun)
            return;
        
        UpdateFishTimer();
        UpdateFishPosition();
        IsFishInHooksRange = (_fishIndicator.position.y >= _hookLowerLimit.position.y &&
                         _fishIndicator.position.y <= _hookUpperLimit.position.y);
        _hookImage.color = IsFishInHooksRange ? _fishInColor : _fishOutColor;
        UpdateEscapeBar();
        if (_escapeBarFill.localScale.x >= 1)
            HasFishEscaped = true;
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
    
    private void UpdateEscapeBar()
    {
        // Updates the escape bar's fill by using the decrement from the hook, and increment from the escape bar.
        // This bar uses it y scale to fill, 0 is empty and 1 is full (whole height).
        // Therefore a Clamp is required to keep it in grange
        Vector3 currentFillScale = _escapeBarFill.localScale;
        if (IsFishInHooksRange) 
            currentFillScale.x -= _hookEscapeDecrement * Time.deltaTime;
        else 
            currentFillScale.x += _escapeBarIncrement * Time.deltaTime;
        currentFillScale.x = Mathf.Clamp01(currentFillScale.x);
        _escapeBarFill.localScale = currentFillScale;
    }
    
    /// <summary>
    /// Resets the Fish and Hook internal positions back to 0, and the escape bar's scale.
    /// </summary>
    [Button]
    public void ResetTheBars()
    {
        _fishPosition = 0;
        
        if (HookStrategy != null)
            HookStrategy.ResetHook();
        
        Vector3 newScale = _escapeBarFill.localScale;
        newScale.x = 0;
        _escapeBarFill.localScale = newScale;
        
        HasFishEscaped = false;
    }
    
}

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class FishingBars : MonoBehaviour
{
    
    [Title("References")]
    [SerializeField] private Transform _topPivot;
    [SerializeField] private Transform _bottomPivot;
    [SerializeField] private Transform _fishIndicator;
    [SerializeField] private Transform _hook;
    
    [Title("Hook Gameplay")]
    [SerializeField] private float _hookLength = 0.1f;
    [SerializeField] private float _hookPower = 5f;  // The power added to the hook for each button press
    [ReadOnly, SerializeField] private float _hookPosition;
    [ReadOnly, SerializeField] private float _hookProgress;
    [ReadOnly, SerializeField] private float _hookPullYVelocity; // Current speed the hook area is moving in the Y-axis.
    [SerializeField] private float _hookPullPower = 0.01f;
    [SerializeField] private float _hookGravity = 0.005f;
    [SerializeField] private float _hookProgressDegradationPower = 0.01f;
    
    [Title("Fish Gameplay")]
    [SerializeField, Tooltip(TipTimerMult)] private float _fishTimerMultiplayer = 2f; 
    [SerializeField, Tooltip(TipSmoothMot)] private float _fishSmoothMotion = 0.5f;
    [ReadOnly, SerializeField] private float _fishVelocity;    
    [ReadOnly, SerializeField] private float _fishNewDestTimer;
    [ReadOnly, SerializeField, Range(0, 1), Tooltip(TipDest)] private float _fishDestination;
    [ReadOnly, SerializeField, Range(0, 1), Tooltip(TipPosi)] private float _fishPosition;
    private const string TipTimerMult = "Multiplied by a random number (0 to 1) to set the new destination timer.";
    private const string TipSmoothMot = "The value used in the fish's SmoothDump motion, the closer to 0, the quicker.";  
    private const string TipDest = "Random value: (0 <-> 1) = (bottom pivot <-> top pivot)";      
    private const string TipPosi = "Value from 1 to 0 used to lerp the fish between the top and bottom pivots.";  
    
    void Update()
    {
        UpdateFishTimer();
        UpdateFishPosition();
    }
    
    private void UpdateFishTimer()
    {
        _fishNewDestTimer -= Time.deltaTime;
        
        if (_fishNewDestTimer <= 0)
        {
            // Gets a new random value for the fish's from 0 to 1,
            // and multiplies it by the timer multiplayer.
            _fishNewDestTimer = Random.value * _fishTimerMultiplayer;
            
            // Assigns a new destination being a random number from 0 to 1.
            // Representing the distance between the top and bottom position.
            _fishDestination = Random.value;
        }
    }
    
    private void UpdateFishPosition()
    {
        _fishPosition = Mathf.SmoothDamp(_fishPosition, _fishDestination, ref _fishVelocity, _fishSmoothMotion);
        _fishIndicator.position = Vector3.Lerp(_bottomPivot.position, _topPivot.position, _fishPosition);
    }

    private void UpdateHook()                                              
    {
        if (Input.GetMouseButtonDown(0))
        {
            _hookPullYVelocity += _hookPullPower * Time.deltaTime;
            
        }
    }
    
}

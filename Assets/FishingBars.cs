using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class FishingBars : MonoBehaviour
{
    
    [TitleGroup("Gameplay Parameters")]
    [SerializeField] private float _timerMultiplayer = 3f;
    [SerializeField] private float _smoothMotion = 1f;
    
    // Value from 1 to 0 used to lerp from between the top and bottom pivots.
    [ReadOnly, SerializeField] private float _fishPositionInterpolation;
    [ReadOnly, SerializeField] private float _fishDestination;
    
    // Sets when the destination position will change,
    // to simulate the fish moving under water.
    [ReadOnly, SerializeField] private float _fishDestinationChangeTimer;
    [ReadOnly, SerializeField] private float _fishVelocity;
    
    [TitleGroup("References")]
    [SerializeField] private Transform _topPivot;
    [SerializeField] private Transform _bottomPivot;
    [SerializeField] private Transform _fishIndicator;
    
    void Update()
    {
        UpdateFishTimer();
        UpdateFishPosition();
    }
    
    private void UpdateFishTimer()
    {
        _fishDestinationChangeTimer -= Time.deltaTime;
        
        if (_fishDestinationChangeTimer <= 0)
        {
            // Gets a new random value for the fish's from 0 to 1,
            // and multiplies it by the timer multiplayer.
            _fishDestinationChangeTimer = Random.value * _timerMultiplayer;
            
            // Assigns a new destination being a random number from 0 to 1.
            // Representing the distance between the top and bottom position.
            _fishDestination = Random.value;
        }
    }
    
    private void UpdateFishPosition()
    {
        _fishPositionInterpolation = Mathf.SmoothDamp(_fishPositionInterpolation, _fishDestination, ref _fishVelocity, _smoothMotion);
        _fishIndicator.position = Vector3.Lerp(_topPivot.position, _bottomPivot.position, _fishPositionInterpolation);
    }

}

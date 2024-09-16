using System;
using UnityEngine;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;


/// <summary>
/// Controls the amount of shaking, scaling with the escape bar's fill.
/// </summary>
public class CameraShakeController : MonoBehaviour
{
    
    [Title("Parameters")]  
    [SerializeField] float _minShake = 0.5f;
    [SerializeField] float _maxShake = 3f;
    [SerializeField, Range(0, 1)] float _startScalingAt = 0.3f;                           
   
    
    [Title("Debugging")]
    [SerializeField, ReadOnly] private FishingBars _fishingBars; 
    [SerializeField, ReadOnly] private Transform _escapeBarFill; 
    [SerializeField, ReadOnly] private CinemachineVirtualCamera _vCam;
    [SerializeField, ReadOnly] private CinemachineBasicMultiChannelPerlin _perlin;
    [SerializeField, ReadOnly] private float _escapeBarFillScaleY;    
    [SerializeField, ReadOnly] private float _shakeIntensity;     
    
    private void Awake()
    {
        _vCam = GetComponent<CinemachineVirtualCamera>();
        _perlin = _vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    
    private void Start()
    {
        _escapeBarFill = GameObject.FindGameObjectWithTag("EscapeBarFill").transform;
        _fishingBars = FindObjectOfType<FishingBars>();
    }

    private void Update()
    {
        // Applies the minimum shake while not running the game.
        // Then, returns.
        if (!_fishingBars.CanRun)
        {
            _perlin.m_FrequencyGain =_minShake;
            return;
        }

        if (_escapeBarFill is null)
            return;

        // Gets the escape bar's fill Y scale
        _escapeBarFillScaleY = _escapeBarFill.localScale.y;

        // Only apply shaking if the fill scale Y is greater than 0.5
        if (_escapeBarFillScaleY > _startScalingAt)
        {
            // Maps the Y scale to the shake intensity range, starting from 0.5
            float normalizedScaleY = (_escapeBarFillScaleY - _startScalingAt) * 2f; // Normalizes from 0.5 to 1, remapping to 0 to 1
            _shakeIntensity = Mathf.Lerp(_minShake, _maxShake, normalizedScaleY);
        }
        else
        {
            // No shaking if the fill is below or equal to half
            _shakeIntensity = _minShake;
        }

        // Applies the shake intensity to the Perlin noise component.
        _perlin.m_FrequencyGain = _shakeIntensity;
    }
    
}

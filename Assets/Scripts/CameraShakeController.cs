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
        _escapeBarFill = GameObject.FindWithTag("EscapeBarFill").transform;
        _fishingBars = FindObjectOfType<FishingBars>();
    }

    private void Update()
    {
        // Applies the minimum shake while not running the game.
        // Then, returns.
        if (!_fishingBars.CanRun)
        {
            _perlin.m_FrequencyGain = _minShake;
            return;
        }
        // Checks if the bar's fill is null, if so, tries to find it.
        if (_escapeBarFill is null)
        {
            _escapeBarFill = GameObject.FindGameObjectWithTag("EscapeBarFill").transform;
            if (_escapeBarFill is null)
                return;
        }
        ScaleWithEscapeBar();
    }

    private void ScaleWithEscapeBar()
    {
        // Gets the escape bar's fill Y scale
        _escapeBarFillScaleY = _escapeBarFill.localScale.y;

        // Only apply shaking if the fill scale Y is greater than x
        if (_escapeBarFillScaleY > _startScalingAt)
        {
            // Maps the Y scale to the shake intensity range, starting from x.
            // Normalizes from x to 1, remapping to 0 to 1.
            float normalizedScaleY = (_escapeBarFillScaleY - _startScalingAt) * 2f; 
            _shakeIntensity = Mathf.Lerp(_minShake, _maxShake, normalizedScaleY);
        }
        else
        {
            // If the fill is below or equal to x, the applied value will be the minimum.
            _shakeIntensity = _minShake;
        }

        // Applies the shake intensity to the Perlin noise component.
        _perlin.m_FrequencyGain = _shakeIntensity;
    }
    
}

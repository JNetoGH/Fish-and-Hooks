using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;


public class FishingRodAnimationsScript : MonoBehaviour
{ 
    [Title("Parameters")]  
    [SerializeField] private float _minAniSpeed = 0.8f;
    [SerializeField] private float _maxAniSpeed = 1.8f;
    [SerializeField, Range(0, 1)] private float _startScalingAt = 0.4f;    
    
    [Title("Debugging")]  
    [SerializeField, ReadOnly] private float _escapeBarFillScaleY;    
    [SerializeField, ReadOnly] private float _animationSpeed; 
    
    [Title("References")]  
    [SerializeField, ReadOnly] private FishingBars _fishingBars;
    [SerializeField, ReadOnly] private Transform _escapeBarFill; 
    private Animator _animator;
    
    private static readonly int Fishing = Animator.StringToHash("Fishing");
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _fishingBars = FindObjectOfType<FishingBars>();
        _escapeBarFill = GameObject.FindWithTag("EscapeBarFill").transform;
    }

    private void Update()
    {
        if (_fishingBars is null)
            return;
        
        _animator.SetBool(Fishing, _fishingBars.CanRun);
        ScaleWithEscapeBar();
    }
    
    private void ScaleWithEscapeBar()
    {
        // Gets the escape bar's fill Y scale
        _escapeBarFillScaleY = _escapeBarFill.localScale.x;

        // Only apply a new speed if the fill scale Y is greater than x
        if (_escapeBarFillScaleY > _startScalingAt)
        {
            // Maps the Y scale to the shake intensity range, starting from x.
            // Normalizes from x to 1, remapping to 0 to 1.
            float normalizedScaleY = (_escapeBarFillScaleY - _startScalingAt) * 2f;
            _animationSpeed = Mathf.Lerp(_minAniSpeed, _maxAniSpeed, normalizedScaleY);
        }
        else
        {
            // If the fill is below or equal to x, the applied value will be the minimum.
            _animationSpeed = _minAniSpeed;
        }

        // Applies the scaled animation speed
        _animator.speed = _animationSpeed;
    }
}

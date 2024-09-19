using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;


public class FishingRodAnimationsScript : MonoBehaviour
{ 
    [Title("Parameters Ani Pivot")]  
    [SerializeField] private float _minAniSpeedPivot = 0.8f;
    [SerializeField] private float _maxAniSpeedPivot = 1.8f;
    [SerializeField, Range(0, 1)] private float _startScalingAt = 0.4f;    
   
    [Title("Parameters Ani Cable")]  
    [SerializeField] private float _minAniSpeedCable = 1f;
    [SerializeField] private float _maxAniSpeedCable = 1.5f;
    
    [Title("Debugging")]  
    [SerializeField, ReadOnly] private float _escapeBarFillScaleY;    
    [SerializeField, ReadOnly] private float _animationSpeedPivot;
    [SerializeField, ReadOnly] private float _animationSpeedCable;
    
    [Title("References")]  
    [SerializeField] private FishingBars _fishingBars;
    [SerializeField] private Transform _escapeBarFill; 
    [FormerlySerializedAs("_pivotanimator")] [FormerlySerializedAs("_animator")] [SerializeField] private Animator _pivotAnimator;
    [SerializeField] private Animator _cableAnimator;
    
    private static readonly int Fishing = Animator.StringToHash("Fishing");
    
    private void Update()
    {
        if (_fishingBars is null)
            return;
        
        _pivotAnimator.SetBool(Fishing, _fishingBars.CanRun);
        _cableAnimator.SetBool(Fishing, _fishingBars.CanRun);
        ScalePivotWithEscapeBar();
    }
    
    private void ScalePivotWithEscapeBar()
    {
        // Gets the escape bar's fill Y scale
        _escapeBarFillScaleY = _escapeBarFill.localScale.x;

        // Only apply a new speed if the fill scale Y is greater than x
        if (_escapeBarFillScaleY > _startScalingAt)
        {
            // Maps the Y scale to the shake intensity range, starting from x.
            // Normalizes from x to 1, remapping to 0 to 1.
            float normalizedScaleY = (_escapeBarFillScaleY - _startScalingAt) * 2f;
            _animationSpeedPivot = Mathf.Lerp(_minAniSpeedPivot, _maxAniSpeedPivot, normalizedScaleY);
            _animationSpeedCable = Mathf.Lerp(_minAniSpeedCable, _maxAniSpeedCable, normalizedScaleY);
        }
        else
        {
            // If the fill is below or equal to x, the applied value will be the minimum.
            _animationSpeedPivot = _minAniSpeedPivot;
            _animationSpeedCable = _minAniSpeedCable;
        }

        // Applies the scaled animation speed
        _pivotAnimator.speed = _animationSpeedPivot;
        _cableAnimator.speed = _animationSpeedCable;
    }
    
}

using UnityEngine;


public class FishingRodAnimationsScript : MonoBehaviour
{
    private static readonly int Fishing = Animator.StringToHash("Fishing");
    
    [SerializeField] private FishingBars _fishingBars;
    private Animator _animator;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_fishingBars is null)
            return;
        
        _animator.SetBool(Fishing, _fishingBars.CanRun);
    }
}

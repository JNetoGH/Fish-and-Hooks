using Sirenix.OdinInspector;
using UnityEngine;

public class FishCatchingController : MonoBehaviour
{

    [SerializeField] private Transform _jumpTowards;
    [SerializeField] private float _jumpForce = 30f;
    [SerializeField] private float _springForce = 10f; 
    [SerializeField] private float _startRailAfter = 2f; 
    
    private Rigidbody _rigidbody;
    private SpringJoint _springJoint;
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private FishMovementController _fishMovementController;
    
    void Awake()
    {
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
        _rigidbody = GetComponent<Rigidbody>();
        _springJoint = GetComponent<SpringJoint>();
        _fishMovementController = GetComponent<FishMovementController>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        StopStringJoint();
    }
    
    [Button]
    public void JumpTowardsTarget()
    {
        _rigidbody.constraints = RigidbodyConstraints.None;
        _fishMovementController.enabled = false;
        Vector3 jumpDirection = _jumpTowards.position - transform.position;
        _rigidbody.AddForce(jumpDirection.normalized * _jumpForce, ForceMode.Impulse);
        Invoke(nameof(StartStringJoint), _startRailAfter);
    }

    [Button]
    private void StartStringJoint()
    {
        _springJoint.spring = _springForce;
        _rigidbody.constraints = RigidbodyConstraints.None;
    }
    
    [Button]
    private void StopStringJoint()
    {
        _springJoint.spring = 0;
    }

    [Button]
    public void ResetFish()
    {
        StopStringJoint();
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        transform.position = _initialPosition;
        transform.rotation = _initialRotation;
        _fishMovementController.enabled = true;
    }

}
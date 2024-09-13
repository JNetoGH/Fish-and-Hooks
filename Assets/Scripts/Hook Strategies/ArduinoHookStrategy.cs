using Sirenix.OdinInspector;
using UnityEngine;


public class ArduinoHookStrategy: MonoBehaviour, IHookStrategy
{
    
    public bool CanRun { get; set; }
    public Transform Hook { get; set; }
    public Transform BottomPivot { get; set; }
    public Transform TopPivot { get; set; }
    
    
    // The force added to the hook upwards.
    // Make it smaller to takes more spins to reach max velocity.
    private const float HookUpDownForce = 0.003f;
    // How fast the hook can ascend or descend.
    private const float HookTopYVelocity = 0.02f;
    // How fast the hook slows down.
    private const float HookDrag = 0.0004f;

    
    // A value from 1 to 0 used to interpolate in a lerp the hook between the top and bottom pivots.     
    [ReadOnly, SerializeField] private float _hookPosition;
    // Current speed which the hook area is moving in the Y-axis.
    [ReadOnly, SerializeField] private float _hookYVelocity;
    // The last value registered from the arduino's encoder.
    [ReadOnly, SerializeField] private int _lastEncoderValue = 0;   
    // My HTTP client modulo for the Arduino ESP32.
    private JNetoArduinoHttpClient _jNetoArduinoHttpClient;
    
    
    private void Start()
    {
        _jNetoArduinoHttpClient = FindObjectOfType<JNetoArduinoHttpClient>();
    }

    private void FixedUpdate()
    {
        if (!CanRun)
            return;
        
        if (!_jNetoArduinoHttpClient.IsConnected) 
            return;
        
        float newEncoderValue = _jNetoArduinoHttpClient.EncoderValue;
        
        // FORCES ON THE HOOK SPECIAL CONTROLLER
        // Adds Up/Down  forces according to the input from the encoder.
        if (_lastEncoderValue < newEncoderValue) // Going up.
        {
            _hookYVelocity += HookUpDownForce;
        }
        if (_lastEncoderValue > newEncoderValue) // Going downs.
        {
            _hookYVelocity -= HookUpDownForce;
        }

        // Applies drag until it stops.
        if (_hookYVelocity > 0)
        {
            _hookYVelocity -= HookDrag;
            if (_hookYVelocity < 0)
                _hookYVelocity = 0;
        }
        else if (_hookYVelocity < 0)
        {
            _hookYVelocity += HookDrag;
            if (_hookYVelocity > 0)
                _hookYVelocity = 0;
        }
        
        // Keeps the velocity in the Min/Max range.
        _hookYVelocity = Mathf.Clamp(_hookYVelocity, -HookTopYVelocity, HookTopYVelocity);
        
        // Applies the hook's Y velocity to its internal position (0 to 1 value)
        // Makes sure to keep it in range by clamping it.
        // Then, lerps the hook's real position towards that previous (0 to 1) internal position, using the pivots.
        _hookPosition += _hookYVelocity;
        _hookPosition = Mathf.Clamp01(_hookPosition);
        // The hook's pivot is on the top, if left as it is, it will trespass the bar's bottom.
        Vector3 offset = new Vector3(0, 250, 0); 
        Hook.position = Vector3.Lerp(BottomPivot.position + offset, TopPivot.position, _hookPosition);
        
        // Updates the last Encoder value.
        _lastEncoderValue = _jNetoArduinoHttpClient.EncoderValue;
    }

    public void ResetHook()
    {
        _hookPosition = 0;
        _hookYVelocity = 0;
    }
    
}


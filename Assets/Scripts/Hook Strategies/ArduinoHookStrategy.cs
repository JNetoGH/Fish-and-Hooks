using Sirenix.OdinInspector;
using UnityEngine;

public class ArduinoHookStrategy : MonoBehaviour, IHookStrategy
{
    public bool CanRun { get; set; }
    public Transform Hook { get; set; }
    public Transform BottomPivot { get; set; }
    public Transform TopPivot { get; set; }

    // As constantes agora serão multiplicadas por Time.fixedDeltaTime para serem independentes de frame rate.
    private const float HookUpDownForcePerSecond = 0.2f;
    private const float HookTopYVelocityPerSecond = 1.5f;
    private const float HookDragPerSecond = 0.04f;

    [ReadOnly, SerializeField] private float _hookPosition;
    [ReadOnly, SerializeField] private float _hookYVelocity;
    [ReadOnly, SerializeField] private int _lastEncoderValue = 0;
    private JNetoArduinoHttpClient _jNetoArduinoHttpClient;

    private void Start()
    {
        _jNetoArduinoHttpClient = FindAnyObjectByType<JNetoArduinoHttpClient>();
    }

    private void FixedUpdate()
    {
        if (!CanRun)
            return;

        if (!_jNetoArduinoHttpClient.IsConnected)
            return;

        float newEncoderValue = _jNetoArduinoHttpClient.EncoderValue;

        float hookUpDownForce = HookUpDownForcePerSecond * Time.fixedDeltaTime;
        float hookDrag = HookDragPerSecond * Time.fixedDeltaTime;
        float hookTopYVelocity = HookTopYVelocityPerSecond * Time.fixedDeltaTime;

        // Controla a força aplicada no gancho.
        if (_lastEncoderValue < newEncoderValue) // Subindo.
        {
            _hookYVelocity += hookUpDownForce;
        }
        if (_lastEncoderValue > newEncoderValue) // Descendo.
        {
            _hookYVelocity -= hookUpDownForce;
        }

        // Aplica arrasto até o gancho parar.
        if (_hookYVelocity > 0)
        {
            _hookYVelocity -= hookDrag;
            if (_hookYVelocity < 0)
                _hookYVelocity = 0;
        }
        else if (_hookYVelocity < 0)
        {
            _hookYVelocity += hookDrag;
            if (_hookYVelocity > 0)
                _hookYVelocity = 0;
        }

        // Limita a velocidade dentro do intervalo definido.
        _hookYVelocity = Mathf.Clamp(_hookYVelocity, -hookTopYVelocity, hookTopYVelocity);

        // Aplica a velocidade Y à posição interna (valor de 0 a 1) e interpola a posição do gancho.
        _hookPosition += _hookYVelocity;
        _hookPosition = Mathf.Clamp01(_hookPosition);

        Vector3 offset = new Vector3(0, 250, 0);
        Hook.position = Vector3.Lerp(BottomPivot.position + offset, TopPivot.position, _hookPosition);

        // Atualiza o valor do encoder.
        _lastEncoderValue = _jNetoArduinoHttpClient.EncoderValue;
    }

    public void ResetHook()
    {
        _hookPosition = 0;
        _hookYVelocity = 0;
    }
}
using System;
using System.Collections;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;


public class ArduinoHttpClient : MonoBehaviour
{
    
    /// <summary>
    /// True when the app is connected to the fishing controller.
    /// </summary>
    /// <remarks>
    /// Internally sends a msg to the arduino to turn a LED on and off
    /// </remarks>
    public bool IsConnected
    {
        get => _isConnected;
        private set
        {
            if (_isConnected == value) 
                return;
            
            _isConnected = value;
            if (_isConnected) 
                UnityWebRequest.Get(_ledOnUrl).SendWebRequest();
            else 
                UnityWebRequest.Get(_ledOffUrl).SendWebRequest();
        }
    }

    /// <summary>
    /// The value captured by the arduino HTTP ser ver about the .
    /// </summary>
    [field: ReadOnly, SerializeField]
    public int EncoderValue { get; private set; }
    
    // IsConnectedProperty backing field.
    [ReadOnly, SerializeField] private bool _isConnected;
    // Arduino server´s URL for the encoder value.
    [SerializeField] private string _encoderValueUrl = "http://192.168.4.1/value";
    // Data refresh interval.
    [SerializeField] private float _updateInterval = 0f; // 1/x is x per second.
    // Arduino server´s URL for turning a LED on.
    [SerializeField] private string _ledOnUrl = "http://192.168.4.1/H";
    // Arduino server´s URL for turning a LED off.
    [SerializeField] private string _ledOffUrl = "http://192.168.4.1/L";
    // Represents the game logic itself.
    private FishingBars _fishingBars;
    
    private void Start()
    {
        _fishingBars = FindAnyObjectByType<FishingBars>();
        StartCoroutine(ReadEncoderValue());
    }

    private void Update()
    {
        // Once the game has connection from the HTTP server, the right hook strategy will be set.
        if (IsConnected && _fishingBars.HookStrategy is not ArduinoHookStrategy)
            _fishingBars.HookStrategy = _fishingBars.AddComponent<ArduinoHookStrategy>();
    }

    private IEnumerator ReadEncoderValue()
    {
        while (true)
        {
            // Sends a GET request to the Arduino server
            UnityWebRequest request = UnityWebRequest.Get(_encoderValueUrl);

            // Waits for the request response
            yield return request.SendWebRequest();

            // Checks if there was an error in the request.
            bool requestHasErrors =
                request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError;
            
            if (requestHasErrors)
                Debug.LogWarning("Could not connect to Arduino: " + request.error);
            else
                EncoderValue = Convert.ToInt32(request.downloadHandler.text);

            IsConnected = !requestHasErrors;
            
            // Waits for the update interval before making the next request.
            yield return new WaitForSeconds(_updateInterval);
        }
    }

    private void OnApplicationQuit()
    {
        IsConnected = false;
    }

    private void OnApplicationPause(bool pause)
    {
        IsConnected = pause;
    }
}
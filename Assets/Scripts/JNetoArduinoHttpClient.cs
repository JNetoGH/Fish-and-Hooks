using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Networking;


public class JNetoArduinoHttpClient : MonoBehaviour
{
 
    
    public bool IsConnected { get; private set; }
    
    // Arduino server URL (replace with the IP displayed by the Arduino).
    [SerializeField] private string _url = "http://192.168.4.1/value";
    // Data refresh interval.
    [SerializeField] private float _updateInterval = 0f; // 1/x is x per second.
    // the encoder value received from the Arduino.
    [ReadOnly, SerializeField] private int _encoderValue;
    
    private void Start()
    {
        StartCoroutine(ReadEncoderValue());
    }

    private IEnumerator ReadEncoderValue()
    {
        while (true)
        {
            // Sends a GET request to the Arduino server
            UnityWebRequest request = UnityWebRequest.Get(_url);

            // Waits for the request response
            yield return request.SendWebRequest();

            // Checks if there was an error in the request.
            bool requestHasErrors =
                request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError;
            
            if (requestHasErrors)
                Debug.LogWarning("Could not connect to Arduino: " + request.error);
            else
                _encoderValue = Convert.ToInt32(request.downloadHandler.text);
            
            IsConnected = !requestHasErrors;
            
            // Waits for the update interval before making the next request.
            yield return new WaitForSeconds(_updateInterval);
        }
    }
}
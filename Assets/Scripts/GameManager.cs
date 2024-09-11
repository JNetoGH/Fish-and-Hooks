using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public class GameManager : MonoBehaviour
{
    
    [Title("References")]
    [SerializeField] private GameObject _newGamePanel;
    [SerializeField] private GameObject _welcomeText; 
    [SerializeField] private GameObject _victoryText;
    [SerializeField] private GameObject _defeatText;
    
    [SerializeField] private FishingBars _fishingBars; 
    [SerializeField] private TextMeshProUGUI _countdownText; 
    
    [Title("Gameplay")]   
    [SerializeField] private float _countdownDuration = 15.5f; 
    
    [FormerlySerializedAs("_countDown")]
    [Title("Debugging")] 
    [ReadOnly, SerializeField] private float _countdown = 0;
    [ReadOnly, SerializeField] private bool _isRunning = false;
    
    [Button]
    public void RunNewGame()
    {
        _newGamePanel.SetActive(false);
        _welcomeText.SetActive(false);
        _victoryText.SetActive(false);   
        _defeatText.SetActive(false);   
        _countdown = _countdownDuration;
        _isRunning = true;
        _fishingBars.ResetTheBars();
        _fishingBars.CanRun = true;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!_isRunning) 
            return;
        
        _countdown -= Time.deltaTime;
        _countdownText.text = ((int)_countdown).ToString("00");
        
        if (_countdown <= 0)
        {
            // win 
            _fishingBars.CanRun = false;
            Debug.Log("You Win");
            _isRunning = false;
            _newGamePanel.SetActive(true);
            _victoryText.SetActive(true);      
        }
        else if (_fishingBars.HasFishEscaped)
        {
            // defeat
            _fishingBars.CanRun = false;
            Debug.Log("You Lose");  
            _isRunning = false;    
            _newGamePanel.SetActive(true);  
            _defeatText.SetActive(true);    
        }
    }
    
}

using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManagerRanked : GameManager
{
    
    [SerializeField] private TextMeshProUGUI _recordText;

    // Update is called once per frame
    private void Update()
    {
        if (!_isRunning) 
            return;
        
        _timer += Time.deltaTime;

        // Calculate seconds and milliseconds
        int seconds = (int)_timer;
        int milliseconds = (int)((_timer - seconds) * 1000);
        
        // Format the timer as seconds:milliseconds
        string formatedTime = $"{seconds}:{milliseconds:000}";

        _timerText.text = formatedTime;
   
        if (_fishingBars.HasFishEscaped)
        {
            // defeat
            _fishingBars.CanRun = false;
            Debug.Log("You Lose");  
            _isRunning = false;    
            _fishingUI.SetActive(false);
            _recordText.text = formatedTime;
            
            _newGamePanel.SetActive(true);  
            _recordText.gameObject.SetActive(true);
        }
    }
    
}

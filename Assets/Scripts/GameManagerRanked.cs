using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManagerRanked : GameManager
{
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_isRunning) 
            return;
        
        _timer += Time.deltaTime;
        _countdownText.text = ((int)_timer).ToString("00");
   
        if (_fishingBars.HasFishEscaped)
        {
            // defeat
            _fishingBars.CanRun = false;
            Debug.Log("You Lose");  
            _isRunning = false;    
            _fishingUI.SetActive(false);
            _fishingUI.SetActive(false);
            ShowDefeatEndGameMenu();
        }
    }
    
    
}

using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;


public class GameManager : MonoBehaviour
{
    
    protected enum Difficulty
    {
        Easy,
        Mid,
        Hard,
        Inferno
    }
    
    [Title("Gameplay")]   
    [SerializeField] protected Difficulty _difficulty = Difficulty.Easy;
    [SerializeField] private float _countdownDuration = 15.5f;
    [SerializeField] protected float _displayEndGameMenuAfter = 3f;
    
    [Title("Debugging")] 
    [ReadOnly, SerializeField] protected float _timer = 0;
    [ReadOnly, SerializeField] protected bool _isRunning = false;
    
    [Title("References")]
    [SerializeField] protected GameObject _musicManagerPrefab;
    [SerializeField] protected GameObject _fishingUI;
    [SerializeField] protected GameObject _newGamePanel;
    [SerializeField] protected GameObject _welcomeText; 
    [SerializeField] protected GameObject _victoryText;
    [SerializeField] protected GameObject _defeatText;
    [SerializeField] protected FishingBars _fishingBars; 
    [SerializeField] protected TextMeshProUGUI _countdownText; 
    [SerializeField] protected FishCatchingController _fishCatchingController;
    
    private void Start()
    {
        _fishingUI.SetActive(false);
        
        // Instantiates a new MusicManager in case there is None in the scene.
        // The MusicPlayer script also checks if it is the only instance in the scene.
        if (FindObjectOfType<MusicPlayer>() is null)
            Instantiate(_musicManagerPrefab);
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (!_isRunning) 
            return;
        
        _timer -= Time.deltaTime;
        _countdownText.text = ((int)_timer).ToString("00");
        
        if (_timer <= 0)
        {
            // win 
            _fishingBars.CanRun = false;
            Debug.Log("You Win");
            _isRunning = false;
            _fishingUI.SetActive(false);
            _fishCatchingController.JumpTowardsTarget();
            Invoke(nameof(ShowVictoryEndGameMenu), _displayEndGameMenuAfter);
        }
        else if (_fishingBars.HasFishEscaped)
        {
            // defeat
            _fishingBars.CanRun = false;
            Debug.Log("You Lose");  
            _isRunning = false;    
            _fishingUI.SetActive(false);
            ShowDefeatEndGameMenu();
        }
    }
    
    public void LoadLevel(int sceneIndex)
    {
        // The current scene shouldn't be loaded.
        if (SceneManager.GetActiveScene().buildIndex == sceneIndex) 
            return;
        SceneManager.LoadScene(sceneIndex);
    }

    public void RunNewGameForSetDifficulty()
    {
        switch (_difficulty)
        {
            case Difficulty.Easy: RunNewGameEasy(); break;
            case Difficulty.Mid: RunNewGameMid(); break;
            case Difficulty.Hard: RunNewGameHard(); break;
            case Difficulty.Inferno: RunNewGameInferno(); break;
        }
    }
    
    [Button]                                                    
    private void RunNewGameEasy()                                 
    {                                                           
        _fishingBars._fishTimerMultiplier = 1.5f;               
        _fishingBars._escapeBarIncrement = 0.2f;                
        _fishingBars._fishSmoothMotion = 0.7f;  
        _fishingBars._hookEscapeDecrement = 0.15f;     
        RunNewGame();                                           
    }                                                           
    
    [Button]                               
    private void RunNewGameMid()
    {
        _fishingBars._fishTimerMultiplier = 1.3f;
        _fishingBars._escapeBarIncrement = 0.25f; 
        _fishingBars._fishSmoothMotion = 0.6f; 
        _fishingBars._hookEscapeDecrement = 0.125f;    
        RunNewGame();
    }                                      
    
    [Button]                                              
    private void RunNewGameHard()                           
    {                                                     
        _fishingBars._fishTimerMultiplier = 1f;
        _fishingBars._escapeBarIncrement = 0.3f;
        _fishingBars._fishSmoothMotion = 0.5f;
        _fishingBars._hookEscapeDecrement = 0.1f;
        RunNewGame();                                     
    }                                                     
                                                                       
    [Button]                                                       
    private void RunNewGameInferno()                                   
    {                                                              
        _fishingBars._fishTimerMultiplier = 0.8f;                    
        _fishingBars._escapeBarIncrement = 0.4f;                   
        _fishingBars._fishSmoothMotion = 0.3f;                     
        _fishingBars._hookEscapeDecrement = 0.07f;                  
        RunNewGame();                                              
    }                                                              
    
    private void RunNewGame()
    {
        _fishCatchingController.ResetFish();
        _fishingUI.SetActive(true);
        _newGamePanel.SetActive(false);
        _welcomeText.SetActive(false);
        _victoryText.SetActive(false);   
        _defeatText.SetActive(false);   
        _timer = _countdownDuration;
        _isRunning = true;
        _fishingBars.ResetTheBars();
        _fishingBars.CanRun = true;
    }

    protected void ShowVictoryEndGameMenu()
    {
        _newGamePanel.SetActive(true);
        _victoryText.SetActive(true); 
    }
    
    protected void ShowDefeatEndGameMenu()
    {
        _newGamePanel.SetActive(true);  
        _defeatText.SetActive(true);    
    }
    
}

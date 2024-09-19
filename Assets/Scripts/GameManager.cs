using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;


public class GameManager : MonoBehaviour
{
    
    private enum Difficulty
    {
        Easy,
        Mid,
        Hard,
        Inferno
    }
    
    [Title("Gameplay")]   
    [SerializeField] private Difficulty _difficulty = Difficulty.Easy;
    [SerializeField] private float _countdownDuration = 15.5f;
    [SerializeField] private float _displayEndGameMenuAfter = 3f;
    
    [Title("Debugging")] 
    [ReadOnly, SerializeField] private float _countdown = 0;
    [ReadOnly, SerializeField] private bool _isRunning = false;
    
    [Title("References")]
    [SerializeField] private GameObject _musicManagerPrefab;
    [SerializeField] private GameObject _fishingUI;
    [SerializeField] private GameObject _newGamePanel;
    [SerializeField] private GameObject _welcomeText; 
    [SerializeField] private GameObject _victoryText;
    [SerializeField] private GameObject _defeatText;
    [SerializeField] private FishingBars _fishingBars; 
    [SerializeField] private TextMeshProUGUI _countdownText; 
    [FormerlySerializedAs("_fishModelController")] [SerializeField] private FishCatchingController _fishCatchingController;
    
    
    private void Start()
    {
        _fishingUI.SetActive(false);
        
        // Instantiates a new MusicManager in case there is None in the scene.
        // The MusicPlayer script also checks if it is the only instance in the scene.
        if (FindObjectOfType<MusicPlayer>() is null)
            Instantiate(_musicManagerPrefab);
    }
    
    public void LoadLevel(int sceneIndex)
    {
        // The current scene shouldn't be loaded.
        if (SceneManager.GetActiveScene().buildIndex == sceneIndex) 
            return;

        _difficulty = (Difficulty) sceneIndex;
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
        _countdown = _countdownDuration;
        _isRunning = true;
        _fishingBars.ResetTheBars();
        _fishingBars.CanRun = true;
    }

    // Update is called once per frame
    private void Update()
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

    private void ShowVictoryEndGameMenu()
    {
        _newGamePanel.SetActive(true);
        _victoryText.SetActive(true); 
    }
    
    private void ShowDefeatEndGameMenu()
    {
        _newGamePanel.SetActive(true);  
        _defeatText.SetActive(true);    
    }
    
}

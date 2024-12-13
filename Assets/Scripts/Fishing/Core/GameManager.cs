using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class GameManager : MonoBehaviour
{

    [Title("Dependence Injection")]
    [Required] public FishingSet currentSet;
    
    [Title("Gameplay")]   
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
    [SerializeField] protected TextMeshProUGUI _timerText; 
    [SerializeField] protected FishCatchingController _fishCatchingController;
    
    private void Start()
    {
        _fishingUI.SetActive(false);
        
        // Instantiates a new MusicManager in case there is None in the scene.
        // The MusicPlayer script also checks if it is the only instance in the scene.
        if (FindAnyObjectByType<MusicPlayer>() is null)
            Instantiate(_musicManagerPrefab);
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (!_isRunning) 
            return;
        
        _timer -= Time.deltaTime;
        _timerText.text = ((int)_timer).ToString("00");
        
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
    
    [Button]
    private void RunNewGame()
    {
        if (currentSet == null)
        {
            Debug.LogWarning("Fishing Set is null, the stage will not load");
            return;
        }
        
        // Setting the stage dependencies.
        _fishingBars._hookEscapeDecrement = currentSet.hookEscapeDecrement;    
        _fishingBars._escapeBarIncrement = currentSet.escapeBarIncrement; 
        _fishingBars._fishTimerMultiplier = currentSet.fishTimerMultiplier;
        _fishingBars._fishSmoothMotion = currentSet.fishSmoothMotion; 
        
        // Hide the rest of the UIs
        _newGamePanel.SetActive(false);
        _welcomeText.SetActive(false);
        _victoryText.SetActive(false);   
        _defeatText.SetActive(false);   
        
        // Set the timer and flag to a new run.
        _timer = _countdownDuration;
        _isRunning = true;
        
        // Reset the fish model to its initial state.
        _fishCatchingController.ResetFish();
        
        // Show the fishing bars.
        _fishingUI.SetActive(true);
        
        // Start the fishing bars logic
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

using TMPro;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

public class LeaderboardCell : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _rankText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _timeText;

    private LeaderboardEntry _playerEntry;
    
    public void Initialize(LeaderboardEntry playerEntry)
    {
        _playerEntry = playerEntry;
        _rankText.text = _playerEntry.Rank.ToString();
        _nameText.text = _playerEntry.PlayerName;
        
        // Calculate seconds and milliseconds
        int seconds = (int) _playerEntry.Score;
        int milliseconds = (int) ((_playerEntry.Score - seconds) * 1000);
        // Format the timer as seconds:milliseconds
        string formatedTime = $"{seconds}:{milliseconds:000}";
        _timeText.text = formatedTime;
    }
}

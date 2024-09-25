using Sirenix.OdinInspector;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;


public class LeaderboardManager : MonoBehaviour
{

    private const string LBID = "Inferno_Top_Players";
    
    [SerializeField] private GameObject _cellPrefab;
    [SerializeField] private Transform _cellsParent;
    [SerializeField] private float _totalEntriesDisplayed;

    
    [Button]
    public void AddTimeScoreTest(float time)
    {
        AddTimeScoreAsync(5750);
    }
    
    public async void AddTimeScoreAsync(float time)
    {
        try
        {
            LeaderboardEntry playerEntry = 
                await LeaderboardsService.Instance.AddPlayerScoreAsync(LBID, time);
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
   
    }

    private void LoadLeaderBoardCells()
    {
        ClearLeaderBoardCells();
        
    }

    private void ClearLeaderBoardCells()
    {
        LeaderboardCell[] cells = _cellsParent.GetComponentsInChildren<LeaderboardCell>();
        if (cells != null) 
            foreach (LeaderboardCell cell in cells) 
                Destroy(cell.gameObject);
    }

}

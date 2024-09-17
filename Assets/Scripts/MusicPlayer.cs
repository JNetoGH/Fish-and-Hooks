using UnityEngine;
using UnityEngine.UI;
using System.IO;   // Required for file handling
using System;
using System.Collections;
using Sirenix.OdinInspector; // Required for serialization

public class MusicPlayer : MonoBehaviour
{
    
    [Title("Parameters")]
    [SerializeField] private AudioSource _audioSource;         // Reference to the AudioSource component
    [SerializeField] private Slider _volumeSlider;             // Reference to the volume slider in the UI
    [SerializeField] private AudioClip[] _playlist;            // Array of songs to play
    
    [Title("Debugging")]
    [SerializeField, ReadOnly] private int _currentSongIndex = 0;       // Tracks the index of the current song
    [SerializeField, ReadOnly] private float _currentVolume = 0.15f;    // Tracks the volume level
    
    private string _saveFilePath;            // Path for saving and loading JSON data

    [Serializable]
    public class SaveData
    {
        public int songIndex;
        public float volume;
    }

    void Start()
    {
        // Set the path for the save file in persistentDataPath
        _saveFilePath = Path.Combine(Application.persistentDataPath, "music_player_save.json");

        // Load saved data (song index and volume) from JSON, if available
        LoadData();

        // Set initial volume from loaded data
        _audioSource.volume = _currentVolume;

        // Set up the slider to reflect the saved volume value
        if (_volumeSlider != null)
        {
            _volumeSlider.value = _currentVolume;
            _volumeSlider.onValueChanged.AddListener(ChangeVolume);
        }
        
        // Start playing the song from the saved index
        PlaySong(_currentSongIndex);
    }

    void Update()
    {
        // Check if the song has finished playing
        if (!_audioSource.isPlaying)
        {
            // Move to the next song, looping if necessary
            _currentSongIndex = (_currentSongIndex + 1) % _playlist.Length;
            // Play the next song
            PlaySong(_currentSongIndex);
        }
    }
    
    // Plays a song asynchronously from the playlist at the given index.
    // If the playlist is empty, the method exits early.
    // The audio clip at the specified index is loaded asynchronously using LoadAudioData().
    // The method waits until the audio data is fully loaded before assigning it to the AudioSource and starting playback.
    // This helps prevent stuttering by not blocking the main thread during audio loading.
    private IEnumerator PlaySongAsync(int index)
    {
        if (_playlist.Length == 0) 
            yield break;
    
        AudioClip clip = _playlist[index];
    
        // Load audio data asynchronously.
        clip.LoadAudioData();

        // Wait until the audio data is loaded
        while (!clip.loadState.Equals(AudioDataLoadState.Loaded))
            yield return null; // Wait for the next frame
        
        _audioSource.clip = clip;
        _audioSource.Play();
    }
    
    // Modify PlaySong to use the coroutine.
    private void PlaySong(int index)
    {
        StartCoroutine(PlaySongAsync(index));
    }
    
    // Change the volume using a slider and save the volume setting
    public void ChangeVolume(float volume)
    {
        _audioSource.volume = volume;
        _currentVolume = volume;

        // Save the updated volume to the JSON file
        SaveDataToFile();
    }

    // Save the current song index and volume to a JSON file
    private void SaveDataToFile()
    {
        SaveData data = new SaveData
        {
            songIndex = _currentSongIndex,
            volume = _currentVolume
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(_saveFilePath, json);
    }

    // Load the song index and volume from the JSON file, if it exists
    private void LoadData()
    {
        if (File.Exists(_saveFilePath))
        {
            string json = File.ReadAllText(_saveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            // Increment the song index to play the next song
            _currentSongIndex = (data.songIndex + 1) % _playlist.Length;

            // Load the saved volume
            _currentVolume = data.volume;
        }
        else
        {
            // Default values if no save file exists
            _currentSongIndex = 0;
            _currentVolume = 0.5f;
        }
    }

    // Save on pause.
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            SaveDataToFile();
    }

    // Save on focus lost.
    private void OnApplicationFocus(bool hasFocus)
    {
        if(!hasFocus)
            SaveDataToFile();
    }

    // Save on quit.
    private void OnApplicationQuit()
    {
        SaveDataToFile();
    }
}
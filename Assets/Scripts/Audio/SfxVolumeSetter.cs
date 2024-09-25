using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SfxVolumeSetter : MonoBehaviour
{
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private AudioSource[] _audioSources;

    private string _saveFilePath;

    [System.Serializable]
    public class VolumeData
    {
        public float volume;
    }

    private void Start()
    {
        // Set the file path for saving and loading JSON data
        _saveFilePath = Path.Combine(Application.persistentDataPath, "sfx_volume_data.json");

        // Load saved volume data from the JSON file
        LoadVolumeData();

        // Add listener to update volume and save it when changed
        _volumeSlider.onValueChanged.AddListener(UpdateSfxAudioSources);
    }

    // Update the volume for all audio sources and save the new volume
    private void UpdateSfxAudioSources(float volume)
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.volume = volume;
        }

        // Save the volume to the JSON file
        SaveVolumeData(volume);
    }

    // Save the current volume to a JSON file
    private void SaveVolumeData(float volume)
    {
        VolumeData data = new VolumeData
        {
            volume = volume
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(_saveFilePath, json);
    }

    // Load the volume from the JSON file
    private void LoadVolumeData()
    {
        if (File.Exists(_saveFilePath))
        {
            string json = File.ReadAllText(_saveFilePath);
            VolumeData data = JsonUtility.FromJson<VolumeData>(json);

            // Set the slider and audio sources to the saved volume
            _volumeSlider.value = data.volume;
            UpdateSfxAudioSources(data.volume);
        }
        else
        {
            // If no file exists, use a default volume and save it
            float defaultVolume = 1.0f;
            _volumeSlider.value = defaultVolume;
            UpdateSfxAudioSources(defaultVolume);
            SaveVolumeData(defaultVolume);
        }
    }
}
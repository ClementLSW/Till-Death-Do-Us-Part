using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    //public AudioManager audioManager;
    [SerializeField] public AudioMixer audioMixer;

    const string MASTER_KEY = "MasterVolume";
    const string MUSIC_KEY = "MusicVolume";
    const string SFX_KEY = "SFXVolume";

    [SerializeField] Slider masterSlider, musicSlider, sfxSlider;

    [SerializeField] GameObject settingsCanvas;

/*    private void Awake()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        AudioManager audioManager = FindObjectOfType<AudioManager>();
#pragma warning restore CS0618 // Type or member is obsolete
        audioMixer = audioManager.audioMixer;
    }*/

    void Start()
    {
        LoadVolumeSettings();
        settingsCanvas.SetActive(false);
    }

    public void SetMasterVolume(float volume)
    {
        Debug.Log($"Master Volume changed to {volume}");
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(MASTER_KEY, volume);
    }

    public void SetMusicVolume(float volume)
    {
        Debug.Log($"Music Volume changed to {volume}");
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(MUSIC_KEY, volume);
    }

    public void SetSFXVolume(float volume)
    {
        Debug.Log($"SFX Volume changed to {volume}");
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(SFX_KEY, volume);
    }

    public void LoadVolumeSettings()
    {
        float master = PlayerPrefs.GetFloat(MASTER_KEY, 1f);
        float music = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        float sfx = PlayerPrefs.GetFloat(SFX_KEY, 1f);

        audioMixer.SetFloat("MasterVolume", Mathf.Log10(master) * 20);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(music) * 20);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfx) * 20);

        masterSlider.value = master;
        musicSlider.value = music;
        sfxSlider.value = sfx;
    }
}

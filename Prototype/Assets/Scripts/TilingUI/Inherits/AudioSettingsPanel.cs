using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsPanel : UIPanel
{
    public AudioMixerGroup musicMixerGroup;
    public AudioMixerGroup sfxMixerGroup;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public override void Start()
    {
        base.Start();

        float musicVolumeSaved = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfxVolumeSaved = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        musicVolumeSlider.value = musicVolumeSaved;
        sfxVolumeSlider.value = sfxVolumeSaved;

    }
    public void SetMusicVolume()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        Debug.Log("Set Music Volume");
        musicMixerGroup.audioMixer.SetFloat("MusicVolume", musicVolumeSlider.value);
    }
    public void SetSFXVolume()
    {
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
        Debug.Log("Set Music Volume");
        musicMixerGroup.audioMixer.SetFloat("SFXVolume", sfxVolumeSlider.value);
    }
}

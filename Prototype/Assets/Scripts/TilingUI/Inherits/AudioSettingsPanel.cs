using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsPanel : UIPanel
{
    public AudioMixerGroup musicMixerGroup;
    public AudioMixerGroup sfxMixerGroup;
    public AudioMixerGroup menuMixerGroup;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider menuVolumeSlider;
    public override void Start()
    {
        base.Start();

        float musicVolumeSaved = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfxVolumeSaved = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        float menuVolumeSaved = PlayerPrefs.GetFloat("MenuVolume", 0.75f);
        musicVolumeSlider.value = musicVolumeSaved;
        sfxVolumeSlider.value = sfxVolumeSaved;
        menuVolumeSlider.value = menuVolumeSaved;

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
        sfxMixerGroup.audioMixer.SetFloat("SFXVolume", sfxVolumeSlider.value);
    }
    public void SetMenuVolume()
    {
        PlayerPrefs.SetFloat("MenuVolume", menuVolumeSlider.value);
        Debug.Log("Set Menu Volume");
        menuMixerGroup.audioMixer.SetFloat("MenuVolume", menuVolumeSlider.value);
    }
}

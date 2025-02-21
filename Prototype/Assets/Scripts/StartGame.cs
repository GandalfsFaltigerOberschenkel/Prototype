using System.Linq;
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public AudioMixerGroup musicMixerGroup;
    public AudioMixerGroup sfxMixerGroup;
    public void Start()
    {
        float amount = PlayerPrefs.GetFloat("MusicVolume");
        musicMixerGroup.audioMixer.SetFloat("MusicVolume", amount);


        float amount2 = PlayerPrefs.GetFloat("SFXVolume");
        sfxMixerGroup.audioMixer.SetFloat("SFXVolume", amount2);

        string resolution = PlayerPrefs.GetString("Resolution", "1920x1080");
        bool isFullScreen = Convert.ToBoolean(PlayerPrefs.GetInt("FullScreen", 1));
        Debug.Log("Resolution: " + resolution);
        int[] values = resolution.Split("x").ToList().Select(e => Convert.ToInt32(e)).ToArray();
        Screen.SetResolution(values[0], values[1], isFullScreen);
    }
    public void StartTheGame()
    {
        Destroy(UIManager.instance.gameObject);
        SceneManager.LoadScene("ScalingTest");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}

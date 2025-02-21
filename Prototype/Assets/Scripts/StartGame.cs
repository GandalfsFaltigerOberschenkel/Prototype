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

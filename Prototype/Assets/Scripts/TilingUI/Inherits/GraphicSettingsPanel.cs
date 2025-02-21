using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using TMPro;
public class GraphicSettingsPanel : UIPanel
{
    public TMP_Dropdown resolutionDropdown;
    string currentSelectedResolution;
    bool currentSelectedIsFullScreen = true;
    public Toggle fullScreenToggle;
    public override void Start()
    {
        UpdateUI();
        base.Start();
    }
    void UpdateUI()
    {
        string resolution = PlayerPrefs.GetString("Resolution", "1920x1080");
        currentSelectedResolution = resolution;
       
        bool isFullScreen = Convert.ToBoolean(PlayerPrefs.GetInt("FullScreen", 1));
        currentSelectedIsFullScreen = isFullScreen;
        Debug.Log("Resolution: " + resolution);
        int[] values = resolution.Split("x").ToList().Select(e => Convert.ToInt32(e)).ToArray();
        Screen.SetResolution(values[0], values[1], isFullScreen);
        resolutionDropdown.SetValueWithoutNotify(resolutionDropdown.options.FindIndex(e => e.text == resolution));
    }
    public void SetResolution(int newPosition)
    {
        currentSelectedResolution = resolutionDropdown.options[newPosition].text;
    }
    public void SetFullScreen()
    {
        currentSelectedIsFullScreen = fullScreenToggle.isOn;
    }
    public void ApplySettings()
    {
        PlayerPrefs.SetString("Resolution", currentSelectedResolution);
        PlayerPrefs.SetInt("FullScreen", Convert.ToInt32(currentSelectedIsFullScreen));
        UpdateUI();
    }
}

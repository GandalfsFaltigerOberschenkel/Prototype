using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager2 : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject controlerUI;
    List<GameObject> controllerUis = new List<GameObject>();
    public Transform controllerList;
    public GameObject keyboardUI;

    public void SetPauseMenuStatus(bool status)
    {
        if (!status)
        {
            HideControllers();
        }
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(status);
            ShowControllers();
        }

    }
    public void ShowControllers()
    {
        HideControllers();
        List<IInputDevice> list = GameManager2.Instance.GetControllerDevices();
        List<GameObject> controllersUI = new List<GameObject>();
        foreach (var device in list)
        {
            if (device.GetType() == typeof(ControllerDevice))
            {
                GameObject controllerDeviceUI = Instantiate(controlerUI, controllerList);
                controllerDeviceUI.SetActive(true);
                controlerUI.GetComponentInChildren<TMP_Text>().text = device.Name;
                controllerUis.Add(controllerDeviceUI);
            }
            else
            {
                GameObject controllerDeviceUI = Instantiate(keyboardUI, controllerList);
                controllerDeviceUI.SetActive(true);
                controllerDeviceUI.GetComponentInChildren<TMP_Text>().text = device.Name;
                controllerUis.Add(controllerDeviceUI);
            }
        }
    }
    public void HideControllers()
    {
        for (int i = 0; i < controllerUis.Count; i++)
        {
            Destroy(controllerUis[i]);
        }
        controllerUis.Clear();
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] UIManager uiManager;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void StartGame()
    {
        //todo: do spawning magic
        LoadSettings();
    }

    public void ExitGame()
    {
        Application.Quit();
    }


    public void SaveSettings(UnityEngine.UI.Selectable element)
    {
        // probably by se ve vetsim meritku vyplatilo si to bud ukladat do souboru nebo do PLyaerPrefs.SetString(), ale davat to tam jako neco jako json
        // lip by se to nacitalo, bylo by to na jednom miste a celkove to dava vic smysl. + by slo kategorizovat settings

        switch (element)
        {
            case UnityEngine.UI.Slider slider: //tohle je hrozne cool syntaxe, jde se chovat k objektu jako k nejakemu typu ve switchi.                
                PlayerPrefs.SetFloat(element.name, slider.value);
                Debug.Log("saving " + element.name + slider.value);
                break;
        }
        PlayerPrefs.Save();        
    }

    public void LoadSettings()
    {
        //to same jako u Save metody: ukladani by bylo ve vetsim meritku potreba udelat absolutne jinak
        uiManager.LoadSetting("VolumeSlider", PlayerPrefs.GetFloat("VolumeSlider", 1));
        Debug.Log("loading " + PlayerPrefs.GetFloat("VolumeSlider", 1));
    }
}

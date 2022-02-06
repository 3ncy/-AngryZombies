using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] UIManager uiManager;


    [SerializeField] Transform spawners;
    private float spawnRate;
    

    Random random;

    //tohle by bylo realne nejakej objekt nebo tak, ale ted to neni potreba
    public float volumeSetting;

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


        random = new Random();
    }

    void Start()
    {
        Time.timeScale = 0;

        LoadSettings();
    }

    public void StartGame()
    {
        random = new Random();
        SpawnZombies();
        Debug.Log(Time.time);
    }

    private void SpawnZombies()
    {
        //todo: do spawning magic

        //Time.time



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

                //
                AudioListener.volume = slider.value;
                break;
        }
        PlayerPrefs.Save();        
    }

    public void LoadSettings()
    {
        //to same jako u Save metody: ukladani by bylo ve vetsim meritku potreba udelat absolutne jinak

        float volume = PlayerPrefs.GetFloat("VolumeSlider", 1);
        uiManager.LoadSetting("VolumeSlider", volume);
        Debug.Log("loading " + volume);
        volumeSetting = volume;

        //
        AudioListener.volume = volume;
    }
}

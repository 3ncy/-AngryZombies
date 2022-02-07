using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] UIManager uiManager;


    [SerializeField] Transform spawners;
    private float spawnRate;
    private int waveNr;
    [SerializeField] GameObject zombie;
    public Transform zombies;

    public Transform Player;
    private int score;


    System.Random random;

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
    }

    void Start()
    {
        Time.timeScale = 0;

        LoadSettings();
    }

    public void StartGame()
    {
        random = new System.Random();
        waveNr = 1;
        StartCoroutine(SpawningCycle());
    }


    private IEnumerator SpawningCycle()
    {
        while (true) //prolly spis "while(!gameover)"
        {
            SpawnZombies();
            yield return new WaitForSeconds(7);
        }
    }

    private void SpawnZombies()
    {
        int nrOfZombies = waveNr * 2 + 2;

        for (int i = 0; i < nrOfZombies; i++)
        {
            /*EnemyController enemy = */
            Instantiate(zombie, spawners.transform.GetChild(random.Next(0, spawners.childCount)).position, Quaternion.identity.normalized, zombies);/*.GetComponent<EnemyController>();*/
        }
        waveNr++;
    }

    public void AddScore(int score)
    {
        this.score += score;
        uiManager.UpdateScore(this.score);
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

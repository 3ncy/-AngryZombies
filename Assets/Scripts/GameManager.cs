﻿using System;
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
    [SerializeField] GameObject zombiePrefab;
    public Transform zombiesParent;

    public Transform Player;
    private int score;

    [SerializeField] private GameObject medkitPrefab;

    System.Random random;

    //tohle by bylo realne nejakej objekt nebo tak, ale ted to neni potreba
    public float volumeSetting;



    private Queue<GameObject> zombiePool;

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

        zombiePool = new Queue<GameObject>();
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
        StartCoroutine(SpawningCycle());  //////////////////////////////// tohle zase odkomentovat v produkci :D 
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
            int randomChild = random.Next(0, spawners.childCount);

            if (zombiePool.Count == 0)
            {
                Instantiate(zombiePrefab, spawners.transform.GetChild(randomChild).position, Quaternion.identity.normalized,
                        zombiesParent);
            }
            else //get zombie from pool
            {
                GameObject zombie = zombiePool.Dequeue();
                zombie.transform.SetPositionAndRotation(spawners.transform.GetChild(randomChild).position, Quaternion.identity);
                zombie.GetComponent<EnemyController>().Restart();
            }
        }
        waveNr++;
    }

    public void AddScore(int score, EnemyController zombie)
    {
        this.score += score;
        uiManager.UpdateScore(this.score);

        //add zombie back to pool
        zombiePool.Enqueue(zombie.gameObject);

        if (random.Next(0, 20) == 1) //v jednom z 20 zombie bude medkit cca
        {
            GameObject medkit = Instantiate(medkitPrefab, zombie.gameObject.transform.position, Quaternion.identity);
            Debug.Log("spawning medkit @ " + medkit.transform.position);
        }
    }


    public void GameOver()
    {
        StopCoroutine(SpawningCycle());

        //todo: show end screen
        //perhaps tady projet vsechny zombies a vypnout agenta
    }

    public void SaveHighscores()
    {

    }

    public void ExitGame() => Application.Quit();

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

using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject HUD;
    [SerializeField] Image damageOverlay;
    [SerializeField] Text ammoText;
    [SerializeField] Text healthText;
    [SerializeField] Text scoreText;
    [SerializeField] List<Text> weaponTexts;
    private int currentWeaponIndex;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject settingsLayout;
    [SerializeField] GameObject mainMenu;

    private bool paused = false;

    void Start() { }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame(!paused);
        }
    }

    public void PauseGame(bool pause = true)
    {
        Time.timeScale = (paused = pause) ? 0 : 1;
        pauseMenu.SetActive(paused);
    }

    public void EnableWeapon(int index)
    {
        weaponTexts[index].gameObject.SetActive(true);
    }

    public void GoToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void ChangeAmmo(int ammo/*, int maxAmmo*/)
    {
        ammoText.text = ammo.ToString()/* + "/" + maxAmmo.ToString()*/;
    }

    public void SwitchWeapon(string toWeapon)
    {
        //check neni potreba kdyz to hrac stejne nevidi
        //if (!weaponTexts[currentWeaponIndex].transform.parent.Find(toWeapon.ToLower()).gameObject.activeSelf)
        //    return;
        weaponTexts[currentWeaponIndex].gameObject.GetComponent<Outline>().enabled = false;
   
        currentWeaponIndex = weaponTexts.FindIndex(t => t.gameObject.name.ToLower().Contains(toWeapon.ToLower()));

        weaponTexts[currentWeaponIndex].gameObject.GetComponent<Outline>().enabled = true;
    }

    public void ChangeHealth(float health, bool heal)
    {
        healthText.text = health.ToString() + "/100 HP";
        damageOverlay.color = Color.clear;
        if (heal)
        {
            damageOverlay.color = Color.green;
        }
        else
        {
            damageOverlay.color = Color.red;            
        }
        damageOverlay.canvasRenderer.SetAlpha(0.6f);
        damageOverlay.CrossFadeAlpha(0, 0.5f, true);
    }


    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString() + " Points";
    }


    //tohle je naprosto fuj a dirty, ale ted to staci 
    public void LoadSetting(string setting, float value)
    {
        Debug.Log(setting + " " + value);
        settingsLayout.transform.Find(setting).GetComponent<Slider>().value = value;
    }

    //public void LoadSetting<T>(string setting, T value, UnityEngine.UI.Selectable element)
    //{
    //    switch (element)
    //    {
    //        case Slider slider:
    //          slider.value = (float)T; //nebo tak neco
    //            break;
    //        case string s:
    //            break;
    //        case bool b:
    //            break;
    //    }

    //}
}

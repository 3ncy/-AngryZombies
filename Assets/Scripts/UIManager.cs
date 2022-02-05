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
    [SerializeField] List<Text> weapons;
    private int currentWeaponIndex;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject settingsLayout;
    [SerializeField] GameObject mainMenu;

    private bool paused = false;


    void Start() { }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PauseGame(!paused);
        }
    }

    public void PauseGame(bool pause = true)
    {
        Time.timeScale = (paused = pause) ? 0 : 1;
        pauseMenu.SetActive(paused);
    }

    public void ChangeAmmo(int ammo/*, int maxAmmo*/)
    {
        ammoText.text = ammo.ToString()/* + "/" + maxAmmo.ToString()*/;
    }

    public void SwitchWeapon(string toWeapon)
    {
        weapons[currentWeaponIndex].gameObject.GetComponent<Outline>().enabled = false;

        currentWeaponIndex = weapons.FindIndex(t => t.gameObject.name.ToLower().Contains(toWeapon.ToLower()));
        Text weapon = weapons[currentWeaponIndex];
        weapon.gameObject.GetComponent<Outline>().enabled = true;

        //predpokladam ze komponent neni null, protoze tam byt musi
    }

    public void ChangeHealth(float health, bool heal)
    {
        healthText.text = health.ToString() + "/100 HP";
        if (heal)
        {
            //damageOverlay.color = new Color(0, 255, 0, 0.4f);
            damageOverlay.color = Color.green;
        }
        else
        {
            damageOverlay.color = Color.red;
            //damageOverlay.color = new Color(255, 0, 0, 0.4f);
        }
        damageOverlay.canvasRenderer.SetAlpha(0.6f);
        //Debug.Log("sth");
        //yield return new WaitForSeconds(2);
        //Debug.Log("potom");
        //damageOverlay.color = new Color(0, 0, 0, 0);

        damageOverlay.CrossFadeAlpha(0, 0.5f, true);
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

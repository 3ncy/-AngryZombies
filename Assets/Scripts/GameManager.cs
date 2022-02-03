using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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




        Time.timeScale = 1;
    }

    public void ExitGame()
    {
        Application.Quit();
    }


    //  neco takoveho bych nejspis pouzil, delat slozitejsi projekt. Ted je to ale overkill
    #region
    //takhle complicated aby slo jednoduse pridavat settings bez nutnosti novych metod. Jedine nove metody co budou potreba jsou "Button Changed" a "text changed"
    //public void SliderChanged(UnityEngine.UI.Slider slider)
    //{
    //    SettingChanged(slider.gameObject.name, slider.value);
    //}

    //public void SettingChanged(string name, object value) //pry jenom setovani playerprefs nelaguje hru, takze toto bude volano pri kazde zmene nastaveni
    //{
    //    switch (value)
    //    {
    //        case string s:
    //            PlayerPrefs.SetString(name, s);
    //            break;
    //        case float f:
    //            PlayerPrefs.SetFloat(name, f);
    //            break;
    //        case int i:
    //            PlayerPrefs.SetInt(name, i);
    //            break;
    //        default:
    //            //invalidni typ, mby parse do jsonu nebo tak, ale to by chtelo cele predelat do ukladani do normalniho souboru a ne PlayerPrefs
    //            break;
    //    }
    //}

    //teoreticky bych tu prosel vsechny nastaveni a revertnul jejich hodnoty v ui, ale mam jenom jedno nastaveni, takze to bude takhle ;P
    //public void DiscardSettings(UnityEngine.UI.Selectable element)
    //{
    //    switch (element)
    //    {
    //        case UnityEngine.UI.Slider slider:
    //            slider.value = PlayerPrefs.GetFloat(element.name);
    //            Debug.Log("loaded pri resetu: " + PlayerPrefs.GetFloat(element.name));
    //            break;
    //    }
    //}

    #endregion

    public void SaveSettings(UnityEngine.UI.Selectable element)
    {
        switch (element)
        {
            case UnityEngine.UI.Slider slider: //tohle je hrozne cool syntaxe, jde se chovat k objektu jako k nejakemu typu ve switchi.                
                PlayerPrefs.SetFloat(element.name, slider.value);
                break;
        }
        PlayerPrefs.Save();        
    }
}

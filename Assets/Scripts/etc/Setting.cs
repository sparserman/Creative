using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public GameObject setting;
    GameManager gm;
    Animator anim;

    public Slider mainVolSlider;
    public Slider effectVolSlider;
    public Slider BGMVolSlider;

    public GameObject mobWindow;

    void Start()
    {
        Init();
        
    }

    
    public void Init()
    {
        gm = GameManager.GetInstance();

        if (gm.sm != null)
        {
            mainVolSlider.value = SoundManager.volume;
            effectVolSlider.value = SoundManager.effectVolume;
            BGMVolSlider.value = SoundManager.BGMVolume;

            gm.sm.mainVolSlider = mainVolSlider;
            gm.sm.effectVolSlider = effectVolSlider;
            gm.sm.BGMVolSlider = BGMVolSlider;
        }

        anim = setting.GetComponent<Animator>();
    }

    public void OpenMobWindow()
    {
        mobWindow.SetActive(true);
        gm.goList.Add(mobWindow);
        if (gm.sm != null)
        {
            gm.sm.PlayEffectSound(gm.sm.click);
        }
    }

    public void ExitMobWindow()
    {
        gm.goList.Remove(mobWindow);
        mobWindow.GetComponent<Animator>().SetTrigger("Off");
        if (gm.sm != null)
        {
            gm.sm.PlayEffectSound(gm.sm.click);
        }
    }

    public void OpenSetting()
    {
        setting.SetActive(true);
        gm.goList.Add(setting);
        gm.timerOn = false;
        if (gm.sm != null)
        {
            gm.sm.PlayEffectSound(gm.sm.click);
        }
    }

    public void ExitSetting()
    {
        gm.goList.Remove(setting);
        anim.SetTrigger("Off");

        if (gm.sm != null)
        {
            gm.sm.PlayEffectSound(gm.sm.click);
        }
    }


    public void ActiveOff()
    {
        gameObject.SetActive(false);
        GameManager.GetInstance().timerOn = true;
    }
}

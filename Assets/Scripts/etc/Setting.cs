using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public GameObject setting;
    GameManager gm;
    SoundManager sm;
    Animator anim;

    public Slider mainVolSlider;
    public Slider effectVolSlider;
    public Slider BGMVolSlider;

    void Start()
    {
        Init();
        anim = setting.GetComponent<Animator>();
    }

    void Update()
    {

    }

    public void OpenSetting()
    {
        gm.goList.Add(setting);
        setting.SetActive(true);
        if (sm != null)
        {
            sm.PlayEffectSound(sm.click);
        }
    }

    public void ExitSetting()
    {
        gm.goList.Remove(setting);
        anim.SetTrigger("Off");

        if (sm != null)
        {
            sm.PlayEffectSound(sm.click);
        }
    }

    public void Init()
    {
        gm = GameManager.GetInstance();
        sm = gm.GetComponent<SoundManager>();

        if (sm != null)
        {
            mainVolSlider.value = SoundManager.volume;
            effectVolSlider.value = SoundManager.effectVolume;
            BGMVolSlider.value = SoundManager.BGMVolume;

            sm.mainVolSlider = mainVolSlider;
            sm.effectVolSlider = effectVolSlider;
            sm.BGMVolSlider = BGMVolSlider;
        }
    }

    public void CloseSetting()
    {
        gameObject.SetActive(false);
    }
}

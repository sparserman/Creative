using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public Slider mainVolSlider;
    public Slider effectVolSlider;
    public Slider BGMVolSlider;

    public static float volume = 1f;
    public static float effectVolume = 1f;
    public static float BGMVolume = 1f;

    public static bool AllMute = false;
    public static bool effectMute = false;
    public static bool BGMMute = false;

    public AudioSource Effect_audioSource;
    public AudioSource BGM_audioSource;

    public AudioClip mainBGM;
    public AudioClip lobbyBGM;
    public AudioClip battleBGM;
    public AudioClip click;

    private void Start()
    {
        
    }

    private void Update()
    {
        // 볼륨 조절
        SetAllVolume();

        // 조절된 볼륨으로 세팅
        SetEffectVolume();
        SetBGMVolume();
        SetVolume();
    }

    public void SetAllVolume()
    {
        if (mainVolSlider != null && effectVolSlider != null && BGMVolSlider != null)
        {
            volume = mainVolSlider.value;
            effectVolume = effectVolSlider.value;
            BGMVolume = BGMVolSlider.value;
        }
    }

    // Effect 사운드 재생
    public void PlayEffectSound(AudioClip clip)
    {
        if (clip != null)
        {
            Effect_audioSource.PlayOneShot(clip);
        }
    }

    public void SetEffectVolume()
    {
        if (Effect_audioSource != null)
        {
            Effect_audioSource.volume = effectVolume;
        }
    }

    public void MuteEffectVolume()
    {
        effectMute = effectMute ? false : true;

        Effect_audioSource.mute = effectMute ? true : false;
    }

    // BGM 사운드
    public void PlayBGMSound(AudioClip clip)
    {
        if (clip != null)
        {
            BGM_audioSource.clip = clip;
            BGM_audioSource.Play();
        }
    }

    public void SetBGMVolume()
    {
        if (BGM_audioSource != null)
        {
            BGM_audioSource.volume = BGMVolume;
        }
    }

    public void MuteBGMVolume()
    {
        BGMMute = BGMMute ? false : true;

        BGM_audioSource.mute = BGMMute ? true : false;
    }

    // 전체 사운드
    public void SetVolume()
    {
        if (BGM_audioSource != null && Effect_audioSource != null)
        {
            Effect_audioSource.volume = volume * effectVolume;
            BGM_audioSource.volume = volume * BGMVolume;
        }
    }

    public void MuteVolume()
    {
        AllMute = AllMute ? false : true;
        if (AllMute)
        {
            BGM_audioSource.mute = true;
            Effect_audioSource.mute = true;
        }
        else
        {
            BGM_audioSource.mute = BGMMute;
            Effect_audioSource.mute = effectMute;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public Slider mainVolSlider;
    public Slider effectVolSlider;
    public Slider BGMVolSlider;

    public static float volume = 0.3f;
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
        // ���� ����
        SetAllVolume();

        // ������ �������� ����
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

    // Effect ���� ���
    public void PlayEffectSound(AudioClip clip)
    {
        Effect_audioSource.PlayOneShot(clip);
    }

    public void SetEffectVolume()
    {
        Effect_audioSource.volume = effectVolume;
    }

    public void MuteEffectVolume()
    {
        effectMute = effectMute ? false : true;

        Effect_audioSource.mute = effectMute ? true : false;
    }

    // BGM ����
    public void PlayBGMSound(AudioClip clip)
    {
        BGM_audioSource.clip = clip;
        BGM_audioSource.Play();
    }

    public void SetBGMVolume()
    {
        BGM_audioSource.volume = BGMVolume;
    }

    public void MuteBGMVolume()
    {
        BGMMute = BGMMute ? false : true;

        BGM_audioSource.mute = BGMMute ? true : false;
    }

    // ��ü ����
    public void SetVolume()
    {
        Effect_audioSource.volume = volume * effectVolume;
        BGM_audioSource.volume = volume * BGMVolume;
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

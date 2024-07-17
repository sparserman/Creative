using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WorldInfo : MonoBehaviour
{
    GameManager gm;
    SoundManager sm;
    Animator anim;

    public TextMeshProUGUI worldName;    // 월드 이름
    public Image worldImage;    // 월드 이미지
    public TextMeshProUGUI worldDescription; // 월드 설명

    public Image managerImage;  // 매니저 이미지
    public TextMeshProUGUI managerName;  // 매니저 이름
    public TextMeshProUGUI worldDetails; // 월드 근황

    public TextMeshProUGUI managerState; // 매니저 상태
    public TextMeshProUGUI management;    // 관리율

    public int worldCode = 0;

    void Start()
    {
        anim = GetComponent<Animator>();   

        gm = GameManager.GetInstance();
        sm = gm.GetComponent<SoundManager>();
    }

    void Update()
    {
        
    }

    public void VisitButton()
    {
        gm.lm.StartFadeIn(worldCode);
    }

    public void WorldInfoOff()
    {
        anim.SetTrigger("Off");
        gm.goList.Remove(gameObject);
        if (sm != null)
        {
            sm.PlayEffectSound(sm.click);
        }
    }

    public void DestroyWorldInfo()
    {
        Destroy(gameObject);
    }
}

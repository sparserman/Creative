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

    public TextMeshProUGUI worldName;    // ���� �̸�
    public Image worldImage;    // ���� �̹���
    public TextMeshProUGUI worldDescription; // ���� ����

    public Image managerImage;  // �Ŵ��� �̹���
    public TextMeshProUGUI managerName;  // �Ŵ��� �̸�
    public TextMeshProUGUI worldDetails; // ���� ��Ȳ

    public TextMeshProUGUI managerState; // �Ŵ��� ����
    public TextMeshProUGUI management;    // ������

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

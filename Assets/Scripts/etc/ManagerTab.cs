using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerTab : MonoBehaviour
{
    GameManager gm;
    SoundManager sm;
    Animator anim;

    public GameObject parent;

    public WorldInfo worldInfo;

    void Start()
    {
        gm = GameManager.GetInstance();
        sm = gm.GetComponent<SoundManager>();
        anim = GetComponent<Animator>();

        CreateInfoTab();
    }


    void CreateInfoTab()
    {
        for (int i = 0; i < gm.gi.managerList.Count; i++)
        {
            ManagerInfo mi = gm.gi.managerList[i];

            // 보유중인 관리자라면
            if (mi.having)
            {
                // 정보탭 생성
                GameObject go = Instantiate(Resources.Load("Prefabs/" + "ManagerInfoTab") as GameObject);
                go.transform.SetParent(parent.transform, false);
                go.GetComponent<ManagerInfoTab>().worldInfo = worldInfo;
                go.GetComponent<ManagerInfoTab>().manager = mi;
            }
        }
    }

    public void ManagerTabOff()
    {
        anim.SetTrigger("Off");
        gm.goList.Remove(gameObject);
        if (sm != null)
        {
            sm.PlayEffectSound(sm.click);
        }
    }

    public void UIOff()
    {
        Destroy(gameObject);
    }
}

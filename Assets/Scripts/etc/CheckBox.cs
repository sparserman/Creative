using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum E_BoxType
{
    ManagerSelect = 0,
    MobPlacement = 1,
    MobRelease
}

public class CheckBox : MonoBehaviour
{
    GameManager gm;
    Animator anim;

    public E_BoxType type;
    public GameObject obj;

    // 내용
    public TextMeshProUGUI description;
    public TextMeshProUGUI button1;
    public TextMeshProUGUI button2;

    void Start()
    {
        gm = GameManager.GetInstance();
        anim = GetComponent<Animator>();
    }


    // yes = true, no = false
    public void ClickButton(bool p_flag)
    {
        if (p_flag)
        {
            switch (type)
            {
                case E_BoxType.ManagerSelect:
                    obj.GetComponent<ManagerInfoTab>().ManagerChange();
                    break;
                case E_BoxType.MobPlacement:
                    obj.GetComponent<MobProfile>().mobInfo.placement = true;
                    // 표시 다시하기
                    obj.GetComponent<MobProfile>().mobWindow.InitMobProfile();
                    obj.GetComponent<MobProfile>().mobWindow.MobProfileSetting();
                    break;
                case E_BoxType.MobRelease:
                    obj.GetComponent<PlacementMobImage>().mobInfo.placement = false;
                    // 표시 다시하기
                    obj.GetComponent<PlacementMobImage>().mobWindow.InitMobProfile();
                    obj.GetComponent<PlacementMobImage>().mobWindow.MobProfileSetting();
                    break;
            }
        }

        anim.SetTrigger("Off");
        gm.goList.Remove(gameObject);
        if (gm.sm != null)
        {
            gm.sm.PlayEffectSound(gm.sm.click);
        }
    }

    public void BoxDestroy()
    {
        Destroy(gameObject);
    }

    public void EnableFalse()
    {
        gameObject.SetActive(false);
    }
}

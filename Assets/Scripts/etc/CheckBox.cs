using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum E_BoxType
{
    ManagerSelect = 0,
    TutorialCheck = 1,
}

public class CheckBox : MonoBehaviour
{
    GameManager gm;
    Animator anim;

    public E_BoxType type;
    public GameObject obj;

    // ����
    [TextArea]
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
                case E_BoxType.TutorialCheck:

                    break;
            }
        }

        anim.SetTrigger("Off");
        gm.goList.Remove(gameObject);

        gm.sm.PlayEffectSound(gm.sm.click);

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

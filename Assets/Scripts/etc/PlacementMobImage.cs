using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacementMobImage : MonoBehaviour
{
    GameManager gm;

    public MobInfo mobInfo;
    public MobWindow mobWindow;

    void Start()
    {
        gm = GameManager.GetInstance();
    }

    public void Click()
    {
        mobWindow.InputMobInfo(mobInfo);
    }

    public void MinusClick()
    {
        // ��ġ ����
        GetComponent<MobProfile>().mobInfo.placement = true;
        // ǥ�� �ٽ��ϱ�
        GetComponent<MobProfile>().mobWindow.InitMobProfile();
        GetComponent<MobProfile>().mobWindow.MobProfileSetting();

    }

}

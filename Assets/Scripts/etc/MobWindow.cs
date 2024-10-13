using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MobWindow : MonoBehaviour
{
    GameManager gm;
    Animator anim;

    public GameObject mobProfileContent;
    public GameObject placementMobImageList;


    public Image illust;
    // ����
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI mpText;
    public TextMeshProUGUI adText;
    public TextMeshProUGUI asText;
    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI msText;
    public TextMeshProUGUI wtText;
    public TextMeshProUGUI ftText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI magicText;
    public TextMeshProUGUI foodText;

    public List<GameObject> profileList = new List<GameObject>();

    void Start()
    {
        gm = GameManager.GetInstance();
        anim = GetComponent<Animator>();

        // ���� �� ����
        InitMobProfile();
        MobProfileSetting();

    }


    // �� ������ ǥ��
    public void MobProfileSetting()
    {
        // ��ġ �� �� ǥ��
        for (int i = 0; i < gm.gi.specialMobList.Count; i++)
        {
            if (gm.gi.specialMobList[i].placement)
            {
                // �� ������ ����
                GameObject go = Instantiate(Resources.Load("Prefabs/" + "PlacementMobImage") as GameObject);

                // Content�ȿ� �ֱ�
                go.transform.SetParent(placementMobImageList.transform, false);

                // ���� �ֱ�
                go.GetComponent<Image>().sprite = gm.gi.specialMobList[i].sprite;
                go.GetComponent<PlacementMobImage>().mobInfo = gm.gi.specialMobList[i];

                go.GetComponent<PlacementMobImage>().mobWindow = this;

                // ������ ������Ʈ ����Ʈ�� �߰� (���߿� �������)
                profileList.Add(go);
            }
        }

        // �������ִ� ���� �ֵ� ǥ��
        for (int i = 0; i < gm.gi.specialMobList.Count; i++)
        {
            if (gm.gi.specialMobList[i].having && !gm.gi.specialMobList[i].placement)
            {
                // �� ������ ����
                GameObject go = Instantiate(Resources.Load("Prefabs/" + "MobProfile") as GameObject);
                MobProfile mobProfile = go.GetComponent<MobProfile>();

                // Content�ȿ� �ֱ�
                mobProfile.transform.SetParent(mobProfileContent.transform, false);

                // ���� ���� (�̹���, �̸�, ����, ��ġ����)
                mobProfile.image.sprite = gm.gi.specialMobList[i].sprite;
                mobProfile.mobName.text = gm.gi.specialMobList[i].mobName;
                mobProfile.level.text = gm.gi.specialMobList[i].stat.level.ToString();
                mobProfile.mobInfo = gm.gi.specialMobList[i];

                mobProfile.mobWindow = this;

                // ������ ������Ʈ ����Ʈ�� �߰� (���߿� �������)
                profileList.Add(go);
            }
        }
    }

    // ��� �ʱ�ȭ
    public void InitMobProfile()
    {
        if(profileList.Count > 0)
        {
            // �� ����������
            for(int i = 0; i < profileList.Count; i++)
            {
                Destroy(profileList[i]);
            }
            profileList.Clear();
        }
    }

    public void InputMobInfo(MobInfo mobInfo)
    {
        anim.SetTrigger("Change");

        illust.sprite = mobInfo.illust;
        nameText.text = mobInfo.mobName;
        levelText.text = "Lv." + mobInfo.stat.level;
        hpText.text = mobInfo.stat.maxHp.ToString();
        mpText.text = mobInfo.stat.maxMp.ToString();
        adText.text = mobInfo.stat.ad.ToString();
        asText.text = mobInfo.stat.attackSpeed.ToString();
        rangeText.text = mobInfo.stat.attackRange.ToString();
        msText.text = mobInfo.stat.moveSpeed.ToString();
        wtText.text = mobInfo.waitingTime.ToString();
        ftText.text = mobInfo.faction;
        goldText.text = mobInfo.gold.ToString();
        magicText.text = mobInfo.magic.ToString();
        foodText.text = mobInfo.food.ToString();
    }

    public void Off()
    {
        gameObject.SetActive(false);
    }
}

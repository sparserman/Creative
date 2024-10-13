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
    // 스탯
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

        // 생성 시 세팅
        InitMobProfile();
        MobProfileSetting();

    }


    // 몹 프로필 표시
    public void MobProfileSetting()
    {
        // 배치 된 몹 표시
        for (int i = 0; i < gm.gi.specialMobList.Count; i++)
        {
            if (gm.gi.specialMobList[i].placement)
            {
                // 몹 프로필 생성
                GameObject go = Instantiate(Resources.Load("Prefabs/" + "PlacementMobImage") as GameObject);

                // Content안에 넣기
                go.transform.SetParent(placementMobImageList.transform, false);

                // 정보 넣기
                go.GetComponent<Image>().sprite = gm.gi.specialMobList[i].sprite;
                go.GetComponent<PlacementMobImage>().mobInfo = gm.gi.specialMobList[i];

                go.GetComponent<PlacementMobImage>().mobWindow = this;

                // 생성된 오브젝트 리스트에 추가 (나중에 지우려고)
                profileList.Add(go);
            }
        }

        // 가지고있는 남은 애들 표시
        for (int i = 0; i < gm.gi.specialMobList.Count; i++)
        {
            if (gm.gi.specialMobList[i].having && !gm.gi.specialMobList[i].placement)
            {
                // 몹 프로필 생성
                GameObject go = Instantiate(Resources.Load("Prefabs/" + "MobProfile") as GameObject);
                MobProfile mobProfile = go.GetComponent<MobProfile>();

                // Content안에 넣기
                mobProfile.transform.SetParent(mobProfileContent.transform, false);

                // 정보 세팅 (이미지, 이름, 레벨, 배치상태)
                mobProfile.image.sprite = gm.gi.specialMobList[i].sprite;
                mobProfile.mobName.text = gm.gi.specialMobList[i].mobName;
                mobProfile.level.text = gm.gi.specialMobList[i].stat.level.ToString();
                mobProfile.mobInfo = gm.gi.specialMobList[i];

                mobProfile.mobWindow = this;

                // 생성된 오브젝트 리스트에 추가 (나중에 지우려고)
                profileList.Add(go);
            }
        }
    }

    // 모두 초기화
    public void InitMobProfile()
    {
        if(profileList.Count > 0)
        {
            // 다 지워버리기
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

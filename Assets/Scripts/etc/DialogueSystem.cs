using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum DialogDBType
{
    Tutorial = 0
}

[System.Serializable]
public struct Speaker
{
    public GameObject dialogObj;          // 상위 오브젝트
    public Image speakerImage;              // 이미지칸
    public Image dialogImage;               // 대화창 UI
    public TextMeshProUGUI nameText;        // 이름칸
    public TextMeshProUGUI dialogText;      // 대사칸
    public GameObject endObj;             // 대사 완료 후 나오는 오브젝트
}

[System.Serializable]
public struct DialogData
{
    public string name;         // 이름
    [TextArea]
    public string dialog;       // 대사
}

public class DialogueSystem : MonoBehaviour
{
    GameManager gm;

    public int branch;

    [SerializeField]
    public DialogDB dialogDB;
    public DialogDBType typeDB;

    [SerializeField]
    public List<Speaker> speakers;      // 대화에 참여중인 캐릭터들 UI

    [SerializeField]
    public List<DialogData> dialogs;    // 대사 목록

    public bool isFirst = true;     // 첫 시작 체크용

    public int curDialogIndex = -1;         // 현재 대사 순번
    public int curSpeakerIndex = 0;         // 화자 순번

    public float typingSpeed = 0.1f;        // 텍스트 재생 속도
    public bool isTyping = false;           // 텍스트 재생중인지

    public GameObject dialogPanel;                 // 대화 패널

    public List<string> DBNameList;         // 등장하는 인물 이름

    private void Awake()
    {
        gm = GameManager.GetInstance();

        List<DialogDBEntity> listDB = null;
        // DB 정보 세팅
        switch (typeDB)
        {
            case DialogDBType.Tutorial:
                listDB = dialogDB.Tutorial;
                break;
        }

        if (listDB != null)
        {
            // 리스트안을 전부 비우고
            dialogs.Clear();

            // DB 정보 입력
            for (int i = 0; i < listDB.Count; i++)
            {
                if (listDB[i].branch == branch)
                {
                    DialogData data = new DialogData();
                    data.name = listDB[i].name;
                    data.dialog = listDB[i].dialog;
                    dialogs.Add(data);

                    NameListAdd(listDB[i].name);
                }
            }
        }
    }

    void NameListAdd(string p_name)
    {
        // 같은 이름이 없다면
        bool flag = true;
        for (int i = 0; i < DBNameList.Count; i++)
        {
            if (p_name == DBNameList[i])
            {
                flag = false;
            }
        }
        
        // 리스트에 추가
        if(flag)
        {
            DBNameList.Add(p_name);
        }
    }

    public void CharacterSetting(string p_name)
    {
        // 패널 켜기
        dialogPanel.SetActive(true);

        // 시간 멈추기
        gm.timerOn = false;

        for (int i = 0; i < gm.gi.managerList.Count;i++)
        {
            // 관리자의 이름을 받아와서 비교
            if (gm.gi.managerList[i].managerName == p_name)
            {
                // 중복 화자가 없다면
                bool flag = true;
                for (int j = 0; j < speakers.Count; j++)
                {
                    if (gm.gi.managerList[i].managerName == speakers[j].nameText.text)
                    {
                        flag = false;
                    }
                }

                // 화자 오브젝트 생성
                if (flag)
                {
                    // 대화창 생성
                    GameObject go = Instantiate(Resources.Load("Prefabs/" + "DialogObj") as GameObject);
                    go.transform.SetParent(dialogPanel.transform, false);
                    // 화자의 UI 정보 입력
                    Speaker speaker = go.GetComponent<DialogCharacter>().speaker;
                    speaker.nameText.text = gm.gi.managerList[i].managerName;
                    speaker.speakerImage.sprite = gm.gi.managerList[i].illust;

                    // 리스트 추가
                    speakers.Add(speaker);
                }
            }
        }
        
    }

    void Setup()
    {
        for(int i = 0; i < speakers.Count; i++)
        {
            // 이미지를 제외한 모든 대화 UI 끄기
            SetActiveDialogUI(speakers[i], false);

            if(speakers.Count >= 2)
            {
                speakers[i].dialogObj.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
            }
        }
    }

    public bool UpdateDialog()
    {
        if (isFirst)
        {
            // 대화 UI창 준비
            Setup();

            SetNextDialog();

            isFirst = false;
        }

        if(Input.GetMouseButtonDown(0))
        {
            // 텍스트 재생중에는 누르면 스킵
            if (isTyping)
            {
                isTyping = false;

                // 재생 정지
                StopCoroutine(m_OnTypingTextCrt);
                m_OnTypingTextCrt = null;

                // 대사 전체 출력
                speakers[curSpeakerIndex].dialogText.text = dialogs[curDialogIndex].dialog;

                // 엔드 표시
                speakers[curSpeakerIndex].endObj.gameObject.SetActive(true);

                return false;
            }

            // 대사가 남아있으면 다음 대사로
            if(dialogs.Count > curDialogIndex + 1)
            {
                SetNextDialog();
            }
            // 없으면 모든 오브젝트 비활성화
            else
            {
                int n = speakers.Count;
                for (int i = 0; i < n; i++)
                {
                    SetActiveDialogUI(speakers[i], false);
                    // 오브젝트 삭제
                    speakers[i].dialogObj.GetComponent<Animator>().SetTrigger("Off");
                }
                // 후 리스트 비우기
                speakers.Clear();

                return true;
            }
        }

        return false;
    }

    void SetNextDialog()
    {
        // 이전 화자 UI Off
        SetActiveDialogUI(speakers[curSpeakerIndex], false);

        // 화자 변경
        curDialogIndex++;
        
        //curSpeakerIndex = dialogs[curDialogIndex].speakerIndex;

        // 화자의 이름을 확인해서 그에 맞는 speakers 배열에 있는 index 번호 넣기
        for(int i = 0; i < speakers.Count; i++)
        {
            if(dialogs[curDialogIndex].name == speakers[i].nameText.text)
            {
                curSpeakerIndex = i;
            }
        }

        // 현재 화자 UI On
        SetActiveDialogUI(speakers[curSpeakerIndex], true);
        // UI 설정
        speakers[curSpeakerIndex].nameText.text = dialogs[curDialogIndex].name;

        //speakers[curSpeakerIndex].dialogText.text = dialogs[curDialogIndex].dialog;
        if (m_OnTypingTextCrt != null)
        {
            StopCoroutine(m_OnTypingTextCrt);
            m_OnTypingTextCrt = null;
        }
        m_OnTypingTextCrt = StartCoroutine(OnTypingTextCrt());
    }

    void SetActiveDialogUI(Speaker p_speaker, bool p_flag)
    {
        // 이미지를 제외한 모든 대화 UI 끄기
        p_speaker.dialogImage.gameObject.SetActive(p_flag);
        p_speaker.nameText.gameObject.SetActive(p_flag);
        p_speaker.dialogText.gameObject.SetActive(p_flag);
        p_speaker.endObj.gameObject.SetActive(p_flag);

        // 대사 종료시 숨김
        p_speaker.endObj.SetActive(false);

        // 이미지 흐리게 만들기
        Color color = p_speaker.speakerImage.color;
        if (p_flag)
        {
            color = new Color32(150, 255, 170, 240);
        }
        else
        {
            color = new Color32(50, 90, 60, 240);
        }
        p_speaker.speakerImage.color = color;
    }

    Coroutine m_OnTypingTextCrt = null;
    IEnumerator OnTypingTextCrt()
    {
        int index = 0;

        isTyping = true;

        // 글자수만큼 반복
        while(index <= dialogs[curDialogIndex].dialog.Length)
        {
            // index만큼 문자열을 잘라서 입력 
            speakers[curSpeakerIndex].dialogText.text = dialogs[curDialogIndex].dialog.Substring(0, index);

            index++;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        // 엔드 표시
        speakers[curSpeakerIndex].endObj.gameObject.SetActive(true);
    }
}

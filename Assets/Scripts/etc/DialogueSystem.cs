using System.Collections;
using System.Collections.Generic;
using System.Data;
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
    public GameObject dialogObj;          // ���� ������Ʈ
    public Image speakerImage;              // �̹���ĭ
    public Image dialogImage;               // ��ȭâ UI
    public TextMeshProUGUI nameText;        // �̸�ĭ
    public TextMeshProUGUI dialogText;      // ���ĭ
    public GameObject endObj;             // ��� �Ϸ� �� ������ ������Ʈ
}

[System.Serializable]
public struct DialogData
{
    public string name;         // �̸�
    [TextArea]
    public string dialog;       // ���
}

public class DialogueSystem : MonoBehaviour
{
    GameManager gm;

    public int branch;

    [SerializeField]
    public DialogDB dialogDB;
    public DialogDBType typeDB;

    [SerializeField]
    public List<Speaker> speakers;      // ��ȭ�� �������� ĳ���͵� UI

    [SerializeField]
    public List<DialogData> dialogs;    // ��� ���

    public bool isFirst = true;     // ù ���� üũ��

    public int curDialogIndex = -1;         // ���� ��� ����
    public int curSpeakerIndex = 0;         // ȭ�� ����

    public float typingSpeed = 0.1f;        // �ؽ�Ʈ ��� �ӵ�
    public bool isTyping = false;           // �ؽ�Ʈ ���������

    GameObject dialogPanel;                 // ��ȭ �г�

    public List<string> DBNameList;         // �����ϴ� �ι� �̸�

    private void Awake()
    {
        gm = GameManager.GetInstance();
        dialogPanel = GameObject.Find("DialogPanel");

        List<DialogDBEntity> listDB = null;
        // DB ���� ����
        switch (typeDB)
        {
            case DialogDBType.Tutorial:
                listDB = dialogDB.Tutorial;
                break;
        }

        if (listDB != null)
        {
            // ����Ʈ���� ���� ����
            dialogs.Clear();

            // DB ���� �Է�
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
        // ���� �̸��� ���ٸ�
        bool flag = true;
        for (int i = 0; i < DBNameList.Count; i++)
        {
            if (p_name == DBNameList[i])
            {
                flag = false;
            }
        }
        
        // ����Ʈ�� �߰�
        if(flag)
        {
            DBNameList.Add(p_name);
        }
    }

    public void CharacterSetting(string p_name)
    {
        for(int i = 0; i < gm.gi.managerList.Count;i++)
        {
            // �������� �̸��� �޾ƿͼ� ��
            if (gm.gi.managerList[i].managerName == p_name)
            {
                // �ߺ� ȭ�ڰ� ���ٸ�
                bool flag = true;
                for (int j = 0; j < speakers.Count; j++)
                {
                    if (gm.gi.managerList[i].managerName == speakers[j].nameText.text)
                    {
                        flag = false;
                    }
                }

                // ȭ�� ������Ʈ ����
                if (flag)
                {
                    // ��ȭâ ����
                    GameObject go = Instantiate(Resources.Load("Prefabs/" + "DialogObj") as GameObject);
                    go.transform.SetParent(dialogPanel.transform, false);
                    // ȭ���� UI ���� �Է�
                    Speaker speaker = go.GetComponent<DialogCharacter>().speaker;
                    speaker.nameText.text = gm.gi.managerList[i].managerName;
                    speaker.speakerImage.sprite = gm.gi.managerList[i].illust;

                    // ����Ʈ �߰�
                    speakers.Add(speaker);
                }
            }
        }
        
    }

    void Setup()
    {
        for(int i = 0; i < speakers.Count; i++)
        {
            // �̹����� ������ ��� ��ȭ UI ����
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
            // ��ȭ UIâ �غ�
            Setup();

            SetNextDialog();

            isFirst = false;
        }

        if(Input.GetMouseButtonDown(0))
        {
            // �ؽ�Ʈ ����߿��� ������ ��ŵ
            if (isTyping)
            {
                isTyping = false;

                // ��� ����
                StopCoroutine(m_OnTypingTextCrt);
                m_OnTypingTextCrt = null;

                // ��� ��ü ���
                speakers[curSpeakerIndex].dialogText.text = dialogs[curDialogIndex].dialog;

                // ���� ǥ��
                speakers[curSpeakerIndex].endObj.gameObject.SetActive(true);

                return false;
            }

            // ��簡 ���������� ���� ����
            if(dialogs.Count > curDialogIndex + 1)
            {
                SetNextDialog();
            }
            // ������ ��� ������Ʈ ��Ȱ��ȭ
            else
            {
                int n = speakers.Count;
                for (int i = 0; i < n; i++)
                {
                    SetActiveDialogUI(speakers[i], false);
                    // ������Ʈ ����
                    speakers[i].dialogObj.GetComponent<Animator>().SetTrigger("Off");
                }
                // �� ����Ʈ ����
                speakers.Clear();

                return true;
            }
        }

        return false;
    }

    void SetNextDialog()
    {
        // ���� ȭ�� UI Off
        SetActiveDialogUI(speakers[curSpeakerIndex], false);

        // ȭ�� ����
        curDialogIndex++;
        
        //curSpeakerIndex = dialogs[curDialogIndex].speakerIndex;

        // ȭ���� �̸��� Ȯ���ؼ� �׿� �´� speakers �迭�� �ִ� index ��ȣ �ֱ�
        for(int i = 0; i < speakers.Count; i++)
        {
            if(dialogs[curDialogIndex].name == speakers[i].nameText.text)
            {
                curSpeakerIndex = i;
            }
        }

        // ���� ȭ�� UI On
        SetActiveDialogUI(speakers[curSpeakerIndex], true);
        // UI ����
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
        // �̹����� ������ ��� ��ȭ UI ����
        p_speaker.dialogImage.gameObject.SetActive(p_flag);
        p_speaker.nameText.gameObject.SetActive(p_flag);
        p_speaker.dialogText.gameObject.SetActive(p_flag);
        p_speaker.endObj.gameObject.SetActive(p_flag);

        // ��� ����� ����
        p_speaker.endObj.SetActive(false);

        // �̹��� �帮�� �����
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

        // ���ڼ���ŭ �ݺ�
        while(index <= dialogs[curDialogIndex].dialog.Length)
        {
            // index��ŭ ���ڿ��� �߶� �Է� 
            speakers[curSpeakerIndex].dialogText.text = dialogs[curDialogIndex].dialog.Substring(0, index);

            index++;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        // ���� ǥ��
        speakers[curSpeakerIndex].endObj.gameObject.SetActive(true);
    }
}
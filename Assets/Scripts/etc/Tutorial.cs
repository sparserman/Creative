using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public List<DialogueSystem> dialogSystemList;
    public int index;

    public int selectBranch = 1000;

    public bool nextChapter = false;

    IEnumerator Start()
    {
        // dialogueSystem���� Tutorial�� �����ϱ����� ����
        for(int i = 0 ; i < dialogSystemList.Count; i++)
        {
            dialogSystemList[i].SetDialogController(gameObject);
        }

        // ��� ��ȭ�� ���������� ����
        while(index < dialogSystemList.Count)
        {
            for (int i = 0; i < dialogSystemList[index].DBNameList.Count; i++)
            {
                dialogSystemList[index].CharacterSetting(dialogSystemList[index].DBNameList[i]);
            }
            // ù ��� ���� �� ��� ��� ���� ������ ���
            yield return new WaitUntil(() => dialogSystemList[index].UpdateDialog());

            int loopNum = PlusIndex();

            while (true)
            {
                if(index >= dialogSystemList.Count)
                {
                    // ������ ��簡 ������ 1�� �� (�г� ������� �ִϸ��̼� ���ð�)
                    yield return new WaitForSeconds(1f);
                    // �гβ���
                    dialogSystemList[0].dialogPanel.SetActive(false);
                    // �ð� �帣���ϱ�
                    GameManager.GetInstance().timerOn = true;
                    break;
                }

                // ���� �귻ġ ��ȭ ���� ���
                yield return new WaitUntil(() => dialogSystemList[index - loopNum].UpdateDialog());

                break;
            }
        }
    }

    // branch���� 1000������ �̵��ϱ�
    int PlusIndex()
    {
        int loop = 1;
        while (index < dialogSystemList.Count)
        {
            // ���� ��ȭ�� �������� �������ϰų� ���� ��ȭ�� �ƴ϶��
            if (dialogSystemList[index].branch == selectBranch)
            {
                selectBranch -= selectBranch % 1000;
                selectBranch += 1000;
                return loop;
            }
            else if (index + 1 < dialogSystemList.Count)
            {
                if (dialogSystemList[++index].branch == selectBranch)
                {
                    selectBranch -= selectBranch % 1000;
                    selectBranch += 1000;
                    return loop;
                }
            }
            else
            {
                index++;
                return loop;
            }

            loop++;
        }

        return loop;
    }
}

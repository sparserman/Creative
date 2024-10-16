using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public List<DialogueSystem> dialogSystemList;
    public int index;

    public bool nextChapter = false;

    IEnumerator Start()
    {
        while(index < dialogSystemList.Count)
        {
            for (int i = 0; i < dialogSystemList[index].DBNameList.Count; i++)
            {
                dialogSystemList[index].CharacterSetting(dialogSystemList[index].DBNameList[i]);
            }
            // ù ��� ����
            yield return new WaitUntil(() => dialogSystemList[index].UpdateDialog());

            // ��簡 ������ 1�� ��
            yield return new WaitForSeconds(1f);
            // �гβ���
            dialogSystemList[index].dialogPanel.SetActive(false);
            // �ð� �帣���ϱ�
            GameManager.GetInstance().timerOn = true;

            index++;

            while(true)
            {
                if(index < dialogSystemList.Count)
                {
                    break;
                }

                if(nextChapter)
                {
                    nextChapter = false;
                    break;
                }

                yield return null;
            }
        }
    }


}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldInfo : MonoBehaviour
{
    GameManager gm;
    Animator anim;

    public TextMeshProUGUI worldName;    // ���� �̸�
    public Image worldImage;    // ���� �̹���
    public TextMeshProUGUI worldDescription; // ���� ����

    public Image managerImage;  // �Ŵ��� �̹���
    public TextMeshProUGUI managerName;  // �Ŵ��� �̸�
    public TextMeshProUGUI worldDetails; // ���� ��Ȳ

    public TextMeshProUGUI managerState; // �Ŵ��� ����
    public TextMeshProUGUI management;    // ������

    void Start()
    {
        anim = GetComponent<Animator>();   

        gm = GameManager.GetInstance();
        gm.goList.Add(gameObject);
    }

    void Update()
    {
        
    }

    public void DestroyWorldInfo()
    {
        Destroy(gameObject);
    }
}

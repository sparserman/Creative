using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class MobTab : MonoBehaviour
{
    Collider2D col;
    public LayerMask mask;

    public string nameText;
    public TextMeshProUGUI mobName;
    public TextMeshProUGUI level;
    public Image image;
    public TextMeshProUGUI hp;
    public TextMeshProUGUI ad;

    GameObject go;

    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        ClickCheck();
    }

    void ClickCheck()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Ray2D ray = new Ray2D(pos, Vector2.zero);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 1, mask);

        // null이 아니면서 본인 collider라면
        if (hit.collider != null)
        {
            if (hit.collider == col)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    go = Instantiate(Resources.Load("Prefabs/" + nameText) as GameObject);
                    go.GetComponent<Enemy>().spawnWaiting = true;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    Destroy(go);
                }
            }
            else if (hit.collider != col)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    go.GetComponent<Enemy>().spawnWaiting = false;
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {

            }
        }
    }
}

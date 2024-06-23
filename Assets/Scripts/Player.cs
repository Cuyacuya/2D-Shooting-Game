using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;

    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>(); //���� �ʱ�ȭ�ϴ� ���
    }
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal"); //GetAxisRaw�� -1, 0, 1�� ���� ����
        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1)) h = 0;
        float v = Input.GetAxisRaw("Vertical");
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1)) v = 0;

        Vector3 curPos = transform.position; //transform�� ��� ����� �޷��ִ� �⺻
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime; //������ �̵��� �ƴ� transform�� ���� �̵��� deltaTime�� �ʼ�!

        transform.position = curPos + nextPos;

        if((Input.GetButtonDown("Horizontal")) || (Input.GetButtonUp("Horizontal")))
        {
            anim.SetInteger("Input", (int)h);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
            }
        }
    }
}
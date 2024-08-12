using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;

    public GameObject bulletObjA; //총알 프리팹을 저장할 변수
    public GameObject bulletObjB; //총알 프리팹을 저장할 변수
    public GameObject boomEffect; //궁극기 변수 


    public int life;
    public int score;
    public float speed;
    public int boom;
    public int maxBoom;
    public int power;
    public int maxPower;
    public float maxShotDelay; //장전하는데 거리는 시간
    public float curShotDelay; //실제 딜레이

    public GameManager gameManager;
    public ObjectManager objectManager;
    public bool isHit;
    public bool isBoomTime;
    public bool[] joyControl; //어느 버튼이 눌렸는지
    public bool isControl; //눌렸는지 화인
    public bool isButtonA;
    public bool isButtonB;


    public bool isRespawnTime;
    SpriteRenderer spriteRenderer;

    Animator anim;


    private void Awake()
    {
        anim = GetComponent<Animator>(); //변수 초기화하는 방법
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        Unbeatable();
        Invoke("Unbeatable", 3);
    }

    void Unbeatable()
    {
        isRespawnTime = !isRespawnTime;

        if(isRespawnTime) //#. 무적 타임
        {
            spriteRenderer.color = new Color(1, 1, 1, 1);
        }
        else
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        }
    }

    void Update()
    {
        Move();
        Fire();
        Boom();
        Reload();
    }

    public void JoyPanel(int type)
    {
        for(int index = 0; index<9; index++)
        {
            joyControl[index] = index == type;
        }
    }

    public void Joydown()
    {
        isControl = true;
    }

    public void JoyUp()
    {
        isControl = false;
    }

    void Move()
    {
        //#. keyboard Control value
        float h = Input.GetAxisRaw("Horizontal"); //GetAxisRaw는 -1, 0, 1의 값만 들어옴
        float v = Input.GetAxisRaw("Vertical");

        //#. Joy Control Value
        if(joyControl[0]) { h = -1; v = 1; }
        if(joyControl[1]) { h = 0; v = 1; }
        if(joyControl[2]) { h = 1; v = 1; }
        if(joyControl[3]) { h = -1; v = 0; }
        if(joyControl[4]) { h = 0; v = 0; }
        if(joyControl[5]) { h = 1; v = 0; }
        if(joyControl[6]) { h = -1; v = -1; }
        if(joyControl[7]) { h = 0; v = -1; }
        if(joyControl[8]) { h = 1; v = -1; }

        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1) || !isControl) h = 0;
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1) || !isControl) v = 0;

        Vector3 curPos = transform.position; //transform은 모노 비헤비어에 달려있는 기본
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime; //물리적 이동이 아닌 transform을 통한 이동은 deltaTime이 필수!

        transform.position = curPos + nextPos;

        if ((Input.GetButtonDown("Horizontal")) || (Input.GetButtonUp("Horizontal")))
        {
            anim.SetInteger("Input", (int)h);
        }
    }

    public void ButtonADown()
    {
        isButtonA = true;
    }
    public void ButtonAUp()
    {
        isButtonA = false;
    }
    public void buttonBDown()
    {
        isButtonB = true;
    }
    void Fire()
    {
        //if (!Input.GetButton("Fire1")) //Fire1 이라는 버튼을 누르지않는다면(!) 아무것도 안함
        //return;

        if (!isButtonA) return;

        if (curShotDelay < maxShotDelay) //현재의 딜레이가 장전 시간보다 작으면 아무것도 안함
            return;

        switch (power)
        {
            case 1:
                GameObject bullet = objectManager.MakeObj("BulletPlayerA");
                bullet.transform.position = transform.position;
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;

            case 2:
                GameObject bulletR = objectManager.MakeObj("BulletPlayerA");
                bulletR.transform.position = transform.position + Vector3.right * 0.1f;
                GameObject bulletL = objectManager.MakeObj("BulletPlayerA");
                bulletL.transform.position = transform.position + Vector3.left * 0.1f;
               
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 3:
                GameObject bulletRR = objectManager.MakeObj("BulletPlayerA");
                bulletRR.transform.position = transform.position + Vector3.right * 0.35f;
                GameObject bulletCC = objectManager.MakeObj("BulletPlayerB");
                bulletCC.transform.position = transform.position;
                GameObject bulletLL = objectManager.MakeObj("BulletPlayerA");
                bulletLL.transform.position = transform.position + Vector3.left * 0.35f;

                Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
                rigidRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

                break;
        }
        curShotDelay = 0;       // 총알을 쏜 다음에는 딜레이 변수 0으로 초기화
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    void Boom()
    {
        //if (!Input.GetButton("Fire2")) //Fire2 이라는 버튼을 누르지않는다면(!) 아무것도 안함(마우스 우측버튼)
        //return;

        if (!isButtonB) return;

        if (isBoomTime) //Fire1 이라는 버튼을 누르지않는다면(!) 아무것도 안함
            return;

        if (boom == 0) return;

        boom -= 1;
        isBoomTime = true;
        gameManager.UpdateBoomIcon(boom);

        //#1. Effect visible
        boomEffect.SetActive(true);
        Invoke("OffBoomEffect", 4f);

        //#2. Remove Enemy (많은 오브젝트(적)을 없애야하므로 배열 사용)
        GameObject[] enemiesL = objectManager.GetPool("EnemyL");
        GameObject[] enemiesM = objectManager.GetPool("EnemyM");
        GameObject[] enemiesS = objectManager.GetPool("EnemyS");

        for (int i = 0; i < enemiesL.Length; i++)
        {
            if (enemiesL[i].activeSelf)
            {
                Enemy enemyLogic = enemiesL[i].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        for (int i = 0; i < enemiesM.Length; i++)
        {
            if (enemiesM[i].activeSelf)
            {
                Enemy enemyLogic = enemiesM[i].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        for (int i = 0; i < enemiesS.Length; i++)
        {
            if (enemiesS[i].activeSelf)
            {
                Enemy enemyLogic = enemiesS[i].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        //#3. Remove Enemy Bullet
        GameObject[] bulletsA = objectManager.GetPool("BulletEnemyA");
        GameObject[] bulletsB = objectManager.GetPool("BulletEnemyB");
        for (int i = 0; i < bulletsA.Length; i++)
        {
            if (bulletsA[i].activeSelf)
            {
                bulletsA[i].SetActive(false);
            }
        }
        for (int i = 0; i < bulletsB.Length; i++)
        {
            if (bulletsB[i].activeSelf)
            {
                bulletsB[i].SetActive(false);
            }
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
        //피격 이벤트
        else if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            if (!isRespawnTime)
                return;

            if (isHit) return;
            
            isHit = true;
            life--;
            gameManager.UpdateLifeIcon(life);
            gameManager.CallExplosion(transform.position, "P");

            if(life == 0)
            {
                gameManager.GameOver();
            }
            else
            {
                gameManager.RespawnPlayer();
            }

            gameObject.SetActive(false); //피격 시 player 비활성화,
            collision.gameObject.SetActive(false);
        }
        //item 충돌
        else if (collision.gameObject.tag == "Item")
        {   
            Item item = collision.gameObject.GetComponent<Item>();
            switch(item.type)
            {
                case "Coin":
                    score += 1000;
                    break;
                case "Power":
                    if (power == maxPower) score += 500;
                    else power++;
                    break;
                case "Boom":
                    if (boom == maxBoom) score += 500;
                    else
                    {
                        boom++;
                        gameManager.UpdateBoomIcon(boom);
                    }
                    break;
            }
            collision.gameObject.SetActive(false); //먹은 아이템 삭제
        }
    }

    void OffBoomEffect()
    {
        boomEffect.SetActive(false);
        isBoomTime = false;
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

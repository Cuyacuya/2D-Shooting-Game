using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SocialPlatforms.Impl;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public int enemyScore;
    public float speed;
    public float health;
    public Sprite[] sprites; //스프라이트가 총 2개이므로 배열로 생성

    public float maxShotDelay; //장전하는데 거리는 시간
    public float curShotDelay; //실제 딜레이

    public GameObject bulletObjA; //총알 프리팹을 저장할 변수
    public GameObject bulletObjB; //총알 프리팹을 저장할 변수
    public GameObject itemCoin;
    public GameObject itemPower;
    public GameObject itemBoom;
    public GameObject player;
    public ObjectManager objectManager;
    public GameManager gameManager;

    SpriteRenderer spriteRenderer; //스프라이트를 바꿔주기위해

    Animator anim;

    public int patternIndex;
    public int curPatternCount;
    public int [] maxPatternCount;


    //변수 초기화
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if(enemyName == "B")
        {
            anim = GetComponent<Animator>();
        }
    }

    void OnEnable() //컴포넌트가 활성화 될 때 호출되는 생명주기 함수
    {
        switch (enemyName)
        {
            case "L":
                health = 30;
                break;
            case "M":
                health = 12;
                break;
            case "S":
                health = 3;
                break;
            case "B":
                health = 60;
                Invoke("Stop", 2);
                break;
        }
    }

    void Stop() //보스 정지
    {
        if (!gameObject.activeSelf)
            return;

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        Invoke("Think", 2);
    }

    void Think()
    {
        patternIndex = patternIndex == 3 ? 0 : patternIndex + 1;
        curPatternCount = 0; //패턴 변경 시, 패턴 실행 횟수 변수 초기화

        switch(patternIndex)
        {
            case 0:
                FireFoward();
                break;
            case 1:
                FireShot();
                break;
            case 2:
                FireArc();
                break;
            case 3:
                FireAround();
                break;
        }
    }

    void FireFoward()
    {
        //#. Fire 4 bullet Foward
        GameObject bulletR = objectManager.MakeObj("BulletBossA");
        GameObject bulletRR = objectManager.MakeObj("BulletBossA");
        GameObject bulletL = objectManager.MakeObj("BulletBossA");
        GameObject bulletLL = objectManager.MakeObj("BulletBossA");
        bulletR.transform.position = transform.position + Vector3.right * 0.3f;
        bulletRR.transform.position = transform.position + Vector3.right * 0.45f;
        bulletL.transform.position = transform.position + Vector3.left * 0.3f;
        bulletLL.transform.position = transform.position + Vector3.left * 0.45f;

        Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();

        rigidR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidRR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidLL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);

        Debug.Log("foward");
        curPatternCount++;

        if(curPatternCount < maxPatternCount[patternIndex])
        {
            Invoke("FireFoward", 2);
        }
        else
        {
            Invoke("Think", 3);
        }
    }

    void FireShot()
    {
        //#. Fire Shotgun
        for (int index = 0; index < 5; index++)
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyB");

            bullet.transform.position = transform.position;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = player.transform.position - transform.position;
            Vector2 ranVec = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0f, 2f));
            dirVec += ranVec;
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        }
        Debug.Log("shot");
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
        {
            Invoke("FireShot", 3.5f);
        }
        else
        {
            Invoke("Think", 3);
        }
    }

    void FireArc()
    {
        //#. Fire arc continue Fire

        GameObject bullet = objectManager.MakeObj("BulletEnemyA");

        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity; //회전 초기화(Quaternion)

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 10 * curPatternCount/ maxPatternCount[patternIndex]), -1);
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);

        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
        {
            Invoke("FireArc", 0.15f);
        }
        else
        {
            Invoke("Think", 3);
        }
    }
    void FireAround()
    {
        //#. Fire Around
        int roundNumA = 50;
        int roundNumB = 40;
        int roundNum = curPatternCount%2 == 0 ? roundNumA : roundNumB;

        for (int index = 0; index < roundNumA; index++)
        {
            GameObject bullet = objectManager.MakeObj("BulletBossB");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity; //회전 초기화(Quaternion)

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            //총알 원형
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * index / roundNum), Mathf.Sin(Mathf.PI * 2 * index / roundNum));
            rigid.AddForce(dirVec.normalized * 2, ForceMode2D.Impulse);

            //총알 나가는 방향으로 회전
            Vector3 rotVec = Vector3.forward * 360 * index / roundNum + Vector3.forward * 90;
            bullet.transform.Rotate(rotVec);
        }

        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
        {
            Invoke("FireAround", 0.7f);
        }
        else
        {
            Invoke("Think", 3);
        }
    }

    void Update()
    {
        if (enemyName == "B")
        {
            return;
        }

        Fire();
        Reload();
    }

    void Fire()
    {
        if (curShotDelay < maxShotDelay)
            return;

        if (enemyName == "S")
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyA");

            bullet.transform.position = transform.position;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector3 dirVec = player.transform.position - transform.position;
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        }
        else if (enemyName == "L")
        {
            GameObject bulletR = objectManager.MakeObj("BulletEnemyB");
            GameObject bulletL = objectManager.MakeObj("BulletEnemyB");
            bulletR.transform.position = transform.position + Vector3.right * 0.3f;
            bulletL.transform.position = transform.position + Vector3.left * 0.3f;

            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();

            Vector3 dirVecR = player.transform.position - (transform.position + Vector3.right * 0.3f);
            Vector3 dirVecL = player.transform.position - (transform.position + Vector3.left * 0.3f);

            rigidR.AddForce(dirVecR.normalized * 4, ForceMode2D.Impulse);
            rigidL.AddForce(dirVecL.normalized * 4, ForceMode2D.Impulse);
        }

        curShotDelay = 0;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    public void OnHit(int dmg)
    {
        if(health <= 0)
            return;

        health -= dmg;
        //피격시 anim 적용
        if (enemyName == "B")
        {
            anim.SetTrigger("OnHit");
        }
        else
        {
            spriteRenderer.sprite = sprites[1];
            Invoke("ReturnSprite", 0.1f);
        }

        if (health <= 0)
        {
            Player playerLogic = player.GetComponent<Player>();
            playerLogic.score += enemyScore;

            //#.Random Ratio Item Drop
            int ran = enemyName == "B" ? 0 : Random.Range(0, 10);
            if (ran < 5)
            {
                Debug.Log("Not Item");
            }
            else if (ran < 8)   // Coin 30%
            {
                GameObject itemCoin = objectManager.MakeObj("ItemCoin");
                itemCoin.transform.position = transform.position;
            }
            else if (ran < 9)   // Power 10%
            {
                GameObject itemPower = objectManager.MakeObj("ItemPower");
                itemPower.transform.position = transform.position;
            }
            else if (ran < 10)   // Boom 10%
            {
                GameObject itemBoom = objectManager.MakeObj("ItemBoom");
                itemBoom.transform.position = transform.position;
            }
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity; //Quaternion.identity : 기본 회전값 0
            gameManager.CallExplosion(transform.position, enemyName);

            //#. Boss Kill
            if (enemyName == "B") gameManager.StageEnd();
        }
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "BorderBullet" && enemyName != "B")
        {
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
        }
        else if(collision.gameObject.tag == "PlayerBullet")
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);

            collision.gameObject.SetActive(false);
        }
    }
}

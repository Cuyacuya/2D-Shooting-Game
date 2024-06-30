using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public float speed;
    public float health;
    public Sprite[] sprites; //스프라이트가 총 2개이므로 배열로 생성
    SpriteRenderer spriteRenderer; //스프라이트를 바꿔주기위해

    public float maxShotDelay; //장전하는데 거리는 시간
    public float curShotDelay; //실제 딜레이

    public GameObject bulletObjA; //총알 프리팹을 저장할 변수
    public GameObject bulletObjB; //총알 프리팹을 저장할 변수
    public GameObject player;
    

    //변수 초기화
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Fire();
        Reload();
    }
    void Fire()
    {
        if (curShotDelay < maxShotDelay) //현재의 딜레이가 장전 시간보다 작으면 아무것도 안함
            return;

        if(enemyName == "S")
        {
            GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector3 dirVec = player.transform.position - transform.position;//플레이어 위치- 내 위치
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        }
        else if(enemyName == "L") {
            Vector3 dirVecR = player.transform.position - transform.position;//플레이어 위치- 내 위치
            GameObject bulletR = Instantiate(bulletObjB , transform.position + Vector3.right * 0.3f, transform.rotation);
            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
            rigidR.AddForce(dirVecR.normalized * 4, ForceMode2D.Impulse); //dirVecR.normalized : 단위 벡터로 만들기 위함

            Vector3 dirVecL = player.transform.position - transform.position;
            GameObject bulletL = Instantiate(bulletObjB, transform.position + Vector3.left * 0.3f, transform.rotation);
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
            rigidL.AddForce(dirVecL.normalized * 4, ForceMode2D.Impulse); //dirVecL.normalized : 단위 벡터로 만들기 위함
        }


        curShotDelay = 0; //총알을 쏜 뒤 딜레이 변수 0으로 초기화
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    void OnHit(int dmg)
    {
        health -= dmg;
        spriteRenderer.sprite = sprites[1];
        Invoke("ReturnSprite", 0.1f); 

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "BorderBullet")
        {
            Destroy(gameObject);
        }
        else if(collision.gameObject.tag == "PlayerBullet")
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);

            Destroy(collision.gameObject);
        }
    }
}

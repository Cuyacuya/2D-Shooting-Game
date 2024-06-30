using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public float speed;
    public float health;
    public Sprite[] sprites; //��������Ʈ�� �� 2���̹Ƿ� �迭�� ����
    SpriteRenderer spriteRenderer; //��������Ʈ�� �ٲ��ֱ�����

    public float maxShotDelay; //�����ϴµ� �Ÿ��� �ð�
    public float curShotDelay; //���� ������

    public GameObject bulletObjA; //�Ѿ� �������� ������ ����
    public GameObject bulletObjB; //�Ѿ� �������� ������ ����
    public GameObject player;
    

    //���� �ʱ�ȭ
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
        if (curShotDelay < maxShotDelay) //������ �����̰� ���� �ð����� ������ �ƹ��͵� ����
            return;

        if(enemyName == "S")
        {
            GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector3 dirVec = player.transform.position - transform.position;//�÷��̾� ��ġ- �� ��ġ
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        }
        else if(enemyName == "L") {
            Vector3 dirVecR = player.transform.position - transform.position;//�÷��̾� ��ġ- �� ��ġ
            GameObject bulletR = Instantiate(bulletObjB , transform.position + Vector3.right * 0.3f, transform.rotation);
            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
            rigidR.AddForce(dirVecR.normalized * 4, ForceMode2D.Impulse); //dirVecR.normalized : ���� ���ͷ� ����� ����

            Vector3 dirVecL = player.transform.position - transform.position;
            GameObject bulletL = Instantiate(bulletObjB, transform.position + Vector3.left * 0.3f, transform.rotation);
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
            rigidL.AddForce(dirVecL.normalized * 4, ForceMode2D.Impulse); //dirVecL.normalized : ���� ���ͷ� ����� ����
        }


        curShotDelay = 0; //�Ѿ��� �� �� ������ ���� 0���� �ʱ�ȭ
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

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] enemyObjs; //�� ����� 3���� �����ϱ����� GameObject�迭
    public Transform[] spawnPoints; //���� �����Ǵ� ��ġ 5���� �����ϱ����� �迭

    public float maxSpawnDelay;
    public float curSpawnDelay;

    public GameObject player;

    private void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if(curSpawnDelay > maxSpawnDelay )
        {
            SpawnEnemy();
            maxSpawnDelay = Random.Range( 0.5f, 3f); //0.5 ~ 3�� ������ ������ �ð� ������ �� ����
            curSpawnDelay = 0;
        }
    }

    void SpawnEnemy()
    {
        int ranEnemy = Random.Range(0, 3);
        int ranPoint = Random.Range(0, 9);
        GameObject enemy = Instantiate(enemyObjs[ranEnemy], spawnPoints[ranPoint].position, spawnPoints[ranPoint].rotation); //������ ���� ������ ��ġ���� ����

        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player; //�� ���� ���Ŀ� �÷��̾� ���� �Ѱ��ֱ�

        if(ranPoint == 5 || ranPoint == 6)//�� -> ��
        {
            rigid.velocity = new Vector2(enemyLogic.speed * - 1, -1); //�ӵ��� �ֱ�
        }
        else if (ranPoint == 7 || ranPoint == 8)//�� -> ��
        {
            rigid.velocity = new Vector2(enemyLogic.speed, -1); 
        }
        else
        {
            rigid.velocity = new Vector2(0, enemyLogic.speed * -1); //�� -> ��
        }
    }

    public void RespawnPlayer()
    {
        Invoke("RespawnPlayerEx", 2f);
    }

    void RespawnPlayerEx()
    {
        player.transform.position = Vector3.down * 4f;
        player.SetActive(true);
    }
}

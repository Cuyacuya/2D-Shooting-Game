using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] enemyObjs; //적 비행기 3개를 저장하기위한 GameObject배열
    public Transform[] spawnPoints; //적이 생성되는 위치 5곳을 저장하기위한 배열

    public float maxSpawnDelay;
    public float curSpawnDelay;

    public GameObject player;

    private void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if(curSpawnDelay > maxSpawnDelay )
        {
            SpawnEnemy();
            maxSpawnDelay = Random.Range( 0.5f, 3f); //0.5 ~ 3초 사이의 랜덤한 시간 값으로 적 생성
            curSpawnDelay = 0;
        }
    }

    void SpawnEnemy()
    {
        int ranEnemy = Random.Range(0, 3);
        int ranPoint = Random.Range(0, 9);
        GameObject enemy = Instantiate(enemyObjs[ranEnemy], spawnPoints[ranPoint].position, spawnPoints[ranPoint].rotation); //랜덤한 적을 랜덤한 위치에서 생성

        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player; //적 생성 직후에 플레이어 변수 넘겨주기

        if(ranPoint == 5 || ranPoint == 6)//오 -> 왼
        {
            rigid.velocity = new Vector2(enemyLogic.speed * - 1, -1); //속도를 주기
        }
        else if (ranPoint == 7 || ranPoint == 8)//왼 -> 오
        {
            rigid.velocity = new Vector2(enemyLogic.speed, -1); 
        }
        else
        {
            rigid.velocity = new Vector2(0, enemyLogic.speed * -1); //왼 -> 오
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

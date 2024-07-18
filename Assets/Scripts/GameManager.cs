using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI; //Text를 사용하기위함
using UnityEngine.SceneManagement; //Scen을 사용하기위해


public class GameManager : MonoBehaviour
{
    public string[] enemyObjs; //적 비행기 3개를 저장하기위한 GameObject배열
    public Transform[] spawnPoints; //적이 생성되는 위치 5곳을 저장하기위한 배열

    public float maxSpawnDelay;
    public float curSpawnDelay;

    public GameObject player;
    public Text scoreText;
    public Image[] lifeImage;
    public Image[] boomImage;
    public GameObject gameOverSet;
    public ObjectManager objectManager;

    void Awake()
    {
        enemyObjs = new string[] { "EnemyL", "EnemyM", "EnemyS" };
    }

    private void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if(curSpawnDelay > maxSpawnDelay )
        {
            SpawnEnemy();
            maxSpawnDelay = Random.Range(0.5f, 3f); //0.5 ~ 3초 사이의 랜덤한 시간 값으로 적 생성
            curSpawnDelay = 0;
        }

        //# UI Score Update
        Player playerLogic = player.GetComponent<Player>();
        scoreText.text = string.Format("{0:n0}", playerLogic.score); //자리수 나누기 (Format)
    }

    void SpawnEnemy()
    {
        int ranEnemy = Random.Range(0, 3);
        int ranPoint = Random.Range(0, 9);
        GameObject enemy = objectManager.MakeObj(enemyObjs[ranEnemy]);
        enemy.transform.position = spawnPoints[ranPoint].position;

        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player; //프리펩은 이미 Scene에 올라온 오브젝트에 접근 불가능 -> Enemy에서 바로 player를 받지 못함
                                    //=> 적 생성 후 플레이어 변수를 넘겨받음
        enemyLogic.objectManager = objectManager;

        if(ranPoint == 5 || ranPoint == 6)//오 -> 왼
        {
            enemy.transform.Rotate(Vector3.back * 45);
            rigid.velocity = new Vector2(enemyLogic.speed * - 1, -1); //속도를 주기
        }
        else if (ranPoint == 7 || ranPoint == 8)//왼 -> 오
        {
            enemy.transform.Rotate(Vector3.forward * 45);
            rigid.velocity = new Vector2(enemyLogic.speed, -1); 
        }
        else
        {
            rigid.velocity = new Vector2(0, enemyLogic.speed * -1); //왼 -> 오
        }
    }

    public void UpdateLifeIcon(int life)
    {
        for (int index = 0; index < 3; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 0);
        }

        for (int index = 0; index < life; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 1);
        }
    }

    public void UpdateBoomIcon(int boom)
    {
        for (int index = 0; index < 3; index++)
        {
            boomImage[index].color = new Color(1, 1, 1, 0);
        }

        for (int index = 0; index < boom; index++)
        {
            boomImage[index].color = new Color(1, 1, 1, 1);
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

        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isHit = false;
    }

    public void GameOver()
    {
        gameOverSet.SetActive(true);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    } 
}

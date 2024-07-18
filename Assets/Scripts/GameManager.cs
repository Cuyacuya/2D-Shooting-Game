using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI; //Text�� ����ϱ�����
using UnityEngine.SceneManagement; //Scen�� ����ϱ�����


public class GameManager : MonoBehaviour
{
    public string[] enemyObjs; //�� ����� 3���� �����ϱ����� GameObject�迭
    public Transform[] spawnPoints; //���� �����Ǵ� ��ġ 5���� �����ϱ����� �迭

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
            maxSpawnDelay = Random.Range(0.5f, 3f); //0.5 ~ 3�� ������ ������ �ð� ������ �� ����
            curSpawnDelay = 0;
        }

        //# UI Score Update
        Player playerLogic = player.GetComponent<Player>();
        scoreText.text = string.Format("{0:n0}", playerLogic.score); //�ڸ��� ������ (Format)
    }

    void SpawnEnemy()
    {
        int ranEnemy = Random.Range(0, 3);
        int ranPoint = Random.Range(0, 9);
        GameObject enemy = objectManager.MakeObj(enemyObjs[ranEnemy]);
        enemy.transform.position = spawnPoints[ranPoint].position;

        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player; //�������� �̹� Scene�� �ö�� ������Ʈ�� ���� �Ұ��� -> Enemy���� �ٷ� player�� ���� ����
                                    //=> �� ���� �� �÷��̾� ������ �Ѱܹ���
        enemyLogic.objectManager = objectManager;

        if(ranPoint == 5 || ranPoint == 6)//�� -> ��
        {
            enemy.transform.Rotate(Vector3.back * 45);
            rigid.velocity = new Vector2(enemyLogic.speed * - 1, -1); //�ӵ��� �ֱ�
        }
        else if (ranPoint == 7 || ranPoint == 8)//�� -> ��
        {
            enemy.transform.Rotate(Vector3.forward * 45);
            rigid.velocity = new Vector2(enemyLogic.speed, -1); 
        }
        else
        {
            rigid.velocity = new Vector2(0, enemyLogic.speed * -1); //�� -> ��
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

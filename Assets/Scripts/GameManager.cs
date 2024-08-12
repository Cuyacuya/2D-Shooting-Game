using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI; //Text를 사용하기위함
using UnityEngine.SceneManagement; //Scen을 사용하기위해
using System.IO;

public class GameManager : MonoBehaviour
{
    public int stage;
    public Animator stageAnim;
    public Animator clearAnim;
    public Animator fadeAnim;
    public Transform playerPos;

    public string[] enemyObjs; //적 비행기 3개를 저장하기위한 GameObject배열
    public Transform[] spawnPoints; //적이 생성되는 위치 5곳을 저장하기위한 배열

    public float nextSpawnDelay;
    public float curSpawnDelay;

    public GameObject player;
    public Text scoreText;
    public Image[] lifeImage;
    public Image[] boomImage;
    public GameObject gameOverSet;
    public ObjectManager objectManager;

    public List<Spawn> spawnList;
    public int spawnIndex;
    public bool spawnEnd;

    void Awake()
    {
        spawnList = new List<Spawn>();
        enemyObjs = new string[] { "EnemyS", "EnemyM", "EnemyL", "EnemyB" };
        StageStart();
    }

    public void StageStart()
    {
        //#. Stage UI Load
        stageAnim.SetTrigger("On");
        stageAnim.GetComponent<Text>().text = "Stage" + stage + "\nStart";
        clearAnim.GetComponent<Text>().text = "Stage" + stage + "\nClear";

        //#. Enemy Spawn File Read
        ReadSpawnFile();

        //#. Fade In
        fadeAnim.SetTrigger("In");
    }    
    public void StageEnd()
    {
        //#. Clear UI Load
        clearAnim.SetTrigger("On");

        //#. Fade Out
        fadeAnim.SetTrigger("Out");

        //#. Player Repos
        player.transform.position = playerPos.position;

        //#. Staage Increament
        stage++;
        if(stage > 2) Invoke("GameOver", 6);
        else Invoke("StageStart", 5);
    }

    void ReadSpawnFile()
    {
        //#1. 변수 초기화
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        //#2. 리스폰 파일 읽기
        TextAsset textFile = Resources.Load("Stage " + stage) as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        while (stringReader != null)
        {
            string line = stringReader.ReadLine();//텍스트 데이터를 한 줄씩 반환.(자동 줄 바꿈)

            if (line == null) break;

            //#3. 리스폰 데이터 생성
            Spawn spawnData = new Spawn();
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(",")[1];
            spawnData.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawnData);
        }
        //텍스트 파일 닫기
        stringReader.Close();

        //첫번째 스폰 딜레이 적용
        nextSpawnDelay = spawnList[0].delay;
    }


    private void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if(curSpawnDelay > nextSpawnDelay && !spawnEnd)
        {
            SpawnEnemy();
            curSpawnDelay = 0;
        }

        //# UI Score Update
        Player playerLogic = player.GetComponent<Player>();
        scoreText.text = string.Format("{0:n0}", playerLogic.score); //자리수 나누기 (Format)
    }

    void SpawnEnemy()
    {
        int enemyIndex = 0;
        switch (spawnList[spawnIndex].type)
        {
            case "S":
                enemyIndex = 0;
                break;
            case "M":
                enemyIndex = 1;
                break;
            case "L":
                enemyIndex = 2;
                break;
            case "B":
                enemyIndex = 3;
                break;
        }
        int enemyPoint = spawnList[spawnIndex].point;

        GameObject enemy = objectManager.MakeObj(enemyObjs[enemyIndex]);
        enemy.transform.position = spawnPoints[enemyPoint].position;

        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.gameManager = this;
        enemyLogic.player = player; //프리펩은 이미 Scene에 올라온 오브젝트에 접근 불가능 -> Enemy에서 바로 player를 받지 못함
                                    //=> 적 생성 후 플레이어 변수를 넘겨받음
        enemyLogic.objectManager = objectManager;

        if (enemyPoint == 5 || enemyPoint == 6)//오 -> 왼
        {
            enemy.transform.Rotate(Vector3.back * 45);
            rigid.velocity = new Vector2(enemyLogic.speed * -1, -1); //속도를 주기
        }
        else if (enemyPoint == 7 || enemyPoint == 8)//왼 -> 오
        {
            enemy.transform.Rotate(Vector3.forward * 45);
            rigid.velocity = new Vector2(enemyLogic.speed, -1);
        }
        else
        {
            rigid.velocity = new Vector2(0, enemyLogic.speed * -1); //왼 -> 오
        }

        //리스폰 인덱스 증가
        spawnIndex++;
        if (spawnIndex == spawnList.Count)
        {
            spawnEnd = true;
            return;
        }

        //다음 리스폰 딜레이 갱신
        nextSpawnDelay = spawnList[spawnIndex].delay;
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

    public void CallExplosion(Vector3 pos, string type)
    {
        GameObject explosion = objectManager.MakeObj("Explosion");
        Explosion explosionLogic = explosion.GetComponent<Explosion>();

        explosion.transform.position = pos;
        explosionLogic.StartExplosion(type);

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

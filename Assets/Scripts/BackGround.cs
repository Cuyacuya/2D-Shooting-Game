using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    public float speed;
    public int startIndex; //시작지점
    public int endIndex; //끝지점
    public Transform[] sprites;

    float viewHeight;
    private void Awake()
    {
        viewHeight = Camera.main.orthographicSize * 2; //메인카메라의 기본설정(orthographic)사이즈 * 2 = 전체사이즈 
    }
    void Update()
    {
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.down * speed * Time.deltaTime;
        transform.position = curPos + nextPos;

        if (sprites[endIndex].position.y < viewHeight*(-1))
        {
            Vector3 backSpritePos = sprites[startIndex].localPosition;
            Vector3 frontSpritePos = sprites[endIndex].localPosition;
            sprites[endIndex].transform.localPosition = backSpritePos + Vector3.up * 10;
            
            int startIndexSave = startIndex;
            startIndex = endIndex;
            endIndex = (startIndexSave - 1 == -1) ? sprites.Length - 1 : startIndexSave - 1;
        }
    }
}

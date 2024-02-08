using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//[System.Serializable]
public class ObjectData_2 {
    public List<Vector2> position ;
    public List<Vector2> boxSize;
}

public class gen_2 : MonoBehaviour
{
    public GameObject boxPrefab; // 상자 프리팹
    public float Interval = 0.1f;

    private ObjectData_2 objectData;
    private int boxIndex = 0;

    public GameObject objectToPlace;
    public float gridSize = 0.3f;

    void Start () {
        this.objectData = new ObjectData_2();
        this.objectData.position = new List<Vector2>() {
            new Vector2(0, 0),
            new Vector2(3, 6),
            new Vector2(6, 0),
            new Vector2(2, 3),
            new Vector2(7, 3),
            new Vector2(9, 1),
            new Vector2(0, 7),
            new Vector2(7, 4),
            new Vector2(3, 2),
            new Vector2(0, 5),
            new Vector2(6, 5),
            new Vector2(5, 9),
            new Vector2(0, 3),
            new Vector2(6, 7),
            new Vector2(8, 7),
            new Vector2(1, 5),
            new Vector2(3, 0)

        };
        this.objectData.boxSize = new List<Vector2>() {
            new Vector2(3, 3),
            new Vector2(3, 3),
            new Vector2(3, 3),
            new Vector2(1, 1),
            new Vector2(1, 1),
            new Vector2(1, 1),
            new Vector2(3, 3),
            new Vector2(3, 3),
            new Vector2(3, 3),
            new Vector2(1, 1),
            new Vector2(1, 1),
            new Vector2(1, 1),
            new Vector2(2, 2),
            new Vector2(2, 2),
            new Vector2(2, 2),
            new Vector2(2, 2),
            new Vector2(2, 2)


        };
        Vector3 gridPosition = new Vector3(1 * gridSize, 0, 1 * gridSize);
        InvokeRepeating("GenerateBox", 0f, Interval);
    }

    void GenerateBox()
    {
        print(objectData.position);
        print(objectData.position.Count);
       if (boxIndex < objectData.position.Count) { // 생성할 박스가 남아있는 경우
            Vector2 pos = objectData.position[boxIndex]*0.3f*0.3f*0.3f;
            Vector2 size = objectData.boxSize[boxIndex]*0.3f*0.3f*0.3f;
            GameObject go = Instantiate(boxPrefab);
            //go.transform.position = new Vector3(pos.x, 0, pos.y);
            go.transform.localScale = new Vector3(size.x ,0.027f, size.y);
            print("sizex:"+size.x+"sizey"+size.y);
            //go.transform.position = new Vector3(pos.x * gridSize + (size.x*0.03f / 2)-0.25f, 0.5f, pos.y * gridSize + (size.y*0.03f / 2)+0.3f); // 월드 좌표 계산
            go.transform.position = new Vector3(pos.x * gridSize +(size.x / 2)-0.26f, 0.70f, pos.y * gridSize+(size.y/2)+0.16f); // 월드 좌표 계산
            print("position"+boxIndex+":"+go.transform.position);
            boxIndex++; // 다음 박스를 생성하기 위해 인덱스 증가
        }
        else { // 모든 박스를 생성한 경우
            CancelInvoke("GenerateBox"); // InvokeRepeating() 함수 중지
        }
    }
}

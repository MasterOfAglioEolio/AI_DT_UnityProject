using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boxscript : MonoBehaviour
{
    public GameObject[] boxPrefabs; // 생성할 박스 프리팹 배열
    public float interval; // 생성 간격
    private float timer; // 시간 측정용 타이머

    void Start()
    {
        timer = 0f; // 타이머 초기화
    }

    void Update()
    {
        timer += Time.deltaTime; // 시간 측정

        if (timer > interval) // 일정 시간 간격이 지났다면
        {
            timer = 0f; // 타이머 초기화

            // 랜덤으로 박스 생성
            int randomIndex = Random.Range(0, boxPrefabs.Length);
            GameObject box = Instantiate(boxPrefabs[randomIndex], transform.position, Quaternion.identity);

            // 생성한 박스를 BoxGenerator의 자식으로 설정
            box.transform.parent = transform;
        }
    }
}

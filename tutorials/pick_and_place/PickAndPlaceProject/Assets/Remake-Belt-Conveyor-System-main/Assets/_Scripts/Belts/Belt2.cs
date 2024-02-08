using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Belt2 : MonoBehaviour
{
    private static int _beltID2 = 0;

    public Belt2 beltInSequence2;
    public BeltItem2 beltItem2;
    public BeltItem2[] new_beltItem2;
    public bool isSpaceTaken;
    public BeltItem2 newItem;

    private BeltManager _beltManager;
    // private bool itemCreated = false;
    private int m_CurrentItemIndex = 0;


    private void Start()
    {


        _beltManager = FindObjectOfType<BeltManager>();
        beltInSequence2 = null;
        beltInSequence2 = FindNextBelt();
        gameObject.name = $"Belt2: {_beltID2++}";
        Debug.Log(gameObject.name);
        // 5초마다 CreateBeltItem() 함수를 호출합니다.
        FindObjectOfType<TrajectoryPlanner>().OnOpenGripper.AddListener(OnOpenGripper);
        if(m_CurrentItemIndex<new_beltItem2.Length)
            InvokeRepeating("CreateBeltItem", 4f,5f);


    }


    private void Update()
    {

        if (beltInSequence2 == null)
        {
            beltInSequence2 = FindNextBelt();
        }

        if (beltItem2 != null && beltItem2.item2 != null)
            StartCoroutine(StartBeltMove());


    }
    private void OnOpenGripper()
    {
        // Belt 스크립트에게 OpenGripper 이벤트가 발생했음을 전달

        Debug.Log("OpenGripper2 이벤트 발생!");
        // m_Targets[m_CurrentTargetIndex]=null;
        GameObject new_belt = GameObject.Find("Belt2: 0"); // 끝지점 기준
        new_belt.GetComponent<Belt2>().beltItem2 = null; //beltItem null
        new_belt.GetComponent<Belt2>().isSpaceTaken=false;
        Debug.Log(new_belt);
        StartBeltMove();

    }

    public Vector3 GetItemPosition()
    {
        var padding = 0.05f;
        var position = transform.position;
        return new Vector3(position.x, position.y + padding, position.z);
    }

    private IEnumerator StartBeltMove() //
    {
        isSpaceTaken = true;

        if (beltItem2.item2 != null && beltInSequence2 != null && beltInSequence2.isSpaceTaken == false)
        {
            Vector3 toPosition = beltInSequence2.GetItemPosition();

            beltInSequence2.isSpaceTaken = true;

            var step = _beltManager.speed * Time.deltaTime;

            while (beltItem2.item2.transform.position != toPosition)
            {
                beltItem2.item2.transform.position =
                    Vector3.MoveTowards(beltItem2.transform.position, toPosition, step);

                yield return null;
            }

            isSpaceTaken = false;
            beltInSequence2.beltItem2 = beltItem2;
            beltItem2 = null;

        }
    }

    private Belt2 FindNextBelt()
    {

        Transform currentBeltTransform = transform;
        //Debug.Log("CBT"+currentBeltTransform);
        RaycastHit hit;

        //Debug.Log("trans"+transform);
        var forward = transform.forward*0.5f;
        //Debug.Log("forward"+forward);
        Ray ray = new Ray(currentBeltTransform.position, forward);


        if (Physics.Raycast(ray, out hit, 0.5f))
        {
            //Debug.Log("HIT"+transform);
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);
            Belt2 belt2 = hit.collider.GetComponent<Belt2>();
            //Debug.Log("b2"+belt2);
            if (belt2 != null)
                {
                    Debug.Log("Move"+belt2);
                    return belt2;
                }
        }

        // Debug.DrawRay(transform.position, transform.forward * 1000f, Color.red);
        Debug.Log("Cant Move");
        return null;
    }

    private void CreateBeltItem()
    {
        // BeltItem을 생성합니다.
        // 컨테이너 시작좌표
        Debug.Log("CreateBeltItem");
        //Vector3 gen_position = new Vector3(2.3f, 0.8f, 0.2f);
        GameObject start_belt = GameObject.Find("Belt2: 6");
        //Debug.Log("stp:"+start_belt.transform.position);
        Vector3 gen_position=new Vector3(start_belt.transform.position[0],start_belt.transform.position[1]+0.1f,start_belt.transform.position[2]);
        int randomIndex = UnityEngine.Random.Range(0, new_beltItem2.Length);
        // 새로운 BeltItem 생성
        // Debug.Log("Length:"+m_CurrentItemIndex);

        newItem = Instantiate(new_beltItem2[m_CurrentItemIndex], gen_position, Quaternion.identity);

        m_CurrentItemIndex++;



        // Debug.Log("NI"+newItem.transform.position);
        // 시작 지점의 Belt 찾기
        Ray ray_item = new Ray(gen_position, new Vector3(0, -1, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray_item, out hit, 1f))
        {
            Belt2 belt2 = hit.collider.GetComponent<Belt2>();
            belt2.beltItem2 = newItem;
            //Debug.Log("Hit Start"+belt2.beltItem2);
        }

        isSpaceTaken = false;
    }
}

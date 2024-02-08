using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Belt3 : MonoBehaviour
{
    private static int _beltID3 = 0;

    public Belt3 beltInSequence3;
    public BeltItem3 beltItem3;
    public BeltItem3[] new_beltItem3;
    public bool isSpaceTaken;
    public BeltItem3 newItem;

    private BeltManager _beltManager;
    // private bool itemCreated = false;
    private int m_CurrentItemIndex = 0;


    private void Start()
    {


        _beltManager = FindObjectOfType<BeltManager>();
        beltInSequence3 = null;
        beltInSequence3 = FindNextBelt();
        gameObject.name = $"Belt3: {_beltID3++}";
        Debug.Log(gameObject.name);
        // 5초마다 CreateBeltItem() 함수를 호출합니다.
        FindObjectOfType<TrajectoryPlanner>().OnOpenGripper.AddListener(OnOpenGripper);
        if(m_CurrentItemIndex<new_beltItem3.Length)
            InvokeRepeating("CreateBeltItem", 5f,5f);


    }


    private void Update()
    {

        if (beltInSequence3 == null)
        {
            beltInSequence3 = FindNextBelt();
        }

        if (beltItem3 != null && beltItem3.item3 != null)
            StartCoroutine(StartBeltMove());


    }
    private void OnOpenGripper()
    {
        // Belt 스크립트에게 OpenGripper 이벤트가 발생했음을 전달

        Debug.Log("OpenGripper3 이벤트 발생!");
        // m_Targets[m_CurrentTargetIndex]=null;
        GameObject new_belt = GameObject.Find("Belt3: 0"); // 끝지점 기준
        new_belt.GetComponent<Belt3>().beltItem3 = null; //beltItem null
        new_belt.GetComponent<Belt3>().isSpaceTaken=false;
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

        if (beltItem3.item3 != null && beltInSequence3 != null && beltInSequence3.isSpaceTaken == false)
        {
            Vector3 toPosition = beltInSequence3.GetItemPosition();

            beltInSequence3.isSpaceTaken = true;

            var step = _beltManager.speed * Time.deltaTime;

            while (beltItem3.item3.transform.position != toPosition)
            {
                beltItem3.item3.transform.position =
                    Vector3.MoveTowards(beltItem3.transform.position, toPosition, step);

                yield return null;
            }

            isSpaceTaken = false;
            beltInSequence3.beltItem3 = beltItem3;
            beltItem3 = null;

        }
    }

    private Belt3 FindNextBelt()
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
            Belt3 belt3 = hit.collider.GetComponent<Belt3>();
            //Debug.Log("b3"+belt3);
            if (belt3 != null)
                {
                    Debug.Log("Move"+belt3);
                    return belt3;
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
        GameObject start_belt = GameObject.Find("Belt3: 6");
        //Debug.Log("stp:"+start_belt.transform.position);
        Vector3 gen_position=new Vector3(start_belt.transform.position[0],start_belt.transform.position[1]+0.1f,start_belt.transform.position[2]);
        int randomIndex = UnityEngine.Random.Range(0, new_beltItem3.Length);
        // 새로운 BeltItem 생성
        // Debug.Log("Length:"+m_CurrentItemIndex);

        newItem = Instantiate(new_beltItem3[m_CurrentItemIndex], gen_position, Quaternion.identity);

        m_CurrentItemIndex++;



        // Debug.Log("NI"+newItem.transform.position);
        // 시작 지점의 Belt 찾기
        Ray ray_item = new Ray(gen_position, new Vector3(0, -1, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray_item, out hit, 1f))
        {
            Belt3 belt3 = hit.collider.GetComponent<Belt3>();
            belt3.beltItem3 = newItem;
            //Debug.Log("Hit Start"+belt3.beltItem3);
        }

        isSpaceTaken = false;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class Belt : MonoBehaviour
{
    private static int _beltID = 0;

    public Belt beltInSequence;
    public BeltItem beltItem;
    public BeltItem[] new_beltItem;
    public bool isSpaceTaken;
    public BeltItem newItem;

    private BeltManager _beltManager;
    // private bool itemCreated = false;
    private int m_CurrentItemIndex = 0;

    //public GameObject boxPrefab; // 상자 프리팹

    private void Start()
    {


        _beltManager = FindObjectOfType<BeltManager>();
        beltInSequence = null;
        beltInSequence = FindNextBelt();
        gameObject.name = $"Belt: {_beltID++}";
        Debug.Log(gameObject.name);

        // TrajectoryPlanner trajectoryPlanner = FindObjectOfType<TrajectoryPlanner>();
        // BeltItem[] new_beltItem = new BeltItem[trajectoryPlanner.m_Targets.Length];
        // Debug.Log("nb1:"+new_beltItem);

        // for (int i = 0; i < trajectoryPlanner.m_Targets.Length; i++)
        // {
        //     print("i:belt"+i);
        //     GameObject item = trajectoryPlanner.m_Targets[i];
        //     BeltItem beltItem = new BeltItem();
        //     beltItem.item = item;
        //     new_beltItem[i] = beltItem;
        // }
        // Debug.Log("nb2:"+new_beltItem[0]);

        // 5초마다 CreateBeltItem() 함수를 호출합니다.
        FindObjectOfType<TrajectoryPlanner>().OnOpenGripper.AddListener(OnOpenGripper);
        if(m_CurrentItemIndex<new_beltItem.Length+2)
            InvokeRepeating("CreateBeltItem", 5f,5f);


    }


    private void Update()
    {
        if (beltInSequence == null)
            beltInSequence = FindNextBelt();

        if (beltItem != null && beltItem.item != null)
            StartCoroutine(StartBeltMove());

    }
    private void OnOpenGripper()
    {
        // Belt 스크립트에게 OpenGripper 이벤트가 발생했음을 전달

        Debug.Log("OpenGripper1 이벤트 발생!");
        // m_Targets[m_CurrentTargetIndex]=null;
        GameObject new_belt = GameObject.Find("Belt: 0"); // 끝지점 기준
        new_belt.GetComponent<Belt>().beltItem = null; //beltItem null
        new_belt.GetComponent<Belt>().isSpaceTaken=false;
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
        //print("isSpaceTaken");
        if (beltItem.item != null && beltInSequence != null && beltInSequence.isSpaceTaken == false)
        {

            Vector3 toPosition = beltInSequence.GetItemPosition();

            beltInSequence.isSpaceTaken = true;

            var step = _beltManager.speed * Time.deltaTime;

            while (beltItem.item.transform.position != toPosition)
            {
                beltItem.item.transform.position =
                    Vector3.MoveTowards(beltItem.transform.position, toPosition, step);

                yield return null;
            }
            Debug.Log("BI:"+beltInSequence);
            isSpaceTaken = false;
            beltInSequence.beltItem = beltItem;
            beltItem = null;

        }
    }

    private Belt FindNextBelt()
    {

        Transform currentBeltTransform = transform;
        RaycastHit hit;

        //Debug.Log("trans"+transform);
        var forward = transform.forward;
        //Debug.Log("forward"+forward);
        Ray ray = new Ray(currentBeltTransform.position, forward);


        if (Physics.Raycast(ray, out hit, 1f))
        {

            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);
            Belt belt = hit.collider.GetComponent<Belt>();
            if (belt != null)
                {
                    Debug.Log("Move");
                    return belt;
                }
        }

        Debug.DrawRay(transform.position, transform.forward * 1000f, Color.red);

        return null;
    }

    private void CreateBeltItem()
    {
        // BeltItem을 생성합니다.
        // 컨테이너 시작좌표
        //Debug.Log("CreateBeltItem");
        //Vector3 gen_position = new Vector3(2.3f, 0.8f, 0.2f);
        GameObject start_belt = GameObject.Find("Belt: 6");
        //Debug.Log("stp:"+start_belt.transform.position);
        Vector3 gen_position=new Vector3(start_belt.transform.position[0],start_belt.transform.position[1]+0.1f,start_belt.transform.position[2]);
        int randomIndex = UnityEngine.Random.Range(0, new_beltItem.Length);
        // 새로운 BeltItem 생성
        // Debug.Log("Length:"+m_CurrentItemIndex);

        newItem = Instantiate(new_beltItem[m_CurrentItemIndex], gen_position, Quaternion.identity);
        newItem.name = $"Target_{m_CurrentItemIndex+2}";
        print("nI"+newItem);
        //print("newItme:"+newItem);
        m_CurrentItemIndex++;


        // Debug.Log("NI"+newItem.transform.position);
        // 시작 지점의 Belt 찾기
        Ray ray_item = new Ray(gen_position, new Vector3(0, -1, 0));
        //print("rayItme:"+ray_item);
        RaycastHit hit;
        if (Physics.Raycast(ray_item, out hit, 1f))
        {

            Belt belt = hit.collider.GetComponent<Belt>();
            belt.beltItem = newItem;
        }
        isSpaceTaken = false;
    }
}

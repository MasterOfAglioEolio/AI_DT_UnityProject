using System;
using System.Collections;
using System.Linq;
using RosMessageTypes.Geometry;
using RosMessageTypes.NiryoMoveit;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class Vector2Data
{
    public float x;
    public float y;
}


[System.Serializable]
public class ObjectData
{
    public List<Vector2Data> position;
    public List<Vector2Data> boxSize;
}

public class TrajectoryPlanner : MonoBehaviour
{
    public GameObject boxPrefab; // 상자 프리팹
    public float Interval = 0.1f;

    private ObjectData objectData;
    private int boxIndex = 0;

    public GameObject objectToPlace;
    public float gridSize = 1f;

    // Hardcoded variables
    const int k_NumRobotJoints = 6;
    const float k_JointAssignmentWait = 0.1f;
    const float k_PoseAssignmentWait = 0.5f;

    // Variables required for ROS communication
    [SerializeField]
    string m_RosServiceName = "niryo_moveit";
    public string RosServiceName { get => m_RosServiceName; set => m_RosServiceName = value; }

    [SerializeField]
    GameObject m_NiryoOne;
    public GameObject NiryoOne { get => m_NiryoOne; set => m_NiryoOne = value; }

    [SerializeField]
    public GameObject[] m_Targets;
    public GameObject[] Targets { get => m_Targets; set => m_Targets = value; }

    [SerializeField]
    GameObject[] m_TargetPlacements;
    public GameObject[] TargetPlacements { get => m_TargetPlacements; set => m_TargetPlacements = value; }

    // Assures that the gripper is always positioned above the m_Target cube before grasping.
    readonly Quaternion m_PickOrientation = Quaternion.Euler(90, 90, 0);
    readonly Vector3 m_PickPoseOffset = Vector3.up * 0.11f;

    // Articulation Bodies
    ArticulationBody[] m_JointArticulationBodies;
    ArticulationBody m_LeftGripper;
    ArticulationBody m_RightGripper;

    // ROS Connector
    ROSConnection m_Ros;

    private int m_CurrentTargetIndex = 1;
    private GameObject m_CurrentTarget;

    private int m_CurrentTargetPlacementIndex = 0;
    private GameObject m_CurrentTargetPlacement;

    private Vector2 m_CurrentTargetPlacement_pos;

    public UnityEvent OnOpenGripper;
    public GameObject new_target;

    public GameObject progressBarObject;
    private RadialProgress progressBar;


    /// <summary>
    ///     Find all robot joints in Awake() and add them to the jointArticulationBodies array.
    ///     Find left and right finger joints and assign them to their respective articulation body objects.
    /// </summary>
    void Start()
    {
        string path_postion = "position.json";
        string path_boxSize = "boxSize.json";

        jsonString1 = File.ReadAllText(path_postion);
        jsonString2 = File.ReadAllText(path_boxSize);
        

        this.objectData = new ObjectData();
        this.objectData.position = JsonUtility.FromJson<ObjectData>(jsonString1);
        this.objectData.boxSize = JsonUtility.FromJson<ObjectData>(jsonString2);
        Vector3 gridPosition = new Vector3(1 * gridSize, 0, 1 * gridSize);


        // Get ROS connection static instance
        m_Ros = ROSConnection.GetOrCreateInstance();
        m_Ros.RegisterRosService<MoverServiceRequest, MoverServiceResponse>(m_RosServiceName);

        m_JointArticulationBodies = new ArticulationBody[k_NumRobotJoints];
        // GameObject currentTarget = m_Targets[0];
        // GameObject currentTargetPlacement = m_TargetPlacements[0];
        // Debug.Log("TP:"+currentTarget.transform.position);

        var linkName = string.Empty;
        for (var i = 0; i < k_NumRobotJoints; i++)
        {
            linkName += SourceDestinationPublisher.LinkNames[i];
            m_JointArticulationBodies[i] = m_NiryoOne.transform.Find(linkName).GetComponent<ArticulationBody>();
        }
        // Debug.Log("TP:"+currentTargetPlacement.transform.position);

        // Find left and right fingers
        var rightGripper = linkName + "/tool_link/gripper_base/servo_head/control_rod_right/right_gripper";
        var leftGripper = linkName + "/tool_link/gripper_base/servo_head/control_rod_left/left_gripper";
        // Debug.Log("TP:"+currentTargetPlacement.transform.position);
        m_RightGripper = m_NiryoOne.transform.Find(rightGripper).GetComponent<ArticulationBody>();
        m_LeftGripper = m_NiryoOne.transform.Find(leftGripper).GetComponent<ArticulationBody>();
        m_CurrentTarget=m_Targets[0];
        print("NNNm_CT:"+m_CurrentTarget);
        //print("NNN")

        m_CurrentTargetPlacement=m_TargetPlacements[0];
        m_CurrentTargetPlacement_pos=objectData.position[0];
        Debug.Log("TP:"+m_CurrentTargetPlacement.transform.position);
        Debug.Log("TP:"+m_CurrentTargetPlacement_pos);

        // GameObject[] boxes = new GameObject[this.objectData.boxSize.Count];
        // for (int i = 0; i < this.objectData.boxSize.Count; i++) {
        //     Vector2 size = this.objectData.boxSize[i];
        //     GameObject box = new GameObject("Box_" + i.ToString());
        //     box.transform.localScale = new Vector3(size.x, size.y, 1);
        //     boxes[i] = box;
        // }
        // this.m_Targets = boxes;
        progressBar = progressBarObject.GetComponent<RadialProgress>();
        Debug.Log("pB:"+progressBar);

    }

    /// <summary>
    ///     Close the gripper
    /// </summary>
    void CloseGripper()
    {
        var leftDrive = m_LeftGripper.xDrive;
        var rightDrive = m_RightGripper.xDrive;

        leftDrive.target = -0.05f;
        rightDrive.target = 0.05f;

        m_LeftGripper.xDrive = leftDrive;
        m_RightGripper.xDrive = rightDrive;
    }

    /// <summary>
    ///     Open the gripper
    /// </summary>
    void OpenGripper()
    {
        var leftDrive = m_LeftGripper.xDrive;
        var rightDrive = m_RightGripper.xDrive;

        leftDrive.target = 0.01f;
        rightDrive.target = -0.01f;

        m_LeftGripper.xDrive = leftDrive;
        m_RightGripper.xDrive = rightDrive;

        if (m_CurrentTargetIndex < m_Targets.Length)
        {

            m_Targets[m_CurrentTargetIndex]=null;
            //Debug.Log("TI"+m_CurrentTargetIndex);
            m_CurrentTarget = m_Targets[++m_CurrentTargetIndex];
            //Debug.Log("TI2"+m_CurrentTargetIndex);



            m_TargetPlacements[m_CurrentTargetPlacementIndex]=null;
            m_CurrentTargetPlacement = m_TargetPlacements[++m_CurrentTargetPlacementIndex];
            m_CurrentTargetPlacement_pos=objectData.position[++boxIndex];

            Debug.Log("TP_pos_index:"+boxIndex);
            Debug.Log("TP_pos:"+m_CurrentTargetPlacement_pos);

        }
        if (OnOpenGripper != null)
        {
            OnOpenGripper.Invoke(); //OpenGRipper 하면 !
            progressBar.IncrementProgress();
            Debug.Log("Increment:"+progressBar);

            Rigidbody rb = new_target.GetComponent<Rigidbody>();
            BoxCollider boxCollider = new_target.GetComponent<BoxCollider>();

            if (rb != null)
            {
                if (rb.constraints == RigidbodyConstraints.None)
                {
                    rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                    boxCollider.enabled = false;
                }

            }
            else
            {
                Debug.LogWarning("Rigidbody not found on target object!");
            }
            Debug.Log("Opengripper");

            Invoke("PublishJoints", 1f); // 1초 뒤에 PublishJoint 실행
            Debug.Log("PublishJoints");
        }


    }

    /// <summary>
    ///     Get the current values of the robot's joint angles.
    /// </summary>
    /// <returns>NiryoMoveitJoints</returns>
    NiryoMoveitJointsMsg CurrentJointConfig()
    {
        var joints = new NiryoMoveitJointsMsg();

        for (var i = 0; i < k_NumRobotJoints; i++)
        {
            joints.joints[i] = m_JointArticulationBodies[i].jointPosition[0];
        }

        return joints;
    }

    /// <summary>
    ///     Create a new MoverServiceRequest with the current values of the robot's joint angles,
    ///     the target cube's current position and rotation, and the targetPlacement position and rotation.
    ///     Call the MoverService using the ROSConnection and if a trajectory is successfully planned,
    ///     execute the trajectories in a coroutine.
    /// </summary>
    public void PublishJoints()
    {

        var request = new MoverServiceRequest();
        request.joints_input = CurrentJointConfig();

        // Pick Pose
        // Debug.Log("mc"+m_CurrentTarget);
        // Debug.Log("mctp"+m_CurrentTarget.transform.position);
        Debug.Log("mcp"+m_CurrentTargetPlacement);
        Debug.Log("NNNCPP_index:"+boxIndex);
        Debug.Log("NNNCPP"+m_CurrentTargetPlacement_pos);
        // Debug.Log("ci"+m_CurrentTargetIndex);
        Debug.Log("NNNCTI:"+m_CurrentTargetIndex);
        new_target = GameObject.Find($"Target_{m_CurrentTargetIndex}");

        Debug.Log("NNNtarget:"+new_target);
        //Debug.Log("ntargetp"+new_target.transform.position);
        // Debug.Log("mcptp"+m_CurrentTargetPlacement.transform.position);

        request.pick_pose = new PoseMsg
        {
            // position = (m_CurrentTarget.transform.position + m_PickPoseOffset).To<FLU>(),
            position = (new_target.transform.position + m_PickPoseOffset).To<FLU>(),
            //position = (new Vector3(0.22f, 0.64f, -0.05f)).To<FLU>(),
            // The hardcoded x/z angles assure that the gripper is always positioned above the target cube before grasping.
            orientation = Quaternion.Euler(90, new_target.transform.eulerAngles.y, 0).To<FLU>()
            // orientation = Quaternion.Euler(90, m_CurrentTarget.transform.eulerAngles.y, 0).To<FLU>()
        };

        // Place Pose
        request.place_pose = new PoseMsg
        {   //new Vector3(pos.x * gridSize + size.x / 2, 0, pos.y * gridSize + size.y / 2)
            position = (new Vector3(m_CurrentTargetPlacement_pos.x * gridSize, 0.7f, m_CurrentTargetPlacement_pos.y * gridSize)+m_PickPoseOffset).To<FLU>(),
            //position = (m_CurrentTargetPlacement.transform.position + m_PickPoseOffset).To<FLU>(),
            orientation = m_PickOrientation.To<FLU>()
        };
        Debug.Log("request");

        m_Ros.SendServiceMessage<MoverServiceResponse>(m_RosServiceName, request, TrajectoryResponse);
    }


    void TrajectoryResponse(MoverServiceResponse response)
    {
        if (response.trajectories.Length > 0)
        {
            Debug.Log("Trajectory returned.");
            StartCoroutine(ExecuteTrajectories(response));
        }
        else
        {
            Debug.LogError("No trajectory returned from MoverService.");
        }
    }

    /// <summary>
    ///     Execute the returned trajectories from the MoverService.
    ///     The expectation is that the MoverService will return four trajectory plans,
    ///     PreGrasp, Grasp, PickUp, and Place,
    ///     where each plan is an array of robot poses. A robot pose is the joint angle values
    ///     of the six robot joints.
    ///     Executing a single trajectory will iterate through every robot pose in the array while updating the
    ///     joint values on the robot.
    /// </summary>
    /// <param name="response"> MoverServiceResponse received from niryo_moveit mover service running in ROS</param>
    /// <returns></returns>
    IEnumerator ExecuteTrajectories(MoverServiceResponse response)
    {
        if (response.trajectories != null)
        {
            // For every trajectory plan returned
            for (var poseIndex = 0; poseIndex < response.trajectories.Length; poseIndex++)
            {
                // For every robot pose in trajectory plan
                foreach (var t in response.trajectories[poseIndex].joint_trajectory.points)
                {
                    var jointPositions = t.positions;
                    var result = jointPositions.Select(r => (float)r * Mathf.Rad2Deg).ToArray();

                    // Set the joint values for every joint
                    for (var joint = 0; joint < m_JointArticulationBodies.Length; joint++)
                    {
                        var joint1XDrive = m_JointArticulationBodies[joint].xDrive;
                        joint1XDrive.target = result[joint];
                        m_JointArticulationBodies[joint].xDrive = joint1XDrive;
                    }

                    // Wait for robot to achieve pose for all joint assignments
                    yield return new WaitForSeconds(k_JointAssignmentWait);
                }

                // Close the gripper if completed executing the trajectory for the Grasp pose
                if (poseIndex == (int)Poses.Grasp)
                {
                    CloseGripper();
                }

                // Wait for the robot to achieve the final pose from joint assignment
                yield return new WaitForSeconds(k_PoseAssignmentWait);
            }

            // All trajectories have been executed, open the gripper to place the target cube
            OpenGripper(); // Target placemenet 기준
            // Debug.Log(m_Targets[0]);
            // m_Targets[0]=null; // 첫 target 해제
            // // var Targets=m_Targets[1];
            // // Debug.Log(m_Targets[1]);




        }
    }

    enum Poses
    {
        PreGrasp,
        Grasp,
        PickUp,
        Place
    }
}

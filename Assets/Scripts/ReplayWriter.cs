using System.Collections.Generic;
using UnityEngine;

public class ReplayWriter : MonoBehaviour
{
    #region Singleton Pattern no.1
    private static ReplayWriter instance;

    public static ReplayWriter Instance
    {
        get
        {
            if (instance == null)
            {
                var go = new GameObject("Replay Writer");
                instance = go.AddComponent<ReplayWriter>();
            }
            return instance;
        }
    }
    void Awake()
    {
        instance = this; //In case we accidentally have one in the scene, this should ensure correctly setting it.
    }
    #endregion
    private List<PhysicsFrameControlLog> inputFrames=new List<PhysicsFrameControlLog>();
    private List<PhysicsFramePositionLog> positionLog=new List<PhysicsFramePositionLog>();
    private List<CheckpointCompletion> checkpointCompletions=new List<CheckpointCompletion>();
    private float? FinishCompletionTime=null;
    private string levelIdentifier="";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
public struct PhysicsFrameControlLog
{
    float pitch;
    float yaw;
    float roll;
    float thrust;
    float vertical;
    float lateral;
    bool spaceBrake;

    public PhysicsFrameControlLog(float pitch, float yaw, float roll, float thrust, float vertical, float lateral, bool spaceBrake)
    {
        this.pitch = pitch;
        this.yaw = yaw;
        this.roll = roll;
        this.thrust = thrust;
        this.vertical = vertical;
        this.lateral = lateral;
        this.spaceBrake = spaceBrake;
    }
}
public struct PhysicsFramePositionLog
{
    Vector3 localPosition;
    Quaternion localRotation;

    public PhysicsFramePositionLog(Vector3 localPosition, Quaternion localRotation)
    {
        this.localPosition = localPosition;
        this.localRotation = localRotation;
    }
}

public struct CheckpointCompletion
{
    int CheckpointRank;
    float timeSinceStart;

    public CheckpointCompletion(int checkpointRank, float timeSinceStart)
    {
        CheckpointRank = checkpointRank;
        this.timeSinceStart = timeSinceStart;
    }
}
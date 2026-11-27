using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private List<PhysicsFramePositionLog> positionFrames=new List<PhysicsFramePositionLog>();
    private List<CheckpointCompletion> checkpointCompletions=new List<CheckpointCompletion>();
    private float? finishCompletionTime=null;
    private string levelIdentifier=null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void WriteReplay()
    {
        if (levelIdentifier == null)
        {
            levelIdentifier = SceneManager.GetActiveScene().name;
        }

        throw new System.Exception();
    }
    private static void WriteReplayStatic(string levelIdentifier, float? finishCompletionTime,string PresetConfig, List<PhysicsFrameControlLog> inputFrames, List<PhysicsFramePositionLog> positionFrames, List<CheckpointCompletion> checkpointCompletions)
    {

        string filename = $"{DateTime.Now.ToString("s").Replace(":", "")}.replay";
        using (StreamWriter sw=new StreamWriter(filename))
        {
            sw.WriteLine(levelIdentifier);
            if (finishCompletionTime != null)
            {
                sw.WriteLine(Timer.GetTimeFromFloat(finishCompletionTime.Value));//For now, I'm doing this human-readable since I will be analyzing it.
            }
            else 
            {
                sw.WriteLine();
            }
            sw.WriteLine();
            sw.WriteLine(PresetConfig);
            sw.WriteLine();
            for (int i = 0; i< checkpointCompletions.Count; i++)
            {
                sw.WriteLine(checkpointCompletions[i].ToString());
            }
            sw.WriteLine();
            for (int i = 0; i<inputFrames.Count; i++)
            {
                sw.WriteLine(inputFrames[i].ToString());
            }
            sw.WriteLine();
            for (int i = 0;i<positionFrames.Count; i++)
            {
                sw.WriteLine(positionFrames[i].ToString());
            }
        }
    }
    public void RecordNewFrame(PhysicsFrameControlLog controlLog, PhysicsFramePositionLog positionLog)
    {
        if (finishCompletionTime == null) 
        { //if we haven't finished, add the logs
            inputFrames.Add(controlLog);
            positionFrames.Add(positionLog);
        }
    }
    public void RecordNewCheckpointCompletion(CheckpointCompletion completion)
    {
        if (finishCompletionTime == null)
        { // if we haven't finished, add the log.
            checkpointCompletions.Add(completion);
        }
    }
    public void Finish(float timerTime, bool write = true)
    {
        finishCompletionTime=timerTime;
        if (write)
        {
            //Write as default, but overwritable since we may do verification later.x
            WriteReplay();
        }
    }
    /// <summary>
    /// Sets specific string identifier for the level
    /// </summary>
    /// <param name="identifier">identifier to set</param>
    /// <remarks>Currently unused, intended for dynamically loaded levels.</remarks>
    public void SetStringIdentifier(string identifier)
    {
        levelIdentifier=identifier;
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
    public override string ToString()
    {
        return $"P{pitch}Y{yaw}R{roll}T{thrust}V{vertical}L{lateral}SB{Convert.ToInt32(spaceBrake)}";
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
    public override string ToString()
    {
        return $"{localPosition.ToString()}|{localRotation.ToString()}";
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
    public override string ToString()
    {
        return $"{CheckpointRank}|{Timer.GetTimeFromFloat(timeSinceStart)}";
    }
}
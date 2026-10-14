using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    #region Singleton Stuff
    private static CheckpointManager instance;

    public static CheckpointManager Instance
    {
        get
        {
            if (instance == null)
            {
                var go= new GameObject("Game Manager");
                instance = go.AddComponent<CheckpointManager>();
            }
            return instance;
        }
    }
    void Awake()
    {
        instance = this; //In case we accidentally have one in the scene, this should ensure correctly setting it.
    }
    #endregion

    #region Checkpoint Management

    private SortedList<int, HashSet<Checkpoint>> Checkpoints= new SortedList<int, HashSet<Checkpoint>>();
    private SortedList<int, HashSet<Checkpoint>> CheckpointsNotPassed = new SortedList<int, HashSet<Checkpoint>>();
    private Checkpoint LastCompleteCheckpoint = null;//In case we do a stationary reset (Desirable)
    public int MaxCheckpointOrdPassed { get; private set; } = 0;
    public int CreatorCheckpointIndex = int.MinValue;
    public bool Finished { get; private set; } = false;
    /// <summary>
    /// Adds a checkpoint to the manager
    /// </summary>
    /// <param name="Checkpoint">GameObject containing the Checkpoint.</param>
    /// <param name="CheckpointNumber">If checkpoints are numbered, which checkpoint this would be.</param>
    /// <returns>True on successful add, False if unsuccessful</returns>
    public bool AddCheckpointToList(Checkpoint Checkpoint, int CheckpointNumber = -1)
    {
        if (CheckpointNumber == -1)
        {
            CheckpointNumber = CreatorCheckpointIndex;
            Checkpoint.RankInSequence = CreatorCheckpointIndex;
            CreatorCheckpointIndex++;
            if (CreatorCheckpointIndex > 0)
            {
                Debug.Log("Congratulations, you've somehow placed enough checkpoints to break the game. HOW?");//Leave this in, it's both a fault code and a message to stop.
            }
        }
        try
        {
            bool success = true;
            if (CheckpointsNotPassed.ContainsKey(CheckpointNumber))
            {
                 success = success && CheckpointsNotPassed[CheckpointNumber].Add(Checkpoint);
            }
            else
            {
                CheckpointsNotPassed.Add(CheckpointNumber, new HashSet<Checkpoint> { Checkpoint });
            }
            if (Checkpoints.ContainsKey(CheckpointNumber))
            {
                success = success && Checkpoints[CheckpointNumber].Add(Checkpoint);
            }
            else
            {
                Checkpoints.Add(CheckpointNumber, new HashSet<Checkpoint>() { Checkpoint });
            }
            return success;
        }
        catch {  return false; }
    }
    /// <summary>
    /// Removes first instance of a given GameObject from both the Checkpoint lists.
    /// </summary>
    /// <param name="Checkpoint">GameObject to search for</param>
    /// <returns>True on successful removal, False if Checkpoint was not present in the immutable list.</returns>
    public bool RemoveCheckpointFromList(Checkpoint Checkpoint)
    { 
        for(int i = 0; i<CheckpointsNotPassed.Count; i++)
        {
            var list = CheckpointsNotPassed[CheckpointsNotPassed.Keys[i]];
            if (list.Contains(Checkpoint)){
                list.Remove(Checkpoint);
                if (list.Count == 0)
                {
                    CheckpointsNotPassed.Remove(CheckpointsNotPassed.Keys[i]);
                    break;//Break is needed since we're now modifying the same list we're iterating over.
                }
            }
        }
        bool found= false;
        for (int i = 0; i < Checkpoints.Count; i++)
        {
            var list = Checkpoints[Checkpoints.Keys[i]];
            if (list.Contains(Checkpoint))
            {
                found = true;
                list.Remove(Checkpoint);
                if (list.Count == 0)
                {
                    Checkpoints.Remove(Checkpoints.Keys[i]);
                    break;
                }
            }
        }

        return found;
    }
    /// <summary>
    /// Repeatedly tries to remove any instances of Checkpoint from the list, even at a high cost
    /// </summary>
    /// <param name="Checkpoint">Checkpoint to purge</param>
    /// <returns></returns>
    public void PurgeFromList(Checkpoint Checkpoint)
    {
        bool found= true;
        while (found)
        {
            found = RemoveCheckpointFromList(Checkpoint);
        }
    }

    /// <summary>
    /// Sets a checkpoint set complete by the checkpoint reference
    /// </summary>
    /// <param name="Checkpoint"></param>
    /// <returns>True if successful, False otherwise.</returns>
    /// <remarks>Will break if you somehow have the same checkpoint multiple times, so fix that if you add the functionality.</remarks>
    public bool SetCheckpointLevelComplete(Checkpoint C)
    {
        if (CheckpointsNotPassed.Keys.Contains(C.RankInSequence))
        {
            var set = CheckpointsNotPassed[C.RankInSequence];
            if (set.Contains(C))
            {
                CheckpointsNotPassed.Remove(C.RankInSequence);
                MaxCheckpointOrdPassed = Mathf.Max(MaxCheckpointOrdPassed, C.RankInSequence);
                LastCompleteCheckpoint = C;
                return true;
            }
        } 
        return false;
    }
    public int GetTotalCheckpoints()
    {
        return Checkpoints.Count;
    }
    public int GetTotalRemainingCheckpoints()
    {
        return CheckpointsNotPassed.Count;
    }
    /// <summary>
    /// Resets the total remaining checkpoints by deep-copying the total list of checkpoints into remaining checkpoints.
    /// </summary>
    public void NewLapReset()
    {
        CheckpointsNotPassed= new SortedList<int, HashSet<Checkpoint>>();
        foreach(KeyValuePair<int, HashSet<Checkpoint>> pair in Checkpoints)
        {
            CheckpointsNotPassed.Add(pair.Key, new HashSet<Checkpoint>());
            foreach(Checkpoint c in pair.Value)
            {
                CheckpointsNotPassed[pair.Key].Add(c);
            }
        }
    }
    /// <summary>
    /// A function call which sets the "Finished" value in checkpoint manager if all prerequisits to finish a level have been completed (We have gone through all checkpoints.)
    /// </summary>
    /// <returns>True if succesfully finished, False if not</returns>
    public bool TryFinish()
    {
        if (GetTotalRemainingCheckpoints() == 0)
        {
            Finished = true;
            //TODO: We do something here if we finish (Like timescale to zero, record our logs and time, etc.)
            return true;
        }
        return false;
    }

    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

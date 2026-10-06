using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public UInt16? RankInSequence=null;//What rank in sequence is this checkpoint? Checkpoints with the same rank are considered linked
    public int MinRankToOpen = 0;//Should this checkpoint be locked until we pass a checkpoint with a rank greater than specified?
    public float TimePassedInBestAttempt = -1;//TODO: Make Functional later: float for best time in replay we load at start once replay/logging system is functional

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (RankInSequence == null) CheckpointManager.Instance.AddCheckpointToList(this);
        else CheckpointManager.Instance.AddCheckpointToList(this, (int)RankInSequence);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Helper to determine if we can currently complete the given checkpoint.
    /// </summary>
    /// <returns>True if checkpoint can be passed, False otherwise</returns>
    private bool CanPass()
    {
        return CheckpointManager.Instance.MaxCheckpointOrdPassed >= MinRankToOpen;
    }
    private bool TryPass()
    {
        if (CanPass())
        {
            return CheckpointManager.Instance.SetCheckpointLevelComplete(this);            
        }
        return false;
    }
}

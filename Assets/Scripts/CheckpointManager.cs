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

    private SortedList<int, List<GameObject>> Checkpoints= new SortedList<int, List<GameObject>>();
    public int currentPriority { get; private set; } = 0;
    public bool AddCheckpointToList(GameObject Checkpoint, int priority=-1)
    {
        //TODO
        return false;//Failure to add.
    }
    public bool RemoveCheckpointFromList(GameObject Checkpoint)
    { 
        //TODO
        return false;//Failure to remove.
    }
    public bool SetCheckpointComplete(GameObject Checkpoint)
    {
        //TODO
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

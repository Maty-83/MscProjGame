using TMPro;
using UnityEngine;

public class CheckpointCounterUI : MonoBehaviour
{
    public TMP_Text text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var cpm = CheckpointManager.Instance;
        if (cpm.Finished)
        {
            text.text = "FINISHED!";
        }
        else
        {
            var remaining = cpm.GetTotalRemainingCheckpoints();
            var total = cpm.GetTotalCheckpoints();
            text.text = $"Checkpoints: {total-remaining}/{total}";
        }
    }
}

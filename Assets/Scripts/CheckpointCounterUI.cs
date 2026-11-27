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
        var lvm = LevelManager.Instance;
        if (lvm.Finished)
        {
            text.text = "FINISH!";
        }
        else
        {
            var remaining = lvm.GetTotalRemainingCheckpoints();
            var total = lvm.GetTotalCheckpoints();
            text.text = $"{total-remaining}/{total}";
        }
    }
}

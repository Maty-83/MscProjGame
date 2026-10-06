using UnityEngine;

/// <summary>
/// Timer using unscaled time as a base
/// </summary>
public class Timer : MonoBehaviour
{
    private float StartTime = -1f;
    private float TimeElapsedBeforePause = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Starts (Or unpauses) the timer
    /// </summary>
    public void StartTimer()
    {
        StartTime = Time.unscaledTime;
    }
    public float GetTimeSinceStart()
    {
        float CurTime = Time.unscaledTime - StartTime;

        if (StartTime >= 0 && TimeElapsedBeforePause >= 0) return CurTime + TimeElapsedBeforePause;
        return TimeElapsedBeforePause; //Timer has been paused, so return time elapsed before pause (Only nonzero on pause)
    }
    /// <summary>
    /// Resets the timer to zero.
    /// </summary>
    public void ResetTimer()
    {
        StartTime = -1f;
        TimeElapsedBeforePause = 0f;
    }
    public void PauseTimer()
    {
        TimeElapsedBeforePause = GetTimeSinceStart();
        StartTime = -1f;
    }
    public void RestartTimer()
    {
        ResetTimer();
        StartTimer();
    }
}

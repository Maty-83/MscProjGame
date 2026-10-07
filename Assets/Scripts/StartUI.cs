using UnityEngine;
using TMPro;
using System.Collections;
using DG.Tweening;

public class StartUI : MonoBehaviour
{
    public uint CountdownTimeSeconds=3;
    public TMP_Text CountdownText;
    public Timer timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {        
        StartCoroutine(DoCountdown(CountdownTimeSeconds));
    }
    /// <summary>
    /// Coroutine function designed to serve as realtime countdown
    /// </summary>
    IEnumerator DoCountdown(uint seconds)
    {
        Time.timeScale = 0;
        while (seconds > 0)
        {                       
            CountdownText.transform.DOScale(Vector3.one*0.5f, 0.5f).SetUpdate(true);
            CountdownText.text = seconds.ToString();
            seconds--;
            yield return new WaitForSecondsRealtime(1);
            CountdownText.transform.localScale = Vector3.one;
        }
        CountdownText.text = "START!";
        Time.timeScale = 1f;
        if (timer != null) timer.StartTimer();
        CountdownText.transform.DOScale(Vector3.one * 0.5f, 0.5f);
        yield return new WaitForSecondsRealtime(0.5f);
        CountdownText.text = "";
        CountdownText.transform.localScale = Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

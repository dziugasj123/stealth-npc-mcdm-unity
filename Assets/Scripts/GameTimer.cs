using UnityEngine;
using TMPro;


public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance;

    private float elapsedTime;
    private bool isTiming;

    public TextMeshProUGUI liveTimerText;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (isTiming)
        {
            elapsedTime += Time.unscaledDeltaTime;

            if (liveTimerText != null)
            {
                liveTimerText.text = $"{elapsedTime:F2} s";
            }
        }
    }


    public void StartTimer()
    {
        elapsedTime = 0f;
        isTiming = true;
    }

    public void StopAndReport()
    {
        isTiming = false;
        Debug.Log($" {elapsedTime:F2} seconds");
    }

    public void ResetTimer() 
    {
        elapsedTime = 0f;
        isTiming = false;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

}

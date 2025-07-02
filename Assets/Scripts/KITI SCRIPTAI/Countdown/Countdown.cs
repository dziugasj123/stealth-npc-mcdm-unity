using UnityEngine;
using TMPro;
using System.Collections;

public class Countdown : MonoBehaviour
{
    public float startTime;
    public TextMeshProUGUI countdownText;
    public CanvasGroup fadeGroup;
    public float fadeDuration;

    private float timer;
    public bool countdownDone = false;

    void Start()
    {
        Time.timeScale = 1f;
        countdownDone = false;
        timer = startTime;
        GameTimer.Instance.ResetTimer();

        countdownText.gameObject.SetActive(true);

        if (fadeGroup != null)
        {
            fadeGroup.alpha = 0.8f;
            fadeGroup.gameObject.SetActive(true);
        }
    }



    void Update()
    {
        if (countdownDone) return;

        timer -= Time.deltaTime;

        if (timer > 0f)
        {
            countdownText.text = Mathf.CeilToInt(timer).ToString();
        }
        else
        {
            countdownText.text = "Start!";
            countdownDone = true;

            GameTimer.Instance.StartTimer();

            if (fadeGroup != null)
                StartCoroutine(FadeOutRoutine());

            Invoke(nameof(HideUI), fadeDuration);
        }
    }

    IEnumerator FadeOutRoutine()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        fadeGroup.alpha = 0f;
        fadeGroup.gameObject.SetActive(false);
    }

    void HideUI()
    {
        countdownText.gameObject.SetActive(false);
    }
}

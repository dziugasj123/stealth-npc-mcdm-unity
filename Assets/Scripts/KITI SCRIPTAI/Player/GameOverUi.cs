using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUi : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI resultText;

    private void Awake()
    {
        panel.SetActive(false);
    }

    public void ShowGameOver()
    {
        panel.SetActive(true);
    }

    public void UpdateResultText(string text)
    {
        if (resultText != null)
        {
            resultText.text = text;
        }
    }


    public void RestartGame()
    {
        Time.timeScale = 1f;


        if (GameTimer.Instance != null)
            Destroy(GameTimer.Instance.gameObject);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}

using UnityEngine;
using TMPro;

public class VisibilityCheck : MonoBehaviour
{
    public Transform npc;
    public float maxViewDistance = 50f;
    public float viewAngle = 60f;
    public LayerMask obstructionMask;

    private bool hasSeenNpc = false;

    public GameOverUi gameOverUi;

    void Update()
    {
        if (hasSeenNpc) return;

        Vector3 dirToNpc = npc.position - transform.position;
        float distanceToNpc = dirToNpc.magnitude;

        if (distanceToNpc < maxViewDistance)
        {
            float angle = Vector3.Angle(transform.forward, dirToNpc.normalized);
            if (angle < viewAngle / 2f)
            {
                if (!Physics.Raycast(transform.position, dirToNpc.normalized, out RaycastHit hit, distanceToNpc, obstructionMask))
                {
                    hasSeenNpc = true;
                    EndGame();
                }
            }
        }
    }

    void EndGame()
    {
        Debug.Log("Žaidimas užbaigtas");

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        float elapsed = GameTimer.Instance.GetElapsedTime();
        gameOverUi.ShowGameOver();
        gameOverUi.UpdateResultText($"Suradai agentą per: {elapsed:F2} s");

        GameTimer.Instance.liveTimerText.gameObject.SetActive(false);

    }
}

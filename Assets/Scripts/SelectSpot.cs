using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class SelectSpot : MonoBehaviour
{
    private List<List<Node>> npcPaths = new List<List<Node>>();
    private List<List<Node>> playerPaths = new List<List<Node>>();

    [Header("Žaidėjas")]
    public Transform player;

    [Header("WSM Svoriai (safety+distance+escape = 1)")]
    [SerializeField] private float safetyWeight = 0.5f;
    [SerializeField] private float distanceWeight = 0.2f;
    [SerializeField] private float escapeWeight = 0.3f;

    [Header("Žaidėjas --> Hiding Spots; NPC --> Hiding Spots")]
    public bool showPlayerPaths = false;
    public bool showNPCPaths = false;

    [Header("Kiekviena slėptuvė")]
    public bool sleptuvesIvertinimas = false;

    [Header("Slėpimosi vietos")]
    public List<RatingSpot> hidingSpots;

    private Pathfinding pathfinder;

    private float gizmoLaikas = 0f;
    private const float gizmoRodymas = 5f; 
    private bool gizmoDraw = false;

    int adjustedSafetyRating;
    int distanceRating;


    private void Start()
    {
        pathfinder = FindObjectOfType<Pathfinding>();
    }

    private void Update()
    {
        if (gizmoDraw)
        {
            gizmoLaikas -= Time.deltaTime;
            if (gizmoLaikas <= 0f)
            {
                gizmoDraw = false;
                npcPaths.Clear();
                playerPaths.Clear();
            }
        }
    }

    public Transform ChooseBestSpot(Vector3 npcPosition)
    {
        RatingSpot bestSpot = null;
        float bestScore = float.MinValue;

        npcPaths.Clear();
        playerPaths.Clear();

        gizmoLaikas = gizmoRodymas;
        gizmoDraw = true;

        for (int i = 0; i < hidingSpots.Count; i++)
        {
            RatingSpot spot = hidingSpots[i];

            List<Node> npcToSpotPath = pathfinder.GetPathNodesDistance(npcPosition, spot.hidingSpot.position);
            int npcToSpotTiles = npcToSpotPath != null ? npcToSpotPath.Count : int.MaxValue;
            if (npcToSpotTiles == 0) npcToSpotTiles = 1;
            npcPaths.Add(npcToSpotPath);

            List<Node> playerToSpotPath = pathfinder.GetPathNodesDistance(player.position, spot.hidingSpot.position);
            int playerToSpotTiles = playerToSpotPath != null ? playerToSpotPath.Count : int.MaxValue;
            if (playerToSpotTiles == 0) playerToSpotTiles = 1;
            playerPaths.Add(playerToSpotPath);

            if (playerToSpotTiles < 31)
                adjustedSafetyRating = 1;
            else if (playerToSpotTiles < 41)
                adjustedSafetyRating = Mathf.Max(1, Mathf.RoundToInt(spot.safetyRating / 1.8f));
            else if (playerToSpotTiles < 51)
                adjustedSafetyRating = Mathf.Max(1, Mathf.RoundToInt(spot.safetyRating / 1.6f));
            else if (playerToSpotTiles < 61)
                adjustedSafetyRating = Mathf.Max(1, Mathf.RoundToInt(spot.safetyRating / 1.4f));
            else if (playerToSpotTiles < 71)
                adjustedSafetyRating = Mathf.Max(1, Mathf.RoundToInt(spot.safetyRating / 1.2f));
            else
                adjustedSafetyRating = spot.safetyRating;

            if (npcToSpotTiles > 60)
                distanceRating = 4;
            else
                distanceRating = Mathf.Clamp(10 - (npcToSpotTiles / 10), 1, 10);

            float score = adjustedSafetyRating * safetyWeight + distanceRating * distanceWeight + spot.escapeRouteRating * escapeWeight;

            if (sleptuvesIvertinimas)
            {
                Debug.Log($"[{spot.hidingSpot.name}] Saugumo įvertinimas: {adjustedSafetyRating}, Atstumo įvertinimas: {distanceRating}, Pabėgimo įvertinimas: {spot.escapeRouteRating} WSM įvertinimas: {score:F2}");
            }


            if (score > bestScore)
            {
                bestScore = score;
                bestSpot = spot;
            }
        }

        if (bestSpot != null)
        {
            Debug.Log($"Pasirinkta slėptuvė: {bestSpot.hidingSpot.name}");
        }

        return bestSpot?.hidingSpot;
    }


    private void OnDrawGizmos()
    {
        if (showPlayerPaths)
        {
            Gizmos.color = Color.red;
            foreach (var path in playerPaths)
            {
                if (path == null || path.Count < 2)
                    continue;

                for (int i = 0; i < path.Count - 1; i++)
                {
                    Gizmos.DrawLine(path[i].worldPosition, path[i + 1].worldPosition);
                }
            }
        }

        if (showNPCPaths)
        {
            Gizmos.color = Color.green;
            foreach (var path in npcPaths)
            {
                if (path == null || path.Count < 2)
                    continue;

                for (int i = 0; i < path.Count - 1; i++)
                {
                    Gizmos.DrawLine(path[i].worldPosition, path[i + 1].worldPosition);
                }
            }
        }
    }


}

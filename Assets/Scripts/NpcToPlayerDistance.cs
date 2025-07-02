using UnityEngine;
using System.Collections.Generic;

public class NPCToPlayerDistance : MonoBehaviour
{
    public Transform player;
    private Pathfinding pathfinder;
    private Countdown countdown;
    private SelectSpot selectHidingSpot;

    public bool isPlayerClose = false;

    private float checkCooldown = 0f;
    private float gizmoShowTimer = 0f;

    private List<Node> npcToPlayerPath = null;
    private List<List<Node>> playerPathss = new List<List<Node>>();

    private void Start()
    {
        pathfinder = FindObjectOfType<Pathfinding>();
        selectHidingSpot = FindObjectOfType<SelectSpot>();
        countdown = FindObjectOfType<Countdown>();
    }

    private void Update()
    {
        checkCooldown -= Time.deltaTime;
        if (checkCooldown <= 0f)
        {
            CheckDistanceToPlayer();
            checkCooldown = 0.2f;
        }

        if (gizmoShowTimer > 0f)
        {
            gizmoShowTimer -= Time.deltaTime;
        }
    }

    void CheckDistanceToPlayer()
    {
        if (!countdown.countdownDone || player == null || pathfinder == null)
            return;

        npcToPlayerPath = pathfinder.GetPathNodesDistance(transform.position, player.position);

        if (npcToPlayerPath != null)
        {
            if (npcToPlayerPath.Count <= 20)
            {
                if (!isPlayerClose)
                {
                    Debug.Log("Žaidėjas yra arti NPC");
                    isPlayerClose = true;
                }

                gizmoShowTimer = 2f;

                playerPathss.Clear();
                if (selectHidingSpot != null)
                {
                    foreach (RatingSpot spot in selectHidingSpot.hidingSpots)
                    {
                        var path = pathfinder.GetPathNodesDistance(player.position, spot.hidingSpot.position);
                        if (path != null)
                            playerPathss.Add(path);
                    }
                }
            }
            else
            {
                if (isPlayerClose)
                {
                    isPlayerClose = false;
                }
                playerPathss.Clear();
            }
        }
        else
        {
            npcToPlayerPath = null;
            playerPathss.Clear();
        }
    }
}

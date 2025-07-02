using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    [Header("NPC Greitis")]
    public float speed = 20f;


    private Transform target;
    private Vector3[] path;
    private int targetIndex;
    private Coroutine followPathCoroutine;
    private List<Node> myBlockedNodes = new List<Node>();
    private NPCToPlayerDistance playerClose;

    private SelectSpot hidingSpotSelector;

    private float pathCooldown = 0f;
    private float cooldownDuration = 5f;

    void Start()
    {
        playerClose = GetComponent<NPCToPlayerDistance>();
        hidingSpotSelector = FindObjectOfType<SelectSpot>();

        if (hidingSpotSelector != null && hidingSpotSelector.hidingSpots.Count > 0)
        {
            int randomIndex = Random.Range(0, hidingSpotSelector.hidingSpots.Count);
            RatingSpot randomSpot = hidingSpotSelector.hidingSpots[randomIndex];

            target = randomSpot.hidingSpot;

            Vector3 spawnPos = target.position;
            spawnPos.y += 1f;
            transform.position = spawnPos;
        }
    }



    void Update()
    {
        if (pathCooldown > 0f)
            pathCooldown -= Time.deltaTime;

        if (playerClose != null && hidingSpotSelector != null)
        {
            if (playerClose.isPlayerClose)
            {
                if (pathCooldown <= 0f)
                {
                    
                    Transform bestSpot = hidingSpotSelector.ChooseBestSpot(transform.position);

                    if (bestSpot != null && bestSpot != target)
                    {
                        target = bestSpot;
                        RequestNewPath();
                    }
                    
                    pathCooldown = cooldownDuration;
                }
            }

        }
    }


    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            targetIndex = 0;

            if (followPathCoroutine != null)
                StopCoroutine(followPathCoroutine);

            followPathCoroutine = StartCoroutine(FollowPath());
        }
    }

    IEnumerator FollowPath()
    {
        if (path == null || path.Length == 0)
            yield break;

        Vector3 currentWaypoint = path[0];

        while (true)
        {
            if (Vector3.Distance(transform.position, currentWaypoint) < 0.1f)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }

                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    void RequestNewPath()
    {
        if (target != null)
        {
            Grid grid = FindObjectOfType<Grid>();
            NPCToPlayerDistance playerDistance = FindObjectOfType<NPCToPlayerDistance>();

            if (grid != null && playerDistance != null)
            {
                Transform player = playerDistance.player;
                Transform viewSource = Camera.main != null ? Camera.main.transform : player;

                grid.UnblockTiles(myBlockedNodes);

                myBlockedNodes = grid.BlockTiles(player, viewSource.forward);
            }

            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        }
    }



    void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                    Gizmos.DrawLine(transform.position, path[i]);
                else
                    Gizmos.DrawLine(path[i - 1], path[i]);
            }
        }
    }
}

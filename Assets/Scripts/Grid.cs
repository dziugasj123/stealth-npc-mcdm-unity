using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Grid : MonoBehaviour
{
    public bool displayGridGizmos;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;

    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    private List<(Node node, bool wasWalkable)> lastBlockedNodes = new List<(Node, bool)>();

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;


        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius * 0.8f, unwalkableMask);
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = Mathf.Clamp01((worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x);
        float percentY = Mathf.Clamp01((worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y);
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    neighbours.Add(grid[checkX, checkY]);
            }
        }

        return neighbours;
    }

    public List<Node> BlockTiles(Transform playerTransform, Vector3 viewDirection, float maxViewDistance = 50f, float viewAngle = 60f)
    {
        List<Node> blockedNodes = new List<Node>();

        Vector3 origin = playerTransform.position;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Node node = grid[x, y];

                if (!node.walkable)
                    continue;

                Vector3 dirToNode = (node.worldPosition - origin).normalized;
                float distanceToNode = Vector3.Distance(origin, node.worldPosition);

                if (distanceToNode <= maxViewDistance)
                {
                    float angle = Vector3.Angle(viewDirection, dirToNode);

                    if (angle < viewAngle / 2f)
                    {
                        if (!Physics.Raycast(origin, dirToNode, distanceToNode, LayerMask.GetMask("WALL")))
                        {
                            node.walkable = false;
                            blockedNodes.Add(node);
                        }
                    }
                }
            }
        }

        return blockedNodes;
    }

    public void UnblockTiles(List<Node> nodesToUnblock)
    {
        if (nodesToUnblock == null) return;

        foreach (Node node in nodesToUnblock)
        {
            if (node != null)
                node.walkable = true;
        }

        nodesToUnblock.Clear();
    }

    public int MaxSize => gridSizeX * gridSizeY;

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (grid != null && displayGridGizmos)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }
}

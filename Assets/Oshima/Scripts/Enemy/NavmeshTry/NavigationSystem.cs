using System.Collections.Generic;
using UnityEngine;

public class NavigationSystem : MonoBehaviour
{
    public int gridSize = 10;
    public LayerMask obstacleLayer;

    private bool[,] grid;

    void Start()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        grid = new bool[gridSize, gridSize];

        Collider[] obstacles = Physics.OverlapSphere(transform.position, 50f, obstacleLayer);
        foreach (var obstacle in obstacles)
        {
            Vector3 obstaclePosition = obstacle.transform.position;
            int x = Mathf.RoundToInt(obstaclePosition.x);
            int z = Mathf.RoundToInt(obstaclePosition.z);

            if (x >= 0 && x < gridSize && z >= 0 && z < gridSize)
            {
                grid[x, z] = true;
            }
        }
    }

    public List<Vector3> FindPath(Vector3 start, Vector3 end)
    {
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();

        Node startNode = new Node(start);
        Node endNode = new Node(end);

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentNode.fCost || (openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == endNode)
            {
                return RetracePath(startNode, endNode);
            }

            List<Node> neighbors = GetNeighbors(currentNode);

            foreach (Node neighbor in neighbors)
            {
                if (!closedList.Contains(neighbor))
                {
                    int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                    if (newCostToNeighbor < neighbor.gCost || !openList.Contains(neighbor))
                    {
                        neighbor.gCost = newCostToNeighbor;
                        neighbor.hCost = GetDistance(neighbor, endNode);
                        neighbor.parent = currentNode;

                        if (!openList.Contains(neighbor))
                        {
                            openList.Add(neighbor);
                        }
                    }
                }
            }
        }

        return null;
    }

    List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        int[] dx = { 0, 0, 1, -1 }; // 右、左、上、下
        int[] dz = { 1, -1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            int newX = Mathf.RoundToInt(node.position.x) + dx[i];
            int newZ = Mathf.RoundToInt(node.position.z) + dz[i];

            if (newX >= 0 && newX < gridSize && newZ >= 0 && newZ < gridSize && !grid[newX, newZ])
            {
                neighbors.Add(new Node(new Vector3(newX, 0, newZ)));
            }
        }

        return neighbors;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(Mathf.RoundToInt(nodeA.position.x) - Mathf.RoundToInt(nodeB.position.x));
        int distZ = Mathf.Abs(Mathf.RoundToInt(nodeA.position.z) - Mathf.RoundToInt(nodeB.position.z));

        return distX + distZ;
    }

    List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        List<Vector3> vectorPath = new List<Vector3>();
        foreach (Node node in path)
        {
            vectorPath.Add(node.position);
        }

        return vectorPath;
    }
}

using UnityEngine;
using System.Collections.Generic;

public class CustomPathfinding : MonoBehaviour
{
    public Transform startPoint; // 開始地点
    public Transform endPoint;   // 終了地点

    public List<Transform> path; // 最終的な経路

    void Start()
    {
        path = FindPath(startPoint, endPoint);
    }

    List<Transform> FindPath(Transform start, Transform end)
    {
        List<Transform> openList = new List<Transform>();
        List<Transform> closedList = new List<Transform>();

        openList.Add(start);

        while (openList.Count > 0)
        {
            Transform currentNode = openList[0];

            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].GetComponent<Node1>().fCost < currentNode.GetComponent<Node1>().fCost ||
                    (openList[i].GetComponent<Node1>().fCost == currentNode.GetComponent<Node1>().fCost &&
                    openList[i].GetComponent<Node1>().hCost < currentNode.GetComponent<Node1>().hCost))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == end)
            {
                return RetracePath(start, end);
            }

            foreach (Transform neighborNode in currentNode.GetComponent<Node1>().neighbors)
            {
                if (!neighborNode.GetComponent<Node1>().walkable || closedList.Contains(neighborNode))
                {
                    continue;
                }

                int newMovementCostToNeighbor = currentNode.GetComponent<Node1>().gCost + GetDistance(currentNode, neighborNode);

                if (newMovementCostToNeighbor < neighborNode.GetComponent<Node1>().gCost || !openList.Contains(neighborNode))
                {
                    neighborNode.GetComponent<Node1>().gCost = newMovementCostToNeighbor;
                    neighborNode.GetComponent<Node1>().hCost = GetDistance(neighborNode, end);
                    neighborNode.GetComponent<Node1>().parent = currentNode;

                    if (!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                }
            }
        }

        return null; // パスが見つからなかった場合
    }

    List<Transform> RetracePath(Transform startNode, Transform endNode)
    {
        List<Transform> path = new List<Transform>();
        Transform currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.GetComponent<Node1>().parent;
        }

        path.Reverse();
        return path;
    }

    int GetDistance(Transform nodeA, Transform nodeB)
    {
        int dstX = Mathf.Abs(nodeA.GetComponent<Node1>().gridX - nodeB.GetComponent<Node1>().gridX);
        int dstY = Mathf.Abs(nodeA.GetComponent<Node1>().gridY - nodeB.GetComponent<Node1>().gridY);

        return dstX + dstY;
    }
}

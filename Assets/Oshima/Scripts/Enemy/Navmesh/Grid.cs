//using UnityEngine;
//using System.Collections.Generic;

//public class Grid
//{
//    public Node[,] grid;
//    public Vector2 gridSize;

//    public Grid(int width, int height, float cellSize)
//    {
//        gridSize = new Vector2(width, height);
//        int gridWidth = Mathf.RoundToInt(width / cellSize);
//        int gridHeight = Mathf.RoundToInt(height / cellSize);

//        grid = new Node[gridWidth, gridHeight];

//        Vector3 worldBottomLeft = Vector3.zero - Vector3.right * width / 2 - Vector3.forward * height / 2;

//        for (int x = 0; x < gridWidth; x++)
//        {
//            for (int y = 0; y < gridHeight; y++)
//            {
//                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * cellSize + cellSize / 2) + Vector3.forward * (y * cellSize + cellSize / 2);
//                bool walkable = !Physics.CheckSphere(worldPoint, cellSize / 2, LayerMask.GetMask("Obstacle"));

//                grid[x, y] = new Node(walkable, worldPoint, x, y);
//            }
//        }
//    }

//    public Node NodeFromWorldPoint(Vector3 worldPosition)
//    {
//        float percentX = (worldPosition.x + gridSize.x / 2) / gridSize.x;
//        float percentY = (worldPosition.z + gridSize.y / 2) / gridSize.y;

//        percentX = Mathf.Clamp01(percentX);
//        percentY = Mathf.Clamp01(percentY);

//        int x = Mathf.RoundToInt((grid.GetLength(0) - 1) * percentX);
//        int y = Mathf.RoundToInt((grid.GetLength(1) - 1) * percentY);

//        return grid[x, y];
//    }

//    public List<Node> GetNeighbors(Node node)
//    {
//        List<Node> neighbors = new List<Node>();

//        for (int x = -1; x <= 1; x++)
//        {
//            for (int y = -1; y <= 1; y++)
//            {
//                if (x == 0 && y == 0)
//                    continue;

//                int checkX = node.gridX + x;
//                int checkY = node.gridY + y;

//                if (checkX >= 0 && checkX < grid.GetLength(0) && checkY >= 0 && checkY < grid.GetLength(1))
//                {
//                    neighbors.Add(grid[checkX, checkY]);
//                }
//            }
//        }

//        return neighbors;
//    }
//}

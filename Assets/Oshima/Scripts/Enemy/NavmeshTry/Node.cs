using UnityEngine;

public class Node
{
    public Vector3 position;
    public int gCost;
    public int hCost;
    public int fCost => gCost + hCost;
    public Node parent;

    public Node(Vector3 pos)
    {
        position = pos;
    }
}

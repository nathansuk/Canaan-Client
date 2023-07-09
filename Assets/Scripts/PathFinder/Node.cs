using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Node
{
    public Vector3Int cellPosition;
    public Node parent;
    public int gCost; // Co�t de mouvement depuis le n�ud de d�part
    public int hCost; // Estimation du co�t de mouvement jusqu'au n�ud cible

    public int fCost => gCost + hCost;

    public Node(Vector3Int cellPosition, Node parent)
    {
        this.cellPosition = cellPosition;
        this.parent = parent;
        gCost = 0;
        hCost = 0;
    }

    public override string ToString()
    {
        return cellPosition.ToString();
    }
}
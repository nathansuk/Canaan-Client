using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Node
{
    public Vector3Int cellPosition;
    public Node parent;
    public int gCost; // Coût de mouvement depuis le nœud de départ
    public int hCost; // Estimation du coût de mouvement jusqu'au nœud cible

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
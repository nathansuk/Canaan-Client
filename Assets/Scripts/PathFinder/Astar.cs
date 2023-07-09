using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class AStar
{
    public static bool isCalculatingPath = false;
    public static List<Vector3> FindPath(Vector3Int startCell, Vector3Int targetCell)
    {
        Debug.Log("Recherche du chemin");
        isCalculatingPath = true;

        Grid grid = GameObject.Find("GridPlayer").GetComponent<Grid>();

        // Créer les nœuds de départ et d'arrivée
        Node startNode = new Node(startCell, null);
        Node targetNode = new Node(targetCell, null);

        // Initialiser les listes ouvertes et fermées
        List<Node> openList = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        // Ajouter le nœud de départ à la liste ouverte
        Debug.Log("Premier noeud" + startNode);

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            // Sélectionner le nœud avec le coût le plus bas de la liste ouverte

            Node currentNode = openList[0];
            Debug.Log("Current node" + startNode.ToString());

            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentNode.fCost || (openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost))
                {
                    currentNode = openList[i];
                    Debug.Log("Node" + currentNode.ToString());
                }
            }

            // Retirer le nœud sélectionné de la liste ouverte et l'ajouter à la liste fermée
            openList.Remove(currentNode);
            closedSet.Add(currentNode);

            // Vérifier si le nœud actuel est le nœud cible
            if (currentNode.cellPosition == targetNode.cellPosition)
            {
                Debug.Log("Cible atteinte");
                // Construire le chemin en remontant de la cible au départ
                return RetracePath(startNode, currentNode);
            }

            // Générer les voisins du nœud actuel
            List<Node> neighbors = GetNeighbors(currentNode, grid);
            foreach (Node neighbor in neighbors)
            {
                // Vérifier si le voisin est déjà dans la liste fermée
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                // Calculer le coût du mouvement vers le voisin
                int movementCost = currentNode.gCost + GetDistance(currentNode, neighbor);

                // Vérifier si le voisin a un coût de mouvement plus bas ou s'il n'est pas dans la liste ouverte
                if (movementCost < neighbor.gCost || !openList.Contains(neighbor))
                {
                    // Mettre à jour les coûts et le parent du voisin
                    neighbor.gCost = movementCost;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    // Ajouter le voisin à la liste ouverte s'il n'est pas déjà présent
                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                        Debug.Log("Ajout du neighbor" + neighbor);
                    }
                }
            }
        }

        isCalculatingPath = false;
        // Aucun chemin n'a été trouvé
        return null;
    }

    private static List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        Debug.Log("Retracage du path");
        List<Vector3> path = new List<Vector3>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.cellPosition);
            currentNode = currentNode.parent;
        }

        isCalculatingPath = false;
        path.Reverse();
        return path;
    }

    private static int GetDistance(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.cellPosition.x - nodeB.cellPosition.x);
        int distanceY = Mathf.Abs(nodeA.cellPosition.y - nodeB.cellPosition.y);
        Debug.Log("Distances X : " + distanceX + "Y : " + distanceY);
        return distanceX + distanceY;
    }

    private static List<Node> GetNeighbors(Node node, Grid grid)
    {
        List<Node> neighbors = new List<Node>();
        Vector3Int[] directions =
        {
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.left,
        Vector3Int.right,
        new Vector3Int(-1, 1, 0), // diagonale haut-gauche
        new Vector3Int(1, 1, 0),  // diagonale haut-droite
        new Vector3Int(-1, -1, 0), // diagonale bas-gauche
        new Vector3Int(1, -1, 0)  // diagonale bas-droite
    };

        foreach (Vector3Int direction in directions)
        {
            Vector3Int neighborCell = node.cellPosition + direction;

            Node neighbor = new Node(neighborCell, null);
            neighbors.Add(neighbor);
        }

        return neighbors;
    }
}
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public Transform targetPosition;
    public float moveSpeed = 5f;

    private bool isWalking = false;
    private List<Vector3> path = new List<Vector3>();

    private void OnGUI()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SetTargetPosition(mousePosition);
        }

        if (path != null && path.Count > 0)
        {
            Move();
        }
    }

    public void SetTargetPosition(Vector3 target)
    {
        Vector3Int targetCell = GameObject.Find("GridPlayer").GetComponent<GridLayout>().WorldToCell(target);
        Vector3Int playerCell = GameObject.Find("GridPlayer").GetComponent<GridLayout>().WorldToCell(transform.position);

        Debug.Log(targetCell + ", " + playerCell);

        if (targetCell != playerCell)
        {
            path = AStar.FindPath(playerCell, targetCell);
        }
    }

    private void Move()
    {
        Debug.Log("Bouge");
        PlayerComponent player = GetComponent<PlayerComponent>();
        if (path != null && path.Count > 0)
        {
            Vector3 targetPosition = path[0];
            player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (player.transform.position == targetPosition)
            {
                path.RemoveAt(0);

                if (path.Count == 0)
                {
                    isWalking = false;
                }
            }
        }
    }
}
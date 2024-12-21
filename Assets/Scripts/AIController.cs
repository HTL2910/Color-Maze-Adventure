using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public GameObject player; 
    private Vector2Int currentPos;
 
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentPos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        InvokeRepeating("Move", 1f, 3f);
    }
    private void Move()
    {
        if(player==null)return;
        Vector2Int playerPos = new Vector2Int(Mathf.RoundToInt(player.transform.position.x), Mathf.RoundToInt(player.transform.position.y));
        Vector2Int nextStep = GetNextStepTowardsPlayer(currentPos, playerPos);
        if (nextStep != currentPos)
        {
            currentPos = nextStep;
            transform.position = new Vector3(currentPos.x, currentPos.y, transform.position.z);
        }
    }


    Vector2Int GetNextStepTowardsPlayer(Vector2Int current, Vector2Int target)
    {
        if (current == target) return current;
        Vector2Int[] directions = {
            new Vector2Int(0, 1),  // Lên
            new Vector2Int(0, -1), // Xuống
            new Vector2Int(1, 0),  // Phải
            new Vector2Int(-1, 0)  // Trái
        };

        Vector2Int bestStep = current;
        float minDistance = float.MaxValue;

        foreach (Vector2Int dir in directions)
        {
            Vector2Int nextPos = current + dir;

            if (IsWalkable(nextPos))
            {
                float distance = Vector2.Distance(nextPos, target);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestStep = nextPos;
                }
            }
        }

        return bestStep;
    }

    bool IsWalkable(Vector2Int pos)
    {
        int width = GameManager.Instance.walkableMap.GetLength(0);
        int height = GameManager.Instance.walkableMap.GetLength(1);

        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height && GameManager.Instance.walkableMap[pos.x, pos.y];
    }
}

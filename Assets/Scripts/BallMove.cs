using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMove : MonoBehaviour
{
    public Vector3 firstTouchPosition;
    public Vector3 finalTouchPosition;
    public float SwipeAngle = 0f;
    public float swipeResist = 1f;
    public float speed = 5f;
    protected Vector2 moveDirection;
     
    private void Update()
    {
        if (moveDirection == Vector2.zero)
        {
            if (Input.GetMouseButtonDown(0))
            {
                firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0))
            {
                finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                CalculateAngle();
            }
        }
    }
 

    protected void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist ||
            Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            SwipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            DetermineMoveDirection();
            StartCoroutine(MoveBall());
        }
    }

    private void DetermineMoveDirection()
    {
        if (IsRightDirection())
        {
            MoveRight();
        }
        else if (IsUpDirection())
        {
            MoveUp();
        }
        else if (IsLeftDirection())
        {
            MoveLeft();
        }
        else if (IsDownDirection())
        {
            MoveDown();
        }
    }

    // Hàm kiểm tra hướng Right
    private bool IsRightDirection()
    {
        return SwipeAngle > -45 && SwipeAngle <= 45;
    }

    // Hàm xử lý di chuyển Right
    private void MoveRight()
    {
        Debug.Log("Right");
        moveDirection = Vector2.right;
    }

    // Hàm kiểm tra hướng Up
    private bool IsUpDirection()
    {
        return SwipeAngle > 45 && SwipeAngle <= 135;
    }

    // Hàm xử lý di chuyển Up
    private void MoveUp()
    {
        Debug.Log("Up");
        moveDirection = Vector2.up;
    }

    // Hàm kiểm tra hướng Left
    private bool IsLeftDirection()
    {
        return SwipeAngle > 135 || SwipeAngle <= -135;
    }

    // Hàm xử lý di chuyển Left
    private void MoveLeft()
    {
        Debug.Log("Left");
        moveDirection = Vector2.left;
    }

    // Hàm kiểm tra hướng Down
    private bool IsDownDirection()
    {
        return SwipeAngle > -135 && SwipeAngle < -45;
    }

    // Hàm xử lý di chuyển Down
    private void MoveDown()
    {
        Debug.Log("Down");
        moveDirection = Vector2.down;
    }

    private IEnumerator MoveBall()
    {
        while (moveDirection != Vector2.zero)
        {
            Vector3 moveDelta = new Vector3(moveDirection.x, moveDirection.y, 0f) * speed * Time.deltaTime;

            if (Physics.Raycast(transform.position, moveDelta.normalized, out RaycastHit hit, moveDelta.magnitude))
            {
                moveDirection = Vector2.zero;
                SnapToGrid();
            }
            else
            {
                transform.position += moveDelta;
            }

            yield return null;
        }
    }

    private void SnapToGrid()
    {
        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.RoundToInt(newPosition.x);
        newPosition.y = Mathf.RoundToInt(newPosition.y);
        transform.position = newPosition;
    }
}
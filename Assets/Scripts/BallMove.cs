﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMove : MonoBehaviour
{
    public Vector3 firstTouchPosition;
    public Vector3 finalTouchPosition;
    public float SwipeAngle = 0f;
    public float swipeResist = 1f;
    public float speed = 5f;
    private Vector2 moveDirection;

    private void OnMouseDown()
    {
        if(moveDirection== Vector2.zero)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp()
    {
        if (moveDirection == Vector2.zero)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    private void CalculateAngle()
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
        if (SwipeAngle > -45 && SwipeAngle <= 45)
        {
            // Right
            moveDirection = Vector2.right;
        }
        else if (SwipeAngle > 45 && SwipeAngle <= 135)
        {
            // Up
            moveDirection = Vector2.up;
        }
        else if (SwipeAngle > 135 || SwipeAngle <= -135)
        {
            // Left
            moveDirection = Vector2.left;
        }
        else if (SwipeAngle > -135 && SwipeAngle < -45)
        {
            // Down
            moveDirection = Vector2.down;
        }
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
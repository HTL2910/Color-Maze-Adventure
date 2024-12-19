using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeDetection : MonoBehaviour
{
    private bool couldBeSwipe;
    private Vector2 startPos;
    private float swipeStartTime;

    public float maxSwipeTime = 0.5f; 
    public float minSwipeDist = 50f;       
    [SerializeField] float speed = 15f;
    [SerializeField] private Vector2 moveDirection;
    void Start()
    {
        StartCoroutine(CheckSwipes());
    }

    IEnumerator CheckSwipes()
    {
        while (true)
        {
            if(moveDirection == Vector2.zero)
            {
                foreach (Touch touch in Input.touches)
                {
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            couldBeSwipe = true;
                            startPos = touch.position;
                            swipeStartTime = Time.unscaledTime;
                            break;

                        case TouchPhase.Stationary:
                            couldBeSwipe = false;
                            break;

                        case TouchPhase.Ended:
                            if (couldBeSwipe)
                            {
                                float swipeTime = Time.unscaledTime - swipeStartTime;
                                Vector2 endPos = touch.position;
                                float swipeDist = (endPos - startPos).magnitude;

                                // Kiểm tra điều kiện vuốt
                                if (swipeTime < maxSwipeTime && swipeDist > minSwipeDist)
                                {
                                    DetectSwipeDirection(endPos - startPos);
                                }
                            }
                            break;
                    }
                }
            }
            
            yield return null;
        }
    }

    private void DetectSwipeDirection(Vector2 swipeVector)
    {
        if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
        {
            if (swipeVector.x > 0)
            {
                Debug.Log("Swipe Right");
                moveDirection = Vector2.right;
            }
            else
            {
                Debug.Log("Swipe Left");
                moveDirection = Vector2.left;
            }
        }
        else
        {
            if (swipeVector.y > 0)
            {
                Debug.Log("Swipe Up");
                moveDirection = Vector2.up;
            }
            else
            {
                Debug.Log("Swipe Down");
                moveDirection = Vector2.down;
            }
        }
    }

    void Update()
    {
        MoveBall();
    }

    private void MoveBall()
    {
        if (moveDirection != Vector2.zero)
        {
            Vector3 moveDelta = new Vector3(moveDirection.x, moveDirection.y, 0f) * speed * Time.deltaTime;
            
            if (Physics.Raycast(gameObject.transform.position, moveDelta.normalized, out RaycastHit hit, moveDelta.magnitude))
            {
                moveDirection = Vector2.zero;
            }
            else
            {
                gameObject.transform.position += moveDelta;
            }
        }
        else
        {
            Vector3 newPosition = gameObject.transform.position;
            newPosition.x = Mathf.RoundToInt(newPosition.x);
            newPosition.y = Mathf.RoundToInt(newPosition.y);
            gameObject.transform.position = newPosition;
        }
    }
 
}

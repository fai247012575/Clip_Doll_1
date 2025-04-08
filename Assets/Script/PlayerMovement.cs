using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeedX = 3f;
    public float moveSpeedY = 5f;
    public float returnSpeedY = 2f;

    public float defaultY;
    private float minX, maxX, minY, maxY;

    private bool movingRight = true;
    private bool isMovingDown = false;
    public bool isReturning = false;
    private bool isPausedX = false;

    private GameObject targetObject = null;

    // Start is called before the first frame update
    void Start()
    {
        defaultY = transform.position.y;

        transform.rotation = Quaternion.Euler(0, 0, 0);

        Camera cam = Camera.main;
        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));

        minX = bottomLeft.x + 0.5f;
        maxX = topRight.x - 0.5f;
        minY = bottomLeft.y + 0.5f;
        maxY = topRight.y - 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPausedX)
        {
            MoveOnX();
        }

        HandleYMovement();
    }

    void MoveOnX()
    {
        Vector3 newPosition = transform.position;

        if (movingRight)
        {
            newPosition.x += moveSpeedX * Time.deltaTime;
            if (newPosition.x >= maxX)
            {
                movingRight = false;
            }
        }
        else
        {
            newPosition.x -= moveSpeedX * Time.deltaTime;
            if (newPosition.x <= minX)
            {
                movingRight = true;
            }
        }

        transform.position = newPosition;
    }

    void HandleYMovement()
    {
        Vector3 newPosition = transform.position;

        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
        {
            isMovingDown = true;
            isReturning = false;
            isPausedX = true;
        }

        if (isMovingDown)
        {
            newPosition.y -= moveSpeedY * Time.deltaTime;

            if (newPosition.y <= minY)
            {
                newPosition.y = minY;
                isMovingDown = false;
                isReturning = true;
            }
        }

        if (isReturning)
        {
            float step = returnSpeedY * Time.deltaTime;
            newPosition.y += step;

            if (newPosition.y >= defaultY)
            {
                newPosition.y = defaultY;
                isReturning = false;
                isPausedX = false;

                transform.rotation = Quaternion.Euler(0, 0, 0);
                StopCircleFollowing();
            }
        }

        transform.position = newPosition;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision Entered with: " + collision.gameObject.name); // 記錄碰撞開始

        // 任何碰撞都會觸發返回
        targetObject = collision.gameObject;
        isReturning = true;
        isMovingDown = false;
        isPausedX = true;

        // 如果是 Circle，通知它跟隨
        if (collision.gameObject.CompareTag("Circle"))
        {
            CircleBoundary circleScript = targetObject.GetComponent<CircleBoundary>();
            if (circleScript != null)
            {
                circleScript.StartFollowing(this.gameObject);
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("Collision Staying with: " + collision.gameObject.name); // 記錄碰撞持續

        // 保持返回狀態
        isReturning = true;
        isMovingDown = false;
        isPausedX = true;

        // 如果是 Circle，保持跟隨
        if (collision.gameObject.CompareTag("Circle"))
        {
            CircleBoundary circleScript = collision.gameObject.GetComponent<CircleBoundary>();
            if (circleScript != null && targetObject == collision.gameObject)
            {
                circleScript.StartFollowing(this.gameObject); // 確保持續跟隨
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("Collision Exited with: " + collision.gameObject.name); // 記錄碰撞結束

        // 碰撞結束時不做任何操作，讓 Circle 繼續跟隨直到 Player 返回 defaultY
    }

    void UpdateBoundariesToMatchTarget()
    {
        if (targetObject != null)
        {
            Vector3 targetPosition = targetObject.transform.position;
            Vector3 targetScale = targetObject.transform.lossyScale;
            Vector3 targetBounds = targetScale * 0.5f;

            minX = targetPosition.x - targetBounds.x;
            maxX = targetPosition.x + targetBounds.x;
            minY = targetPosition.y - targetBounds.y;
            maxY = targetPosition.y + targetBounds.y;

            Vector3 newPosition = transform.position;
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
            transform.position = newPosition;
        }
    }

    void ResetBoundaries()
    {
        Camera cam = Camera.main;
        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));

        minX = bottomLeft.x + 0.5f;
        maxX = topRight.x - 0.5f;
        minY = bottomLeft.y + 0.5f;
        maxY = topRight.y - 0.5f;
    }

    void StopCircleFollowing()
    {
        if (targetObject != null)
        {
            CircleBoundary circleScript = targetObject.GetComponent<CircleBoundary>();
            if (circleScript != null)
            {
                circleScript.StopFollowing();
            }
            targetObject = null;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(minX, minY, 0), new Vector3(maxX, minY, 0));
        Gizmos.DrawLine(new Vector3(minX, maxY, 0), new Vector3(maxX, maxY, 0));
        Gizmos.DrawLine(new Vector3(minX, minY, 0), new Vector3(minX, maxY, 0));
        Gizmos.DrawLine(new Vector3(maxX, minY, 0), new Vector3(maxX, maxY, 0));
    }
}
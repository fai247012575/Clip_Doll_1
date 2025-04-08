using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleBoundary : MonoBehaviour
{
    private GameObject targetPlayer = null;
    private Vector3 offset;
    private bool isFollowing = false;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("CircleBoundary 需要 Rigidbody2D 組件！");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isFollowing && targetPlayer != null)
        {
            FollowPlayer();
        }
        
    }

    
    void FixedUpdate()
    {
        if (!isFollowing && rb != null)
        {
            
            Vector2 position = rb.position;
            Camera cam = Camera.main;
            Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));
            Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));

            float minX = bottomLeft.x + 0.5f;
            float maxX = topRight.x - 0.5f;
            float minY = bottomLeft.y + 0.5f;
            float maxY = topRight.y - 0.5f;

            if (position.x < minX) position.x = minX;
            else if (position.x > maxX) position.x = maxX;

            if (position.y < minY) position.y = minY;
            else if (position.y > maxY) position.y = maxY;

            rb.MovePosition(position);
        }
    }

    // 開始跟隨 Player
    public void StartFollowing(GameObject player)
    {
        targetPlayer = player;
        isFollowing = true;
        offset = transform.position - player.transform.position;
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    // 停止跟隨
    public void StopFollowing()
    {
        targetPlayer = null;
        isFollowing = false;
        if (rb != null)
        {
            
        }
    }

    
    void FollowPlayer()
    {
        if (targetPlayer != null)
        {
            Vector3 targetPosition = targetPlayer.transform.position;
            Vector3 newPosition = targetPosition + offset;

            
            Camera cam = Camera.main;
            Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));
            Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));

            float minX = bottomLeft.x + 0.5f;
            float maxX = topRight.x - 0.5f;
            float minY = bottomLeft.y + 0.5f;
            float maxY = topRight.y - 0.5f;

            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

            
            if (rb != null)
            {
                rb.MovePosition(newPosition);
            }
            else
            {
                transform.position = newPosition;
            }

            
            PlayerMovement playerScript = targetPlayer.GetComponent<PlayerMovement>();
            if (playerScript != null && playerScript.isReturning)
            {
                float step = playerScript.returnSpeedY * Time.deltaTime;
                if (newPosition.y < playerScript.defaultY)
                {
                    newPosition.y += step;
                }
                else
                {
                    newPosition.y = playerScript.defaultY;
                }
                if (rb != null)
                {
                    rb.MovePosition(newPosition);
                }
                else
                {
                    transform.position = newPosition;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));
            Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));

            float minX = bottomLeft.x + 0.5f;
            float maxX = topRight.x - 0.5f;
            float minY = bottomLeft.y + 0.5f;
            float maxY = topRight.y - 0.5f;

            Gizmos.DrawLine(new Vector3(minX, minY, 0), new Vector3(maxX, minY, 0));
            Gizmos.DrawLine(new Vector3(minX, maxY, 0), new Vector3(maxX, maxY, 0));
            Gizmos.DrawLine(new Vector3(minX, minY, 0), new Vector3(minX, maxY, 0));
            Gizmos.DrawLine(new Vector3(maxX, minY, 0), new Vector3(maxX, maxY, 0));
        }
    }
}
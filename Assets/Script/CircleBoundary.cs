using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleBoundary : MonoBehaviour
{
    private GameObject targetPlayer = null; // 跟隨的 Player
    private Vector3 offset; // Circle 相對於 Player 的偏移量
    private bool isFollowing = false;

    private Rigidbody2D rb; // Circle 的 Rigidbody2D

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // 獲取 Rigidbody2D
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
        // 沒有跟隨時不干預，讓 Rigidbody2D 完全控制
    }

    // FixedUpdate 用於物理更新
    void FixedUpdate()
    {
        if (!isFollowing && rb != null)
        {
            // 確保位置在邊界內，但優先讓物理引擎控制
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

            rb.MovePosition(position); // 使用 Rigidbody 移動
        }
    }

    // 開始跟隨 Player
    public void StartFollowing(GameObject player)
    {
        targetPlayer = player;
        isFollowing = true;
        offset = transform.position - player.transform.position; // 計算初始偏移
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // 停止物理運動，開始跟隨
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
            // 恢復物理控制，但不干預
        }
    }

    // 跟隨 Player 的邏輯
    void FollowPlayer()
    {
        if (targetPlayer != null)
        {
            Vector3 targetPosition = targetPlayer.transform.position;
            Vector3 newPosition = targetPosition + offset;

            // 確保不超出邊界（可選，根據需求）
            Camera cam = Camera.main;
            Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));
            Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));

            float minX = bottomLeft.x + 0.5f;
            float maxX = topRight.x - 0.5f;
            float minY = bottomLeft.y + 0.5f;
            float maxY = topRight.y - 0.5f;

            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

            // 使用 Rigidbody2D 移動
            if (rb != null)
            {
                rb.MovePosition(newPosition);
            }
            else
            {
                transform.position = newPosition; // 備用，如果沒有 Rigidbody
            }

            // 如果 Player 正在返回，Circle 也跟隨返回
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
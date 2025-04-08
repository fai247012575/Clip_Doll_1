using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleBoundary : MonoBehaviour
{
    private GameObject targetPlayer = null; // ���H�� Player
    private Vector3 offset; // Circle �۹�� Player �������q
    private bool isFollowing = false;

    private Rigidbody2D rb; // Circle �� Rigidbody2D

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // ��� Rigidbody2D
        if (rb == null)
        {
            Debug.LogError("CircleBoundary �ݭn Rigidbody2D �ե�I");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isFollowing && targetPlayer != null)
        {
            FollowPlayer();
        }
        // �S�����H�ɤ��z�w�A�� Rigidbody2D ��������
    }

    // FixedUpdate �Ω󪫲z��s
    void FixedUpdate()
    {
        if (!isFollowing && rb != null)
        {
            // �T�O��m�b��ɤ��A���u�������z��������
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

            rb.MovePosition(position); // �ϥ� Rigidbody ����
        }
    }

    // �}�l���H Player
    public void StartFollowing(GameObject player)
    {
        targetPlayer = player;
        isFollowing = true;
        offset = transform.position - player.transform.position; // �p���l����
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // ����z�B�ʡA�}�l���H
            rb.angularVelocity = 0f;
        }
    }

    // ������H
    public void StopFollowing()
    {
        targetPlayer = null;
        isFollowing = false;
        if (rb != null)
        {
            // ��_���z����A�����z�w
        }
    }

    // ���H Player ���޿�
    void FollowPlayer()
    {
        if (targetPlayer != null)
        {
            Vector3 targetPosition = targetPlayer.transform.position;
            Vector3 newPosition = targetPosition + offset;

            // �T�O���W�X��ɡ]�i��A�ھڻݨD�^
            Camera cam = Camera.main;
            Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));
            Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));

            float minX = bottomLeft.x + 0.5f;
            float maxX = topRight.x - 0.5f;
            float minY = bottomLeft.y + 0.5f;
            float maxY = topRight.y - 0.5f;

            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

            // �ϥ� Rigidbody2D ����
            if (rb != null)
            {
                rb.MovePosition(newPosition);
            }
            else
            {
                transform.position = newPosition; // �ƥΡA�p�G�S�� Rigidbody
            }

            // �p�G Player ���b��^�ACircle �]���H��^
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleBoundary : MonoBehaviour
{
    private float minX, maxX, minY, maxY;
    private GameObject targetPlayer = null; // ���H�� Player
    private Vector3 offset; // Circle �۹�� Player �������q
    private bool isFollowing = false;

    // Start is called before the first frame update
    void Start()
    {
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
        if (isFollowing && targetPlayer != null)
        {
            FollowPlayer();
        }
        else
        {
            // �S�����H�ɫO���b��ɤ�
            Vector3 position = transform.position;

            if (position.x < minX) position.x = minX;
            else if (position.x > maxX) position.x = maxX;

            if (position.y < minY) position.y = minY;
            else if (position.y > maxY) position.y = maxY;

            transform.position = position;
        }
    }

    // �}�l���H Player
    public void StartFollowing(GameObject player)
    {
        targetPlayer = player;
        isFollowing = true;
        offset = transform.position - player.transform.position; // �p���l����
    }

    // ������H
    public void StopFollowing()
    {
        targetPlayer = null;
        isFollowing = false;
    }

    // ���H Player ���޿�
    void FollowPlayer()
    {
        if (targetPlayer != null)
        {
            Vector3 targetPosition = targetPlayer.transform.position;
            Vector3 newPosition = targetPosition + offset;

            // ���H Player �� Y ��m�A���T�O���W�X���
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

            transform.position = newPosition;

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
                transform.position = newPosition;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(minX, minY, 0), new Vector3(maxX, minY, 0));
        Gizmos.DrawLine(new Vector3(minX, maxY, 0), new Vector3(maxX, maxY, 0));
        Gizmos.DrawLine(new Vector3(minX, minY, 0), new Vector3(minX, maxY, 0));
        Gizmos.DrawLine(new Vector3(maxX, minY, 0), new Vector3(maxX, maxY, 0));
    }
}
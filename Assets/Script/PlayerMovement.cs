using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private List<GameObject> targetObjects = new List<GameObject>();
    private GameObject squareObject = null;
    private Text scoreText = null;
    private int score = 1;

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

        squareObject = GameObject.Find("Square");
        if (squareObject == null)
        {
            Debug.LogError("Square Object not found in Hierarchy!");
        }

        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        if (scoreText == null)
        {
            Debug.LogError("ScoreText not found in Hierarchy or missing Text component!");
        }
        else
        {
            UpdateScoreText();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPausedX)
        {
            MoveOnX();
        }

        HandleYMovement();

        CheckSquareXRange();
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

        if (score > 0 && ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0)))
        {
            isMovingDown = true;
            isReturning = false;
            isPausedX = true;

            score -= 1;
            UpdateScoreText();
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
            }
        }

        transform.position = newPosition;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision Entered with: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Circle"))
        {
            
            if (!targetObjects.Contains(collision.gameObject))
            {
                targetObjects.Add(collision.gameObject);
            }

            isReturning = true;
            isMovingDown = false;
            isPausedX = true;

            CircleBoundary circleScript = collision.gameObject.GetComponent<CircleBoundary>();
            if (circleScript != null)
            {
                circleScript.StartFollowing(this.gameObject);
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("Collision Staying with: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Circle"))
        {
            
            if (!targetObjects.Contains(collision.gameObject))
            {
                targetObjects.Add(collision.gameObject);
            }

            isReturning = true;
            isMovingDown = false;
            isPausedX = true;

            CircleBoundary circleScript = collision.gameObject.GetComponent<CircleBoundary>();
            if (circleScript != null)
            {
                circleScript.StartFollowing(this.gameObject);
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("Collision Exited with: " + collision.gameObject.name);
    }

    void CheckSquareXRange()
    {
        if (squareObject != null)
        {
            float playerX = transform.position.x;

            Vector3 squarePosition = squareObject.transform.position;
            Vector3 squareScale = squareObject.transform.lossyScale;
            float squareWidth = squareScale.x;
            float squareMinX = squarePosition.x - (squareWidth / 2f);
            float squareMaxX = squarePosition.x + (squareWidth / 2f);

            if (playerX >= squareMinX && playerX <= squareMaxX)
            {
                Debug.Log("Player entered Square's X range. Player X: " + playerX + ", Square X Range: [" + squareMinX + ", " + squareMaxX + "]");
                StopCircleFollowing();
            }
        }
    }

    void UpdateBoundariesToMatchTarget()
    {
        
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
        
        foreach (GameObject target in targetObjects)
        {
            if (target != null)
            {
                CircleBoundary circleScript = target.GetComponent<CircleBoundary>();
                if (circleScript != null)
                {
                    circleScript.StopFollowing();
                }
            }
        }
        
        targetObjects.Clear();
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
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
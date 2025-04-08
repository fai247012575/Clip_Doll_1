using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareObjectScript : MonoBehaviour
{
    void OnDrawGizmos()
    {
        
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(transform.position.x, 0, 0), new Vector3(transform.position.x, transform.position.y, 0));

        
        Gizmos.color = Color.green;
        Vector3 size = transform.localScale;
        Gizmos.DrawLine(transform.position + new Vector3(-size.x / 2, -size.y / 2, 0),
                        transform.position + new Vector3(size.x / 2, -size.y / 2, 0));
        Gizmos.DrawLine(transform.position + new Vector3(-size.x / 2, size.y / 2, 0),
                        transform.position + new Vector3(size.x / 2, size.y / 2, 0));
        Gizmos.DrawLine(transform.position + new Vector3(-size.x / 2, -size.y / 2, 0),
                        transform.position + new Vector3(-size.x / 2, size.y / 2, 0));
        Gizmos.DrawLine(transform.position + new Vector3(size.x / 2, -size.y / 2, 0),
                        transform.position + new Vector3(size.x / 2, size.y / 2, 0));
    }
}
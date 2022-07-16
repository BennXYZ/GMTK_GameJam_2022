using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AiPath : MonoBehaviour
{
    [SerializeField]
    List<Vector2Int> wayPoints;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0.5f, 0, 1);
        for (int i = 0; i < wayPoints.Count; i++)
        {
            Gizmos.DrawSphere(new Vector3(wayPoints[i].x + 0.5f, 0.5f, wayPoints[i].y + 0.5f), 0.3f);
            Gizmos.color = Color.yellow;
            if (i > 0)
            {
                DrawGizmoLineBetween(new Vector3(wayPoints[i - 1].x + 0.5f, 0.5f, wayPoints[i - 1].y + 0.5f),
                    new Vector3(wayPoints[i].x + 0.5f, 0.5f, wayPoints[i].y + 0.5f));
            }
            if(wayPoints.Count >= 3)
            {
                DrawGizmoLineBetween(new Vector3(wayPoints[wayPoints.Count - 1].x + 0.5f,
                    0.5f, wayPoints[wayPoints.Count - 1].y + 0.5f),
                    new Vector3(wayPoints[0].x + 0.5f, 0.5f, wayPoints[0].y + 0.5f));
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int i = 0; i < wayPoints.Count; i++)
        {
            Gizmos.DrawWireSphere(new Vector3(wayPoints[i].x + 0.5f, 0.5f, wayPoints[i].y + 0.5f), 0.2f);
        }
    }

    void DrawGizmoLineBetween(Vector3 from, Vector3 to)
    {
        Gizmos.DrawLine(from, to);
        Gizmos.DrawSphere(from + (to - from) * 0.8f, 0.15f);
    }
}
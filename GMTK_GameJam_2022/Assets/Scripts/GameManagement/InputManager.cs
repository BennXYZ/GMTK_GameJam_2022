using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    Vector3 mouseDragStart = Vector3.zero;

    [SerializeField]
    float selectionDragTolerance;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            mouseDragStart = Input.mousePosition;
        }
        if(Input.GetMouseButtonUp(0))
        {
            if((Input.mousePosition - mouseDragStart).magnitude < selectionDragTolerance)
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    Vector2Int gridPoint = new Vector2Int(Mathf.FloorToInt(hit.point.x), Mathf.FloorToInt(hit.point.z));
                    Debug.Log($"Hit Gripoint: {gridPoint}");
                }
            }

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    Vector3 mouseDragStart = Vector3.zero;

    [SerializeField]
    float selectionDragTolerance;

    UnityEvent<Vector2Int> onGridPointSelected;

    public static InputManager Instance { get => GameManager.Instance.inputManager; }

    public UnityEvent<Vector2Int> OnGridPointSelected
    {
        get => onGridPointSelected;
    }

    private void Awake()
    {
        onGridPointSelected = new UnityEvent<Vector2Int>();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            mouseDragStart = Input.mousePosition;
        }
        if(Input.GetMouseButtonUp(0))
        {
            if(GameManager.Instance.gameStateManager.CurrentGameState == GameStateManager.GameState.TileSelection
                && (Input.mousePosition - mouseDragStart).magnitude < selectionDragTolerance)
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    Vector2Int gridPoint = new Vector2Int(Mathf.FloorToInt(hit.point.x), Mathf.FloorToInt(hit.point.z));
                    Debug.Log($"Hit Gripoint: {gridPoint}");
                    onGridPointSelected.Invoke(gridPoint);
                }
            }
        }
    }
}

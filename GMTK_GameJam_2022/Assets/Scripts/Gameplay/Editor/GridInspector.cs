using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GMTKJam2022.Gameplay.Editor
{
    [CustomEditor(typeof(CasinoGrid))]
    public class GridInspector : UnityEditor.Editor
    {
        CasinoGrid grid = null;

        List<Vector2Int> selectedTiles = new List<Vector2Int>();
        Vector2Int newSize;
        int floodFillDistance = 0;

        CasinoGrid.TileType multiselectionTileType;
        DirectionFlag multiselectionBlocker;

        private void OnEnable()
        {
            grid = target as CasinoGrid;
            newSize = grid.Size;
            ClearSelection();
        }

        private void OnSceneGUI()
        {
            bool selectionChanged = false;
            for (int y = 0; y < grid.Size.y; y++)
            {
                for (int x = 0; x < grid.Size.x; x++)
                {
                    bool isSelected = selectedTiles.Contains(new Vector2Int(x, y));
                    Handles.color = isSelected ? Color.blue : Color.white;

                    if (Handles.Button(new Vector3(grid.Origin.x + x + 0.5f, 0, grid.Origin.y + y + 0.5f), Quaternion.identity, 0.3f, 0.3f, Handles.SphereHandleCap))
                    {
                        if (isSelected)
                            selectedTiles.Remove(new Vector2Int(x, y));
                        else
                            selectedTiles.Add(new Vector2Int(x, y));

                        selectionChanged = true;
                    }
                }
            }
            // Refresh the inspector
            if (selectionChanged)
                Repaint();
        }

        public override void OnInspectorGUI()
        {
            newSize = EditorGUILayout.Vector2IntField("Size", newSize);

            if (grid.Size != newSize)
            {
                using var scope = new GUILayout.HorizontalScope();
                if (GUILayout.Button("Resize"))
                {
                    ClearSelection();
                    grid.Resize(newSize);
                }

                if (GUILayout.Button("Reset"))
                    newSize = grid.Size;
            }

            DrawSelectionInspector();

            if (selectedTiles.Count == 1)
            {
                GUILayout.Space(20);
                using var scope = new GUILayout.HorizontalScope();
                floodFillDistance = EditorGUILayout.IntField("Distance", floodFillDistance);

                if (GUILayout.Button("Flood Fill"))
                {
                    Dictionary<Vector2Int, int> floodFillResult = grid.FloodFill(selectedTiles[0], floodFillDistance);
                    foreach (var item in floodFillResult)
                    {
                        Debug.Log(item);
                    }
                }
            }
        }

        private void DrawSelectionInspector()
        {
            GUILayout.Space(20);
            GUILayout.Label(selectedTiles.Count <= 1 ? "Selected Tile" : "Selected Tile (multiple)", EditorStyles.boldLabel);
            if (selectedTiles.Count > 0)
            {
                if (selectedTiles.Count == 1)
                    DrawSingleSelectionInspector(selectedTiles[0]);
                else
                    DrawMultiSelectionInspector();

                if (GUILayout.Button("Clear Selection"))
                {
                    ClearSelection();
                }
            }
            else
            {
                GUILayout.Label("None");
            }
        }

        private void DrawMultiSelectionInspector()
        {
            HashSet<CasinoGrid.TileType> selectedTypes = new();
            HashSet<DirectionFlag> selectedBlockers = new();

            for (int i = 0; i < selectedTiles.Count; i++)
            {
                Vector2Int coordinate = selectedTiles[i];
                CasinoGrid.GridTile? gridTile = grid.GetTile(coordinate);
                if (gridTile.HasValue)
                {
                    selectedTypes.Add(gridTile.Value.Type);
                    selectedBlockers.Add(gridTile.Value.BlockedDirection);
                }
                else
                {
                    ClearSelection();
                    break;
                }
            }

            { // Selected types
                if (selectedTypes.Count > 1)
                {
                    EditorGUI.showMixedValue = true;
                    multiselectionTileType = (CasinoGrid.TileType)int.MaxValue;
                }
                else
                {
                    foreach (var type in selectedTypes)
                        multiselectionTileType = type;
                }

                CasinoGrid.TileType tileType = (CasinoGrid.TileType)EditorGUILayout.EnumPopup("Type", multiselectionTileType);
                if (tileType != multiselectionTileType)
                {
                    for (int i = 0; i < selectedTiles.Count; i++)
                    {
                        Undo.RecordObject(grid, "Tiles changed");
                        EditorUtility.SetDirty(grid);
                        Vector2Int coordinate = selectedTiles[i];
                        CasinoGrid.GridTile gridTile = grid.GetTile(coordinate).Value;
                        gridTile.Type = tileType;
                        grid.SetTile(coordinate, gridTile);
                    }
                    multiselectionTileType = tileType;
                }

                EditorGUI.showMixedValue = false;
            }

            { // Selected blocker
                if (selectedBlockers.Count > 1)
                {
                    EditorGUI.showMixedValue = true;
                    multiselectionBlocker = (DirectionFlag)int.MaxValue;
                }
                else
                {
                    foreach (var selectedBlocker in selectedBlockers)
                        multiselectionBlocker = selectedBlocker;
                }

                DirectionFlag blocker = (DirectionFlag)EditorGUILayout.EnumFlagsField("Blocked Directions", multiselectionBlocker);
                if (blocker != multiselectionBlocker)
                {
                    for (int i = 0; i < selectedTiles.Count; i++)
                    {
                        Undo.RecordObject(grid, "Tiles changed");
                        EditorUtility.SetDirty(grid);
                        Vector2Int coordinate = selectedTiles[i];
                        CasinoGrid.GridTile gridTile = grid.GetTile(coordinate).Value;
                        gridTile.BlockedDirection = blocker;
                        grid.SetTile(coordinate, gridTile);
                    }
                    multiselectionBlocker = blocker;
                }

                EditorGUI.showMixedValue = false;
            }
        }

        private void DrawSingleSelectionInspector(Vector2Int coordinate)
        {
            CasinoGrid.GridTile? gridTile = grid.GetTile(coordinate);
            if (gridTile.HasValue)
            {
                CasinoGrid.GridTile tile = gridTile.Value;
                CasinoGrid.TileType tileType = (CasinoGrid.TileType)EditorGUILayout.EnumPopup("Type", tile.Type);
                if (tileType != tile.Type)
                {
                    Undo.RecordObject(grid, "Tiles changed");
                    EditorUtility.SetDirty(grid);
                    tile.Type = tileType;
                    grid.SetTile(coordinate, tile);
                }

                DirectionFlag blockedDirection = (DirectionFlag)EditorGUILayout.EnumFlagsField("Blocked Directions", tile.BlockedDirection);
                if (blockedDirection != tile.BlockedDirection)
                {
                    Undo.RecordObject(grid, "Tiles changed");
                    EditorUtility.SetDirty(grid);
                    tile.BlockedDirection = blockedDirection;
                    grid.SetTile(coordinate, tile);
                }
            }
            else
            {
                ClearSelection();
            }
        }

        private void ClearSelection()
        {
            EditorUtility.SetDirty(grid);
            selectedTiles.Clear();
        }
    }
}

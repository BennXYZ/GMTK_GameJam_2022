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
        int multiselectionHeightOffset;

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
                    Vector2Int coordinate = new Vector2Int(x, y);
                    bool isSelected = selectedTiles.Contains(coordinate);
                    Handles.color = isSelected ? Color.blue : Color.white;

                    CasinoGrid.GridTile? tile = grid.GetTile(coordinate);

                    if (Handles.Button(new Vector3(grid.Origin.x + x + 0.5f, tile.Value.HeightOffset * 0.5f, grid.Origin.y + y + 0.5f), Quaternion.identity, 0.3f, 0.3f, Handles.SphereHandleCap))
                    {
                        if (isSelected)
                            selectedTiles.Remove(coordinate);
                        else
                            selectedTiles.Add(coordinate);

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
            Vector2Int origin = EditorGUILayout.Vector2IntField("Origin", grid.Origin);
            if (origin != grid.Origin)
            {
                Undo.RecordObject(grid, "Moved grid");
                EditorUtility.SetDirty(grid);
                grid.Origin = origin;
            }

            newSize = EditorGUILayout.Vector2IntField("Size", newSize);

            if (grid.Size != newSize)
            {
                using var scope = new GUILayout.HorizontalScope();
                if (GUILayout.Button("Resize"))
                {
                    ClearSelection();
                    Undo.RecordObject(grid, "Resized grid");
                    EditorUtility.SetDirty(grid);
                    grid.Resize(newSize);
                }

                if (GUILayout.Button("Reset"))
                    newSize = grid.Size;
            }

            using (var selectionScope = new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Select all"))
                {
                    for (int y = 0; y < grid.Size.y; y++)
                        for (int x = 0; x < grid.Size.x; x++)
                        {
                            EditorUtility.SetDirty(grid);
                            selectedTiles.Add(new Vector2Int(x, y));
                        }
                }
                GUI.enabled = selectedTiles.Count > 0;
                if (GUILayout.Button("Deselect all"))
                {
                    ClearSelection();
                }
                GUI.enabled = true;
            }

            DrawSelectionInspector();

            if (selectedTiles.Count == 1)
            {
                GUILayout.Space(20);
                using var scope = new GUILayout.HorizontalScope();
                floodFillDistance = EditorGUILayout.IntField("Distance", floodFillDistance);

                if (GUILayout.Button("Flood Fill"))
                {
                    Dictionary<Vector2Int, CasinoGrid.GridPathInformation> floodFillResult = grid.FloodFill(selectedTiles[0], floodFillDistance);
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
            HashSet<int> selectedHeightOffsets = new();

            for (int i = 0; i < selectedTiles.Count; i++)
            {
                Vector2Int coordinate = selectedTiles[i];
                CasinoGrid.GridTile? gridTile = grid.GetTile(coordinate);
                if (gridTile.HasValue)
                {
                    selectedTypes.Add(gridTile.Value.Type);
                    selectedBlockers.Add(gridTile.Value.BlockedDirection);
                    selectedHeightOffsets.Add(gridTile.Value.HeightOffset);
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

            { // Height offset
                if (selectedHeightOffsets.Count > 1)
                {
                    EditorGUI.showMixedValue = true;
                    multiselectionHeightOffset = int.MaxValue;
                }
                else
                {
                    foreach (var selectedHeightOffset in selectedHeightOffsets)
                        multiselectionHeightOffset = selectedHeightOffset;
                }

                int heightOffset = EditorGUILayout.IntSlider("Height Offset", multiselectionHeightOffset, -1, 1);
                if (heightOffset != multiselectionHeightOffset)
                {
                    for (int i = 0; i < selectedTiles.Count; i++)
                    {
                        Undo.RecordObject(grid, "Tiles changed");
                        EditorUtility.SetDirty(grid);
                        Vector2Int coordinate = selectedTiles[i];
                        CasinoGrid.GridTile gridTile = grid.GetTile(coordinate).Value;
                        gridTile.HeightOffset = heightOffset;
                        grid.SetTile(coordinate, gridTile);
                    }
                    multiselectionHeightOffset = heightOffset;
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

                int heightOffset = EditorGUILayout.IntSlider("Height Offset", tile.HeightOffset, -1, 1);
                if (heightOffset != tile.HeightOffset)
                {
                    Undo.RecordObject(grid, "Tiles changed");
                    EditorUtility.SetDirty(grid);
                    tile.HeightOffset = heightOffset;
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

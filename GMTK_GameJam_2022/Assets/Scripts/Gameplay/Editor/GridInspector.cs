using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GMTKJam2022.Gameplay.Editor
{
    [CustomEditor(typeof(Grid))]
    public class GridInspector : UnityEditor.Editor
    {
        Grid grid = null;

        List<Vector2Int> selectedTiles = new List<Vector2Int>();
        Vector2Int newSize;

        Grid.TileType multiselectionTileType;

        private void OnEnable()
        {
            grid = target as Grid;
            newSize = grid.Size;
            selectedTiles.Clear();
        }

        private void OnSceneGUI()
        {
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
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            newSize = EditorGUILayout.Vector2IntField("Size", newSize);

            if (grid.Size != newSize)
            {
                using var scope = new GUILayout.HorizontalScope();
                if (GUILayout.Button("Resize"))
                {
                    selectedTiles.Clear();
                    grid.Resize(newSize);
                }

                if (GUILayout.Button("Reset"))
                    newSize = grid.Size;
            }

            if (selectedTiles.Count > 0)
            {
                if (selectedTiles.Count == 1)
                    DrawSingleSelectionInspector(selectedTiles[0]);
                else
                    DrawMultiSelectionInspector();
            }
        }

        private void DrawMultiSelectionInspector()
        {
            HashSet<Grid.TileType> selectedTypes = new();

            for (int i = 0; i < selectedTiles.Count; i++)
            {
                Vector2Int coordinate = selectedTiles[i];
                Grid.GridTile? gridTile = grid.GetTile(coordinate);
                if (gridTile.HasValue)
                {
                    selectedTypes.Add(gridTile.Value.Type);
                }
                else
                {
                    selectedTiles.Clear();
                    break;
                }
            }

            if (selectedTypes.Count > 1)
            {
                EditorGUI.showMixedValue = true;
                multiselectionTileType = (Grid.TileType)int.MaxValue;
            }
            else
            {
                foreach (var type in selectedTypes)
                    multiselectionTileType = type;
            }

            Grid.TileType tileType = (Grid.TileType)EditorGUILayout.EnumPopup(multiselectionTileType);
            if (tileType != multiselectionTileType)
            {
                for (int i = 0; i < selectedTiles.Count; i++)
                {
                    Vector2Int coordinate = selectedTiles[i];
                    Grid.GridTile gridTile = grid.GetTile(coordinate).Value;
                    gridTile.Type = tileType;
                    grid.SetTile(coordinate, gridTile);
                }
                multiselectionTileType = tileType;
            }

            EditorGUI.showMixedValue = false;
        }

        private void DrawSingleSelectionInspector(Vector2Int coordinate)
        {
            Grid.GridTile? gridTile = grid.GetTile(coordinate);
            if (gridTile.HasValue)
            {
                Grid.GridTile tile = gridTile.Value;
                Grid.TileType tileType = (Grid.TileType)EditorGUILayout.EnumPopup(tile.Type);
                if (tileType != tile.Type)
                {
                    tile.Type = tileType;
                    grid.SetTile(coordinate, tile);
                }
            }
            else
            {
                selectedTiles.Clear();
            }
        }
    }
}

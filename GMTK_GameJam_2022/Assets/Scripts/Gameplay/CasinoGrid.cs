using System;
using System.Collections.Generic;
using UnityEngine;

namespace GMTKJam2022.Gameplay
{
    public class CasinoGrid : MonoBehaviour
    {
        public enum TileType
        {
            Empty,
            Blocked
        }

        [Serializable]
        public struct GridTile
        {
            public TileType Type;
        }

        [SerializeField]
        [HideInInspector]
        private GridTile[] tiles = new GridTile[0];

        [field: SerializeField]
        public Vector2Int Origin { get; private set; }

        [field: SerializeField]
        [HideInInspector]
        public Vector2Int Size { get; private set; }

        public void Resize(Vector2Int newSize)
        {
            Dictionary<Vector2Int, GridTile> tempTiles = new();

            for (int i = 0; i < tiles.Length; i++)
            {
                Vector2Int coordinate = new Vector2Int(i % Size.x, i / Size.x);
                tempTiles.Add(coordinate, tiles[i]);
            }

            Size = newSize;

            tiles = new GridTile[Size.x * Size.y];

            foreach (var tempTile in tempTiles)
            {
                if (tempTile.Key.x <= Size.x && tempTile.Key.y <= Size.y)
                    tiles[tempTile.Key.x + tempTile.Key.y * Size.x] = tempTile.Value;
            }
        }

        private void OnDrawGizmos()
        {
            for (int y = 0; y < Size.y; y++)
                for (int x = 0; x < Size.x; x++)
                {
                    switch (tiles[x + y * Size.x].Type)
                    {
                        case TileType.Empty:
                            Gizmos.color = Color.black;
                            break;

                        case TileType.Blocked:
                            Gizmos.color = Color.red;
                            break;
                    }
                    Gizmos.DrawWireSphere(new Vector3(Origin.x + x + 0.5f, 0, Origin.y + y + 0.5f), 0.3f);
                }
        }

        public GridTile? GetTile(Vector2Int coordinate)
        {
            if (coordinate.x >= 0 && Size.x >= coordinate.x && coordinate.y >= 0 && Size.y >= coordinate.y)
            {
                return tiles[coordinate.x + coordinate.y * Size.x];
            }
            return null;
        }

        public void SetTile(Vector2Int coordinate, GridTile tile)
        {
            if (coordinate.x >= 0 && Size.x >= coordinate.x && coordinate.y >= 0 && Size.y >= coordinate.y)
            {
                tiles[coordinate.x + coordinate.y * Size.x] = tile;
            }
        }

        public Vector2Int Raycast(Vector2Int location, Direction direction)
        {
            if (!GetTile(location).HasValue)
                return location;

            Vector2Int newCoordinate = location;
            int maxDistance = Math.Max(Size.x, Size.y);
            for (int i = 0; i < maxDistance; i++)
            {
                newCoordinate += direction.ToVector();
                GridTile? tile = GetTile(newCoordinate);
                if (!tile.HasValue || tile.Value.Type == TileType.Blocked)
                    break;
            }

            return newCoordinate;
        }

        public Dictionary<Vector2Int, int> FloodFill(Vector2Int location, int distance)
        {
            distance = Math.Max(0, distance);
            Dictionary<Vector2Int, int> data = new();

            FloodFill(data, location, 0, distance);

            return data;
        }

        private void FloodFill(Dictionary<Vector2Int, int> closedList, Vector2Int location, int currentDistance, int maxDistance)
        {
            GridTile? tile = GetTile(location);
            if (!tile.HasValue || tile.Value.Type == TileType.Blocked)
                return;

            if (closedList.TryGetValue(location, out int distance))
            {
                if (distance > currentDistance)
                    closedList[location] = currentDistance;
                return;
            }

            closedList.Add(location, currentDistance);

            if (currentDistance < maxDistance)
            {
                FloodFill(closedList, location + Direction.Up.ToVector(), currentDistance + 1, maxDistance);
                FloodFill(closedList, location + Direction.Left.ToVector(), currentDistance + 1, maxDistance);
                FloodFill(closedList, location + Direction.Right.ToVector(), currentDistance + 1, maxDistance);
                FloodFill(closedList, location + Direction.Down.ToVector(), currentDistance + 1, maxDistance);
            }
        }
    }
}

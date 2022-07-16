using System;
using System.Collections.Generic;
using UnityEngine;

namespace GMTKJam2022.Gameplay
{
    public class CasinoGrid : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        private GridTile[] tiles = new GridTile[0];

        public enum TileType
        {
            Empty,
            Blocked
        }

        public List<Entity> Entities { get; } = new List<Entity>();

        public List<LivingEntity> LivingEntities { get; } = new List<LivingEntity>();

        [field: SerializeField]
        public Vector2Int Origin { get; set; }

        [field: SerializeField]
        [HideInInspector]
        public Vector2Int Size { get; private set; }

        public Dictionary<Vector2Int, int> FloodFill(Vector2Int location, int distance)
        {
            distance = Math.Max(0, distance);
            Dictionary<Vector2Int, int> data = new();

            FloodFill(data, location, null, 0, distance);

            return data;
        }

        public GridTile? GetTile(Vector2Int coordinate)
        {
            if (coordinate.x >= 0 && Size.x > coordinate.x && coordinate.y >= 0 && Size.y > coordinate.y)
            {
                return tiles[coordinate.x + coordinate.y * Size.x];
            }
            return null;
        }

        public Vector2Int Raycast(Vector2Int location, Direction direction)
        {
            if (!GetTile(location).HasValue)
                return location;

            Vector2Int newCoordinate = location;
            int maxDistance = Math.Max(Size.x, Size.y);
            for (int i = 0; i < maxDistance; i++)
            {
                GridTile? myTile = GetTile(newCoordinate);
                if (myTile.HasValue && (myTile.Value.BlockedDirection.Mirror() & direction.ToFlag()) == direction.ToFlag())
                    break;

                GridTile? nextTile = GetTile(newCoordinate + direction.ToVector());
                if (!nextTile.HasValue || nextTile.Value.Type == TileType.Blocked || (nextTile.Value.BlockedDirection & direction.ToFlag()) == direction.ToFlag())
                    break;

                newCoordinate += direction.ToVector();
            }

            return newCoordinate;
        }

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
                if (tempTile.Key.x < Size.x && tempTile.Key.y < Size.y)
                    tiles[tempTile.Key.x + tempTile.Key.y * Size.x] = tempTile.Value;
            }
        }

        public void SetTile(Vector2Int coordinate, GridTile tile)
        {
            if (coordinate.x >= 0 && Size.x > coordinate.x && coordinate.y >= 0 && Size.y > coordinate.y)
            {
                tiles[coordinate.x + coordinate.y * Size.x] = tile;
            }
        }

        private void FloodFill(Dictionary<Vector2Int, int> closedList, Vector2Int location, Direction? direction, int currentDistance, int maxDistance)
        {
            GridTile? tile = GetTile(location);
            if (!tile.HasValue || tile.Value.Type == TileType.Blocked)
                return;

            if (direction.HasValue)
            {
                if ((direction.Value.ToFlag() & tile.Value.BlockedDirection.Mirror()) == direction.Value.ToFlag())
                    return;
            }

            if (closedList.TryGetValue(location, out int distance))
            {
                if (distance > currentDistance)
                    closedList[location] = currentDistance;
                else
                    return;
            }
            else
                closedList.TryAdd(location, currentDistance);

            if (currentDistance < maxDistance)
            {
                if ((DirectionFlag.Up & tile.Value.BlockedDirection) == 0)
                    FloodFill(closedList, location + Direction.Up.ToVector(), Direction.Up, currentDistance + 1, maxDistance);
                if ((DirectionFlag.Left & tile.Value.BlockedDirection) == 0)
                    FloodFill(closedList, location + Direction.Left.ToVector(), Direction.Left, currentDistance + 1, maxDistance);
                if ((DirectionFlag.Right & tile.Value.BlockedDirection) == 0)
                    FloodFill(closedList, location + Direction.Right.ToVector(), Direction.Right, currentDistance + 1, maxDistance);
                if ((DirectionFlag.Down & tile.Value.BlockedDirection) == 0)
                    FloodFill(closedList, location + Direction.Down.ToVector(), Direction.Down, currentDistance + 1, maxDistance);
            }
        }

        private void OnDrawGizmos()
        {
            for (int y = 0; y < Size.y; y++)
                for (int x = 0; x < Size.x; x++)
                {
                    Vector3 location = new Vector3(Origin.x + x + 0.5f, 0, Origin.y + y + 0.5f);
                    GridTile tile = tiles[x + y * Size.x];
                    switch (tile.Type)
                    {
                        case TileType.Empty:
                            Gizmos.color = Color.black;
                            break;

                        case TileType.Blocked:
                            Gizmos.color = Color.red;
                            break;
                    }
                    Gizmos.DrawWireSphere(location, 0.2f);

                    Gizmos.color = Color.red;
                    if ((tile.BlockedDirection & DirectionFlag.Down) == DirectionFlag.Down)
                        Gizmos.DrawWireCube(location + Vector3.back * 0.45f, new Vector3(1, 1, 0.1f));
                    if ((tile.BlockedDirection & DirectionFlag.Up) == DirectionFlag.Up)
                        Gizmos.DrawWireCube(location + Vector3.forward * 0.45f, new Vector3(1, 1, 0.1f));
                    if ((tile.BlockedDirection & DirectionFlag.Right) == DirectionFlag.Right)
                        Gizmos.DrawWireCube(location + Vector3.right * 0.45f, new Vector3(0.1f, 1, 1));
                    if ((tile.BlockedDirection & DirectionFlag.Left) == DirectionFlag.Left)
                        Gizmos.DrawWireCube(location + Vector3.left * 0.45f, new Vector3(0.1f, 1, 1));
                }
        }

        private void Start()
        {
            foreach (Transform child in transform)
            {
                Entity entity = child.GetComponent<Entity>();
                if (entity != null)
                {
                    entity.Init(this);
                    Entities.Add(entity);
                    if (entity is LivingEntity livingEntity)
                        LivingEntities.Add(livingEntity);
                }
            }
        }

        [Serializable]
        public struct GridTile
        {
            public TileType Type;
            public DirectionFlag BlockedDirection;
        }
    }
}
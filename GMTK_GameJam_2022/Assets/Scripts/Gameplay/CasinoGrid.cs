using System;
using System.Collections.Generic;
using System.Linq;
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

        public Dictionary<Vector2Int, GridPathInformation> FloodFill(Vector2Int location, int distance, bool ignoreLivingEntities)
        {
            distance = Math.Max(0, distance);
            Dictionary<Vector2Int, GridPathInformation> data = new();

            FloodFill(data, location, null, 0, distance, ignoreLivingEntities);

            return data;
        }

        public Dictionary<Vector2Int, GridPathInformation> GetReachableNeighbors(Vector2Int location)
        {
            Dictionary<Vector2Int, GridPathInformation> data = new();

            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
                TryAddingNeighbor(data, direction, location);

            return data;
        }

        void TryAddingNeighbor(Dictionary<Vector2Int, GridPathInformation> data, Direction direction, Vector2Int location)
        {
            Vector2Int targetToCheck = location + direction.ToVector();
            if (targetToCheck.x < 0 || targetToCheck.x >= Size.x || targetToCheck.y < 0 || targetToCheck.y >= Size.y)
                return;
            GridTile? tile = GetTile(targetToCheck);
            if ((direction.ToFlag() & tile.Value.BlockedDirection) == 0 && !((direction.ToFlag() & tile.Value.BlockedDirection.Mirror()) == direction.ToFlag()))
            {
                data.Add(targetToCheck, new GridPathInformation(1, direction.Mirror()));
            }
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

        private void FloodFill(Dictionary<Vector2Int, GridPathInformation> closedList, Vector2Int location, Direction? direction, int currentDistance,
            int maxDistance, bool ignoreLivingEntities)
        {
            GridTile? tile = GetTile(location);
            if (!tile.HasValue || tile.Value.Type == TileType.Blocked || (!ignoreLivingEntities &&
                LivingEntities.Any(l => !(l is PlayerEntity) && l.GetNearestGridPoint(l.transform.position) == location) && currentDistance > 0))
                return;

            if (direction.HasValue)
            {
                if ((direction.Value.ToFlag() & tile.Value.BlockedDirection.Mirror()) == direction.Value.ToFlag())
                    return;
            }

            if (closedList.Any(p => p.Key == location))
            {
                int distance = closedList.First(p => p.Key == location).Value.Distance;
                if (distance > currentDistance)
                    closedList[location] = new GridPathInformation(currentDistance, direction.Mirror());
                else
                    return;
            }
            else if (currentDistance > 0)
                closedList.TryAdd(location, new GridPathInformation(currentDistance, direction.Mirror()));

            if (currentDistance < maxDistance)
            {
                if ((DirectionFlag.Up & tile.Value.BlockedDirection) == 0)
                    FloodFill(closedList, location + Direction.Up.ToVector(), Direction.Up, currentDistance + 1, maxDistance, ignoreLivingEntities);
                if ((DirectionFlag.Left & tile.Value.BlockedDirection) == 0)
                    FloodFill(closedList, location + Direction.Left.ToVector(), Direction.Left, currentDistance + 1, maxDistance, ignoreLivingEntities);
                if ((DirectionFlag.Right & tile.Value.BlockedDirection) == 0)
                    FloodFill(closedList, location + Direction.Right.ToVector(), Direction.Right, currentDistance + 1, maxDistance, ignoreLivingEntities);
                if ((DirectionFlag.Down & tile.Value.BlockedDirection) == 0)
                    FloodFill(closedList, location + Direction.Down.ToVector(), Direction.Down, currentDistance + 1, maxDistance, ignoreLivingEntities);
            }
        }

        private void OnDrawGizmos()
        {
            for (int y = 0; y < Size.y; y++)
                for (int x = 0; x < Size.x; x++)
                {
                    GridTile tile = tiles[x + y * Size.x];
                    Vector3 location = new Vector3(Origin.x + x + 0.5f, tile.HeightOffset * 0.5f, Origin.y + y + 0.5f);
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
            public int HeightOffset;
        }

        public struct GridPathInformation
        {
            public int Distance;
            public Direction? PreviousPoint;

            public GridPathInformation(int distance, Direction? previousPoint)
            {
                Distance = distance;
                PreviousPoint = previousPoint;
            }
        }
    }
}
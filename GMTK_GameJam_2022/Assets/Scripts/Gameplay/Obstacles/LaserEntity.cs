using GMTKJam2022.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEntity : Entity
{
    public bool IsOpen { get; private set; } = false;

    [SerializeField]
    GameObject laserObject;

    public override void Init(CasinoGrid grid)
    {
        base.Init(grid);
        BlockGridPoint(true);
    }

    public void Open()
    {
        IsOpen = true;
        BlockGridPoint(false);
        laserObject.SetActive(false);
    }

    private void BlockGridPoint(bool block)
    {
        CasinoGrid.GridTile? tile = Grid.GetTile(GridPosition);
        if (tile.HasValue)
        {
            CasinoGrid.GridTile gridTile = tile.Value;
            gridTile.Type = block ? CasinoGrid.TileType.Blocked : CasinoGrid.TileType.Empty;
            Grid.SetTile(GridPosition, gridTile);
        }
    }
}

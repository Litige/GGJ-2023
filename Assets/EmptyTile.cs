using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EmptyTile: Tile {

    public EmptyTile(int x, int y): base(TileType.Empty, x, y) {}
    public override String toASCII() {
        return "E";
    }
}
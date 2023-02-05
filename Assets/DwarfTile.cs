using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DwarfTile: Tile {
    [SerializeField]
    public int nbDwarf;
    [SerializeField]
    public int maxDwarf;

    public DwarfTile(int x, int y): base(TileType.Dwarf, x, y) {}
    public override String toASCII() {
        return "D";
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RegularRootTile: RootTile {
    [SerializeField]
    public int pointsByNeighboor;
    [SerializeField]
    private int startingHitPoint;
    // [SerializeField]
    // public int hitPoint {get; set;}
    // [SerializeField]
    // public int maxHitPoint  {get; set;}

    public RegularRootTile(int x, int y): base(RootType.Regular, x, y) {
        this.hitPoint = startingHitPoint;
        this.maxHitPoint = startingHitPoint;
    }

    public RegularRootTile initFromInplace(RegularRootTile t, int x, int y) {
        this.tileType = TileType.Root;
        this.rootType = RootType.Regular;
        this.x = x;
        this.y = y;
        this.hitPoint = t.startingHitPoint;
        this.maxHitPoint = t.startingHitPoint;
        this.pointsByNeighboor = t.pointsByNeighboor;
        this.placementCost = t.placementCost;
        this.maintenanceCost = t.maintenanceCost;
        return this;
    }
    
    public override String toASCII() {
        return "R";
    }
}
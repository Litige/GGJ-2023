using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SpikeRootTile: RootTile {
    [SerializeField]
    public int counterDamage;
    [SerializeField]
    private int startingHitPoint;
    // [SerializeField]
    // public int hitPoint {get; set;}
    // [SerializeField]
    // public int maxHitPoint  {get; set;}

    public SpikeRootTile(int x, int y): base(RootType.Spike, x, y) {
        this.hitPoint = startingHitPoint;
        this.maxHitPoint = startingHitPoint;
    }

    public SpikeRootTile initFromInplace(SpikeRootTile t, int x, int y) {
        this.tileType = TileType.Root;
        this.rootType = RootType.Spike;
        this.x = x;
        this.y = y;
        this.hitPoint = t.startingHitPoint;
        this.maxHitPoint = t.startingHitPoint;
        this.counterDamage = t.counterDamage;
        this.placementCost = t.placementCost;
        this.maintenanceCost = t.maintenanceCost;
        return this;
    }

    public override String toASCII() {
        return "S";
    }
}
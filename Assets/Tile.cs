using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType {
    Dwarf,
    Empty,
    Terrain,
    Root,
}

public enum RootType {
    Regular,
    Spike,
}

public abstract class Tile: ScriptableObject {
    public TileType tileType {get; protected set;}
    public int x {get; protected set;}
    public int y {get; protected set;}

    public Tile(TileType type, int x, int y) {
        this.tileType = type;
        this.x = x;
        this.y = y;
    }

    public Tile initInplace(TileType type, int x, int y) {
        this.tileType = type;
        this.x = x;
        this.y = y;
        return this;
    }
    public abstract String toASCII();
}

public abstract class RootTile: Tile, IDestructible {
    public RootType rootType {get; protected set;}
    [SerializeField]
    public int placementCost;
    [SerializeField]
    public int maintenanceCost;
    public int hitPoint {get; set;}
    public int maxHitPoint {get; set;}

    public void takeDamage(int damage) {
        this.hitPoint -= damage;
    }

    public bool isDestroyed() {
        return this.hitPoint <= 0;
    }

    public RootTile(RootType type, int x, int y): base(TileType.Root, x, y) {
        this.rootType = type;
    }
}
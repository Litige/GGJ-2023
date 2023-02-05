using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TerrainTile: Tile, IDestructible {
    [SerializeField]
    public int hitPoint {get; set;}
    [SerializeField]
    public int maxHitPoint  {get; set;}
    [SerializeField]
    private int startingHitPoint;
    
    public TerrainTile(int x, int y): base(TileType.Terrain, x, y) {
        this.hitPoint = startingHitPoint;
        this.maxHitPoint = startingHitPoint;
    }

    public TerrainTile initFromInplace(TerrainTile t, int x, int y) {
        this.tileType = TileType.Terrain;
        this.x = x;
        this.y = y;
        this.hitPoint = t.startingHitPoint;
        this.maxHitPoint = t.startingHitPoint;
        return this;
    }

    public void takeDamage(int damage) {
        this.hitPoint -= damage;
    }

    public bool isDestroyed() {
        Debug.Log($"current HP:{this.hitPoint}");
        return this.hitPoint <= 0;
    }
    
    public override String toASCII() {
        return "T";
    }
}
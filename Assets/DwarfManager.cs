using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DwarfNumberUpdateEventArguments {
    public int nbDwarf { get; } 
    public DwarfNumberUpdateEventArguments(int nbDwarf) {
        this.nbDwarf = nbDwarf;
    }
}

public class DwarfManager : MonoBehaviour {
    [SerializeField]
    public int initialNumberOfDwarfs;
    [SerializeField]
    public Dwarf defaultDwarf;
    [SerializeField]
    public GridManager gridManager;

    public static event DwarfNumberUpdateSubscription DwarfNumberUpdateEvent;
    public delegate void DwarfNumberUpdateSubscription(DwarfNumberUpdateEventArguments arg);
    
    public static event DwarfDiedSubscription DwarfDiedEvent;
    public delegate void DwarfDiedSubscription();

    public List<Dwarf> dwarfs;

    // Start is called before the first frame update
    void Start()
    {
        this.spawnDwarf();
        DwarfNumberUpdateEvent.Invoke(new DwarfNumberUpdateEventArguments(this.dwarfs.Count));
        TurnManager.NewTurnEvent += this.dwarfActivity;
    }

    void spawnDwarf() {
        var emptyTiles = 
            from line in this.gridManager.grid
            from tile in line
            where tile.tileType == TileType.Empty
            select tile;
        var validTiles = new List<Tile>();

        foreach(var tile in emptyTiles) {
            var inRange = 
                from range in Enumerable.Range(1, 1 + (this.defaultDwarf.miningRange - 1))
                from irTile in this.gridManager.tilesInRadius(tile.x, tile.y, range)
                where irTile.tileType == TileType.Terrain
                select irTile;
            if (inRange.Count() > 0) {
                validTiles.Add(tile);
            }
        }
        var random = new System.Random(Time.frameCount);
        foreach(int i in Enumerable.Range(0, this.initialNumberOfDwarfs)) {
            int index = random.Next(validTiles.Count);
            this.gridManager.addDwarfs(validTiles[index].x, validTiles[index].y, 1);
            var newDwarf = (Dwarf) ScriptableObject.CreateInstance(this.defaultDwarf.GetType());
            newDwarf.initFrom(this.defaultDwarf);
            newDwarf.x = validTiles[index].x;
            newDwarf.y = validTiles[index].y;
            this.dwarfs.Add(newDwarf);
        }
        this.gridManager.updateTilemap();
    }

    void dwarfActivity() {
        this.mine();
    }

    void mine() {
        var random = new System.Random(Time.frameCount);

        foreach(var dwarf in this.dwarfs) {
            var inRange = (
                from range in Enumerable.Range(1, 1 + (this.defaultDwarf.miningRange - 1))
                from irTile in this.gridManager.tilesInRadius(dwarf.x, dwarf.y, range)
                where irTile.tileType == TileType.Terrain || irTile.tileType == TileType.Root
                select irTile).ToList();
            Debug.Log($"Found {inRange.Count} target in range of ({dwarf.x};{dwarf.y})");
            foreach(var tile in inRange) {
                Debug.Log($"target ({tile.x};{tile.y})");
            }
            if (inRange.Count > 0) {
                int index = random.Next(inRange.Count);
                this.gridManager.damageTile(dwarf, inRange[index].x, inRange[index].y, dwarf.miningDamage);
            } else {
                var closestTarget = this.gridManager.closestBreakable(dwarf.x, dwarf.y, this.defaultDwarf.miningRange);
                this.moveToward(dwarf, closestTarget);
            }
        }

        this.purgeDeadDwarf();
    }

    void moveToward(Dwarf dwarf, Tile target) {
        var dwarfVect2 = new Vector2(dwarf.x, dwarf.y);
        var targetVect2 = new Vector2(target.x, target.y);
        var dwarfToTargetNormal = (targetVect2 - dwarfVect2).normalized;
        Debug.Log($"dwarf: {dwarfVect2.ToString()}");
        Debug.Log($"target: {targetVect2.ToString()}");

        foreach(var distance in Enumerable.Range(1, 1 + (this.defaultDwarf.movementSpeed - 1))) {
            var moveTarget = dwarfVect2 + (dwarfToTargetNormal * distance);
            var moveTargetNeightboor = this.gridManager.tilesInRadius((int) Math.Round(moveTarget.x, 0), (int) Math.Round(moveTarget.y, 0), 1);
            if ((moveTargetNeightboor.Select(e => e.x == target.x && e.y == target.y)).Count() > 0) {
                this.gridManager.addDwarfs((int) Math.Round(moveTarget.x, 0), (int) Math.Round(moveTarget.y, 0), 1);
                this.gridManager.removeDwarfs(dwarf.x, dwarf.y, 1);
                dwarf.x = (int) Math.Round(moveTarget.x, 0);
                dwarf.y = (int) Math.Round(moveTarget.y, 0);
                return;
            }
        }

        Debug.Log($"Could not reach ({target.x};{target.y}) from ({dwarf.x};{dwarf.y})");
    }

    void purgeDeadDwarf() {
        var deadDwarfs =
            from dwarf in this.dwarfs
            where dwarf.isDestroyed()
            select dwarf;
        this.dwarfs = (
            from dwarf in this.dwarfs
            where !dwarf.isDestroyed()
            select dwarf).ToList();

        Debug.Log($"Found {deadDwarfs.Count()} dead dwarf(s)");

        foreach(var dwarf in deadDwarfs) {
            this.gridManager.removeDwarfs(dwarf.x, dwarf.y, 1);
        }

        if (deadDwarfs.Count() > 0)
            DwarfNumberUpdateEvent.Invoke(new DwarfNumberUpdateEventArguments(this.dwarfs.Count));
        if(this.dwarfs.Count <= 0)
            DwarfDiedEvent();
    }
}
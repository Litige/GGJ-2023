using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    public int xSize;
    [SerializeField]
    public int ySize;
    [SerializeField]
    private Tilemap boardMap;

    [SerializeField]
    private RuleTile dwarfRuleTile;
    [SerializeField]
    private RuleTile emptyRuleTile;
    [SerializeField]
    private RuleTile terrainRuleTile;
    [SerializeField]
    private RuleTile regularRootRuleTile;
    [SerializeField]
    private RuleTile spikeRootRuleTile;

    [SerializeField]
    public DwarfTile dwarfTile;
    [SerializeField]
    public EmptyTile emptyTile;
    [SerializeField]
    public TerrainTile terrainTile;
    [SerializeField]
    public RegularRootTile regularRootTile;
    [SerializeField]
    public SpikeRootTile spikeRootTile;

    public List<List<Tile>> grid {get; private set;}

    void OnEnable()
    {
        this.generateBasicGrid(xSize, ySize);
        this.generateDwarfEntry();
        this.generateRootStart();
        this.updateTilemap();
    }

    private void generateBasicGrid(int x, int y) {
        var xIndices = Enumerable.Range(0, x);
        var yIndices = Enumerable.Range(0, y);
        this.grid = xIndices.Select(_x =>
            yIndices.Select(_y => {
                var newTile = ((TerrainTile) ScriptableObject.CreateInstance(this.terrainTile.GetType())).initFromInplace(this.terrainTile, _x, _y);
                return (Tile) newTile;
            }).ToList()
        ).ToList();
    }

    private void generateRootStart() {
        int x = this.xSize / 2 - 1;
        int y = this.ySize - 1;
        this.grid[x][y] = ((Tile)
            ((RegularRootTile) ScriptableObject.CreateInstance(this.regularRootTile.GetType()))
                .initFromInplace(this.regularRootTile, x, y)
        );
    }

    private void generateDwarfEntry() {
        int targetX = this.xSize / 2 - 1;
        int targetY = 5;
        foreach(int x in Enumerable.Range(targetX, 3)) {
            foreach(int y in Enumerable.Range(targetY, 3)) {
                this.grid[x][y] = (
                    ((Tile) ScriptableObject.CreateInstance(this.emptyTile.GetType()))
                        .initInplace(TileType.Empty, x, y)
                );
            }
        }
    }

    public void addDwarfs(int x, int y, int n) {
        switch (this.grid[x][y].tileType) {
            case TileType.Dwarf:
                ((DwarfTile) this.grid[x][y]).nbDwarf += n;
                break;
            case TileType.Empty:
                this.grid[x][y] = (((Tile) ScriptableObject.CreateInstance(this.dwarfTile.GetType()))
                    .initInplace(TileType.Dwarf, x, y));
                ((DwarfTile) this.grid[x][y]).nbDwarf += n;
                this.updateTile(x, y);
                break;
            default:
                Debug.Log($"Impossible to add {n} dwarfs at {this.grid[x][y].tileType} ({x};{y})");
                break;
        }
    }

    public void removeDwarfs(int x, int y, int n) {
        switch (this.grid[x][y].tileType) {
            case TileType.Dwarf:
                ((DwarfTile) this.grid[x][y]).nbDwarf -= n;
                if (((DwarfTile) this.grid[x][y]).nbDwarf <= 0) {
                    this.grid[x][y] = (
                        ((Tile) ScriptableObject.CreateInstance(this.emptyTile.GetType()))
                            .initInplace(TileType.Empty, x, y)
                    );
                    this.updateTile(x, y);
                }
                break;
            default:
                Debug.Log($"Impossible to add {n} dwarfs at {this.grid[x][y].tileType} ({x};{y})");
                break;
        }
    }

    public void damageTile(Dwarf d, int x, int y, int damage) {
        ((IDestructible) this.grid[x][y]).takeDamage(damage);
        if (this.grid[x][y].tileType == TileType.Root &&
            ((RootTile) this.grid[x][y]).rootType == RootType.Spike) {
            Debug.Log($"Took damage for {((SpikeRootTile) this.grid[x][y]).counterDamage}");
            d.takeDamage(((SpikeRootTile) this.grid[x][y]).counterDamage);
        }
        Debug.Log($"Target is a {((Tile) this.grid[x][y]).tileType}");
        if (((IDestructible) this.grid[x][y]).isDestroyed()) {
            Debug.Log($"Destroying ({x};{y})");
            this.grid[x][y] = (
                    ((Tile) ScriptableObject.CreateInstance(this.emptyTile.GetType()))
                        .initInplace(TileType.Empty, x, y)
                );
            this.updateTile(x, y);
        }
    }

    public List<Tile> tilesInRadius(int x, int y, int r) {
        var tilesInRadius = 
            from line in this.grid
            from tile in line
            where
                (tile.x == x + r && tile.y <= y + r && tile.y >= y - r) ||
                (tile.x == x - r && tile.y <= y + r && tile.y >= y - r) ||
                (tile.y == y + r && tile.x <= x + r && tile.x >= x - r) ||
                (tile.y == y - r && tile.x <= x + r && tile.x >= x - r)
            select tile;
        return tilesInRadius.ToList();
    }

    public bool canBuildOnTile(int x, int y) {
        try {
            if (this.grid[x][y].tileType == TileType.Root || this.grid[x][y].tileType == TileType.Dwarf)
                return false;
            var neightboors = this.tilesInRadius(x, y, 1);
            return neightboors.Exists (t => t.tileType == TileType.Root);
        } catch (ArgumentOutOfRangeException e) {
            return false;
        } catch (NullReferenceException e) {
            return false;
        }
    }

    public void growRoots(int x, int y) {
        if (this.grid[x][y].tileType == TileType.Empty)
            this.grid[x][y] = ((Tile)
                ((SpikeRootTile) ScriptableObject.CreateInstance(this.spikeRootTile.GetType()))
                    .initFromInplace(this.spikeRootTile, x, y)
            );
        else
            this.grid[x][y] = ((Tile)
                ((RegularRootTile) ScriptableObject.CreateInstance(this.regularRootTile.GetType()))
                    .initFromInplace(this.regularRootTile, x, y)
            );
        this.updateTile(x, y);
    }

    public Tile closestBreakable(int x, int y, int r) {
        List<Tile> closest = null;
        int radius = r;
        while (closest == null || closest.Count <= 0) {
            closest = (
                from tile in this.tilesInRadius(x, y, radius)
                where tile.tileType == TileType.Terrain || tile.tileType == TileType.Root
                select tile
            ).ToList();
            radius += 1;
        }

        var random = new System.Random(Time.frameCount);
        int index = random.Next(closest.Count);
        return closest[index];
    }

    public void updateTilemap() {
        foreach (var line in this.grid) {
            foreach(var tile in line) {
                switch (tile.tileType) {
                    case TileType.Dwarf:
                        this.boardMap.SetTile(new Vector3Int(tile.x, tile.y, 0), this.dwarfRuleTile);
                        break;
                    case TileType.Empty:
                        this.boardMap.SetTile(new Vector3Int(tile.x, tile.y, 0), this.emptyRuleTile);
                        break;
                    case TileType.Terrain:
                        this.boardMap.SetTile(new Vector3Int(tile.x, tile.y, 0), this.terrainRuleTile);
                        break;
                    case TileType.Root:
                        var root = (RootTile) tile;
                        if (root.rootType == RootType.Regular)
                            this.boardMap.SetTile(new Vector3Int(tile.x, tile.y, 0), this.regularRootRuleTile);
                        else
                            this.boardMap.SetTile(new Vector3Int(tile.x, tile.y, 0), this.spikeRootRuleTile);
                        break;
                }
            }
        }
    }

    public void updateTile(int x, int y) {
        switch (this.grid[x][y].tileType) {
            case TileType.Dwarf:
                this.boardMap.SetTile(new Vector3Int(x, y, 0), this.dwarfRuleTile);
                break;
            case TileType.Empty:
                this.boardMap.SetTile(new Vector3Int(x, y, 0), this.emptyRuleTile);
                break;
            case TileType.Terrain:
                this.boardMap.SetTile(new Vector3Int(x, y, 0), this.terrainRuleTile);
                break;
            case TileType.Root: {}
                var root = (RootTile) this.grid[x][y];
                if (root.rootType == RootType.Regular)
                    this.boardMap.SetTile(new Vector3Int(x, y, 0), this.regularRootRuleTile);
                else
                    this.boardMap.SetTile(new Vector3Int(x, y, 0), this.spikeRootRuleTile);
                break;
        }
    }

    public void printGrid() {
        string asciiGrid = "";
        foreach (var line in this.grid) {
            foreach (var tile in line) {
                // Debug.Log($"{tile.tileType} {tile.x};{tile.y}");
                asciiGrid += tile.toASCII();
            }
            asciiGrid += Environment.NewLine;
        }
        Debug.Log(asciiGrid);
    }
}
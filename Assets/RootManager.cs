using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PointUpdateEventArgument {
    public int points { get; } 
    public PointUpdateEventArgument(int points) {
        this.points = points;
    }
}

public class RootManager : MonoBehaviour
{
    [SerializeField]
    private GridManager gridManager;
    [SerializeField]
    private int points;

    private int pointsOnNextTurn = 0;

    public static event PlayerDiedSubscription PlayerDiedEvent;
    public delegate void PlayerDiedSubscription();

    public static event CurrentPointsUpdateSubscription CurrentPointsUpdateEvent;
    public delegate void CurrentPointsUpdateSubscription(PointUpdateEventArgument arg);

    public static event NextTurnPointsUpdateSubscription NextTurnPointsUpdateEvent;
    public delegate void NextTurnPointsUpdateSubscription(PointUpdateEventArgument arg);

    void Start()
    {
        OverlayManager.UserClickedEvent += this.userInput;
        TurnManager.NewTurnEvent += this.gainPoints;
        this.calculateNextTurnPoints();
        CurrentPointsUpdateEvent.Invoke(new PointUpdateEventArgument(this.points));    
        NextTurnPointsUpdateEvent.Invoke(new PointUpdateEventArgument(this.pointsOnNextTurn));    
    }

    private void calculateNextTurnPoints() {
        var allRegularRoots = from line in this.gridManager.grid
            from tile in line
            where
                tile.tileType == TileType.Root &&
                ((RootTile) tile).rootType == RootType.Regular
            select tile;
        var allSpikeRoots = from line in this.gridManager.grid
            from tile in line
            where
                tile.tileType == TileType.Root &&
                ((RootTile) tile).rootType == RootType.Spike
            select tile;
        var allRegularRootsNeightboors = (
            from tile in allRegularRoots
            from neightboor in this.gridManager.tilesInRadius(tile.x, tile.y, 1)
            where neightboor.tileType == TileType.Terrain
            select (neightboor.x, neightboor.y)
        ).Distinct();

        var old = this.pointsOnNextTurn;
        this.pointsOnNextTurn =
            allRegularRootsNeightboors.Count() * this.gridManager.regularRootTile.pointsByNeighboor 
            - allRegularRoots.Count() * this.gridManager.regularRootTile.maintenanceCost
            - allSpikeRoots.Count() * this.gridManager.spikeRootTile.maintenanceCost;

        NextTurnPointsUpdateEvent.Invoke(new PointUpdateEventArgument(this.pointsOnNextTurn));
        Debug.Log($"Points on next turn update, old {old} new {this.pointsOnNextTurn}");
    }

    private void gainPoints() {
        this.calculateNextTurnPoints();
        this.points += this.pointsOnNextTurn;
        if (this.pointsOnNextTurn != 0)
            CurrentPointsUpdateEvent.Invoke(new PointUpdateEventArgument(this.points));
        if (this.points <= 0 && this.pointsOnNextTurn <= 0) {
            PlayerDiedEvent();
        }
    }
    private void userInput(UserClickedEventArguments e) {
        var cost = (this.gridManager.grid[e.x][e.y].tileType == TileType.Empty) ? this.gridManager.spikeRootTile.placementCost : this.gridManager.regularRootTile.placementCost;
        if (this.points > cost) {
            this.gridManager.growRoots(e.x, e.y);
            this.points -= cost;
            Debug.Log($"Placed new root for {cost} (old total {this.points + cost} new total {this.points})");
            this.calculateNextTurnPoints();
            CurrentPointsUpdateEvent.Invoke(new PointUpdateEventArgument(this.points));
        }
    }
}

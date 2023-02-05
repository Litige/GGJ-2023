using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UserClickedEventArguments {
    public int x { get; } 
    public int y { get; } 
    public UserClickedEventArguments(int x, int y) {
        this.x = x;
        this.y = y;
    }
}

public class OverlayManager : MonoBehaviour
{
    [SerializeField]
    private RuleTile canBuildTile;
    [SerializeField]
    private RuleTile cantBuildTile;
    [SerializeField]
    private Grid grid;    
    [SerializeField]
    private GridManager gridManager;
    [SerializeField]
    private Tilemap overlayMap;

    private Vector3Int previousMousePos = new Vector3Int();
    public static event UserClickedSubscription UserClickedEvent;
    public delegate void UserClickedSubscription(UserClickedEventArguments a);

    void Start() {
        EndgameManager.GameOverEvent += deactivateSelf;
    }

    void deactivateSelf() {
        this.enabled = false;
    }

    void Update()
    {
        Vector3Int mousePos = GetMousePosition();
        bool canBuild = this.gridManager.canBuildOnTile(mousePos.x, mousePos.y);
        if (!mousePos.Equals(previousMousePos)) {
            overlayMap.SetTile(previousMousePos, null);
            if (canBuild)
                overlayMap.SetTile(mousePos, canBuildTile);
            previousMousePos = mousePos;
        }
        if (!canBuild) {
            overlayMap.SetTile(previousMousePos, null);
        }
        if (Input.GetMouseButton(0) && canBuild)
            UserClickedEvent.Invoke(new UserClickedEventArguments(mousePos.x, mousePos.y));
    }

    Vector3Int GetMousePosition () {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return grid.WorldToCell(mouseWorldPos);
    }
}

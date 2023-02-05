using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndgameManager : MonoBehaviour
{
    public static event GameOverSubscription GameOverEvent;
    public delegate void GameOverSubscription();

    public String endText {get; private set;}

    void Start() {
        DwarfManager.DwarfDiedEvent += this.plantVictory;
        RootManager.PlayerDiedEvent += this.dwarfVictory;
    }

    public void plantVictory() {
        this.endText = "You Won!";
        GameOverEvent();
    }

    public void dwarfVictory() {
        this.endText = "You Lost!";
        GameOverEvent();
    }
}

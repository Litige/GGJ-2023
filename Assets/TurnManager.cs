using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField]
    public int turnLength = 10;

    public float timeSinceLastTurn {get; private set;} = 0.0f; 

    private static float VERY_HIGH_SPEED = 4.0f;
    private static float HIGH_SPEED = 2.0f;
    private static float DEFAULT_SPEED = 1.0f;
    
    private float timeSpeed = DEFAULT_SPEED;

    public static event NewTurnSubscription NewTurnEvent;
    public delegate void NewTurnSubscription();

    public void setTimeSpeed_DEFAULT() {
        this.timeSpeed = DEFAULT_SPEED;
    }

    public void setTimeSpeed_HIGH() {
        this.timeSpeed = HIGH_SPEED;
    }

    public void setTimeSpeed_VERY_HIGH() {
        this.timeSpeed = VERY_HIGH_SPEED;
    }
    void Start() {
        EndgameManager.GameOverEvent += deactivateSelf;
    }

    void deactivateSelf() {
        this.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        this.timeSinceLastTurn += Time.deltaTime * timeSpeed;
        if (this.timeSinceLastTurn >= this.turnLength) {
            Debug.Log($"Entering new turn");
            this.timeSinceLastTurn %= this.turnLength;
            NewTurnEvent();
        }
    }
}

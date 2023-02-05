using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GrowthPointManager : MonoBehaviour
{
    [SerializeField]
    private Text currentPoints;

    [SerializeField]
    private Text nextTurnPoints;

    void OnEnable() {
        RootManager.CurrentPointsUpdateEvent += currentPointTextUpdate;
        RootManager.NextTurnPointsUpdateEvent += nextPointTextUpdate;
    }

    void currentPointTextUpdate(PointUpdateEventArgument e) {
        this.currentPoints.text = e.points.ToString();
    }
    
    void nextPointTextUpdate(PointUpdateEventArgument e) {
        this.nextTurnPoints.text = (e.points >= 0) ? $"+{e.points.ToString()}" : $"{e.points.ToString()}";
    }
}

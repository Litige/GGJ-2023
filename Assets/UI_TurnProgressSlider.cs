using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TurnProgressSlider : MonoBehaviour
{
    [SerializeField]
    TurnManager turnManager;
    [SerializeField]
    Slider slider;
    // Update is called once per frame
    void Update()
    {
        this.slider.value = this.turnManager.timeSinceLastTurn / this.turnManager.turnLength;
    }
}

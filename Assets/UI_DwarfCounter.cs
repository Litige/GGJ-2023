using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DwarfCounter : MonoBehaviour
{
    [SerializeField]
    private Text UIText;

    void OnEnable() {
        DwarfManager.DwarfNumberUpdateEvent += textUpdate;
    }

    void textUpdate(DwarfNumberUpdateEventArguments e) {
        this.UIText.text = e.nbDwarf.ToString();
    }
}

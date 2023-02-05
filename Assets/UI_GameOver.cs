using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_GameOver : MonoBehaviour
{
    [SerializeField]
    private Text epitath;
    [SerializeField]
    private Canvas gameOverCanvas;
    [SerializeField]
    private EndgameManager endgameManager;

    void Start() {
        EndgameManager.GameOverEvent += setupGameOverUI;
    }

    void setupGameOverUI() {
        this.gameOverCanvas.enabled = true;
        this.epitath.text = endgameManager.endText;
    }

    public void quitGame() {
        Application.Quit();
    }

    public void resetGame() {
        SceneManager.LoadScene("GameScene");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; 

public class CanvasManager : MonoBehaviour
{
    public Button replayButton;
    public Text endText;


    void Start()
    {
        if (replayButton)
        {
            replayButton.onClick.AddListener(Replay);
        }
    }

    public void Replay()
    {
        SceneManager.LoadScene("TicTacToe");
    }

    void Update()
    {
        if (GameManager.Instance.GetGameState() != GameManager.GameState.GAME)
        {
            replayButton.gameObject.SetActive(true);
            endText.gameObject.SetActive(true);
            if (GameManager.Instance.GetGameState() == GameManager.GameState.DEFEAT)
            {
                endText.text = "YOU LOSE!"; 
            }
            else if (GameManager.Instance.GetGameState() == GameManager.GameState.DRAW)
            {
                endText.text = "DRAW!";
            }
            else //SHOULD NEVER OCCUR!!!
            {
                endText.text = "WIN!";
            }
        }
    }
}

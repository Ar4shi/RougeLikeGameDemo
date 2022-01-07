using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    BoardManager boardManager;

    void Awake()
    {
        boardManager = GetComponent<BoardManager>();
        this.InitGame();
    }

    private void InitGame()
    {
        boardManager.SetupScene();
    }
}

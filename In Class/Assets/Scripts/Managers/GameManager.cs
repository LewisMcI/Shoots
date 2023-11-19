using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    enum GameState
    {
        Playing,
        CardSelection,
        Paused
    }

    public CardManager cardManager;
    // Game Initializes in Card Selection
    GameState currentGameState = GameState.CardSelection;


    private void Awake()
    {
        // Create Next Player Card
        cardManager.SpawnNextPlayerCard(0);
    }

}

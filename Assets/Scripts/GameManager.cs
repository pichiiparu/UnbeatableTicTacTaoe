using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public enum GameState
    {
        GAME,
        DEFEAT,
        WIN,
        DRAW
    }
    [SerializeField] private GameState currentState;

    public event Action OnGameStateChanged;

    private void Start()
    {
        if (currentState == GameState.GAME)
        {

        }
    }

    public void Awake()
    {
        if (Instance == null)
            Instance = this;

        if (Instance != this)
            Destroy(gameObject);
    }

    public GameState GetGameState()
    {
        return currentState;
    }

    public void SwitchState(GameState newState)
    {
        Debug.Log("New state has been set to " + newState);
        currentState = newState;
        OnGameStateChanged?.Invoke();
    }
}

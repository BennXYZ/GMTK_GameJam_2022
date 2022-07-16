using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameStateManager : MonoBehaviour
{
    public enum GameState
    {
        ActionSelection,
        TileSelection,
        EnemyTurn,
        Waiting
    }

    public static GameStateManager Instance { get => GameManager.Instance.gameStateManager; }

    Dictionary<GameState, UnityEvent> stateStartEvents = new Dictionary<GameState, UnityEvent>();
    Dictionary<GameState, UnityEvent> stateFinishEvents = new Dictionary<GameState, UnityEvent>();

    private void Awake()
    {
        foreach (GameState enumName in Enum.GetValues(typeof(GameState)))
        {
            stateStartEvents.Add(enumName, new UnityEvent());
            stateFinishEvents.Add(enumName, new UnityEvent());
        }
    }

    public void AddStartEventsListener(GameState state, UnityAction action)
    {
        stateStartEvents[state].AddListener(action);
    }

    public void AddFinishEventsListener(GameState state, UnityAction action)
    {
        stateFinishEvents[state].AddListener(action);
    }

    public void RemoveStartEventsListener(GameState state, UnityAction action)
    {
        stateStartEvents[state].RemoveListener(action);
    }

    public void RemoveFinishEventsListener(GameState state, UnityAction action)
    {
        stateFinishEvents[state].RemoveListener(action);
    }

    GameState currentGameState = GameState.Waiting;

    public GameState CurrentGameState { 
        get => currentGameState;
        set
        {
            if (currentGameState != value)
            {
                stateFinishEvents[currentGameState].Invoke();
                stateStartEvents[value].Invoke();
            }
            currentGameState = value;
        }
    }
}

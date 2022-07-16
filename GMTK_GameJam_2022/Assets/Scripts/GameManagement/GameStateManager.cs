using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public enum GameState
    {
        ActionSelection,
        TileSelection,
        EnemyTurn,
        Waiting
    }

    public PlayerEntity playerEntity;
    List<LivingEntity> livingEntities = new List<LivingEntity>();
    List<EnemyEntity> enemies = new List<EnemyEntity>();
    List<Entity> entities = new List<Entity>();

    public void AssignEntity(Entity entity)
    {
        if (entity is PlayerEntity)
        {
            playerEntity = entity as PlayerEntity;
            return;
        }
        if (entity is EnemyEntity)
        {
            enemies.Add(entity as EnemyEntity);
            return;
        }
        entities.Add(entity);
    }

    internal void RemoveEntity(Entity entity)
    {
        if (playerEntity == entity)
            playerEntity = null;
        if (entity is LivingEntity)
            livingEntities.Remove(entity as LivingEntity);
        if (entity is EnemyEntity)
            enemies.Remove(entity as EnemyEntity);
        entities.Remove(entity as LivingEntity);
    }

    public void DoEnemyTurns()
    {
        CurrentGameState = GameState.EnemyTurn;
        if(enemies.Count > 0)
        {
            enemies[0].DoTurn(delegate { EnemyTurnFinished(1); });
        }
    }

    void EnemyTurnFinished(int nextEnemyId)
    {
        if(nextEnemyId < enemies.Count)
            enemies[nextEnemyId].DoTurn(delegate { EnemyTurnFinished(nextEnemyId + 1); });
        else
        {
            CurrentGameState = GameState.ActionSelection;
        }    
    }

    int currentLevel = 0;

    public int CurrentLevel
    {
        get
        {
            return currentLevel;
        }
        set
        {
            if(currentLevel != value)
            {
                currentLevel = value;
                LoadCurrentLevel();
            }
        }
    }

    private void LoadCurrentLevel()
    {
        SceneManager.LoadScene(string.Format("Level{0:00}", currentLevel));
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

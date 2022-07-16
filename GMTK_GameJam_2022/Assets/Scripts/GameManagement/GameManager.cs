using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void GameStart() => DontDestroyOnLoad(Instantiate(Resources.Load("GameManager")));

    public GameStateManager gameStateManager;
    public InputManager inputManager;

    [Space(15)]

    public GameObject tilePrefab;

    public PlayerEntity playerEntity;
    static GameManager instance;
    public static GameManager Instance { get => instance; }

    private void Awake()
    {
        instance = this;

        gameStateManager = GetComponent<GameStateManager>();
        inputManager = GetComponent<InputManager>();
    }
}
